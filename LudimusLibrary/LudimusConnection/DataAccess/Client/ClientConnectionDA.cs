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
        private void OnMessageReceived<T>(DataBO<T> data)
        {
            messageReceivedDel.Invoke(data, self);
        }

        internal protected static bool Teardown()
        {
            self.Socket.Close();
            return true;
        }

        public static bool Write<T>(DataBO<T> data)
        {
            return Write<T>(data, self);
        }

        public static void Connect<T>(string playername, string serverIp, MessageReceivedDel<T> onMessageReceived, ConnectionChangeHandler onConnected)
        {

            Thread t = new Thread(new ParameterizedThreadStart(LookForConnection));
            List<object> p = new List<object> { playername, serverIp, onMessageReceived, onConnected };
            t.Start(p);

        }

        private static void LookForConnection(object obj)
        {
            var p = (List<object>)obj;
            var playername = (string)p[0];
            var serverIp = (string)p[1];
            var onMessageReceived = (MessageReceivedDel<object>)p[2];
            var onConnected = (ConnectionChangeHandler)p[3];
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
                OnMessageReceived = onMessageReceived,
                Name = playername
            };
            Write<string>(new DataBO<string>(LudimusConnectionKeys.CHANGE_PLAYER_NAME, self.Name));
            try
            {
                client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, self);
            }
            catch (SocketException)
            {
                Teardown();
            }
            onConnected.Invoke(self);
        }
    }
}
