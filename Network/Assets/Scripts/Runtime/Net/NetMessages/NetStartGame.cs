using Unity.Collections;
using Unity.Networking.Transport;

public class NetStartGame : NetMessage
{
    public NetStartGame() //constructor for making the message
    {
        code = OpCode.START_GAME;
    }
    public NetStartGame(DataStreamReader reader)    //constructor for recieving the message
    {
        code = OpCode.START_GAME;
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
        NetUtility.C_START_GAME?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }
}