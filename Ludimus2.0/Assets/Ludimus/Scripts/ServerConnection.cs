using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ConnectionController;

public class ServerConnection : ControllerBase
{

    public delegate void WaitForGroupActionDel(string d);
    private Queue<Connection> playernameChangeQueue = new Queue<Connection>();
    private Dictionary<string, (int, WaitForGroupActionDel, List<Connection>, string)> groupActions = new Dictionary<string, (int, WaitForGroupActionDel, List<Connection>, string)>();


    void LateUpdate()
    {
        if (playernameChangeQueue.Count > 0)
        {
            var c = playernameChangeQueue.Dequeue();
            c.PlayerUI.UpdatePlayer(c.Playername);
        }
        var finished = groupActions.Where(a => a.Value.Item1 <= a.Value.Item3.Count).FirstOrDefault();
        if (finished.Key == null)
            return;
        finished.Value.Item2.Invoke(finished.Value.Item4);
        groupActions.Remove(finished.Key);
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

    public void MessageCallback(Data data, Connection connection)
    {
        if (groupActions.ContainsKey(data.Key))
        {
            groupActions[data.Key].Item3.Add(connection);
        }
        else
        {
            messageQueue.Enqueue((data, connection));
        }
    }

    private void PlayernameChangedCallback(Connection c)
    {
        playernameChangeQueue.Enqueue(c);
    }


    public void AddPlayerUIRef(PlayerUI playerUI, Connection c)
    {
        c.PlayerUI = playerUI;
    }

    public void WaitForGroupAction(string keyToLookOutFor, string value, WaitForGroupActionDel callback, int minPlayers = -1)
    {
        if (minPlayers < 0)
            minPlayers = ConnectionController.connectedClients.Count;
        Debug.Log("New Group action with " + minPlayers + " players to accept");
        if (groupActions.ContainsKey(keyToLookOutFor))
            groupActions.Remove(keyToLookOutFor);
        groupActions.Add(keyToLookOutFor, (minPlayers, callback, new List<Connection>(), value));
    }
}