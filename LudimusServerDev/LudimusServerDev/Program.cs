using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudimusServerDev
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server");
            ConnectionController.StartServer();
            ConnectionController.AttachConnectionHandler(ConnectedCallback);
            ConnectionController.AttachMsgHandler(MessageCallback);
            Console.ReadKey();
        }

        private static void MessageCallback(Data data)
        {
            Console.WriteLine("Got message: " + data);
            ConnectionController.Write(new Data { Key = "Test", Value = "Got message: " + data });
        }

        private static void ConnectedCallback(Connection connection)
        {
            Console.WriteLine("New Player connected");
        }
    }
}
