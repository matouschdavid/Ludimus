using LudimusConnection.BusinessObjects.Client;
using LudimusConnection.BusinessObjects.General;
using LudimusConnection.DataAccess.General;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace LudimusConnection.DataAccess.Client
{
    public class ClientConnectionDA : BaseConnectionDA
    {

        internal protected static void Teardown()
        {
            self.Socket.Close();
        }

        public static bool Write<T>(DataBO<T> data)
        {
            return Write<T>(data, self);
        }

        public static void Connect(string playername, string serverIp, ConnectionChangeHandler onConnected)
        {
            onCrashDel += Teardown;
            Thread t = new Thread(new ParameterizedThreadStart(LookForConnection));
            List<object> p = new List<object> { playername, serverIp, onConnected };
            t.Start(p);

        }

        private static void LookForConnection(object obj)
        {
            var p = (List<object>)obj;
            var playername = (string)p[0];
            var serverIp = (string)p[1];
            var onConnected = (ConnectionChangeHandler)p[2];
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int attempts = 0;
            while (!client.Connected)
            {
                try
                {
                    attempts++;
                    client.Connect(serverIp, 8080);
                }
                catch (SocketException) { }
            }
            self = new ClientConnectionBO
            {
                Socket = client,
                Guid = Guid.Empty,
                Name = playername
            };
            self.Socket.NoDelay = true;
            Write(new DataBO<string>(LudimusConnectionKeys.CHANGE_PLAYER_NAME, self.Name));
            client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, self);
            onConnected.Invoke(self);
        }
    }
}
