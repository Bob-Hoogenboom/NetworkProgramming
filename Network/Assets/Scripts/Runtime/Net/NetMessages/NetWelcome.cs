using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetWelcome : NetMessage
{
    public int assignedTeam { set; get; }

    public NetWelcome() //constructor for making the message
    {
        code = OpCode.WELCOME;
    }
    public NetWelcome(DataStreamReader reader)    //constructor for recieving the message
    {
        code = OpCode.WELCOME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteInt(assignedTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        assignedTeam = reader.ReadInt();
    }

    public override void RecievedOnClient()
    {
        NetUtility.C_WELCOME?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
