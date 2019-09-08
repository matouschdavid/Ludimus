using LudimusServerDev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudimusClientDev
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionController.Connect(ReceiveCallback, ConnectedCallback, "David");
            Console.ReadKey();
        }

        private static void ConnectedCallback(Connection connection)
        {
            Console.WriteLine("Connected");
        }

        private static void ReceiveCallback(Data data)
        {
            Console.WriteLine("Received: " + data.ToString());
        }
    }
}
