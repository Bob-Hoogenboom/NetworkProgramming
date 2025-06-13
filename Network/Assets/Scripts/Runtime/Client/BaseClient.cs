using System.Collections;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Net.Sockets;


public class BaseClient : MonoBehaviour
{
    public string ipAdress = "127.0.0.1";
    public ushort port = 5530;
    public NetworkDriver driver;
    protected NetworkConnection connection;

    private float reconnectTimer = 0f;
    private float reconnectInterval = 3f;
    public bool isTryingToConnect = false;
    public bool isConnected = false;


    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { ShutDown(); }


    public virtual void Init()
    {
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);
        TryConnect();
    }

    private void TryConnect()
    {
        if (driver.IsCreated)
        {
            var endpoint = NetworkEndpoint.Parse(ipAdress, port);
            connection = driver.Connect(endpoint);
            isTryingToConnect = true;
            Debug.Log($"Trying to connect to server at {ipAdress}:{port}");
        }
    }

    public virtual void ShutDown()
    {
        driver.Dispose();
    }

    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete(); //derive from job system to cleanup connection


        // Only try to reconnect if not connected or already trying
        if (!isConnected && !isTryingToConnect)
        {
            reconnectTimer += Time.deltaTime;
            if (reconnectTimer >= reconnectInterval)
            {
                reconnectTimer = 0f;
                TryConnect();
            }
        }

        CheckAlive();
        UpdateMessagePump();    //Parse all the messages the client is sending us and applying that
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated)
        {
            Debug.Log("Somethign went wrong, no connection to server");
        }
    }

    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;

        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Client got connected to the server");
                isTryingToConnect = false;
                isConnected = true;
            }
            else if(cmd == NetworkEvent.Type.Data)
            {
                //uint value = stream.ReadByte();
                //Debug.Log("Got the value = " + value + " back from the server");
                OnData(stream);
                Debug.Log($"Got the stream = {stream} back from the server");
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                connection = default(NetworkConnection);
                isTryingToConnect = false;
                isConnected = false;
            }
        }
    }

    public virtual void OnData(DataStreamReader stream)
    {
        var opCode = (OpCode)stream.ReadByte(); //recieve a stream of data and then read the first byte
        switch (opCode)
        {
            case OpCode.CHAT_MESSAGE:
                Net_ChatMessage chat = new Net_ChatMessage(stream);
                chat.RecievedOnClient();
                break;
            case OpCode.PLAYER_POSITION:
                Net_PlayerPosition pos = new Net_PlayerPosition(stream);
                pos.RecievedOnClient();
                transform.position = new Vector3(pos.positionX, pos.positionY, pos.positionZ);
                break;

            default:
                Debug.Log("Message recieved had no OpCode");
                break;

        }
    }

    public virtual void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);

        msg.Serialize(ref writer);

        driver.EndSend(writer);
    }
}