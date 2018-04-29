using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    public static class ClientDataManager
    {
        public static void INIT()
        {
            Network.DataRecieved += OnDataRecieved;
        }
        static Dictionary<string, Dictionary<int,string>> SYNC_CACHE = new Dictionary<string, Dictionary<int, string>>();
        public static void OnDataRecieved(object sender, DataRecievedArgs e)
        {
            byte command = e.RawResponse[0];
            Network.Retrieve(command);
            byte error = e.RawResponse[1];
            byte[] data = e.RawResponse.SubArray(2, e.RawResponse.Length-2);
            RoomUpdateArgs roomargs;
            //If there is an error you'll get the command byte[0] error byte[1] and a string of what the error is
            object[] objects;
            string[] pids;
            ChatDataArgs globalchat;
            switch (command)
            {
                case Network.HELLO:
                    Console.WriteLine("Got a hello from the server!");
                    Network.SendData(new byte[] { 0, 0 });
                    break;
                case Network.LOGIN:
                    //FIXED
                    break;
                case Network.REGISTER:
                    //Registering was successful! Make callback?
                    break;
                case Network.UPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.DOUPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.LISTR: //Gets the roomids
                    // CHANGED
                    break;
                case Network.JROOM:
                    //FIXED
                    break;
                case Network.CHAT:
                    objects = NetUtils.FormCommand(data, new string[] { "m" });
                    Message msgglobal = (Message)objects[0];
                    globalchat = new ChatDataArgs();
                    globalchat.Flag = Network.CHAT_GLOBAL;
                    globalchat.Message = msgglobal;
                    Network.OnChat(globalchat);
                    break;
                case Network.REQUEST:
                    // Nothing is needed here
                    break;
                case Network.LISTF:
                    objects = NetUtils.FormCommand(data, new string[] { "s[]" });
                    pids = (string[])objects[0];
                    FriendsArgs friendArr = new FriendsArgs();
                    PID[] friends = new PID[pids.Length];
                    for (int i = 0; i < pids.Length; i++)
                    {
                        friends[i] = Network.GetPID(pids[i]);
                    }
                    friendArr.Friends = friends;
                    Network.OnFriendsList(friendArr);
                    break;
                case Network.LISTRF:
                    //Fixed
                    break;
                case Network.ADDF:
                    //Not needed
                    break;
                case Network.FORMR:
                    // FIXED
                    break;
                case Network.IHOST:
                    //Command not needed, server handles and sends this data over
                    break;
                case Network.KICK:
                    //FIXED
                    break;
                case Network.INVITEF:
                    objects = NetUtils.FormCommand(data, new string[] {  "s" });
                    string playerid = (string)objects[0];
                    //HOOK EVENT
                    break;
                case Network.READY:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "bool" });
                    Network.CurrentRoom.SetReady((bool)objects[1], Network.GetPID((string)objects[0]));
                    break;
                case Network.LEAVER: // only sent if you are in the room
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    Network.CurrentRoom.RemoveMember(Network.GetPID((string)objects[1]));
                    break;
                case Network.GRESORCE:
                    //NOT NEEDED
                    break;
                case Network.INIT:
                    Network.HANDSHAKEDONE = true; //This is a handshake that your ready for continuous datastreams
                    //Fixed
                    break;
                case Network.CHATDM:
                    objects = NetUtils.FormCommand(data, new string[] { "m" });
                    Message msgdm = (Message)objects[0];
                    globalchat = new ChatDataArgs();
                    globalchat.Flag = Network.CHATDM;
                    globalchat.Message = msgdm;
                    Network.OnChat(globalchat);
                    break;
                case Network.CHATRM:
                    objects = NetUtils.FormCommand(data, new string[] { "m", "s" });
                    Message msgrm = (Message)objects[0];
                    globalchat = new ChatDataArgs();
                    globalchat.Room = Network.GetRoom((string)objects[1]);
                    globalchat.Flag = Network.CHATDM;
                    globalchat.Message = msgrm;
                    Network.OnChat(globalchat);
                    break;
                case Network.ROOMS:
                    //FIXED
                    break;
                case Network.GROOM:
                    objects = NetUtils.FormCommand(data, new string[] { "r" });
                    Room temp = (Room)objects[0];
                    Network.rooms[temp.GetRoomID()] = temp;
                    GotRoomEventArgs room = new GotRoomEventArgs();
                    room.Room = temp;
                    Network.OnRoomDataRecieved(room);
                    break;
                case Network.GPID:
                    objects = NetUtils.FormCommand(data, new string[] { "p" });
                    PID temppid = (PID)objects[0];
                    Network.players[temppid.GetID()] = temppid;
                    GotPIDEventArgs pid = new GotPIDEventArgs();
                    pid.Pid = (PID)objects[0];
                    Network.OnPIDDataRecieved(pid);
                    break;
                case Network.SYNC://id, numpieces, cpiece, payload
                    objects = NetUtils.FormCommand(data, new string[] { "s", "n", "n", "s" });
                    string syncid = (string)objects[0];
                    int numpieces = (int)objects[1];
                    int cpiece = (int)objects[2];
                    string piece = (string)objects[3];
                    if (SYNC_CACHE.ContainsKey(syncid))
                    {
                        SYNC_CACHE[syncid][cpiece] = piece;
                    }
                    else
                    {
                        var dict = new Dictionary<int, string>();
                        dict.Add(cpiece, piece);
                        SYNC_CACHE.Add(syncid, dict);
                    }
                    int PCount = SYNC_CACHE[syncid].Count;
                    string whole = "";
                    if (PCount == numpieces){
                        for(int i = 0; i < PCount; i++)
                        {
                            whole += SYNC_CACHE[syncid][i];
                        }
                        SyncEventArgs fulldata = new SyncEventArgs();
                        fulldata.Data = whole;
                        Network.OnSyncData(fulldata);
                    }
                    break;
                case Network.LOGOUT:
                    Console.WriteLine("Logged out!");
                    break;
                default:
                    Console.WriteLine("Unknown command!");
                    break;
            }
        }
    }
}
