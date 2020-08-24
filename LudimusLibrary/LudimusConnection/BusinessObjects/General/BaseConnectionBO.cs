using System.Net.Sockets;

namespace LudimusConnection.BusinessObjects.General
{
    public class BaseConnectionBO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Socket Socket { get; set; }

    }

    public delegate void MessageReceivedDel<T>(DataBO<T> data, BaseConnectionBO connectionBO);
    public delegate void MessageSentDel<T>(DataBO<T> data);
    public delegate void ConnectionChangeHandler(BaseConnectionBO connectionBO);
}
