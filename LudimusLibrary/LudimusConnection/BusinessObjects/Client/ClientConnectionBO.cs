using LudimusConnection.BusinessObjects.General;

namespace LudimusConnection.BusinessObjects.Client
{
    class ClientConnectionBO<T> : BaseConnectionBO
    {
        public MessageReceivedDel<T> OnMessageReceived { get; set; }
    }
}
