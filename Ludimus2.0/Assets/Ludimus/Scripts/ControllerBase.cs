using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConnectionController;

public class ControllerBase : MonoBehaviour
{
    protected NewConnectionDel newConnectionHandler;
    protected MessageDel messageHandler;
    public delegate void WaitForGroupActionDel(string d);
    protected Queue<Connection> connectionQueue = new Queue<Connection>();
    protected Queue<(Data, Connection)> messageQueue = new Queue<(Data, Connection)>();
    private Dictionary<string, (int, WaitForGroupActionDel, List<Connection>, string)> groupActions = new Dictionary<string, (int, WaitForGroupActionDel, List<Connection>, string)>();
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
        var finished = groupActions.Where(a => a.Value.Item1 <= a.Value.Item3.Count);
        foreach (var item in finished)
        {
            item.Value.Item2.Invoke(item.Value.Item4);
            groupActions.Remove(item.Key);
            break;
        }
        if (messageQueue.Count > 0)
        {
            var d = messageQueue.Dequeue();
            if (d.Item1.Key == "StartPause")
            {
                if (IsServer)
                {
                    SceneManager.LoadScene("PauseOverlay_Server", LoadSceneMode.Additive);

                    ConnectionController.Write("StartPause", "");
                    WaitForGroupAction("EndPause", "", AllClientsWantToResume);
                    // Time.timeScale = 0;
                }
            }
            messageHandler.Invoke(d.Item1, d.Item2);
        }
    }

    private void AllClientsWantToResume(string d)
    {
        Debug.Log("All clients want to resume");
        SceneManager.UnloadSceneAsync("PauseOverlay_Server");
        ConnectionController.Write("EndPause", "");
    }

    protected void MessageCallback(Data data, Connection connection)
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

    public void WaitForGroupAction(string keyToLookOutFor, string value, WaitForGroupActionDel callback, int minPlayers = -1)
    {
        if (minPlayers < 0)
            minPlayers = ConnectionController.connectedClients.Count;
        Debug.Log("New Group action with " + minPlayers + " players to accept");
        groupActions.Add(keyToLookOutFor, (minPlayers, callback, new List<Connection>(), value));
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
