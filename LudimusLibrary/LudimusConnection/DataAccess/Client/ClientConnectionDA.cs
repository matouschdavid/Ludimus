using LudimusConnection.BusinessObjects.General;
using LudimusConnection.DataAccess.General;

namespace LudimusConnection.DataAccess.Client
{
    class ClientConnectionDA<T> : BaseConnectionDA<T>
    {
        private void OnMessageReceived(DataBO<T> data)
        {
            messageReceivedDel.Invoke(data, base.self);
        }

        internal protected static bool Teardown()
        {
            throw new System.NotImplementedException();
        }

        public bool Write(DataBO<T> data)
        {
            throw new System.NotImplementedException();
        }
    }
}
