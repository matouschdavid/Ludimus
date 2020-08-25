using LudimusConnection.BusinessObjects.General;

namespace LudimusConnection.DataAccess.General
{
    public abstract class BaseConnectionDA<T>
    {
        internal protected static MessageReceivedDel<T> messageReceivedDel;
        internal protected static MessageSentDel<T> messageSentDel;
        internal protected static ConnectionChangeHandler playerConnectedDel;
        internal protected static ConnectionChangeHandler beforePlayerDisconnectedDel;
        internal protected static ConnectionChangeHandler afterPlayerDisconnectedDel;

        internal protected BaseConnectionBO self;


        #region EventHandler
        public static bool AttachMessageReceivedHandler(MessageReceivedDel<T> handler)
        {
            if (messageReceivedDel == null)
                return false;
            messageReceivedDel += handler;
            return true;
        }
        public static bool AttachMessageSentHandler(MessageSentDel<T> handler)
        {
            if (messageSentDel == null)
                return false;
            messageSentDel += handler;
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
        public static bool DetachMessageReceivedHandler(MessageReceivedDel<T> handler)
        {
            if (messageReceivedDel == null)
                return false;
            messageReceivedDel -= handler;
            return true;
        }
        public static bool DetachMessageSentHandler(MessageSentDel<T> handler)
        {
            if (messageSentDel == null)
                return false;
            messageSentDel -= handler;
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
    }
}