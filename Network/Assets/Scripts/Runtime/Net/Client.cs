using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance { get; set; }

    public Action connectionDropped;

    public NetworkDriver driver;
    private NetworkConnection _connection;

    private bool _isActive = false;



    private void Awake()
    {
        instance = this;
    }

    //methods
    public void Init(string ip, ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.Parse(ip, port);

        _connection = driver.Connect(endpoint);

        Debug.Log($"Attempting to connect to Server on {endpoint.Address}");

        _isActive = true;

        RegisterToEvent();
    }

    public void Shutdown()
    {
        if (_isActive)
        {
            UnregisterToEvent();
            driver.Dispose();
            _connection = default(NetworkConnection);
            _isActive = false;
        }
    }

    public void OnDestroy()
    {
        Shutdown();
    }

    public void Update()
    {
        if (!_isActive) return;

        driver.ScheduleUpdate().Complete();
        CheckAlive();

        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!_connection.IsCreated && _isActive)
        {
            Debug.Log("Something went wrong, Lost connection to server");
            connectionDropped?.Invoke();
            Shutdown();
        }
    }

    //Check message recieve and if we should return a message
    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = _connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                SendToServer(new NetWelcome());
                Debug.Log("We're connected");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, default(NetworkConnection));

            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client Disconnected from server");
                _connection = default(NetworkConnection);
                connectionDropped?.Invoke();
                Shutdown();
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(_connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    //Event Parsing
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }


    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }


    private void OnKeepAlive(NetMessage msg)
    {
            SendToServer(msg);
    }



}
