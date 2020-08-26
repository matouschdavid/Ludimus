using LudimusConnection.BusinessObjects.Client;
using LudimusConnection.BusinessObjects.General;
using System;
using System.Net.Sockets;

namespace LudimusConnection.DataAccess.General
{
    public abstract class BaseConnectionDA
    {
        internal protected static MessageReceivedDel<object> messageReceivedDel;
        internal protected static MessageSentDel<object> messageSentDel;
        internal protected static ConnectionChangeHandler playerConnectedDel;
        internal protected static ConnectionChangeHandler beforePlayerDisconnectedDel;
        internal protected static ConnectionChangeHandler afterPlayerDisconnectedDel;

        internal protected static BaseConnectionBO self;

        protected const int BUFFER_SIZE = 255;
        internal protected static readonly byte[] buffer = new byte[BUFFER_SIZE];

        #region EventHandler
        public static bool AttachMessageReceivedHandler<T>(MessageReceivedDel<T> handler)
        {
            if (messageReceivedDel == null)
                return false;

            messageReceivedDel += handler as MessageReceivedDel<object>;
            return true;
        }
        public static bool AttachMessageSentHandler<T>(MessageSentDel<T> handler)
        {
            if (messageSentDel == null)
                return false;
            messageSentDel += handler as MessageSentDel<object>;
            return true;
        }
        public static bool AttachPlayerConnectedHandler(ConnectionChangeHandler handler)
        {
            if (playerConnectedDel == null)
                return false;
            playerConnectedDel += handler;
            return true;
        }
        public static bool AttachBeforePlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (beforePlayerDisconnectedDel == null)
                return false;
            beforePlayerDisconnectedDel += handler;
            return true;
        }
        public static bool AttachAfterPlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (afterPlayerDisconnectedDel == null)
                return false;
            afterPlayerDisconnectedDel += handler;
            return true;
        }
        public static bool DetachMessageReceivedHandler<T>(MessageReceivedDel<T> handler)
        {
            if (messageReceivedDel == null)
                return false;
            messageReceivedDel -= handler as MessageReceivedDel<object>;
            return true;
        }
        public static bool DetachMessageSentHandler<T>(MessageSentDel<T> handler)
        {
            if (messageSentDel == null)
                return false;
            messageSentDel -= handler as MessageSentDel<object>;
            return true;
        }
        public static bool DetachPlayerConnectedHandler(ConnectionChangeHandler handler)
        {
            if (playerConnectedDel == null)
                return false;
            playerConnectedDel -= handler;
            return true;
        }
        public static bool DetachBeforePlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (beforePlayerDisconnectedDel == null)
                return false;
            beforePlayerDisconnectedDel -= handler;
            return true;
        }
        public static bool DetachAfterPlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (afterPlayerDisconnectedDel == null)
                return false;
            afterPlayerDisconnectedDel -= handler;
            return true;
        }
        #endregion

        internal protected static void ReceiveCallBack(IAsyncResult ar)
        {
            ClientConnectionBO client = (ClientConnectionBO)ar.AsyncState;
            int received = 0;
            received = client.Socket.EndReceive(ar);

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            var str = System.Text.Encoding.ASCII.GetString(recBuf);
            var T = Type.GetType(str.Split('|')[0]);
            var type = typeof(DataBO<>).MakeGenericType(T.GetType());
            var d = Activator.CreateInstance(type) as DataBO<>;
            Console.WriteLine("Received text: " + d.ToString());
            if (d.Key == LudimusConnectionKeys.CHANGE_PLAYER_NAME)
            {
                client.Name = d.Value.ToString();
            }
            if (client.OnMessageReceived == null) return;
            foreach (var method in client.OnMessageReceived.GetInvocationList())
            {
                if(method.GetType() == T)
                {

                }
            }
            client.OnMessageReceived.Invoke(d, client);

            //Wait for next Message
            client.Socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, client);
        }

        internal protected static bool Write<T>(DataBO<T> data, BaseConnectionBO connectionBO)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(data.GetValueAsJson());

            return connectionBO.Socket.Send(bytes, 0, bytes.Length, SocketFlags.None) == bytes.Length;
        }
    }
}