using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NetworkManager
{
    public class CID
    {
        private PID player;
        private IPEndPoint connection;
        private Room currentRoom;

        public CID(PID self, IPEndPoint conn)
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
        public static Dictionary<string, CID> Connections = new Dictionary<string, CID>();
        // the string here is the users session key
        private static uint ROOMID = 0;
        // when converted to hex we get our 8byte id
        private static uint PLAYERID = 0;
        // when converted to hex we get our 8byte id
        public static void INIT()
        {
            SpicyNetwork.DataRecieved += OnDataRecieved;
        }

        public static string GenRoomID()
        {
            ROOMID++;
            return ROOMID.ToString("X8");
        }

        public static void OnDataRecieved(object sender, DataRecievedArgs e)
        {
            byte command = e.RawResponse[0];
            byte[] data = e.RawResponse.SubArray(1, e.RawResponse.Length - 1);
            IPEndPoint send = e.SenderRef;
            object[] objects;
            PID player;
            string pid;
            string self;
            Message msg;
            string roomid;
            string playerid;
            byte[] dat;
            string id = "";
            switch (command)
            {
                case SpicyNetwork.HELLO:
                    Console.WriteLine("Got a hello from the client!");
                    //Handle data for this
                    break;
                case SpicyNetwork.LOGIN:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    string username = (string)objects[0];
                    string password = (string)objects[1];
                    //Handled by the TCP manager
                    //COMPARE LOGINDATA
                    id = "12345678";
                    //FAKE STUFF HERE!!!!!!!!!
                    if (username == "epicknex" && password == "password")
                    {
                        Console.WriteLine("¯\\_(ツ)_/¯ you logged in nice!");
                    }
                    else
                    {
                        Console.WriteLine("¯\\_(ツ)_/¯ you done goof!");
                        SendError(SpicyNetwork.LOGIN, SpicyNetwork.INVALID_USERPASS, "Invalid Username or Password", send);
                        break;
                    }
                    //END OF FAKE STUFF
                    string sessionkey = GenUniqueSessionKey();
                    PID tempPID = new PID(id, username, false);
                    PIDs[id] = tempPID;
                    //Need to get the players name... You are not a friend of yourself are you?
                    Connections[sessionkey] = new CID(tempPID, send);
                    Console.WriteLine("Sending data");
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] {
                    SpicyNetwork.LOGIN,
                    SpicyNetwork.NO_ERROR,
                    tempPID,
                    sessionkey
                }), send);
                    break;
                case SpicyNetwork.REGISTER:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s", "s", "s" });//user pass, email
                    self = (string)objects[0];
                    string reg_username = (string)objects[1];
                    string reg_password = (string)objects[2];
                    string reg_email = (string)objects[3];
                    //Handled by the TCP manager
                    //GenUniqueSessionKey();
                    break;
                case SpicyNetwork.UPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case SpicyNetwork.DOUPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case SpicyNetwork.LISTR: //Gets the roomids
                    objects = NetUtils.FormCommand(data, new string[] { "s", "i", "i" });
                    self = (string)objects[0];
                    int pos = (int)objects[1];
                    int rcount = (int)objects[2];
                    string[] roomlist = new string[rcount];
                    int c = 0;
                    foreach (var kvp in Rooms.ToArray())
                    {
                        roomlist[c++] = Rooms[kvp.Key].GetRoomID();
                    }
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.LISTR, SpicyNetwork.NO_ERROR, roomlist }), send);
                    break;
                case SpicyNetwork.JROOM:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    roomid = (string)objects[1];
                    self = (string)objects[0];
                    player = GetPlayerPID(self);
                    playerid = player.GetID();
                    if (RoomExists(roomid) && Rooms[roomid].GetNumPlayers() != Room.MAX_MEMBERS)
                    {
                        Rooms[roomid].AddMember(player, true);
                        SendToRoom(NetUtils.PieceCommand(new object[] {
                        SpicyNetwork.JROOM,
                        SpicyNetwork.NO_ERROR,
                        roomid,
                        playerid
                    }), roomid);
                    }
                    else
                    {
                        SendError(SpicyNetwork.JROOM, SpicyNetwork.CANT_JOIN_ROOM, "Cannot join Room: Room full!", send);
                    }
                    break;
                case SpicyNetwork.CHAT:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "m" });
                    self = (string)objects[0];
                    msg = (Message)objects[1];
                    dat = NetUtils.PieceCommand(new object[] { SpicyNetwork.CHAT, SpicyNetwork.NO_ERROR });
                    SendToAll(dat);
                    break;
                case SpicyNetwork.REQUEST:
                    //self, playerid 
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    self = (string)objects[0];
                    playerid = (string)objects[1];
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] {
                        SpicyNetwork.REQUEST,
                        SpicyNetwork.NO_ERROR,
                        GetPlayerPID (self).GetID ()
                    }));
                    //TODO FINISH THIS! This needs the database, but will alert the user if online
                    break;
                case SpicyNetwork.LISTF:
                    //self
                    objects = NetUtils.FormCommand(data, new string[] { "s" });
                    //TODO FINISH THIS! Needs database
                    break;
                case SpicyNetwork.LISTRF:
                    //self
                    objects = NetUtils.FormCommand(data, new string[] { "s" });
                    //TODO FINISH THIS! Needs database
                    break;
                case SpicyNetwork.ADDF:
                    //self, playerid
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    //TODO FINISH THIS! Needs database
                    break;
                case SpicyNetwork.FORMR:
                    //self, roompassword
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    roomid = GenRoomID();
                    Rooms[roomid] = new Room(roomid, "", true);// Server is always a "host"
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.GROOM, Rooms[roomid] }), send);//Send a copy of the room data to the player
                    break;
                case SpicyNetwork.IHOST:
                    //Command not needed, server handles and sends this data over
                    break;
                case SpicyNetwork.KICK:
                    //self, roomid, playerid
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s", "s" });
                    self = (string)objects[0];
                    roomid = (string)objects[1];
                    playerid = (string)objects[2];
                    Rooms[roomid].Kick(PIDs[playerid]);
                    dat = NetUtils.PieceCommand(new object[] { SpicyNetwork.KICK, SpicyNetwork.NO_ERROR, roomid, playerid });
                    SendToRoom(dat, roomid);
                    break;
                case SpicyNetwork.INVITEF:
                    //self, playerid, roomid
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s", "s" });
                    self = (string)objects[0];
                    playerid = (string)objects[1];
                    roomid = (string)objects[2];
                    if (PlayerExists(playerid) && RoomExists(roomid) && ISOnfriendList(self, playerid))
                    {
                        SendToPlayer(NetUtils.PieceCommand(new object[] {
                        SpicyNetwork.INVITEF,
                        SpicyNetwork.NO_ERROR,
                        playerid,
                        roomid
                    }), playerid);
                    }
                    else
                    {
                        SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { }), send);
                        SendError(SpicyNetwork.INVITEF, SpicyNetwork.INVALID_FID, "Invalid playerid!", send);
                    }
                    break;
                case SpicyNetwork.READY:
                    //self, isready. roomid
                    objects = NetUtils.FormCommand(data, new string[] { "s", "bo", "s" });
                    self = (string)objects[0];
                    bool isReady = (bool)objects[1];
                    roomid = (string)objects[2];
                    Rooms[roomid].SetReady(isReady, GetPlayerPID(self));
                    dat = NetUtils.PieceCommand(new object[] {
                    SpicyNetwork.READY,
                    SpicyNetwork.NO_ERROR,
                    isReady,
                    GetPlayerPID (self).GetID ()
                });
                    SendToRoom(dat, roomid);
                    break;
                case SpicyNetwork.LEAVER: // only sent if you are in the room
                                     //self, roomid
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    self = (string)objects[0];
                    roomid = (string)objects[1];
                    Rooms[roomid].RemoveMember(GetPlayerPID(self));
                    dat = NetUtils.PieceCommand(new object[] {
                    SpicyNetwork.LEAVER,
                    SpicyNetwork.NO_ERROR,
                    GetPlayerPID (self).GetID ()
                });
                    SendToRoom(dat, roomid);
                    break;
                case SpicyNetwork.INIT:
                    self = (string)NetUtils.FormCommand(data, new string[] { "s" })[0];
                    GetPlayerPID(self).SetConnection(send); // Allows for faster player lookup
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.INIT, SpicyNetwork.NO_ERROR }), send);
                    break;
                case SpicyNetwork.CHATDM:
                    //self, msg, playerid
                    objects = NetUtils.FormCommand(data, new string[] { "s", "m", "s" });
                    self = (string)objects[0];
                    msg = (Message)objects[1];
                    playerid = (string)objects[2];
                    dat = NetUtils.PieceCommand(new object[] { SpicyNetwork.CHAT, SpicyNetwork.NO_ERROR });
                    SendToRoom(dat, playerid);
                    break;
                case SpicyNetwork.CHATRM:
                    //self, msg, roomid
                    objects = NetUtils.FormCommand(data, new string[] { "s", "m", "s" });
                    self = (string)objects[0];
                    msg = (Message)objects[1];
                    roomid = (string)objects[2];
                    dat = NetUtils.PieceCommand(new object[] { SpicyNetwork.CHAT, SpicyNetwork.NO_ERROR });
                    SendToRoom(dat, roomid);
                    break;
                case SpicyNetwork.ROOMS:
                    objects = NetUtils.FormCommand(data, new string[] { "s" });
                    self = (string)objects[0];
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.ROOMS, SpicyNetwork.NO_ERROR, Rooms.Count }), send);
                    break;
                case SpicyNetwork.GROOM:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    self = (string)objects[0];
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.GROOM, SpicyNetwork.NO_ERROR, Rooms[(string)objects[1]] }), send);
                    break;
                case SpicyNetwork.GPID:
                    Console.WriteLine("Get Request!");
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    self = (string)objects[0];
                    string key = (string)objects[1];
                    if (PIDs.ContainsKey(key))
                    {
                        Console.WriteLine("Sending PID data");
                        SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.GPID, SpicyNetwork.NO_ERROR, PIDs[key] }), send);
                    }
                    else
                    {
                        Console.WriteLine("NO PID" + key);
                        SendError(SpicyNetwork.GPID, SpicyNetwork.INVALID_UID, "Cannot retreive that pid data", send);
                    }
                    break;
                case SpicyNetwork.SYNC:
                    //self, randomid, groups.Length, i, groups[i] 
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    self = (string)objects[0];
                    id = (string)objects[1];
                    int numpieces = (int)objects[2];
                    int cpiece = (int)objects[3];
                    string payload = (string)objects[4];
                    dat = NetUtils.PieceCommand(new object[] {
                    SpicyNetwork.SYNC,
                    SpicyNetwork.NO_ERROR,
                    id,
                    numpieces,
                    cpiece,
                    payload
                });
                    SendToRoom(dat, Connections[self].GetCurrentRoom().GetRoomID());// remove the self tag and send the data to the clients
                    break;
                case SpicyNetwork.LOGOUT:
                    self = (string)(NetUtils.FormCommand(data, new string[] { "s" })[0]);
                    pid = GetPlayerPID(self).GetID();
                    Connections[self] = null;
                    PIDs[pid] = null;
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.LOGOUT, SpicyNetwork.NO_ERROR }), send);
                    break;
                default:
                    Console.WriteLine("Unknown command!");
                    break;
            }
        }

        public static PID GetPlayerPID(string cid)
        {
            return Connections[cid].GetPID(); //Get the playerid from self(the cid)
        }

        public static bool RoomExists(string roomid0)
        {
            return Rooms[roomid0] != null;
        }

        public static bool PlayerExists(string roomid0)
        {
            return PIDs[roomid0] != null;
        }

        public static bool ISOnfriendList(string cid, string player)
        {
            // Need the database to be done
            return true; // FIX THIS!
        }

        public static void SendToAll(byte[] data)
        {
            foreach (var kvp in Connections.ToArray())
            {
                SpicyNetwork.SendData(data, kvp.Value.GetConn());
            }
        }

        public static void SendToRoom(byte[] data, string roomid)
        {
            PID[] pids = Rooms[roomid].GetMembers();
            for (int i = 0; i < pids.Length; i++)
            {
                if (pids[i] != null)
                {
                    SpicyNetwork.SendData(data, pids[i].GetConnection());
                }
            }
        }

        public static void SendToAll(byte[] data, string self)
        {
            PID temp = GetPlayerPID(self);
            foreach (var kvp in Connections.ToArray())
            {
                if (temp != kvp.Value.GetPID())
                {
                    SpicyNetwork.SendData(data, kvp.Value.GetConn());
                }
            }
        }

        public static void SendToRoom(byte[] data, string roomid, string self)
        {
            PID[] pids = Rooms[roomid].GetMembers();
            PID temp = GetPlayerPID(self);
            for (int i = 0; i < pids.Length; i++)
            {
                if (pids[i] != null && pids[i] != temp)
                {
                    SpicyNetwork.SendData(data, pids[i].GetConnection());
                }
            }
        }

        public static void SendToPlayer(byte[] data, string playerid)
        {
            SpicyNetwork.SendData(data, PIDs[playerid].GetConnection());
        }

        public static string GenUniqueSessionKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static void SendError(byte cmd, byte err, string msg, IPEndPoint send)
        {
            SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { cmd, err, msg }), send);
        }
    }
}
