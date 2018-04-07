using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkManager
{
    public static class Network
    {
        //Data Types
        public const byte PID       = 0x00; //Player ID This is sent once, then player id's are used
        public const byte ROOM      = 0x01; //Room data This is sent once then room id's are used
        public const byte MESSAGE   = 0x02; //This is the message object that is sent when needed
        //BOOLS
        public const byte TRUE      = 0x00;
        public const byte FALSE     = 0xFF;
        //Commands
        public const byte HELLO        = 0x00; //The server needs to know if you're still there
        public const byte LOGIN        = 0x01; //Logins in the user
        public const byte REGISTER     = 0x02; //Register an account
        public const byte UPDATES      = 0x03; //CheckFor updates
        public const byte DOUPDATES    = 0x04; //Gets the update files from the server
        public const byte LISTR        = 0x05; //Gets a list of rooms returns Room[] in callback
        public const byte JROOM        = 0x06; //Joins a room
        public const byte CHAT         = 0x07; //Sends chat to global chat
        public const byte REQUEST      = 0x08; //Sends a friend request
        public const byte LISTF        = 0x09; //Gets list of friends
        public const byte LISTRF       = 0x0A; //Gets list of friend requests
        public const byte ADDF         = 0x0B; //Adds a friend who has sent you a request
        public const byte FORMR        = 0x0C; //Creates a room
        public const byte IHOST        = 0x0D; //Are you the host of a room
        public const byte KICK         = 0x0E; //kick a player from a room
        public const byte INVITEF      = 0x0F; //invites a friend to a room
        public const byte READY        = 0x10; //Tell the server you are ready
        public const byte LEAVER       = 0x11; //Leave a room
        public const byte GRESORCE     = 0x12; //Get resource
        public const byte INIT         = 0x13; //INIT
        public const byte CHATDM       = 0x14; //sends a chat to a player
        public const byte CHATRM       = 0x15; //sends a chat to a room
        public const byte ROOMS        = 0x16; //Gets a room count
        public const byte GROOM        = 0x17; //gets a room object from a roomid
        public const byte GPID         = 0x18; //gets a pid object from a playerid
        //Error Codes
        public const byte NO_ERROR              = 0x00;
        public const byte UNKNOWN_ERROR         = 0x01;
        public const byte INVALID_USERPASS      = 0x02;
        public const byte INVALID_UID           = 0x03;
        public const byte INVALID_FID           = 0x04;
        public const byte INVALID_RID           = 0x05;
        public const byte INVALID_EMAIL         = 0x06;
        public const byte INVALID_CID           = 0x07;
        public const byte CANT_FORM_ROOM        = 0x08;
        public const byte CANT_SEND_CHAT        = 0x09;
        public const byte CANNOT_KICK_PLAYER    = 0x0A;
        public const byte NO_UPDATES            = 0x0B;
        public const byte CANT_JOIN_ROOM        = 0x0C;
        public const byte MESSAGE_TO_LARGE      = 0x0D;
        //Chat codes
        public const byte CHAT_DM = 0x00;
        public const byte CHAT_RM = 0x01;
        public const byte CHAT_GLOBAL = 0x02;
        //Client stuff
        public static UdpClient connection;
        public static string self;
        public static PID player;
        public static Dictionary<string, PID> players;
        public static Dictionary<string, Room> rooms;
        public static Room CurrentRoom;
        public static bool HANDSHAKEDONE = false;
        public static void Connect(string ip, int port,string user,string pass)
        {
            Login(ip, port, user, pass);
            ClientDataManager.INIT();
            connection = new UdpClient(port);
            try
            {
                connection.Connect(ip, port);
                Byte[] sendBytes = new byte[] { 0, 0 };
                SendData(sendBytes);
                Thread t = new Thread(new ThreadStart(ThreadProc));
                t.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static void Host(int port)
        {
            ServerDataManager.INIT();
            byte[] receiveBytes;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            connection = new UdpClient(ipep);

            Console.WriteLine("Server Hosted on port: "+port+" Waiting for clients!");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (true) // No need to run the main loop in a thread, but processes will run on threads
            {
                receiveBytes = connection.Receive(ref sender);

                string returnData=Encoding.ASCII.GetString(receiveBytes, 0, receiveBytes.Length);
                DataRecievedArgs data = new DataRecievedArgs();
                data.Response = returnData;
                data.RawResponse = receiveBytes;
                OnDataRecieved(data);
            }
        }
        public static PID GetPID(string pid)
        {
            if (players.ContainsKey(pid))
            {
                return players[pid];
            } else
            {
                DownloadPID(pid);
                while (!players.ContainsKey(pid))
                {
                    //Lets wait it out...
                }
                return players[pid];
            }
        }
        public static Room GetRoom(string rid)
        {
            if (rooms.ContainsKey(rid))
            {
                return rooms[rid];
            }
            else
            {
                DownloadRoom(rid);
                while (!rooms.ContainsKey(rid))
                {
                    //Lets wait it out...
                }
                return rooms[rid];
            }
        }
        public static void RegisterAccount(string user, string pass)
        {
            //TODO SSL connection to register 
            byte[] temp = NetUtils.PieceCommand(new object[] { REGISTER, user, pass });
            //DO IT
        }
        public static string Login(string ip, int port, string user, string pass)
        {
            //Log the user in and return its session token
            return "";
        }
        public static void Consume(byte cmd) //Consumes commands
        {
            //
        }
        public static void Retrieve(byte cmd) //Collects commands
        {
            //
        }
        public static void CreateRoom(string password) //This will trigger the player joined room event with you as the argument
        {//Might need to send map data as well
            byte[] temp = NetUtils.PieceCommand(new object[] { FORMR, self });
            SendData(temp);
        }
        public static void JoinRoom(string roomid)
        {//This will trigger the player joined room event with you as the argument
            byte[] temp = NetUtils.PieceCommand(new object[] { JROOM, self, roomid });
            SendData(temp);
        }
        public static void HasUpdates()
        {
            //byte[] temp = NetUtils.PieceCommand(new object[] { UPDATES });
            //This can be done through Managed UDP or TCP... This will probably not be implemented for the current project
        }
        public static void DoUpdates()
        {
            //Does the updates if any... This will probably not be implemented for the current project
        }
        public static void GetNumberOfRooms()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { ROOMS, self });
            SendData(temp);
        }
        public static void ListRooms(int pos, int count)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LISTR, self, pos, count });
            SendData(temp);
        }
        public static void SendFriendRequest(string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { REQUEST, self, playerid });
            SendData(temp);
        }
        public static void ListFriends()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LISTF, self });
            SendData(temp);
        }
        public static void ListRequestedFriends()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LISTRF, self });
            SendData(temp);
        }
        public static void AddFriend(string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { ADDF, self, playerid });
            SendData(temp);
        }
        public static void IsHostOf(string roomid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { IHOST, self , roomid });
            SendData(temp);
        }
        public static void KickPlayer(string roomid,string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { KICK, self , roomid, playerid });
            SendData(temp);
        }
        public static void InviteFriend(string playerid, string roomid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { INVITEF, self, playerid, roomid });
            SendData(temp);
        }
        public static void SetReady(bool ready)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { READY, self, ready });
            SendData(temp);
        }
        public static void LeaveRoom(string roomid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LEAVER, self, roomid });
            SendData(temp);
        }
        public static void GetResourceList()
        {
            //This will work interesting... The data returned by the callback will be in pieces... This will have items and recipies ill figure it out :P
            byte[] temp = NetUtils.PieceCommand(new object[] { GRESORCE, self });
            SendData(temp);
        }
        public static void INITCONNECTION()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { INIT });
            SendData(temp);
        }
        public static void SatHello()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { HELLO });
            SendData(temp);
        }
        public static void SendDM(Message msg,string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { CHATDM, self, msg, playerid });
            SendData(temp);
        }
        public static void SendRoomChat(Message msg, string roomid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { CHATRM, self, msg, roomid });
            SendData(temp);
        }
        public static void SendChat(Message msg)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { CHAT, self, msg});
            SendData(temp);
        }
        public static void DownloadRoom(string roomid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { GROOM, self, roomid });
            SendData(temp);
        }
        public static void DownloadPID(string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { GPID, self, playerid });
            SendData(temp);
        }
        public static void SendData(byte[] data)
        {
            Consume(data[0]);
            connection.Send(data,data.Length);
        }
        public static void SendData(byte[] data, IPEndPoint sender)
        {
            Consume(data[0]);
            connection.Send(data, data.Length, sender);
        }
        public static void ThreadProc()
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                Byte[] receiveBytes = connection.Receive(ref RemoteIpEndPoint);
                if (receiveBytes.Length > 0)
                {
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    DataRecievedArgs data = new DataRecievedArgs();
                    data.Response = returnData;
                    data.RawResponse = receiveBytes;
                    OnDataRecieved(data);
                }
            }
        }
        public static void OnDataRecieved(DataRecievedArgs e)
        {
            EventHandler<DataRecievedArgs> handler = DataRecieved;
            handler(null, e);
        }
        public static void OnChat(ChatDataArgs e)
        {
            EventHandler<ChatDataArgs> handler = Chat;
            handler(null, e);
        }
        public static void OnFriendRequest(FriendRequestArgs e)
        {
            EventHandler<FriendRequestArgs> handler = FriendRequested;
            handler(null, e);
        }
        public static void OnGameStarted(BasicResponseArgs e)
        {
            EventHandler<BasicResponseArgs> handler = GameStarted;
            handler(null, e);
        }
        public static void OnPlayerJoined(RoomUpdateArgs e)
        {
            EventHandler<RoomUpdateArgs> handler = PlayerJoinedRoom;
            handler(null, e);
        }
        public static void OnPlayerLeft(RoomUpdateArgs e)
        {
            EventHandler<RoomUpdateArgs> handler = PlayerLeftRoom;
            handler(null, e);
        }
        public static void OnError(ErrorArgs e)
        {
            EventHandler<ErrorArgs> handler = Error;
            handler(null, e);
        }
        public static void OnRoomList(RoomListArgs e)
        {
            EventHandler<RoomListArgs> handler = RoomList;
            handler(null, e);
        }
        public static void OnRoomCount(RoomCountArgs e)
        {
            EventHandler<RoomCountArgs> handler = RoomCount;
            handler(null, e);
        }
        public static void OnFriendsList(FriendsArgs e)
        {
            EventHandler<FriendsArgs> handler = FriendsList;
            handler(null, e);
        }
        public static event EventHandler<DataRecievedArgs> DataRecieved;
        public static event EventHandler<ChatDataArgs> Chat;
        public static event EventHandler<FriendRequestArgs> FriendRequested;
        public static event EventHandler<BasicResponseArgs> GameStarted;
        public static event EventHandler<RoomUpdateArgs> PlayerJoinedRoom;
        public static event EventHandler<RoomUpdateArgs> PlayerLeftRoom;
        public static event EventHandler<RoomCountArgs> RoomCount;
        public static event EventHandler<RoomListArgs> RoomList;
        public static event EventHandler<FriendsArgs> FriendsList;
        public static event EventHandler<ErrorArgs> Error;
    }
}