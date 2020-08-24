using LudimusConnection.BusinessObjects.General;
using LudimusConnection.DataAccess.General;

namespace LudimusConnection.DataAccess.Client
{
    class ClientConnectionDA<T> : BaseConnectionDA<T>
    {
        public void OnMessageReceived(DataBO<T> data)
        {
            throw new System.NotImplementedException();
        }

        public override bool Teardown()
        {
            throw new System.NotImplementedException();
        }

        public bool Write(DataBO<T> data)
        {
            throw new System.NotImplementedException();
        }
    }
}
