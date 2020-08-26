using LudimusConnection.BusinessObjects.General;

namespace LudimusConnection.BusinessObjects.Client
{
    public class ClientConnectionBO : BaseConnectionBO
    {
        public MessageReceivedDel<object> OnMessageReceived { get; set; }
    }
}
