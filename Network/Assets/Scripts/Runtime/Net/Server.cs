using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Server instance { get; set; }

    public Action connectionDropped;

    public NetworkDriver driver;
    private NativeList<NetworkConnection> _connections;

    private bool _isActive = false;
    private const float _keepAliveTick = 20.0f; //send a connection message every 20 Seconds*
    private float _lastKeepAlive;


    private void Awake()
    {
        instance = this;
    }
    
    //methods
    public void Init(ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;     //allow anybody to connect
        endpoint.Port = port;

        if (driver.Bind(endpoint) != 0)
        {
            Debug.Log($"Unable to bind to port {endpoint.Port}");
            return;
        }
        else
        {
            driver.Listen();
            Debug.Log($"Listening on port {endpoint.Port}");
        }

        _connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        _isActive = true;
    }

    public void Shutdown()
    {
        if (_isActive)
        {
            driver.Dispose();
            _connections.Dispose();
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

        //KeepAlive();

        driver.ScheduleUpdate().Complete();
        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }
    
    private void KeepAlive()
    {

    }

    //Do we still have a reference to a connection that doesn't excist anymore?
    private void CleanupConnections()
    {
        for (int i = 0; i < _connections.Length; i++)
        {
            if (!_connections[i].IsCreated)
            {
                _connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    //Is someone trying to connect?
    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while((c = driver.Accept()) != default(NetworkConnection))
        {
            _connections.Add(c);
        }
    }

    //Check message recieve and if we should return a message
    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i = 0; i < _connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(_connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data)
                {
                    //NetUtility.OnData(stream, _connections[i], this.);
                }
                else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client Disconnected from server");
                    _connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown(); //this is not common but its a 1v1 game so we have to shutdown the server obviously 
                }
            }

        }
    }

    #region Server Specific Methods
    //send data to specific client
    private void SendToClient(NetworkConnection connection, NetMessage msg) 
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        //msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    //send data to every client
    public void Broadcast(NetMessage msg)
    {
        for (int i = 0; i < _connections.Length; i++)
        {
            if (_connections[i].IsCreated)
            {
                //Debug.Log($"sending {msg.Code} to : {_connections[i].InternalId}");
                SendToClient(_connections[i], msg);
            }
        }
    }
    #endregion
}
