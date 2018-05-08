using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;

namespace NetworkManager
{
    public static class SpicyNetwork
    {
        //Data Types
        public const byte PID = 0x00; //Player ID This is sent once, then player id's are used
        public const byte ROOM = 0x01; //Room data This is sent once then room id's are used
        public const byte MESSAGE = 0x02; //This is the message object that is sent when needed
        //BOOLS
        public const byte TRUE = 0x00;
        public const byte FALSE = 0xFF;
        //Commands
        public const byte HELLO = 0x00; //The server needs to know if you're still there
        public const byte LOGIN = 0x01; //Logins in the user
        public const byte REGISTER = 0x02; //Register an account
        public const byte UPDATES = 0x03; //CheckFor updates
        public const byte DOUPDATES = 0x04; //Gets the update files from the server
        public const byte LISTR = 0x05; //Gets a list of rooms returns Room[] in callback
        public const byte JROOM = 0x06; //Joins a room
        public const byte CHAT = 0x07; //Sends chat to global chat
        public const byte REQUEST = 0x08; //Sends a friend request
        public const byte LISTF = 0x09; //Gets list of friends
        public const byte LISTRF = 0x0A; //Gets list of friend requests
        public const byte ADDF = 0x0B; //Adds a friend who has sent you a request
        public const byte FORMR = 0x0C; //Creates a room
        public const byte IHOST = 0x0D; //Are you the host of a room
        public const byte KICK = 0x0E; //kick a player from a room
        public const byte INVITEF = 0x0F; //invites a friend to a room
        public const byte READY = 0x10; //Tell the server you are ready
        public const byte LEAVER = 0x11; //Leave a room
        public const byte SYNCB = 0x12; //Get resource
        public const byte INIT = 0x13; //INIT
        public const byte CHATDM = 0x14; //sends a chat to a player
        public const byte CHATRM = 0x15; //sends a chat to a room
        public const byte ROOMS = 0x16; //Gets a room count
        public const byte GROOM = 0x17; //gets a room object from a roomid
        public const byte GPID = 0x18; //gets a pid object from a playerid
        public const byte SYNC = 0x19; //Syncs data between rooms
        public const byte LOGOUT = 0x1A; //Tell the server you want to log out
        public const byte SENDFR = 0x1B; //Tell the server you want to log out
        public const byte LEAVERO = 0x1C; //Tell the server you want to log out
        public const byte READYO = 0x1D; //Tell the server you are ready
        public const byte SYNCO = 0x1E;
        public const byte RELAY = 0x1F;
        public const byte JOINO = 0x20;
        //Error Codes
        public const byte NO_ERROR = 0x00;
        public const byte UNKNOWN_ERROR = 0x01;
        public const byte INVALID_USERPASS = 0x02;
        public const byte INVALID_UID = 0x03;
        public const byte INVALID_FID = 0x04;
        public const byte INVALID_RID = 0x05;
        public const byte INVALID_EMAIL = 0x06;
        public const byte INVALID_CID = 0x07;
        public const byte CANT_FORM_ROOM = 0x08;
        public const byte CANT_SEND_CHAT = 0x09;
        public const byte CANNOT_KICK_PLAYER = 0x0A;
        public const byte NO_UPDATES = 0x0B;
        public const byte CANT_JOIN_ROOM = 0x0C;
        public const byte MESSAGE_TO_LARGE = 0x0D;
        public const byte ROOM_ERROR = 0x0E;
        //Chat codes
        public const byte CHAT_DM = 0x00;
        public const byte CHAT_RM = 0x01;
        public const byte CHAT_GLOBAL = 0x02;
        //Client stuff
        public static UdpClient connection;
        public static string self="0000000000000000000000000000000000000";
        public static PID player;
        public static Dictionary<string, PID> players = new Dictionary<string, PID>();
        public static Dictionary<string, Room> rooms = new Dictionary<string, Room>();
        public static Room CurrentRoom;
        public static bool HANDSHAKEDONE = false;
        public static object[] objects;
        public static bool Wait = false;
        public static byte[] lastMessage = null;
        public static byte[] capturedMsg = null;
        public static Dictionary<string, Message> msgCache = new Dictionary<string, Message>();
        public static bool Connect(string ip, int port)
        {
            ClientDataManager.INIT();
            connection = new UdpClient(port);
            connection.Client.ReceiveTimeout = 1;
            try
            {
                connection.Connect(ip, port);
                Thread t = new Thread(new ThreadStart(ThreadProc));
                t.Start();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public static PID GetPID(string pid)
        {
            {
                if (players.ContainsKey(pid))
                {
                    return players[pid];
                }
                else
                {
                    Wait = true;
                    byte[] temp = NetUtils.PieceCommand(new object[] { GPID, self, pid });
                    SendData(temp);
                    byte[] tmprec = ClientHoldManager();
                    var time = DateTime.Now;
                    while (tmprec == null && !(lastMessage == null && capturedMsg != null))
                    {
                        tmprec = ClientHoldManager();
                        if (tmprec != null && tmprec[0] != GPID)
                            tmprec = null;
                        if ((DateTime.Now - time).TotalSeconds > 3)
                        {
                            Wait = false;
                            return null;
                        }
                    }
                    if (capturedMsg != null && lastMessage == null)
                    {
                        tmprec = capturedMsg;
                        capturedMsg = null;
                    }
                    Wait = false;
                    byte error = tmprec[1];
                    byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
                    if (error != 0)
                    {
                        Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                        return null;
                    }
                    try
                    {
                        objects = NetUtils.FormCommand(data, new string[] { "p" });
                    } catch
                    {
                        return GetPID(pid);
                    }
                    PID temppid = (PID)objects[0];
                    players[temppid.GetID()] = temppid;
                    return players[pid];
                }
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
                Wait = true;
                byte[] temp = NetUtils.PieceCommand(new object[] { GROOM, self, rid });
                SendData(temp);
                byte[] tmprec = ClientHoldManager();
                var time = DateTime.Now;
                while (tmprec == null && !(lastMessage == null && capturedMsg != null))
                {
                    tmprec = ClientHoldManager();
                    if (tmprec != null && tmprec[0] != GROOM)
                        tmprec = null;
                    if ((DateTime.Now - time).TotalSeconds > 3)
                    {
                        Wait = false;
                        return null;
                    }
                }
                if (capturedMsg != null && lastMessage == null)
                {
                    tmprec = capturedMsg;
                    capturedMsg = null;
                }
                Wait = false;
                byte error = tmprec[1];
                byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
                if (error != 0)
                {
                    Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                    return null;
                }
                try
                {
                    objects = NetUtils.FormCommand(data, new string[] { "r" });
                }
                catch
                {
                    return GetRoom(rid);
                }
                Room rtemp = (Room)objects[0];
                rooms[rtemp.GetRoomID()] = rtemp;
                return rooms[rid];
            }
        }
        public static bool RegisterAccount(string email, string user, string pass)
        {
            Wait = true;
            SendData(NetUtils.PieceCommand(new object[] { REGISTER, email, user, pass }));
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != LOGIN)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static bool Login(string user, string pass)
        {
            Wait = true;
            SendData(NetUtils.PieceCommand(new object[] { LOGIN, user, pass }));
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != LOGIN)
                    tmprec = null;
                if((DateTime.Now-time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            objects = NetUtils.FormCommand(data, new string[] { "p", "s" });
            player = (PID)objects[0];
            self = (string)objects[1];
            INITCONNECTION(); // Set up certain data on the server
            return true;
        }
        public static void Consume(byte[] cmd) //Consumes commands
        {
            lastMessage = cmd;
            connection.Client.ReceiveTimeout = 500;
        }
        public static void ResendData()
        {
            if (lastMessage != null)
            {
                SendData(lastMessage);
            }
        }
        public static Room CreateRoom(string password = "NONE") //DONE
        {
            //Might need to send map data as well
            byte[] temp = NetUtils.PieceCommand(new object[] { FORMR, self, password });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != FORMR)
                {
                    tmprec = null;
                }
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
                Console.WriteLine("Waiting for room data! " + (lastMessage == null && capturedMsg != null));
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return null;
            }
            objects = NetUtils.FormCommand(data, new string[] { "s" });
            string tempS = (string)objects[0];
            Room tempR = GetRoom(tempS);
            rooms[tempR.GetRoomID()] = tempR;
            CurrentRoom = tempR;
            return tempR;
        }
        public static Room JoinRoom(string roomid) //DONE
        {//This will trigger the player joined room event with you as the argument
            byte[] temp = NetUtils.PieceCommand(new object[] { JROOM, self, roomid });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != JROOM)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return null;
            }
            objects = NetUtils.FormCommand(data, new string[] { "s" });
            string tempS = (string)objects[0];
            Room tempR = GetRoom(tempS);
            rooms[tempR.GetRoomID()] = tempR;
            CurrentRoom = tempR;
            return tempR;
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
        public static string Logout()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LOGOUT, self });
            Wait = true;
            SendData(temp);
            SendData(NetUtils.PieceCommand(new object[] { LOGOUT }));
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != LOGIN)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            return self;
        }
        public static int? GetNumberOfRooms() //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { ROOMS, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != ROOMS)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return null;
            }
            objects = NetUtils.FormCommand(data, new string[] { "i" });
            return (int)objects[0];
        }
        public static Room[] ListRooms(int pos = 0, int count = 100) //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LISTR, self, pos, count });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != LISTR)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return null;
            }
            objects = NetUtils.FormCommand(data, new string[] { "s[]" });
            string[] rids = (string[])objects[0];
            RoomListArgs args = new RoomListArgs();
            Room[] rooms = new Room[rids.Length];
            for (int i = 0; i < rids.Length; i++)
            {
                rooms[i] = GetRoom(rids[i]);
            }
            return rooms;
        }
        public static void Sync(string data) //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { SYNC, false, self, data }); // 1 packet of data being sent... the server will simply mirror this data
            SendData(temp);
        }
        public static void Sync(byte[] data) //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { SYNC, true, self, data }); // 1 packet of data being sent... the server will simply mirror this data
            SendData(temp);
        }
        public static bool SendFriendRequest(string playerid) //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { SENDFR, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != SENDFR)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static PID[] ListFriends() //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LISTF, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != LISTF)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return null;
            }
            objects = NetUtils.FormCommand(data, new string[] { "s[]" });
            string[] pids = (string[])objects[0];
            PID[] req = new PID[pids.Length];
            for (int i = 0; i < pids.Length; i++)
            {
                req[i] = GetPID(pids[i]);
            }
            return req;
        }
        public static PID[] ListRequestedFriends() //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LISTRF, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != LISTRF)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return null;
            }
            objects = NetUtils.FormCommand(data, new string[] { "s[]" });
            string[] pids = (string[])objects[0];
            PID[] req = new PID[pids.Length];
            for (int i = 0; i < pids.Length; i++)
            {
                req[i] = GetPID(pids[i]);
            }
            return req;
        }
        public static bool AddFriend(string playerid) //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { ADDF, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != ADDF)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static bool? IsHostOf() //DONE
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { IHOST, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != IHOST)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return null;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return null;
            }
            return (bool)NetUtils.FormCommand(data, new string[] { "bo" })[0];
        }
        public static bool KickPlayer(string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { KICK, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != KICK)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static bool InviteFriend(string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { INVITEF, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != INVITEF)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static bool SetReady(bool ready)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { READY, self, ready });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != READY)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static bool LeaveRoom()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { LEAVER, self });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != LEAVER)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static void INITCONNECTION()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { INIT, self });
            SendData(temp);
        }
        public static bool SayHello()
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { HELLO });
            Wait = true;
            SendData(temp);
            byte[] tmprec = ClientHoldManager();
            var time = DateTime.Now;
            while (tmprec == null && !(lastMessage == null && capturedMsg != null))
            {
                tmprec = ClientHoldManager();
                if (tmprec != null && tmprec[0] != HELLO)
                    tmprec = null;
                if ((DateTime.Now - time).TotalSeconds > 3)
                {
                    Wait = false;
                    return false;
                }
            }
            if (capturedMsg != null && lastMessage == null)
            {
                tmprec = capturedMsg;
                capturedMsg = null;
            }
            Wait = false;
            byte error = tmprec[1];
            byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            if (error != 0)
            {
                Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
                return false;
            }
            return true;
        }
        public static void SendDM(Message msg, string playerid)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { CHATDM, self, msg, playerid });
            SendData(temp, false);
            //byte[] tmprec = ClientHoldManager();
            //while (tmprec == null && !(lastMessage==null && capturedMsg!=null))
            //{
            //    tmprec = ClientHoldManager();
            //    if (tmprec != null && tmprec[0] != CHATDM)
            //        tmprec = null;
            //}
            //Wait = false;
            //byte error = tmprec[1];
            //byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            //if (error != 0)
            //{
            //    Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
            //    return false;
            //}
            //return true;
        }
        public static void SendRoomChat(Message msg)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { CHATRM, self, msg });
            SendData(temp, false);
            //byte[] tmprec = ClientHoldManager();
            //while (tmprec == null && !(lastMessage==null && capturedMsg!=null))
            //{
            //    tmprec = ClientHoldManager();
            //    if (tmprec != null && tmprec[0] != CHATRM)
            //        tmprec = null;
            //}
            //Wait = false;
            //byte error = tmprec[1];
            //byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            //if (error != 0)
            //{
            //    Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
            //    return false;
            //}
            //return true;
        }
        public static void SendChat(Message msg)
        {
            byte[] temp = NetUtils.PieceCommand(new object[] { CHAT, self, msg });
            SendData(temp, false);
            //byte[] tmprec = ClientHoldManager();
            //while (tmprec == null && !(lastMessage==null && capturedMsg!=null))
            //{
            //    tmprec = ClientHoldManager();
            //    if (tmprec != null && tmprec[0] != CHAT)
            //        tmprec = null;
            //}
            //Wait = false;
            //byte error = tmprec[1];
            //byte[] data = tmprec.SubArray(2, tmprec.Length - 2);
            //if (error != 0)
            //{
            //    Console.WriteLine((string)NetUtils.FormCommand(data, new string[] { "s" })[0]);
            //    return false;
            //}
            //return true;
        }
        public static void SendData(byte[] data, bool doit = true)
        {
            if (doit)
                Consume(data);
            if (data != null)
                connection.Send(data, data.Length);
        }
        public static void SendData(byte[] data, IPEndPoint sender)
        {
            connection.Send(data, data.Length, sender);
        }
        public static IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        public static void ThreadProc()
        {
            while (true)
            {
                if (!Wait)
                    DoMainClientStuff();
            }
        }
        public static void DoMainClientStuff()
        {
            try
            {
                Byte[] receiveBytes = connection.Receive(ref RemoteIpEndPoint);
                if (receiveBytes.Length > 0)
                {

                    DataRecievedArgs data = new DataRecievedArgs();
                    data.RawResponse = receiveBytes;
                    OnDataRecieved(data);
                    if (lastMessage[0] == receiveBytes[0])
                    {
                        capturedMsg = receiveBytes;
                        lastMessage = null;
                    }
                }
            }
            catch
            {
                ResendData();
            }
        }
        public static byte[] ClientHoldManager()
        {
            try
            {
                Byte[] receiveBytes = connection.Receive(ref RemoteIpEndPoint);
                if (receiveBytes.Length > 0)
                {
                    connection.Client.ReceiveTimeout = 1;
                    lastMessage = null;
                    DataRecievedArgs data = new DataRecievedArgs();
                    data.RawResponse = receiveBytes;
                    OnDataRecieved(data);
                    return receiveBytes;
                }
            }
            catch
            {
                if (Wait)
                {
                    ResendData();
                }
            }
            return null;
        }
        /* -----------
         * EVENT STUFF
         * -----------
         */
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
        public static void OnSyncData(SyncEventArgs e)
        {
            EventHandler<SyncEventArgs> handler = SyncData;
            handler(null, e);
        }
        public static void OnLogin(LoginEventArgs e)
        {
            EventHandler<LoginEventArgs> handler = LoggedIn;
            handler(null, e);
        }
        public static void OnRegistered(RegisteredEventArgs e)
        {
            EventHandler<RegisteredEventArgs> handler = Registered;
            handler(null, e);
        }
        public static void OnRoomDataRecieved(GotRoomEventArgs e)
        {
            EventHandler<GotRoomEventArgs> handler = GotRoomData;
            handler(null, e);
        }
        public static void OnPIDDataRecieved(GotPIDEventArgs e)
        {
            EventHandler<GotPIDEventArgs> handler = GotPIDData;
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
        public static event EventHandler<SyncEventArgs> SyncData;
        public static event EventHandler<LoginEventArgs> LoggedIn;
        public static event EventHandler<RegisteredEventArgs> Registered;
        public static event EventHandler<GotRoomEventArgs> GotRoomData;
        public static event EventHandler<GotPIDEventArgs> GotPIDData;
    }
    public class SyncEventArgs : EventArgs
    {
        public int Type { get; set; }
        public string SData { get; set; }
        public byte[] BData { get; set; }
    }
    public class GotRoomEventArgs : EventArgs
    {
        public Room Room { get; set; }
    }
    public class GotPIDEventArgs : EventArgs
    {
        public PID Pid { get; set; }
    }
    public class RegisteredEventArgs : EventArgs
    {
        public string response { get; set; }
    }
    public class LoginEventArgs : EventArgs
    {
        public string response { get; set; }
    }
    public class DataRecievedArgs : EventArgs
    {
        public string Response { get; set; }
        public byte[] RawResponse { get; set; }
        public IPEndPoint SenderRef { get; set; }
    }
    public class ErrorArgs : EventArgs
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class MessageArgs : EventArgs
    {
        public string Message { get; set; }
        public PID Player { get; set; }
    }
    public class BasicResponseArgs : EventArgs
    {
        public string Response { get; set; }
    }
    public class RoomUpdateArgs : EventArgs
    {
        public PID Player { get; set; }
        public Room Room { get; set; }
        public bool Ready { get; set; }
        public byte Command { get; set; }
    }
    public class RoomCountArgs : EventArgs
    {
        public int count { get; set; }
    }
    public class RoomListingArgs : EventArgs
    {
        public Room[] Rooms { get; set; }
    }
    public class FriendsArgs : EventArgs
    {
        public PID[] Friends { get; set; }
    }
    public class FriendRequestArgs : EventArgs
    {
        public PID[] Requests { get; set; }
    }
    public class RoomIsHostArgs : EventArgs
    {
        public bool host { get; set; }
        public Room Room { get; set; }
    }
    public class RoomListArgs : EventArgs
    {
        public Room[] Rooms { get; set; }
    }
    public class ResourceListArgs : EventArgs
    {
        //Need info from Kham
    }
    public class ChatDataArgs : EventArgs
    {
        public Message Message { get; set; }
        public int Flag { get; set; }
        public Room Room { get; set; }
    }
}