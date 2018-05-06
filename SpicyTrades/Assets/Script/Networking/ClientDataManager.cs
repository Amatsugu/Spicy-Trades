using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    public static class ClientDataManager
    {
        public static void INIT()
        {
            SpicyNetwork.DataRecieved += OnDataRecieved;
        }
        static Dictionary<string, Dictionary<int,string>> SYNC_CACHE = new Dictionary<string, Dictionary<int, string>>();
        public static void OnDataRecieved(object sender, DataRecievedArgs e)
        {
            byte command = e.RawResponse[0];
            byte error = e.RawResponse[1];
            byte[] data = e.RawResponse.SubArray(2, e.RawResponse.Length-2);
            RoomUpdateArgs roomargs;
            //If there is an error you'll get the command byte[0] error byte[1] and a string of what the error is
            object[] objects;
            string[] pids;
            ChatDataArgs globalchat;
            switch (command)
            {
                case SpicyNetwork.HELLO:
                    Console.WriteLine("Got a hello from the server!");
                    SpicyNetwork.SendData(new byte[] { 0, 0 });
                    break;
                case SpicyNetwork.REQUEST:
                    // Nothing is needed here
                    break;
                case SpicyNetwork.LISTF:
                    objects = NetUtils.FormCommand(data, new string[] { "s[]" });
                    pids = (string[])objects[0];
                    FriendsArgs friendArr = new FriendsArgs();
                    PID[] friends = new PID[pids.Length];
                    for (int i = 0; i < pids.Length; i++)
                    {
                        friends[i] = SpicyNetwork.GetPID(pids[i]);
                    }
                    friendArr.Friends = friends;
                    SpicyNetwork.OnFriendsList(friendArr);
                    break;
                case SpicyNetwork.INVITEF:
                    objects = NetUtils.FormCommand(data, new string[] {  "s" });
                    string playerid = (string)objects[0];
                    //HOOK EVENT
                    break;
                case SpicyNetwork.JOINO:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    playerid = (string)objects[0];
                    Console.WriteLine(playerid+"|"+ (string)objects[1]);
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self, (string)objects[1] }), false);
                    SpicyNetwork.CurrentRoom.AddMember(SpicyNetwork.GetPID(playerid), true);
                    break;
                case SpicyNetwork.READYO:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "bool", "s" });
                    SpicyNetwork.CurrentRoom.SetReady((bool)objects[1], SpicyNetwork.GetPID((string)objects[0]));
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self, (string)objects[2] }), false);
                    Console.WriteLine("Player Set Ready!");
                    break;
                case SpicyNetwork.LEAVERO: // only sent if you are in the room
                    Console.WriteLine("Removing member!");
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s", "s" });
                    SpicyNetwork.CurrentRoom.RemoveMember(SpicyNetwork.GetPID((string)objects[1]));
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self, (string)objects[2] }), false);
                    break;
                case SpicyNetwork.INIT:
                    SpicyNetwork.HANDSHAKEDONE = true; //This is a handshake that your ready for continuous datastreams
                    break;
                case SpicyNetwork.CHAT:
                    objects = NetUtils.FormCommand(data, new string[] { "m","s" });
                    Message msgglobal = (Message)objects[0];
                    if (SpicyNetwork.msgCache.ContainsKey(msgglobal.GetMessage()))
                    {
                        break;
                    } else
                    {
                        SpicyNetwork.msgCache.Add(msgglobal.GetMessage(), msgglobal);
                    }
                    globalchat = new ChatDataArgs();
                    globalchat.Flag = SpicyNetwork.CHAT_GLOBAL;
                    globalchat.Message = msgglobal;
                    SpicyNetwork.OnChat(globalchat);
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self, (string)objects[1] }), false);
                    break;
                case SpicyNetwork.CHATDM:
                    objects = NetUtils.FormCommand(data, new string[] { "m","s" });
                    Message msgdm = (Message)objects[0];
                    if (SpicyNetwork.msgCache.ContainsKey(msgdm.GetMessage()))
                    {
                        break;
                    }
                    else
                    {
                        SpicyNetwork.msgCache.Add(msgdm.GetMessage(), msgdm);
                    }
                    globalchat = new ChatDataArgs();
                    globalchat.Flag = SpicyNetwork.CHATDM;
                    globalchat.Message = msgdm;
                    SpicyNetwork.OnChat(globalchat);
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self, (string)objects[1] }), false);
                    break;
                case SpicyNetwork.CHATRM:
                    objects = NetUtils.FormCommand(data, new string[] { "m", "s", "s" });
                    Message msgrm = (Message)objects[0];
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self, (string)objects[2] }), false);
                    Console.WriteLine("Got Room Chat! " + (string)objects[2]);
                    globalchat = new ChatDataArgs();
                    globalchat.Room = SpicyNetwork.GetRoom((string)objects[1]);
                    globalchat.Flag = SpicyNetwork.CHATDM;
                    globalchat.Message = msgrm;
                    SpicyNetwork.OnChat(globalchat);
                    break;
                case SpicyNetwork.SYNCB://payload,key
                    objects = NetUtils.FormCommand(data, new string[] { "b[]", "s" });
                    SyncEventArgs sync = new SyncEventArgs();
                    sync.Type = 0;
                    sync.BData = (byte[])objects[0];
                    SpicyNetwork.OnSyncData(sync);
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self,(string)objects[1] }), false);
                    break;
                case SpicyNetwork.SYNCO:
                    objects = NetUtils.FormCommand(data, new string[] { "s", "s" });
                    SyncEventArgs syncb = new SyncEventArgs();
                    syncb.Type = 1;
                    syncb.SData=(string)objects[0];
                    SpicyNetwork.OnSyncData(syncb);
                    SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.RELAY, SpicyNetwork.self, (string)objects[1] }), false);
                    break;
                default:
                    break;
            }
        }
    }
}
