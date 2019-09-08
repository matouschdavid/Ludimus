using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    // public static bool IsServer = false;
    // private static TcpListener server;
    // private static Thread waitForPlayers;
    // public static Connection Client;
    // public static List<Connection> connectedClients;
    // private static int currClientId = 0;

    // private static string serverIp = "192.168.0.159";


    // public delegate void NewConnectionDel(Connection connection);


    // private static NewConnectionDel handleNewConnection;

    // public static void StartServer()
    // {
    //     server = new TcpListener(IPAddress.Parse(serverIp), 8080);
    //     connectedClients = new List<Connection>();
    //     IsServer = true;
    //     server.Start();
    //     Debug.Log("Server started");
    //     waitForPlayers = new Thread(new ParameterizedThreadStart(WaitForPlayers));
    //     waitForPlayers.Start(server);
    // }

    // public static void TeardownServer()
    // {
    //     waitForPlayers.Interrupt();
    //     server.Stop();
    // }

    // public static void TeardownClient()
    // {
    //     Client.Loop.Interrupt();
    //     Client.Client.Close();
    // }

    // private static void WaitForPlayers(object s)
    // {
    //     TcpListener server = s as TcpListener;
    //     currClientId = 0;
    //     do
    //     {
    //         TcpClient c = server.AcceptTcpClient();
    //         Connection conn = new Connection
    //         {
    //             Client = c,
    //             Stream = c.GetStream(),
    //             ClientId = currClientId
    //         };
    //         Thread t = new Thread(new ParameterizedThreadStart(CheckForData));
    //         conn.Loop = t;
    //         t.Start(conn);
    //         Debug.Log("A client connected.");
    //     } while (connectedClients.Count < 8);
    // }

    // internal static void AttachConnectionHandler(NewConnectionDel handler)
    // {
    //     handleNewConnection += handler;
    // }

    // public static void Connect(HandleInputDel handleInput, NewConnectionDel newConnectionHandler, string playername)
    // {
    //     TcpClient c = new TcpClient(serverIp, 8080);
    //     Client = new Connection
    //     {
    //         Client = c,
    //         ClientId = 0,
    //         Stream = c.GetStream()
    //     };
    //     Write(new Data
    //     {
    //         Key = "Playername",
    //         Value = playername,
    //         Region = "public"
    //     }, Client);
    //     Client.HandleInput = handleInput;
    //     StartListening();
    //     newConnectionHandler.Invoke(Client);
    // }

    // public static void AttachMsgHandler(Connection.HandleInputDel handler)
    // {
    //     Client.HandleInput += handler;
    // }

    // private static void StartListening()
    // {
    //     Thread t = new Thread(new ParameterizedThreadStart(CheckForData));
    //     Client.Loop = t;
    //     t.Start(Client);
    // }

    // public static void Write(Data data, Connection c)
    // {
    //     var bytes = System.Text.Encoding.ASCII.GetBytes(Data.ConvertFromData(data));

    //     c.Stream.Write(bytes, 0, bytes.Length);
    // }

    // public static void Write(Data data)
    // {
    //     connectedClients.ForEach(c => Write(data, c));
    // }

    // private static void CheckForData(object c)
    // {
    //     Connection conn = c as Connection;
    //     while (true)
    //     {
    //         while (conn.Client.Available < 255) ;
    //         Data d = Read(conn);
    //         if (d.Key == "Playername")
    //         {
    //             NewPlayer(d.Value, conn);
    //         }
    //         Debug.Log(d);
    //         if (conn.HandleInput == null) return;
    //         Debug.Log("Here");
    //         conn.HandleInput.Invoke(d);
    //     }
    // }

    // private static void NewPlayer(string value, Connection c)
    // {
    //     c.Playername = value;
    //     handleNewConnection.Invoke(c);
    //     connectedClients.Add(c);
    //     currClientId++;
    // }

    // public static Data Read(Connection c)
    // {
    //     byte[] bytes = new byte[255];
    //     c.Stream.Read(bytes, 0, bytes.Length);
    //     return Data.ConvertToData(System.Text.Encoding.ASCII.GetString(bytes));
    // }
}

public class Data
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Region { get; set; }

    public static Data ConvertToData(string data)
    {
        string[] split = data.Split('|');
        return new Data
        {
            Key = split[0],
            Value = split[1],
            Region = split[2]
        };
    }

    public static string ConvertFromData(Data data)
    {
        string str = data.Key + "|" + data.Value + "|" + data.Region + "|";

        for (int i = str.Length; i < 255; i++)
        {
            str += " ";
        }
        Debug.Log(str.Length);
        return str;
    }

    public override string ToString()
    {
        return Region + ":  " + Key + " | " + Value;
    }
}

// public class Connection
// {
//     public delegate void HandleInputDel(Data data);


//     public int ClientId { get; set; }
//     public string Playername { get; set; }
//     public NetworkStream Stream { get; set; }
//     public TcpClient Client { get; set; }
//     public bool DataAvailable { get { return Stream.DataAvailable; } }
//     public Thread Loop;
//     public HandleInputDel HandleInput;
// }
