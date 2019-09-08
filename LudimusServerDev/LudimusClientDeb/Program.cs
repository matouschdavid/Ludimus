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
            
            Console.ReadKey();
        }

        private static void MessageCallback(Data data, Connection connection)
        {
            Console.WriteLine("Got message: " + data);
        }

        private static void ConnectedCallback(Connection connection)
        {
            Console.WriteLine("Connected");
            ConnectionController.Write(new Data { Key = "FromClientToServer", Value = "Hopefully worked" }, connection);
            //ConnectionController.Client = connection;
            ConnectionController.AttachMsgHandler(MessageCallback);
        }

        private static void ReceiveCallback(Data data, Connection connection)
        {
            Console.WriteLine("Received: " + data);
        }
    }
}
