using Unity.Collections;
using Unity.Networking.Transport;

public class NetMakeMove : NetMessage
{
    public int originalX;
    public int originalY;
    public int destinationX;
    public int destinationY;
    public int teamId;

    public NetMakeMove() //constructor for making the message
    {
        code = OpCode.MAKE_MOVE;
    }
    public NetMakeMove(DataStreamReader reader)    //constructor for recieving the message
    {
        code = OpCode.MAKE_MOVE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);

        writer.WriteInt(originalX);
        writer.WriteInt(originalY);
        writer.WriteInt(destinationX);
        writer.WriteInt(destinationY);
        writer.WriteInt(teamId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        originalX = reader.ReadInt();
        originalY = reader.ReadInt();
        destinationX = reader.ReadInt();
        destinationY = reader.ReadInt();
        teamId = reader.ReadInt();
    }

    public override void RecievedOnClient()
    {
        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
    }
}