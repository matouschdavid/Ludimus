using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static ConnectionController;

public class ClientConnection : ControllerBase
{
    public void Connect(string lobbycode, string name, MessageDel ReceiveCallback, NewConnectionDel ConnectedCallback)
    {
        Debug.Log("Start connecting");
        messageHandler += ReceiveCallback;
        newConnectionHandler += ConnectedCallback;
        ConnectionController.Connect(DecryptLobbyCode(lobbycode), this.MessageCallback, this.ConnectedCallback, name);
    }

    private string DecryptLobbyCode(string code)
    {
        var b = Convert.FromBase64String(code + "==");
        var a = new IPAddress(b);
        return a.ToString();
    }

    private void ConnectedCallback(Connection connection)
    {
        Debug.Log("Connected");
        connectionQueue.Enqueue(connection);
    }

    private void MessageCallback(Data data, Connection connection)
    {
        messageQueue.Enqueue((data, connection));
    }

    private void PlayernameChangedCallback(Connection c)
    {
        c.PlayerUI.UpdatePlayer(c.Playername);
    }
}