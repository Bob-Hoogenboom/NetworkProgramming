using Unity.Collections;
using Unity.Networking.Transport;

public class NetMessage
{
    public OpCode code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
    }

    public virtual void Deserialize(DataStreamReader reader)
    {

    }

    public virtual void RecievedOnClient()
    {

    }

    public virtual void RecievedOnServer(NetworkConnection cnn)
    {

    }
}
