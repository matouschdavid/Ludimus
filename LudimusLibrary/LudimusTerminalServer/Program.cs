using LudimusConnection.BusinessObjects.General;
using LudimusConnection.DataAccess.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudimusTerminalServer
{
    class Program
    {
        static int messagesReceived = 0;
        static void Main(string[] args)
        {
            ServerConnectionDA.AttachMessageReceivedHandler(OnReceived);
            ServerConnectionDA.AttachMessageReceivedHandler<string>(OnStringReceived);
            ServerConnectionDA.AttachPlayerConnectedHandler(OnPlayerConnected);
            ServerConnectionDA.StartServer();
            Console.ReadKey();
        }

        private static void OnStringReceived(DataBO<string> arg1, BaseConnectionBO arg2)
        {
            if(arg1.Key == "Important")
                Console.WriteLine("Got important");
            else if(arg1.Key == "Finish")
                Console.WriteLine(messagesReceived);
        }

        private static void OnPlayerConnected(BaseConnectionBO connectionBO)
        {
            Console.WriteLine("Player " + connectionBO.Name + " joined");
        }

        private static void OnReceived(DataBO<object> data, BaseConnectionBO connectionBO)
        {
            messagesReceived++;
            //Console.WriteLine("Nr. " + messagesReceived + " Received " + data.ToString() + "\nFrom: " + connectionBO.Name);
        }
    }
}
