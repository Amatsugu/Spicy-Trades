using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetworkManager
{
    public class CID
    {
        private PID player;
        private IPEndPoint connection;
        private Room currentRoom;
        public CID(PID self,IPEndPoint conn)
        {
            player = self;
            connection = conn;
        }
        public PID GetPID()
        {
            return player;
        }
        public IPEndPoint GetConn()
        {
            return connection;
        }
        public void SetCurrentRoom(Room room)
        {
            currentRoom = room;
        }
        public Room GetCurrentRoom()
        {
            return currentRoom;
        }
    }
    public static class ServerDataManager
    {
        //Unlike the client data will live in this class
        private static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();
        private static Dictionary<string, PID> PIDs = new Dictionary<string, PID>();
        public static Dictionary<string, CID> Connections = new Dictionary<string, CID>(); // the string here is the users session key
        public static void INIT()
        {
            Network.DataRecieved += OnDataRecieved;
        }
        public static void OnDataRecieved(object sender, DataRecievedArgs e)
        {
            byte command = e.RawResponse[0];
            Network.Retrieve(command);
            byte[] data = e.RawResponse.SubArray(1, e.RawResponse.Length-2);
            IPEndPoint send = e.SenderRef;
            object[] objects;
            string pid;
            string self;
            switch (command)
            {
                case Network.HELLO:
                    Console.WriteLine("Got a hello from the client!");
                    //Handle data for this
                    break;
                case Network.LOGIN:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s", "s" });
                    self = (string)objects[0];
                    string username = (string)objects[1];
                    string password = (string)objects[2];
                    //Handled by the TCP manager
                    string key="";
                    //Need to get the players name... You are not a friend of yourself are you?
                    Connections[key] = new CID(new PID(key, "", false), send);
                    break;
                case Network.REGISTER:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s", "s", "s" });//user pass, email
                    self = (string)objects[0];
                    string reg_username = (string)objects[1];
                    string reg_password = (string)objects[2];
                    string reg_email = (string)objects[3];
                    //Handled by the TCP manager
                    //GenUniqueSessionKey();
                    break;
                case Network.UPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.DOUPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.LISTR: //Gets the roomids
                    objects = NetUtils.FormCommand(data, new string[] { "s", "i", "i" });
                    self = (string)objects[0];
                    int pos = (int)objects[1];
                    int rcount = (int)objects[2];
                    break;
                case Network.JROOM:
                    //
                    break;
                case Network.CHAT:
                    //
                    break;
                case Network.REQUEST:
                    // Nothing is needed here
                    break;
                case Network.LISTF:
                    //
                    break;
                case Network.LISTRF:
                    //
                    break;
                case Network.ADDF:
                    //Not needed
                    break;
                case Network.FORMR:
                    //
                    break;
                case Network.IHOST:
                    //Command not needed, server handles and sends this data over
                    break;
                case Network.KICK:
                    //
                    break;
                case Network.INVITEF:
                    //
                    break;
                case Network.READY:
                    //
                    break;
                case Network.LEAVER: // only sent if you are in the room
                    //
                    break;
                case Network.GRESORCE:
                    // Need more data from kham
                    break;
                case Network.INIT:
                    break;
                case Network.CHATDM:
                    //
                    break;
                case Network.CHATRM:
                    //
                    break;
                case Network.ROOMS:
                    //
                    break;
                case Network.GROOM:
                    //
                    break;
                case Network.GPID:
                    //
                    break;
                default:
                    Console.WriteLine("Unknown command!");
                    break;
            }
        }
        public static string GenUniqueSessionKey()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
