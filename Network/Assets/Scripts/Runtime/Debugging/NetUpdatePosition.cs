using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetUpdatePosition : MonoBehaviour
{
    private float lastSend;
    private BaseServer Server;

    private void Start()
    {
        Server = FindObjectOfType<BaseServer>();
        if (Server == null)
        {
            Debug.LogWarning("No Server object was found. Updates will 'NOT' be broadcasted");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Time.time - lastSend > 1.0f)
        {
            Vector3 pos = transform.position;
            Net_PlayerPosition ps = new Net_PlayerPosition(1, pos.x, pos.y, pos.z);
            Server.Broadcast(ps);
            lastSend = Time.time;
        }
    }
}
