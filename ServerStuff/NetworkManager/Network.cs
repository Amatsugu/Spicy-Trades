using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkManager
{
    static class Network
    {
        //Data Types
        public static byte PID       = 0x00; //Player ID This is sent once, then player id's are used
        public static byte ROOM      = 0x01; //Room data This is sent once then room id's are used
        public static byte MESSAGE   = 0x02; //This is the message object that is sent when needed
        //Commands
        public static byte HELLO        = 0x00; //The server needs to know if you're still there
        public static byte LOGIN        = 0x01; //Logins in the user
        public static byte REGISTER     = 0x02; //Register an account
        public static byte UPDATES      = 0x03; //CheckFor updates
        public static byte DOUPDATES    = 0x04; //Gets the update files from the server
        public static byte LISTR        = 0x05; //Gets a list of rooms returns Room[] in callback
        public static byte JROOM        = 0x06; //Joins a room
        public static byte CHAT         = 0x07; //
        public static byte REQUEST      = 0x08;
        public static byte LISTF        = 0x09;
        public static byte LISTRF       = 0x0A;
        public static byte ADDF         = 0x0B;
        public static byte FORMR        = 0x0C;
        public static byte IHOST        = 0x0D;
        public static byte KICK         = 0x0E;
        public static byte INVITEF      = 0x0F;
        public static byte READY        = 0x10;
        public static byte LEAVER       = 0x11;
        public static byte GRESORCE     = 0x12;
        public static byte INIT         = 0x13;
        public static byte CHATDM       = 0x14;
        public static byte CHATRM       = 0x15;
        public static byte ROOMS        = 0x16; //Gets a room count
        //Error Codes
        public static byte NO_ERROR              = 0x00;
        public static byte UNKNOWN_ERROR         = 0x01;
        public static byte INVALID_USERPASS      = 0x02;
        public static byte INVALID_UID           = 0x03;
        public static byte INVALID_FID           = 0x04;
        public static byte INVALID_RID           = 0x05;
        public static byte INVALID_EMAIL         = 0x06;
        public static byte INVALID_CID           = 0x07;
        public static byte CANT_FORM_ROOM        = 0x08;
        public static byte CANT_SEND_CHAT        = 0x09;
        public static byte CANNOT_KICK_PLAYER    = 0x0A;
        public static byte NO_UPDATES            = 0x0B;
        public static byte CANT_JOIN_ROOM        = 0x0C;
        public static byte MESSAGE_TO_LARGE      = 0x0D;
        //Client stuff
        public static Client connection;
        private static string self;
        public static void Connect(string ip, int port,string user,string pass)
        {//We need to be able to log in
            //self = Login(ip,port,user,pass);
            IPHostEntry ipHostInfo;
            IPAddress ipAddress;
            try
            {
                ipHostInfo = Dns.GetHostEntry(ip);
                ipAddress = ipHostInfo.AddressList[0];
            }
            catch
            {
                throw new Exception("Unable to connect to the server! Unknown Hostname: " + ip);
            }
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                connection = new Client(client, remoteEP, ipHostInfo, ipAddress, ip, port);
                Thread t = new Thread(new ThreadStart(ThreadProc));
                t.Start();
            }
            catch
            {
                throw new Exception("Unable to connect to the server! Is the server running? " + ip + ":" + port);
            }
        }
        public static string Login(string ip, int port, string user, string pass)
        {
            return "";
        }
        public static void CreateRoom(string password) //This will trigger the player joined room event with you as the argument
        {//Might need to send map data as well
            byte[] temp = NetUtils.PieceCommand(new object[] {FORMR, self});
            SendData(temp);
        }
        public static void JoinRoom(string roomid)
        {//This will trigger the player joined room event with you as the argument
            byte[] temp = NetUtils.PieceCommand(new object[] { JROOM, self, roomid });
            SendData(temp);
        }
        public static void RegisterAccount(string user,string pass)
        {
            //TODO SSL connection to register 
            byte[] temp = NetUtils.PieceCommand(new object[] { REGISTER, user, pass });
            //DO IT
        }
        public static void HasUpdates()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { UPDATES });
            //This can be done through Managed UDP or TCP
        } 
        public static void DoUpdates()
        {
            //Does the updates if any...
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
            byte[] temp = NetUtils.PieceCommand(new object[] { REQUEST, playerid });
            SendData(temp);
        }
        public static void ListFriends()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LISTF, self });
            SendData(temp);
        }
        public static void AddFriend()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { ADDF, self });
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
        public static void SendData(byte[] data)
        {
            //Add a way to ensure data goes where it needs to
            connection.Send(data);
        }
        public static void SendDataUnMonitored(byte[] data)
        {
            connection.Send(data);
        }
        public static void SendData(string data)
        {
            connection.Send(data);
        }
        public static void ThreadProc()
        {
            connection.Receive();
            connection.sendDone.WaitOne();
            while (true)
            {
                //Keep the thread going
                Thread.Sleep(10);
            }
        }
        public static void OnDataRecieved(DataRecievedArgs e)
        {
            EventHandler<DataRecievedArgs> handler = DataRecieved;
            handler(null, e);
        }
        public static void OnChat(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = Chat;
            handler(null, e);
        }
        public static void OnFriendRequest(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = FriendRequest;
            handler(null, e);
        }
        public static void OnGameStarted(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = GameStarted;
            handler(null, e);
        }
        public static void OnPlayerJoined(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = PlayerJoined;
            handler(null, e);
        }
        public static void OnPlayerLeft(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = PlayerLeft;
            handler(null, e);
        }
        public static void OnError(ErrorArgs e)
        {
            EventHandler<ErrorArgs> handler = Error;
            handler(null, e);
        }
        public static event EventHandler<DataRecievedArgs> DataRecieved;
        public static event EventHandler<DataEventArgs> Chat;
        public static event EventHandler<DataEventArgs> FriendRequest;
        public static event EventHandler<DataEventArgs> GameStarted;
        public static event EventHandler<DataEventArgs> PlayerJoined;
        public static event EventHandler<DataEventArgs> PlayerLeft;
        public static event EventHandler<ErrorArgs> Error;
    }
    public class DataEventArgs : EventArgs
    {
        public string Response { get; set; }
        public int errorcode { get; set; }
        public byte[] RawResponse { get; set; }
        public object ObjectRef { get; set; }
    }
    public class DataRecievedArgs : EventArgs
    {
        public string Response { get; set; }
        public byte[] RawResponse { get; set; }
    }
    public class ErrorArgs : EventArgs
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}