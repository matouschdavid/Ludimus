using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using static ConnectionNew;

public static class ConnectionControllerNew
{
    public static bool IsServer = false;
    private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    // private static Thread waitForPlayers;
    public static ConnectionNew Client;
    public static List<ConnectionNew> connectedClients;
    private static int currClientId = 0;

    private static string serverIp = "192.168.0.159";

    private const int BUFFER_SIZE = 255;
    private static readonly byte[] buffer = new byte[BUFFER_SIZE];


    public delegate void NewConnectionDel(ConnectionNew connection);


    private static NewConnectionDel handleNewConnection;

    public static void StartServer()
    {
        server.Bind(new IPEndPoint(IPAddress.Any, 8080));
        server.Listen(0);
        server.BeginAccept(AcceptCallback, null);
    }

    private static void AcceptCallback(IAsyncResult ar)
    {
        Debug.Log("Accept new client");
        Socket socket;
        try
        {
            socket = server.EndAccept(ar);
        }
        catch (ObjectDisposedException)
        {
            Debug.LogError("Failed establishing connection");
            return;
        }

        ConnectionNew conn = new ConnectionNew
        {
            Client = socket,
            ClientId = currClientId
        };
        connectedClients.Add(conn);
        conn.Client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, socket);

        //Wait for next player
        server.BeginAccept(AcceptCallback, null);
    }

    private static void ReceiveCallBack(IAsyncResult ar)
    {
        Socket current = (Socket)ar.AsyncState;
        int received = 0;
        var conn = connectedClients.Find(c => c.Client == current);
        try
        {
            received = current.EndReceive(ar);
        }
        catch (SocketException)
        {
            Debug.LogError("Client disconnected");
            current.Close();
            connectedClients.Remove(conn);
            return;
        }

        byte[] recBuf = new byte[received];
        Array.Copy(buffer, recBuf, received);
        Data d = Data.ConvertToData(System.Text.Encoding.ASCII.GetString(recBuf));
        Debug.Log("Received text: " + d.ToString());
        if (d.Key == "Playername")
        {
            //NewPlayer(d.Value, conn);
        }
        if (conn.HandleInput == null) return;
        Debug.Log("Here");
        conn.HandleInput.Invoke(d);

        //Wait for next Message
        current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, current);
    }

    public static void TeardownServer()
    {
        foreach (ConnectionNew socket in connectedClients)
        {
            socket.Client.Shutdown(SocketShutdown.Both);
            socket.Client.Close();
        }

        server.Close();
    }

    public static void TeardownClient()
    {
        Client.Client.Close();
    }

    internal static void AttachConnectionHandler(NewConnectionDel handler)
    {
        handleNewConnection += handler;
    }

    public static void Connect(HandleInputDel handleInput, NewConnectionDel newConnectionHandler, string playername)
    {
        Thread t = new Thread(new ParameterizedThreadStart(LookForConnection));
        List<object> p = new List<object> { handleInput, newConnectionHandler };
        t.Start(p);

    }

    private static void LookForConnection(object obj)
    {
        var p = (List<object>)obj;
        var handleInput = (HandleInputDel)p[0];
        var newConnectionHandler = (NewConnectionDel)p[1];
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        int attempts = 0;
        while (!client.Connected)
        {
            try
            {
                attempts++;
                client.Connect(serverIp, 8080);
            }
            catch (SocketException)
            {
                Debug.LogError("Failed to connect at attempt: " + attempts);
            }
        }
        Debug.Log("Connected");
        Client = new ConnectionNew
        {
            Client = client,
            ClientId = 0,
            HandleInput = handleInput
        };
        client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, client);
        newConnectionHandler.Invoke(Client);
    }

    public static void AttachMsgHandler(ConnectionNew.HandleInputDel handler)
    {
        Client.HandleInput += handler;
    }

    public static void Write(Data data, ConnectionNew c)
    {
        var bytes = System.Text.Encoding.ASCII.GetBytes(Data.ConvertFromData(data));

        c.Client.Send(bytes, 0, bytes.Length, SocketFlags.None);
    }

    public static void Write(Data data)
    {
        connectedClients.ForEach(c => Write(data, c));
    }

    // private static void NewPlayer(string value, ConnectionNew c)
    // {
    //     c.Playername = value;
    //     handleNewConnection.Invoke(c);
    //     connectedClients.Add(c);
    //     currClientId++;
    // }
}

public class ConnectionNew
{
    public delegate void HandleInputDel(Data data);


    public int ClientId { get; set; }
    public string Playername { get; set; }
    public Socket Client { get; set; }
    public HandleInputDel HandleInput;
}