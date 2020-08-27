using LudimusConnection.BusinessObjects.General;
using System;

namespace LudimusConnection.BusinessObjects.Client
{
    public class ClientConnectionBO : BaseConnectionBO
    {
        public Action<Object, BaseConnectionBO> OnMessageReceived { get; set; }
    }
}
