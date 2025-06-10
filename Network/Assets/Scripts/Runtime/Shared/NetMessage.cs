using Unity.Collections;


public class NetMessage
{
    public OpCode code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {

    }

    public virtual void Deserialize(DataStreamReader reader)
    {

    }
    public virtual void RecievedOnServer(BaseServer server)
    {

    }

    public virtual void RecievedOnClient()
    {

    }
}
