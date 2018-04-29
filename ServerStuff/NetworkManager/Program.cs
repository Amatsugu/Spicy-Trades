using System;
using System.Threading;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace NetworkManager
{
    class Program
    {
        static void Main(string[] args)
        {
            bool conn = Network.Connect("192.168.1.6", 12344);
            Network.Chat += OnChat;
            if (!conn)
            {
                Console.WriteLine("Could not connect!");
            }
            if(Network.Login("epicknex", "password"))
            {
                Console.WriteLine("Login Success! "+Network.player.GetName()+" "+Network.player.GetID());
                Network.SendChat(new Message("Hello this is a test"));
                Network.CreateRoom();
                Network.SendRoomChat(new Message("Hello this is a Room test"));
                Network.LeaveRoom();
                Network.Logout();
                Console.WriteLine("Logged out!");
                Console.ReadLine();
                Environment.Exit(0);
            } else
            {
                Console.WriteLine("Cannot Login!");
            }
        }
        public static void OnChat(object sender, ChatDataArgs e)
        {
            Console.WriteLine(e.Message.GetPID().GetName()+": " + e.Message.GetMessage());
        }
    }
}
