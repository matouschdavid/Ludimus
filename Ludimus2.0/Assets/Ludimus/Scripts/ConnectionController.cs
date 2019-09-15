using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public static class ConnectionController
{
    public static bool IsServer = false;
    private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    // private static Thread waitForPlayers;
    public static Connection Client;
    public static List<Connection> connectedClients = new List<Connection>();
    private static int currClientId = 0;

    private const int BUFFER_SIZE = 255;
    private static readonly byte[] buffer = new byte[BUFFER_SIZE];


    public delegate void NewConnectionDel(Connection connection);
    public delegate void MessageDel(Data data, Connection connection);
    private static MessageDel messageDel;


    private static NewConnectionDel handleNewConnection;
    private static NewConnectionDel playernameChangedHandler;

    public static void StartServer(MessageDel m, NewConnectionDel connectedCallback, NewConnectionDel playernameChangedCallback)
    {
        IsServer = true;
        messageDel = m;
        handleNewConnection = connectedCallback;
        playernameChangedHandler = playernameChangedCallback;
        server.Bind(new IPEndPoint(IPAddress.Any, 8080));
        server.NoDelay = true;
        server.Listen(0);
        server.BeginAccept(AcceptCallback, null);
    }

    public static T GetControllerInstance<T>()
    {
        return GameObject.Find("Controller").GetComponent<T>();
    }

    private static void AcceptCallback(IAsyncResult ar)
    {
        Socket socket;
        try
        {
            socket = server.EndAccept(ar);
        }
        catch (ObjectDisposedException)
        {
            Debug.LogError("Error when connecting");
            return;
        }

        Connection conn = new Connection
        {
            Client = socket,
            ClientId = currClientId,
            Playername = ""
        };
        conn.HandleInput = messageDel;
        connectedClients.Add(conn);
        conn.Client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, socket);

        //Wait for next player
        server.BeginAccept(AcceptCallback, null);
    }

    private static void ReceiveCallBack(IAsyncResult ar)
    {
        Socket current = (Socket)ar.AsyncState;
        int received = 0;
        Connection conn;
        if (IsServer)
            conn = connectedClients.Find(c => c.Client == current);
        else
            conn = Client;
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
        // Console.WriteLine("Received text: " + d.ToString());
        if (d.Key == "Playername")
        {
            Debug.Log("Got playername: " + d.Value);
            if (conn.Playername == "")
            {
                Debug.Log("New player");
                conn.Playername = d.Value;
                handleNewConnection.Invoke(conn);
            }
            else
            {
                Debug.Log("Playername change");
                conn.Playername = d.Value;
                playernameChangedHandler.Invoke(conn);
            }
            //NewPlayer(d.Value, conn);
        }
        if (conn.HandleInput == null) return;
        conn.HandleInput.Invoke(d, conn);

        //Wait for next Message
        current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, current);
    }

    public static void TeardownServer()
    {
        foreach (Connection socket in connectedClients)
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

    public static void Connect(string ip, MessageDel handleInput, NewConnectionDel newConnectionHandler, string playername)
    {
        IsServer = false;
        Thread t = new Thread(new ParameterizedThreadStart(LookForConnection));
        List<object> p = new List<object> { handleInput, newConnectionHandler, playername, ip };
        t.Start(p);

    }

    private static void LookForConnection(object obj)
    {
        var p = (List<object>)obj;
        var handleInput = (MessageDel)p[0];
        var newConnectionHandler = (NewConnectionDel)p[1];
        var playername = (string)p[2];
        var ip = (string)p[3];
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        int attempts = 0;
        while (!client.Connected)
        {
            try
            {
                attempts++;
                client.Connect(ip, 8080);
            }
            catch (SocketException)
            {
                Console.WriteLine("Failed to connect at attempt: " + attempts);
            }
        }
        Client = new Connection
        {
            Client = client,
            ClientId = 0,
            HandleInput = handleInput,
            Playername = playername
        };
        Write(new Data { Key = "Playername", Value = playername }, Client);
        Debug.Log("Wrote playername: " + playername);
        client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, client);
        newConnectionHandler.Invoke(Client);
    }

    public static void AttachMsgHandler(MessageDel handler)
    {
        Client.HandleInput += handler;
    }

    public static void Write(Data data, Connection c)
    {
        Debug.Log("Write...");
        var bytes = System.Text.Encoding.ASCII.GetBytes(Data.ConvertFromData(data));
        c.Client.Send(bytes, 0, bytes.Length, SocketFlags.None);
    }

    public static void Write(Data data)
    {
        if (IsServer)
            connectedClients.ForEach(c => Write(data, c));
        else
            Write(data, Client);
    }

    public static void Write(string key, string value, string region = "")
    {
        Write(new Data { Key = key, Value = value, Region = region });
    }
    public static void Write(Connection c, string key, string value, string region = "")
    {
        Write(new Data { Key = key, Value = value, Region = region }, c);
    }
}