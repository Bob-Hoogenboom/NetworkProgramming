using Unity.Collections;
using Unity.Networking.Transport;

public class NetRematch : NetMessage
{
    public int teamId;
    public byte wantRematch;

    public NetRematch() //constructor for making the message
    {
        code = OpCode.REMATCH;
    }
    public NetRematch(DataStreamReader reader)    //constructor for recieving the message
    {
        code = OpCode.REMATCH;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);

        writer.WriteInt(teamId);
        writer.WriteByte(wantRematch);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        teamId = reader.ReadInt();
        wantRematch = reader.ReadByte();
    }

    public override void RecievedOnClient()
    {
        NetUtility.C_REMATCH?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
}
