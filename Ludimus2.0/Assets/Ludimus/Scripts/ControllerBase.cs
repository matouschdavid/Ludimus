using System.Collections.Generic;
using UnityEngine;
using static ConnectionController;

public class ControllerBase : MonoBehaviour
{
    protected NewConnectionDel newConnectionHandler;
    protected MessageDel messageHandler;
    public delegate void PauseDel(bool isPaused);

    protected Queue<Connection> connectionQueue = new Queue<Connection>();
    protected Queue<(Data, Connection)> messageQueue = new Queue<(Data, Connection)>();
    private PauseDel pauseHandler;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void FixedUpdate()
    {
        if (connectionQueue.Count > 0)
        {
            newConnectionHandler.Invoke(connectionQueue.Dequeue());
        }

        if (messageQueue.Count > 0)
        {
            var d = messageQueue.Dequeue();
            if (d.Item1.Key == "StartPause")
                pauseHandler(true);
            else if (d.Item1.Key == "EndPause")
                pauseHandler(false);
            messageHandler.Invoke(d.Item1, d.Item2);
        }
    }





    public void AttachPauseHandler(PauseDel callback)
    {
        pauseHandler += callback;
    }

    public void DetachPauseHandler(PauseDel callback)
    {
        pauseHandler -= callback;
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
