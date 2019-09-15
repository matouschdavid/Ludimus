using System.Collections.Generic;
using UnityEngine;
using static ConnectionController;

public class ServerConnection : ControllerBase
{


    private Queue<Connection> playernameChangeQueue = new Queue<Connection>();



    void LateUpdate()
    {
        if (playernameChangeQueue.Count > 0)
        {
            var c = playernameChangeQueue.Dequeue();
            c.PlayerUI.UpdatePlayer(c.Playername);
        }

    }
    public void StartServer(MessageDel MessageCallback, NewConnectionDel ConnectedCallback)
    {
        Debug.Log("Starting server");
        AttachMessageHandler(MessageCallback);
        AttachNewConnectionHandler(ConnectedCallback);
        ConnectionController.StartServer(this.MessageCallback, this.ConnectedCallback, PlayernameChangedCallback);
    }

    private void ConnectedCallback(Connection connection)
    {
        connectionQueue.Enqueue(connection);
    }



    private void PlayernameChangedCallback(Connection c)
    {
        playernameChangeQueue.Enqueue(c);
    }


    public void AddPlayerUIRef(PlayerUI playerUI, Connection c)
    {
        c.PlayerUI = playerUI;
    }


}