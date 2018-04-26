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
            if (error != Network.NO_ERROR)
            {
                objects = NetUtils.FormCommand(data, new string[] { "s" });
                ErrorArgs dat = new ErrorArgs();
                dat.ErrorCode = error;
                dat.ErrorMessage = (string) objects[0];
                Network.OnError(dat);
                return;
            }
            ChatDataArgs globalchat;
            switch (command)
            {
                case Network.HELLO:
                    Console.WriteLine("Got a hello from the server!");
                    Network.SendData(new byte[] { 0, 0 });
                    break;
                case Network.LOGIN:
                    objects = NetUtils.FormCommand(data, new string[] { "p", "s" });
                    Network.player = (PID)objects[0];
                    Network.self = (string)objects[1];
                    Network.INITCONNECTION(); // Set up certain data on the server
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
                    objects = NetUtils.FormCommand(data, new string[] { "s[]" });
                    string[] rids = (string[])objects[0];
                    RoomListArgs args = new RoomListArgs();
                    Room[] rooms = new Room[rids.Length];
                    for (int i = 0; i < rids.Length; i++)
                    {
                        rooms[i] = Network.GetRoom(rids[i]);
                    }
                    args.Rooms = rooms;
                    Network.OnRoomList(args);
                    break;
                case Network.JROOM:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    roomargs = new RoomUpdateArgs();
                    roomargs.Room = Network.GetRoom((string)objects[0]);
                    roomargs.Player = Network.GetPID((string)objects[1]);
                    if(roomargs.Player.GetID() == Network.player.GetID())
                    {
                        Network.CurrentRoom = roomargs.Room;
                    }
                    Network.CurrentRoom.AddMember(roomargs.Player);
                    break;
                case Network.CHAT:
                    objects = NetUtils.FormCommand(data, new string[] { "m" });
                    Message msgglobal = (Message)objects[0];
                    globalchat = new ChatDataArgs();
                    globalchat.Speaker = msgglobal.GetPID();
                    globalchat.Flag = Network.CHAT_GLOBAL;
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
                    objects = NetUtils.FormCommand(data, new string[] { "s[]" });
                    pids = (string[])objects[0];
                    FriendRequestArgs requestArr = new FriendRequestArgs();
                    PID[] req = new PID[pids.Length];
                    for (int i = 0; i < pids.Length; i++)
                    {
                        req[i] = Network.GetPID(pids[i]);
                    }
                    requestArr.Requests = req;
                    Network.OnFriendRequest(requestArr);
                    break;
                case Network.ADDF:
                    //Not needed
                    break;
                case Network.FORMR:
                    //Server will not send you this command
                    break;
                case Network.IHOST:
                    //Command not needed, server handles and sends this data over
                    break;
                case Network.KICK:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    roomargs = new RoomUpdateArgs();
                    roomargs.Room = Network.GetRoom((string)objects[0]);
                    roomargs.Player = Network.GetPID((string)objects[1]);
                    Network.OnPlayerLeft(roomargs);
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
                    roomargs = new RoomUpdateArgs();
                    roomargs.Room = Network.GetRoom((string)objects[0]);
                    roomargs.Player = Network.GetPID((string)objects[1]);
                    Network.CurrentRoom.RemoveMember(roomargs.Player);
                    break;
                case Network.GRESORCE:
                    // Need more data from kham
                    break;
                case Network.INIT:
                    Network.HANDSHAKEDONE = true; //This is a handshake that your ready for continuous datastreams
                    LoginEventArgs logargs = new LoginEventArgs();
                    logargs.response = "Logged in!";
                    Network.OnLogin(logargs);
                    break;
                case Network.CHATDM:
                    objects = NetUtils.FormCommand(data, new string[] { "m" });
                    Message msgdm = (Message)objects[0];
                    globalchat = new ChatDataArgs();
                    globalchat.Speaker = msgdm.GetPID();
                    globalchat.Flag = Network.CHATDM;
                    Network.OnChat(globalchat);
                    break;
                case Network.CHATRM:
                    objects = NetUtils.FormCommand(data, new string[] { "m", "s" });
                    Message msgrm = (Message)objects[0];
                    globalchat = new ChatDataArgs();
                    globalchat.Speaker = msgrm.GetPID();
                    globalchat.Room = Network.GetRoom((string)objects[1]);
                    globalchat.Flag = Network.CHATDM;
                    Network.OnChat(globalchat);
                    break;
                case Network.ROOMS:
                    objects = NetUtils.FormCommand(data, new string[] { "i" });
                    RoomCountArgs count = new RoomCountArgs();
                    count.count =(int)objects[0];
                    Network.OnRoomCount(count);
                    break;
                case Network.GROOM:
                    objects = NetUtils.FormCommand(data, new string[] { "r", "s" });
                    Network.rooms[(string)objects[1]] = (Room)objects[0];
                    break;
                case Network.GPID:
                    objects = NetUtils.FormCommand(data, new string[] { "p" });
                    PID temppid = (PID)objects[0];
                    Network.players[temppid.GetID()] = temppid;
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
                default:
                    Console.WriteLine("Unknown command!");
                    break;
            }
        }
    }
}
