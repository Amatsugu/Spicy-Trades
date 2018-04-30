using System;
using System.Threading;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace NetworkManager
{
    class Program
    {
        static bool done = false;
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
                while (!done)
                {
                    DoStuff();
                }
                Console.ReadLine();
                Environment.Exit(0);
            } else
            {
                Console.WriteLine("Cannot Login!");
            }
        }
        public static void DoStuff()
        {
            Console.WriteLine(@"1. Create Room
2. Send Chat
3. Send Room Chat
4. Send Player Chat
5. Join Room
6. Get Number Of Rooms
7. List Rooms
8. Is Room Host
9. kick player
10. invite player
11. set ready
12. Leave Room
13. Logout");
            Console.Write("What do you want to do? ");
            string choice = Console.ReadLine();
            int cho;
            string msg;
            string id;
            Room rm;
            if (int.TryParse(choice, out cho))
            {
                switch (cho)
                {
                    case 1:
                        if (Network.CreateRoom())
                        {
                            Console.WriteLine("Created a room!");
                        }
                        break;
                    case 2:
                        Console.Write("Global Message: ");
                        msg = Console.ReadLine();
                        Network.SendChat(new Message(msg));
                        Console.WriteLine("Message Sent!");
                        break;
                    case 3:
                        Console.Write("Room Message: ");
                        msg = Console.ReadLine();
                        Network.SendRoomChat(new Message(msg));
                        Console.WriteLine("Message Sent!");
                        break;
                    case 4:
                        Console.Write("Direct Message: ");
                        msg = Console.ReadLine();
                        Console.Write("PlayerID: ");
                        id = Console.ReadLine();
                        Network.SendDM(new Message(msg),id);
                        Console.WriteLine("Message Sent!");
                        break;
                    case 5:
                        Console.Write("RoomID: ");
                        id = Console.ReadLine();
                        rm = Network.JoinRoom(id);
                        if (rm!=null)
                        {
                            Console.WriteLine("Joined a room!");
                        }
                        break;
                    case 6:
                        int? num = Network.GetNumberOfRooms();
                        Console.WriteLine("There are: "+num+" Rooms!");
                        break;
                    case 7:
                        Room[] rms = Network.ListRooms();
                        Console.WriteLine("RoomList:");
                        for(int i = 0; i < rms.Length; i++)
                        {
                            Console.WriteLine(rms[0].GetRoomID());
                        }
                        break;
                    case 8:
                        bool? host = Network.IsHostOf();
                        Console.WriteLine("Host: {0}",host);
                        break;
                    case 9:
                        Console.Write("PlayerID: ");
                        id = Console.ReadLine();
                        if (Network.KickPlayer(id))
                        {
                            Console.WriteLine("Player Kicked!");
                        }
                        break;
                    case 10:
                        Console.Write("PlayerID: ");
                        id = Console.ReadLine();
                        Network.InviteFriend(id);
                        Console.WriteLine("Sent Invite!");
                        break;
                    case 11:
                        Console.Write("Ready (y/n): ");
                        bool ready = Console.ReadLine()=="y";
                        Network.SetReady(ready);
                        Console.WriteLine("Set Ready!");
                        break;
                    case 12:
                        Network.LeaveRoom();
                        Console.WriteLine("Left Room!");
                        break;
                    case 13:
                        Console.WriteLine(Network.Logout());
                        Console.WriteLine("Logged Out!");
                        break;
                    default:
                        Console.WriteLine("Unknown command!");
                        break;
                }
            }
        }
        public static void OnChat(object sender, ChatDataArgs e)
        {
            Console.WriteLine(e.Message.GetPID().GetName()+": " + e.Message.GetMessage());
        }
    }
}
