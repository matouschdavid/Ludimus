
using System;
using LudimusConnection.BusinessObjects.General;
using LudimusConnection.DataAccess.Client;

namespace LudimusTerminalClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientConnectionDA<string>.Connect("test", "192.168.1.38", OnMessageReceived, OnPlayerConnected); 
        }

        private static void OnPlayerConnected(BaseConnectionBO connectionBO)
        {
            Console.WriteLine("We are connected sir");
        }

        private static void OnMessageReceived(DataBO<string> data, BaseConnectionBO connectionBO)
        {
            Console.WriteLine("Incoming message: " + data.Value);
        }
    }
}
