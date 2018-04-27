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
            bool conn = Network.Connect("192.168.1.6", 12344, "epicknex", "password");
            if (!conn)
            {
                Console.WriteLine("Could not connect!");
            }
            Network.Error += OnError;
            Network.LoggedIn += OnLoggedIn;
            Network.GotRoomData += OnRoomDataRecieved;
            Network.GotPIDData += OnPIDDataRecieved;
            Network.Chat += OnChat;
            Network.RoomList += OnRoomList;
        }
        public static void OnError(object sender, ErrorArgs e)
        {
            Console.WriteLine("We have an error: "+e.ErrorCode+" Error Msg: "+e.ErrorMessage);
        }
        public static void OnLoggedIn(object sender, LoginEventArgs e)
        {
            Console.WriteLine("We are logged it!");
            PID test = Network.GetPID("12345678");
            Network.CreateRoom();
            Network.LeaveRoom();
            Network.ListRooms();
        }
        public static void OnPIDDataRecieved(object sender, GotPIDEventArgs e)
        {
            Console.WriteLine("PID: "+e.Pid.GetName());
        }
        public static void OnRoomDataRecieved(object sender, GotRoomEventArgs e)
        {
            Console.WriteLine("Room: " + e.Room.GetRoomID());
            //Network.SendRoomChat(new Message("Hello this is a test of Room chat!", Network.player));
        }
        public static void OnChat(object sender, ChatDataArgs e)
        {
            Console.WriteLine(e.Message.GetPID().GetName()+": " + e.Message.GetMessage());
        }
        public static void OnRoomList(object sender, RoomListArgs e)
        {
            Console.WriteLine(e.Rooms.Length);
        }
    }
}
