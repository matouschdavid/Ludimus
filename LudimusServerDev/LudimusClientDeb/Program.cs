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
            Console.WriteLine("Start connecting");
            ConnectionController.Connect(ReceiveCallback, ConnectedCallback, "David");
            ConnectionController.AttachMsgHandler(MessageCallback);
            Console.ReadKey();
        }

        private static void MessageCallback(Data data)
        {
            Console.WriteLine("Got message: " + data);
        }

        private static void ConnectedCallback(Connection connection)
        {
            Console.WriteLine("Connected");
            ConnectionController.Write(new Data { Key = "Test", Value = "Hopefully worked" }, connection);
        }

        private static void ReceiveCallback(Data data)
        {
            Console.WriteLine("Received: " + data);
        }
    }
}
