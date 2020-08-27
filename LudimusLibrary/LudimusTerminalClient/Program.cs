
using System;
using LudimusConnection.BusinessObjects.General;
using LudimusConnection.DataAccess.Client;

namespace LudimusTerminalClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientConnectionDA.AttachMessageReceivedHandler(OnMessageReceived);

            Console.Write("Input your name: ");
            ClientConnectionDA.Connect(Console.ReadLine(), "192.168.1.38", OnPlayerConnected);
            Console.ReadKey();
        }

        private static void OnPlayerConnected(BaseConnectionBO connectionBO)
        {
            Console.WriteLine("We're connected");
            for (int i = 0; i < 500; i++)
            {
                ClientConnectionDA.Write(new DataBO<Test>("Input", new Test
                {
                    TestInt = 456456645,
                    TestStr = "LOLOLOLOLOL"
                }));
            }
            ClientConnectionDA.Write(new DataBO<string>("Important", "wichtig"));
            for (int i = 0; i < 500; i++)
            {
                ClientConnectionDA.Write(new DataBO<Test>("Input", new Test
                {
                    TestInt = 456456645,
                    TestStr = "LOLOLOLOLOL"
                }));
            }
            ClientConnectionDA.Write(new DataBO<string>("Finish", ""));
        }

        private static void OnMessageReceived(DataBO<object> arg1, BaseConnectionBO arg2)
        {
            Console.WriteLine("Got the message: " + arg1.ToString());
        }
    }
}
