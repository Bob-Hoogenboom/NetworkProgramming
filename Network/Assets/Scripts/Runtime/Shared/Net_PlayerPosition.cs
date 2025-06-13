using Unity.Collections;
using UnityEngine;

public class Net_PlayerPosition : NetMessage
{
    // 0 - 8 OP CODE
    public int playerID { set; get; }

    public float positionX { set; get; }
    public float positionY { set; get; }
    public float positionZ { set; get; }

    //every net message have the base constructor and 'reader' constructor
    public Net_PlayerPosition()
    {
        code = OpCode.PLAYER_POSITION;
    }
    public Net_PlayerPosition(DataStreamReader reader)
    {
        code = OpCode.PLAYER_POSITION;
        Deserialize(reader);
    }

    public Net_PlayerPosition(int playerId, float x, float y, float z)
    {
        code = OpCode.PLAYER_POSITION;
        playerID = playerId;

        positionX = x;
        positionY = y;
        positionZ = z;

    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteInt(playerID);
        writer.WriteFloat(positionX);
        writer.WriteFloat(positionY);
        writer.WriteFloat(positionZ);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        // The first bite is handled by another function in the baseClient
        playerID = reader.ReadInt();
        positionX = reader.ReadFloat();
        positionY = reader.ReadFloat();
        positionZ = reader.ReadFloat();
    }

    public override void RecievedOnServer(BaseServer server)
    {
        Debug.Log($"SERVER:: {playerID} ::Pos {positionX}, {positionY}, {positionZ}");
        server.Broadcast(this);
    }

    public override void RecievedOnClient()
    {
        Debug.Log($"CLIENT:: {playerID} ::Pos {positionX}, {positionY}, {positionZ}");

    }
}
