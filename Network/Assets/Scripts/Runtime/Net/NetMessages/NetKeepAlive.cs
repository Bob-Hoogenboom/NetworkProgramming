using Unity.Collections;
using Unity.Networking.Transport;
public class NetKeepAlive : NetMessage
{
    public NetKeepAlive() //constructor for making the message
    {
        code = OpCode.KEEP_ALIVE;
    }
    public NetKeepAlive(DataStreamReader reader)    //constructor for recieving the message
    {
        code = OpCode.KEEP_ALIVE;
        Deserialize(reader);
    }


    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        
    }

    public override void RecievedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }
}
