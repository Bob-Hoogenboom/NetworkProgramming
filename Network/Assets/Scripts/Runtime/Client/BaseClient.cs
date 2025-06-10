using System.Collections;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Net.Sockets;


public class BaseClient : MonoBehaviour
{
    public string ipAdress = "127.0.0.1";
    public ushort port = 8000;
    public NetworkDriver driver;
    protected NetworkConnection connection;

#if UNITY_EDITOR
    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { ShutDown(); }
#endif

    public virtual void Init()
    {
        //init the driver
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);

        NetworkEndpoint endpoint = NetworkEndpoint.LoopbackIpv4;
        endpoint.Port = 5522; //should be the same portnumber as the server

        connection = driver.Connect(endpoint);
    }

    public virtual void ShutDown()
    {
        driver.Dispose();
    }

    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete(); //derive from job system to cleanup connection

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

        msg.RecievedOnClient();
    }

    public virtual void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);

        msg.Serialize(ref writer);

        driver.EndSend(writer);
    }
}