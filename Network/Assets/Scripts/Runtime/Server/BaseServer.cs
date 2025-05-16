using System.Collections;
using UnityEngine;

using Unity.Collections;
using Unity.Networking.Transport;
using System.Net.Sockets;

/// <summary>
/// https://www.youtube.com/watch?v=S3jFXcY0UNo
/// </summary>

public class BaseServer : MonoBehaviour
{
    public NetworkDriver driver;
    protected NativeList<NetworkConnection> connections;

#if UNITY_EDITOR
    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { ShutDown(); }
#endif

    public virtual void Init()
    {
        //init the driver
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4; //Define who can connect to us (AnyIpv4 is anyone basicly)
        endpoint.Port = 5522; //Can be any number

        if(driver.Bind(endpoint) != 0)
        {
            Debug.Log("There was an error binding to port " + endpoint.Port);
        }
        else
        {
            driver.Listen(); //Defining that we are a server and we started
        }

        //init the connection list
        connections = new NativeList<NetworkConnection>(4, Allocator.Persistent); //Number of max players that can connect, network connections are never destroyed
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
                    uint number = stream.ReadByte();
                    Debug.Log("Got " + number + " from the client");
                }
                else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }
}
