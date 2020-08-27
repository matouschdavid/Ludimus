
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
    public class ServerConnectionDA : BaseConnectionDA
    {
        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static List<ClientConnectionBO> connectedClients = new List<ClientConnectionBO>();

        internal protected static void Teardown()
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
            }
        }

        public static bool Write<T>(DataBO<T> data)
        {
            return connectedClients.Select(c => Write(c, data)).All(res => res);
        }

        public static bool Write<T>(BaseConnectionBO connectionBO, DataBO<T> data)
        {
            return Write(data, connectionBO);
        }

        public static void StartServer()
        {
            onCrashDel += Teardown;
            isServer = true;
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

            ClientConnectionBO newClient = new ClientConnectionBO
            {
                Guid = Guid.NewGuid(),
                Socket = socket,
                Name = "Connecting..."
            };
            newClient.Socket.NoDelay = true;
            newClient.OnMessageReceived += messageReceivedDel;
            connectedClients.Add(newClient);
            playerConnectedDel?.Invoke(newClient);
            try
            {
                newClient.Socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, newClient);
            }
            catch (SocketException)
            {
                newClient.Socket.Close();
                connectedClients.Remove(newClient);
            }

            //Wait for next player
            server.BeginAccept(AcceptCallback, null);
        }
    }
}
