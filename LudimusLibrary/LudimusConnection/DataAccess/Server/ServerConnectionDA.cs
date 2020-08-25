
using LudimusConnection.BusinessObjects.Client;
using LudimusConnection.BusinessObjects.General;
using LudimusConnection.DataAccess.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace LudimusConnection.DataAccess.Server
{
    class ServerConnectionDA<T> : BaseConnectionDA<T>
    {
        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static List<ClientConnectionBO<T>> connectedClients = new List<ClientConnectionBO<T>>();

        private const int BUFFER_SIZE = 255;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];


        public void OnMessageReceived(DataBO<T> data, BaseConnectionBO connectionBO)
        {
            throw new System.NotImplementedException();
        }

        internal protected static bool Teardown()
        {
            try
            {
                foreach (BaseConnectionBO socket in connectedClients)
                {
                    socket.Socket.Shutdown(SocketShutdown.Both);
                    socket.Socket.Close();
                }

                server.Close();
            }catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool Write(DataBO<T> data)
        {
            return connectedClients.Select(c => Write(data, c)).All(res => res);
        }

        public bool Write(DataBO<T> data, BaseConnectionBO connectionBO)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(data.GetValueAsJson());

            return connectionBO.Socket.Send(bytes, 0, bytes.Length, SocketFlags.None) == bytes.Length;
        }

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

            ClientConnectionBO<T> newClient = new ClientConnectionBO<T>
            {
                Guid = Guid.NewGuid(),
                Socket = socket,
                Name = "Connecting..."
            };
            newClient.OnMessageReceived = messageReceivedDel;
            connectedClients.Add(newClient);
            newClient.Socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, newClient);

            //Wait for next player
            server.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallBack(IAsyncResult ar)
        {
            ClientConnectionBO<T> client = (ClientConnectionBO<T>) ar.AsyncState;
            int received = 0;
            try
            {
                received = client.Socket.EndReceive(ar);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client disconnected");
                client.Socket.Close();
                connectedClients.Remove(client);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            DataBO<T> d = new DataBO<T>(System.Text.Encoding.ASCII.GetString(recBuf));
            Console.WriteLine("Received text: " + d.ToString());
            if (d.Key == LudimusConnectionKeys.CHANGE_PLAYER_NAME)
            {
                client.Name = d.Value.ToString();
            }
            if (client.OnMessageReceived == null) return;
            client.OnMessageReceived.Invoke(d, client);

            //Wait for next Message
            client.Socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, client);
        }
    }
}
