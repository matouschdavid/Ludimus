using LudimusConnection.BusinessObjects.Client;
using LudimusConnection.BusinessObjects.General;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Reflection;

namespace LudimusConnection.DataAccess.General
{
    public class BaseConnectionDA
    {
        internal protected static Action<Object, BaseConnectionBO> messageReceivedDel;
        internal protected static Action<Object, BaseConnectionBO> messageSentDel;
        internal protected static ConnectionChangeHandler playerConnectedDel;
        internal protected static ConnectionChangeHandler beforePlayerDisconnectedDel;
        internal protected static ConnectionChangeHandler afterPlayerDisconnectedDel;

        internal protected static Action onCrashDel;

        internal protected static BaseConnectionBO self;
        internal protected static bool isServer = false;

        protected const int BUFFER_SIZE = 4048;
        internal protected static readonly byte[] buffer = new byte[BUFFER_SIZE];

        #region EventHandler
        public static bool AttachMessageReceivedHandler<T>(Action<DataBO<T>, BaseConnectionBO> handler)
        {
            if (handler == null)
                return false;

            messageReceivedDel += (data, client) =>
            {
                if (handler != null && data is DataBO<T>)
                {
                    handler(data as DataBO<T>, client);
                }
            };
            return true;
        }

        public static bool AttachMessageReceivedHandler(Action<DataBO<object>, BaseConnectionBO> handler)
        {
            return AttachMessageReceivedHandler<object>(handler);
        }
        public static bool AttachMessageSentHandler<T>(Action<DataBO<T>, BaseConnectionBO> handler)
        {
            if (handler == null)
                return false;
            messageSentDel += (data, client) =>
            {
                if (handler != null && data is DataBO<T>)
                {
                    handler(data as DataBO<T>, client);
                }
            }; ;
            return true;
        }
        public static bool AttachPlayerConnectedHandler(ConnectionChangeHandler handler)
        {
            if (handler == null)
                return false;
            playerConnectedDel += handler;
            return true;
        }
        public static bool AttachBeforePlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (handler == null)
                return false;
            beforePlayerDisconnectedDel += handler;
            return true;
        }
        public static bool AttachAfterPlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (handler == null)
                return false;
            afterPlayerDisconnectedDel += handler;
            return true;
        }
        #endregion

        internal protected static void ReceiveCallBack(IAsyncResult ar)
        {
            ClientConnectionBO client = (ClientConnectionBO)ar.AsyncState;
            int received = 0;
            try
            {
                received = client.Socket.EndReceive(ar);
            }
            catch (Exception)
            {
                onCrashDel?.Invoke();
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            var str = System.Text.Encoding.ASCII.GetString(recBuf);
            DataBO<object> data = null;
            do
            {
                data = new DataBO<object>(str);
                if (!(data.Key == null && data.LeftOver == String.Empty))
                {
                    if (data.Key == LudimusConnectionKeys.CHANGE_PLAYER_NAME)
                    {
                        client.Name = data.Value.ToString();
                    }

                    var dataBOType = typeof(DataBO<>).MakeGenericType(data.Type);
                    var result = Activator.CreateInstance(dataBOType);
                    FieldInfo keyInfo = dataBOType.GetField("Key");
                    keyInfo.SetValue(result, data.Key);
                    FieldInfo valueInfo = dataBOType.GetField("Value");
                    valueInfo.SetValue(result, JsonConvert.DeserializeObject(data.GetValueAsJson(), data.Type));
                    var objectResult = new DataBO<object>
                    {
                        Key = data.Key,
                        Value = data.Value,
                    };
                    if (isServer)
                    {
                        client.OnMessageReceived?.Invoke(result, client);
                        client.OnMessageReceived?.Invoke(objectResult, client);
                    }
                    else
                    {
                        messageReceivedDel?.Invoke(result, self);
                        messageReceivedDel?.Invoke(objectResult, self);
                    }
                    str = data.LeftOver;
                }
            } while (data.LeftOver.Length > 0);
            //Wait for next Message
            client.Socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, client);
        }

        internal protected static bool Write<T>(DataBO<T> data, BaseConnectionBO connectionBO)
        {
            data.Type = typeof(T);
            var len = data.GetAsJson().Length;
            var dataBOType = typeof(DataBO<>).MakeGenericType(data.Type);
            var result = Activator.CreateInstance(dataBOType);
            FieldInfo keyInfo = dataBOType.GetField("Key");
            keyInfo.SetValue(result, data.Key);
            FieldInfo valueInfo = dataBOType.GetField("Value");
            valueInfo.SetValue(result, data.Value);
            messageSentDel?.Invoke(result, connectionBO);

            var bytes = System.Text.Encoding.ASCII.GetBytes(len + "|" + data.GetAsJson());
            connectionBO.Socket.SendBufferSize = bytes.Length;
            int received = connectionBO.Socket.Send(bytes, 0, bytes.Length, SocketFlags.None);
            
            return received == bytes.Length;
        }
    }
}