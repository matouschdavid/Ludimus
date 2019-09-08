using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LudimusServerDev.Connection;

namespace LudimusServerDev
{
    public static class ConnectionController
    {
        public static bool IsServer = false;
        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // private static Thread waitForPlayers;
        public static Connection Client;
        public static List<Connection> connectedClients = new List<Connection>();
        private static int currClientId = 0;

        private static string serverIp = "192.168.0.159";

        private const int BUFFER_SIZE = 255;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];


        public delegate void NewConnectionDel(Connection connection);


        private static NewConnectionDel handleNewConnection;

        public static void StartServer()
        {
            
            server.Bind(new IPEndPoint(IPAddress.Any, 8080));
            server.Listen(0);
            server.BeginAccept(AcceptCallback, null);
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
                Console.WriteLine("Error when connecting");
                return;
            }

            Connection conn = new Connection
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
                Console.WriteLine("Client disconnected");
                current.Close();
                connectedClients.Remove(conn);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            Data d = Data.ConvertToData(System.Text.Encoding.ASCII.GetString(recBuf));
            Console.WriteLine("Received text: " + d.ToString());
            if (d.Key == "Playername")
            {
                //NewPlayer(d.Value, conn);
            }
            if (conn.HandleInput == null) return;
            conn.HandleInput.Invoke(d);

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
                    Console.WriteLine("Failed to connect at attempt: " + attempts);
                }
            }
            Client = new Connection
            {
                Client = client,
                ClientId = 0,
                HandleInput = handleInput
            };
            client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, client);
            newConnectionHandler.Invoke(Client);
        }

        public static void AttachMsgHandler(Connection.HandleInputDel handler)
        {
            Client.HandleInput += handler;
        }

        public static void Write(Data data, Connection c)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(Data.ConvertFromData(data));

            c.Client.Send(bytes, 0, bytes.Length, SocketFlags.None);
        }

        public static void Write(Data data)
        {
            connectedClients.ForEach(c => Write(data, c));
        }
    }
}
