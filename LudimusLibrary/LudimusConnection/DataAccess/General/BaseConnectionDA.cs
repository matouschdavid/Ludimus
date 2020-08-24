using LudimusConnection.BusinessObjects.General;

namespace LudimusConnection.DataAccess.General
{
    public abstract class BaseConnectionDA<T>
    {
        private MessageReceivedDel<T> messageReceivedDel;
        private MessageSentDel<T> messageSentDel;
        private ConnectionChangeHandler playerConnectedDel;
        private ConnectionChangeHandler beforePlayerDisconnectedDel;
        private ConnectionChangeHandler afterPlayerDisconnectedDel;
        
        public abstract bool Teardown();

        #region EventHandler
        bool AttachMessageReceivedHandler(MessageReceivedDel<T> handler)
        {
            if (this.messageReceivedDel == null)
                return false;
            this.messageReceivedDel += handler;
            return true;
        }
        bool AttachMessageSentHandler(MessageSentDel<T> handler)
        {
            if (this.messageSentDel == null)
                return false;
            this.messageSentDel += handler;
            return true;
        }
        bool AttachPlayerConnectedHandler(ConnectionChangeHandler handler)
        {
            if (this.playerConnectedDel == null)
                return false;
            this.playerConnectedDel += handler;
            return true;
        }
        bool AttachBeforePlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (this.beforePlayerDisconnectedDel == null)
                return false;
            this.beforePlayerDisconnectedDel += handler;
            return true;
        }
        bool AttachAfterPlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (this.afterPlayerDisconnectedDel == null)
                return false;
            this.afterPlayerDisconnectedDel += handler;
            return true;
        }
        bool DetachMessageReceivedHandler(MessageReceivedDel<T> handler)
        {
            if (this.messageReceivedDel == null)
                return false;
            this.messageReceivedDel -= handler;
            return true;
        }
        bool DetachMessageSentHandler(MessageSentDel<T> handler)
        {
            if (this.messageSentDel == null)
                return false;
            this.messageSentDel -= handler;
            return true;
        }
        bool DetachPlayerConnectedHandler(ConnectionChangeHandler handler)
        {
            if (this.playerConnectedDel == null)
                return false;
            this.playerConnectedDel -= handler;
            return true;
        }
        bool DetachBeforePlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (this.beforePlayerDisconnectedDel == null)
                return false;
            this.beforePlayerDisconnectedDel -= handler;
            return true;
        }
        bool DetachAfterPlayerDisconnectedHandler(ConnectionChangeHandler handler)
        {
            if (this.afterPlayerDisconnectedDel == null)
                return false;
            this.afterPlayerDisconnectedDel -= handler;
            return true;
        }
        #endregion
    }
}