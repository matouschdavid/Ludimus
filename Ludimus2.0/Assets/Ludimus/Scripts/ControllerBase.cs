using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConnectionController;
using static ConnectionNew;

public class ControllerBase : MonoBehaviour
{
    private bool isServer = false;
    private Queue<Data> handleInputQ = new Queue<Data>();
    private Queue<ConnectionNew> newConnectionQ = new Queue<ConnectionNew>();
    private HandleInputDel privateMsgHandler;
    private HandleInputDel publicMsgHandler;
    private ConnectionControllerNew.NewConnectionDel connectedHandler;
    private NewConnectionDel newConnectionHandler;
    public delegate void NewConnectionDel(ConnectionNew connection, PlayerController player);
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

    private void NewConnectionQueue(ConnectionNew connection)
    {
        var p = Instantiate(player);
        p.GetComponent<PlayerController>().SetUp(connection, HandleInput);

        if (newConnectionHandler == null) return;

        newConnectionHandler.Invoke(connection, p.GetComponent<PlayerController>());
    }

    private void OnApplicationQuit()
    {
        ConnectionControllerNew.TeardownClient();
    }

    private void NewConnection(ConnectionNew connection)
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
            ConnectionControllerNew.Write(data);
        else
            ConnectionControllerNew.Write(data, ConnectionControllerNew.Client);
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

    public void AttachConnectedHandler(ConnectionControllerNew.NewConnectionDel handler)
    {
        connectedHandler += handler;
    }

    #region Client
    public void Connect(string playername)
    {
        isServer = false;
        ConnectionControllerNew.Connect(HandleInput, connectedHandler, playername);
    }
    #endregion
    #region Server
    public void StartServer(GameObject p)
    {
        isServer = true;
        player = p;
        ConnectionControllerNew.StartServer();
        ConnectionControllerNew.AttachConnectionHandler(NewConnection);
    }
    #endregion
}
