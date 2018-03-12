using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace NetworkManager
{
    static class Network
    {
        //Data Types
        public static byte PID       = 0x00;
        public static byte ROOM      = 0x01;
        public static byte MESSAGE   = 0x02;
        //Commands
        public static byte HELLO         = 0x00;
        public static byte LOGIN         = 0x01;
        public static byte REGISTER      = 0x02;
        public static byte UPDATES       = 0x03;
        public static byte DOUPDATES     = 0x04;
        public static byte LISTR         = 0x05;
        public static byte JROOM         = 0x06;
        public static byte CHAT          = 0x07;
        public static byte REQUEST       = 0x08;
        public static byte LISTF         = 0x09;
        public static byte LISTRF        = 0x0A;
        public static byte ADDF          = 0x0B;
        public static byte FORMR         = 0x0C;
        public static byte IHOST         = 0x0D;
        public static byte KICK          = 0x0E;
        public static byte INVITEF       = 0x0F;
        public static byte READY         = 0x10;
        public static byte LEAVER        = 0x11;
        public static byte GRESORCE      = 0x12;
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
        public static Client Connect(string ip, int port)
        {
            IPHostEntry ipHostInfo;
            IPAddress ipAddress;
            try
            {
                ipHostInfo = Dns.GetHostEntry(ip);
                ipAddress = ipHostInfo.AddressList[0];
            }
            catch
            {
                // Looks like we could not connect!
                throw new Exception("Unable to connect to the server! Unknown Hostname: " + ip);
            }
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                return new Client(client, remoteEP, ipHostInfo, ipAddress, ip, port);
            }
            catch
            {
                throw new Exception("Unable to connect to the server! Is the server running? " + ip + ":" + port);
            }
        }
        public static void OnDataRecieved(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = DataRecieved;
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
        public static void OnError(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = Error;
            handler(null, e);
        }
        public static event EventHandler<DataEventArgs> DataRecieved;
        public static event EventHandler<DataEventArgs> Chat;
        public static event EventHandler<DataEventArgs> FriendRequest;
        public static event EventHandler<DataEventArgs> GameStarted;
        public static event EventHandler<DataEventArgs> PlayerJoined;
        public static event EventHandler<DataEventArgs> PlayerLeft;
        public static event EventHandler<DataEventArgs> Error;
    }
    public class DataEventArgs : EventArgs
    {
        public string Response { get; set; }
        public int errorcode { get; set; }
        public byte[] RawResponse { get; set; }
        public object ObjectRef { get; set; }
    }
}