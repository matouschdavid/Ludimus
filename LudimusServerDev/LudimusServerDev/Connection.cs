using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static LudimusServerDev.ConnectionController;

namespace LudimusServerDev
{
    public class Connection
    {
        public delegate void HandleInputDel(Data data);

        public int ClientId { get; set; }
        public string Playername { get; set; }
        public Socket Client { get; set; }
        public MessageDel HandleInput;
    }
}
