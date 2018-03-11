using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace NetworkManager
{
    static class Network
    {
        //Error Codes
        private static string[] _ERROR_CODES = new string[]{
            "NO_ERROR",
            "UNKNOWN_ERROR",
            "INVALID_USERPASS",
            "INVALID_UID",
            "INVALID_FID",
            "INVALID_RID",
            "INVALID_EMAIL",
            "INVALID_CID",
            "CANT_FORM_ROOM",
            "CANT_SEND_CHAT",
            "CANNOT_KICK_PLAYER",
            "NO_UPDATES",
            "CANT_JOIN_ROOM",
        };
        //Commands
        private static string[] _COMMANDS = new string[]{
            "HELLO",
            "LOGIN",
            "REGISTER",
            "UPDATES",
            "DOUPDATES",
            "LISTR",
            "JROOM",
            "CHAT",
            "REQUEST",
            "LISTF",
            "LISTRF",
            "ADDF",
            "FORMR",
            "IHOST",
            "KICK",
            "INVITEF",
            "READY",
            "LEAVER",
            "GRESORCE",
        };

        public static string[] ERROR_CODES { get => _ERROR_CODES; set => _ERROR_CODES = value; }
        public static string[] COMMANDS { get => _COMMANDS; set => _COMMANDS = value; }

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
        public static event EventHandler<DataEventArgs> DataRecieved;
        public static event EventHandler<DataEventArgs> Chat;
        public static event EventHandler<DataEventArgs> FriendRequest;
        public static event EventHandler<DataEventArgs> GameStarted;
        public static event EventHandler<DataEventArgs> PlayerJoined;
        public static event EventHandler<DataEventArgs> PlayerLeft;
    }
    public class DataEventArgs : EventArgs
    {
        public string Response { get; set; }
    }
}