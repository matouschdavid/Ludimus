using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ConnectionController;

public class ServerConnection : ControllerBase
{

    public delegate void WaitForGroupActionDel(Data d);
    private Queue<Connection> playernameChangeQueue = new Queue<Connection>();

    private Dictionary<string, (int, WaitForGroupActionDel, List<Connection>, Data)> groupActions = new Dictionary<string, (int, WaitForGroupActionDel, List<Connection>, Data)>();

    void LateUpdate()
    {
        if (playernameChangeQueue.Count > 0)
        {
            var c = playernameChangeQueue.Dequeue();
            c.PlayerUI.UpdatePlayer(c.Playername);
        }
        var finished = groupActions.Where(a => a.Value.Item1 >= a.Value.Item3.Count);
        foreach (var item in finished)
        {
            item.Value.Item2.Invoke(item.Value.Item4);
            groupActions.Remove(item.Key);
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

    private void MessageCallback(Data data, Connection connection)
    {
        if (groupActions.ContainsKey(data.Key))
        {
            groupActions[data.Key].Item3.Add(connection);
            groupActions[data.Key].Item4.Key = data.Key;
            groupActions[data.Key].Item4.Value = data.Value;
        }
        messageQueue.Enqueue((data, connection));
    }

    private void PlayernameChangedCallback(Connection c)
    {
        playernameChangeQueue.Enqueue(c);
    }


    public void AddPlayerUIRef(PlayerUI playerUI, Connection c)
    {
        c.PlayerUI = playerUI;
    }

    public void WaitForGroupAction(string keyToLookOutFor, WaitForGroupActionDel callback, int minPlayers = -1)
    {
        if (minPlayers < 0)
            minPlayers = ConnectionController.connectedClients.Count;
        groupActions.Add(keyToLookOutFor, (minPlayers, callback, new List<Connection>(), new Data()));
    }
}