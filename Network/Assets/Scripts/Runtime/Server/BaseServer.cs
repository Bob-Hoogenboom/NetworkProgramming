using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Net;



/// <summary>
/// https://www.youtube.com/watch?v=S3jFXcY0UNo
/// </summary>

public class BaseServer : MonoBehaviour
{
    public ushort port = 5530;

    public NetworkDriver driver;
    protected NativeList<NetworkConnection> connections;


    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { ShutDown(); }

    public virtual void Init()
    {
        //init the driver
        driver = NetworkDriver.Create();

        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4; //Define who can connect to us (AnyIpv4 is anyone basicly)
        endpoint.Port = 5530; //Can be any number

        if (driver.Bind(endpoint) != 0)
        {
            Debug.LogError("❌ Failed to bind to port " + endpoint.Port);
        }
        else
        {
            driver.Listen();
            Debug.Log("Server is now listening on port " + endpoint.Port);
        }

        //init the connection list
        connections = new NativeList<NetworkConnection>(4, Allocator.Persistent); //Number of max players that can connect, network connections are never destroyed

        Debug.Log($"Server binding to: {endpoint}");
    }

    public virtual void ShutDown()
    {
        driver.Dispose();
        connections.Dispose();
    }

    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete(); //derive from job system to cleanup connection

        CleanupConnections();   //When someone drops out without pressing disconnect
        AcceptNewConnections(); //Looks if someone is trying to connect concentually
        UpdateMessagePump();    //Parse all the messages the client is sending us and applying that
    }

    private void CleanupConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;    //Create empty connection
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a connection");
        }
    }

    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i =0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream);

                    /*byte opCode = stream.ReadByte();
                    FixedString512Bytes chatMessage = stream.ReadFixedString512(); 
                    Debug.Log("Got " + opCode + " as operation code");
                    Debug.Log("Got " + chatMessage + " as chat message");*/
                }
                else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public virtual void OnData(DataStreamReader stream) 
    {
        NetMessage msg = null;
        var opCode = (OpCode)stream.ReadByte(); //recieve a stream of data and then read the first byte
        switch (opCode)
        {
            case OpCode.CHAT_MESSAGE:
                msg = new Net_ChatMessage(stream);
                break;
            case OpCode.PLAYER_POSITION:
                msg = new Net_PlayerPosition(stream);
                break;

            default:
                Debug.Log("Message recieved had no OpCode");
                break;

        }

        msg.RecievedOnServer(this);
    }

    public virtual void Broadcast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                SendToClient(connections[i], msg);
            }
        }
    }

    public virtual void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);

        msg.Serialize(ref writer);

        driver.EndSend(writer);
    }
}