using System;
using System.Net.Sockets;

namespace LudimusConnection.BusinessObjects.General
{
    public class BaseConnectionBO
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Socket Socket { get; set; }

    }

    public class Test
    {
        public int TestInt { get; set; }
        public string TestStr { get; set; }
    }

    public delegate void MessageReceivedDel<T>(T data, BaseConnectionBO connectionBO);
    public delegate void MessageSentDel<T>(T data);
    public delegate void ConnectionChangeHandler(BaseConnectionBO connectionBO);
}
