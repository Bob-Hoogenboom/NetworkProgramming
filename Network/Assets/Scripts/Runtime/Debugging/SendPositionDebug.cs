using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPositionDebug : MonoBehaviour
{
    private float lastSend;
    private BaseClient client;

    private void Start()
    {
        client = FindObjectOfType<BaseClient>();
    }

    private void Update()
    {
        if(Time.time - lastSend > 1.0f)
        {
            Vector3 pos = transform.position;
            Net_PlayerPosition ps = new Net_PlayerPosition(1, pos.x, pos.y, pos.z);
            client.SendToServer(ps);
            lastSend = Time.time;
        }
    }
}
