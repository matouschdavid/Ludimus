using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Connection;
using static ConnectionController;

public class ControllerBase : MonoBehaviour
{
    private bool isServer = false;
    private Queue<Data> handleInputQ = new Queue<Data>();
    private Queue<Connection> newConnectionQ = new Queue<Connection>();
    private HandleInputDel privateMsgHandler;
    private HandleInputDel publicMsgHandler;
    private ConnectionController.NewConnectionDel connectedHandler;
    private NewConnectionDel newConnectionHandler;
    public delegate void NewConnectionDel(Connection connection, PlayerController player);
    private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && newConnectionQ.Count > 0)
        {
            NewConnectionQueue(newConnectionQ.Dequeue());
        }
        if (handleInputQ.Count > 0)
        {
            HandleInputQueue(handleInputQ.Dequeue());
        }
    }

    private void NewConnectionQueue(Connection connection)
    {
        var p = Instantiate(player);
        p.GetComponent<PlayerController>().SetUp(connection, HandleInput);

        if (newConnectionHandler == null) return;

        newConnectionHandler.Invoke(connection, p.GetComponent<PlayerController>());
    }

    private void OnApplicationQuit()
    {
        ConnectionController.TeardownClient();
    }

    private void NewConnection(Connection connection)
    {
        newConnectionQ.Enqueue(connection);
    }

    private void HandleInput(Data data)
    {
        handleInputQ.Enqueue(data);
    }

    private void HandleInputQueue(Data data)
    {
        if (isServer)
        {
            if (publicMsgHandler == null)
                return;
            publicMsgHandler.Invoke(data);
        }
        else
        {
            if (privateMsgHandler == null)
                return;
            privateMsgHandler.Invoke(data);
        }
    }

    public void Write(Data data)
    {
        if (isServer)
            ConnectionController.Write(data);
        else
            ConnectionController.Write(data, ConnectionController.Client);
    }

    public void AttachMsgHandler(HandleInputDel handler)
    {
        if (isServer)
            publicMsgHandler += handler;
        else
            privateMsgHandler += handler;
    }

    public void AttachConnectedHandler(NewConnectionDel handler)
    {
        newConnectionHandler += handler;
    }

    public void AttachConnectedHandler(ConnectionController.NewConnectionDel handler)
    {
        connectedHandler += handler;
    }

    #region Client
    public void Connect(string playername)
    {
        isServer = false;
        ConnectionController.Connect(HandleInput, connectedHandler, playername);
    }
    #endregion
    #region Server
    public void StartServer(GameObject p)
    {
        isServer = true;
        player = p;
        ConnectionController.StartServer();
        ConnectionController.AttachConnectionHandler(NewConnection);
    }
    #endregion
}
