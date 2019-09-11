using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConnectionController;

public class ControllerBase : MonoBehaviour
{
    protected NewConnectionDel newConnectionHandler;
    protected MessageDel messageHandler;
    protected Queue<Connection> connectionQueue = new Queue<Connection>();
    protected Queue<(Data, Connection)> messageQueue = new Queue<(Data, Connection)>();
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (connectionQueue.Count > 0)
        {
            newConnectionHandler.Invoke(connectionQueue.Dequeue());
        }
        if (messageQueue.Count > 0)
        {
            var d = messageQueue.Dequeue();
            messageHandler.Invoke(d.Item1, d.Item2);
        }
    }

    public void AttachNewConnectionHandler(NewConnectionDel callback)
    {
        newConnectionHandler += callback;
    }
    public void DetachNewConnectionHandler(NewConnectionDel callback)
    {
        newConnectionHandler -= callback;
    }
    public void AttachMessageHandler(MessageDel callback)
    {
        messageHandler += callback;
    }

    public void DetachMessageHandler(MessageDel callback)
    {
        messageHandler -= callback;
    }
}
