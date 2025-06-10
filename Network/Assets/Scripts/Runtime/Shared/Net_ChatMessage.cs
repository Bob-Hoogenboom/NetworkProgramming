using Unity.Collections;
using UnityEngine;


public class Net_ChatMessage : NetMessage
{
    // 0 - 8 OP CODE
    public FixedString512Bytes chatMessage { set; get; }


    public Net_ChatMessage() 
    {
        code = OpCode.CHAT_MESSAGE;
    }
    public Net_ChatMessage(DataStreamReader reader)
    {
        code = OpCode.CHAT_MESSAGE;
        Deserialize(reader);
    }
    public Net_ChatMessage(string msg)
    {
        code = OpCode.CHAT_MESSAGE;
        chatMessage = msg;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteFixedString512(chatMessage);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        // The first bite is handled by another function in the baseClient
        chatMessage = reader.ReadFixedString512();
    }

    public override void RecievedOnServer(BaseServer server)
    {
        Debug.Log("SERVER:: " + chatMessage);
        server.Broadcast(this);
    }

    public override void RecievedOnClient()
    {
        Debug.Log("CLIENT:: " + chatMessage);
    }
}
