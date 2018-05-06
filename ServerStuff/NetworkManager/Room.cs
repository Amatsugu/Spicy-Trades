using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    public class Room
    {
        public const int MAX_MEMBERS = 4; //Max members that can be on a server
        private int MAX_ID_SIZE = 8;
        private string roomID;//nedds to be sent
        private string roomPass;//needs to be sent
        private PID[] members;//needs to be sent
        private bool[] theyReady;
        private byte theyHost;//set to the index in the array whos host
        private bool host;//ture if your that lucky guy, tbh if you create a room then this is true anyway
        private bool isReady;//client side
        private bool roomJoined = false;
        private int numOfPlayers;

        public Room(string roomID, string roomPass, bool host)
        {
            this.roomID = roomID;
            this.roomPass = roomPass;
            this.host = host;
            members = new PID[MAX_MEMBERS];
            theyReady = new bool[MAX_MEMBERS];
        }
        /* Room Structure
         * byte[0] Type
         * byte[1] NumPplInRoom
         * byte[2] boolArrayOfReadyPlayers
         * byte[3] int of host
         * byte[4] bool _pass
         * byte[5,+8] RID
         * byte[13,+128]? if pass none if none
         * byte[13+pass,138-522]?depending on members
         */
        public Room(byte[] roomdata)
        {
            members = new PID[MAX_MEMBERS];
            theyReady = new bool[MAX_MEMBERS];
            byte Type = roomdata[0];
            if (Type == SpicyNetwork.ROOM) // Make sure the datatype is correct
            {
                numOfPlayers = roomdata[1];
                theyReady = NetUtils.ConvertByteToBoolArray(roomdata[2]);
                theyHost = roomdata[3];
                int _pass = roomdata[4];
                roomID = NetUtils.ConvertByteToString(roomdata.SubArray(5, 8));
                if (_pass != 0)
                {
                    roomPass = NetUtils.ConvertByteToString(roomdata.SubArray(13, 128));
                }
                for (int i = 0; i < numOfPlayers; i++)
                {
                    members[i] = new PID(roomdata.SubArray(13 + _pass, PID.PID_SIZE));
                }
            }
            else
            {
                throw new ArgumentException("Attempt to form an object (Room) from non Room data! TYPECODE: " + (int)Type);
            }
        }
        public byte[] ToBytes()
        {
            int ppl = numOfPlayers;
            int _pass;
            byte[] pass;
            if (roomPass != "")
            {
                _pass = 128;
                pass = new byte[128]; // Pass is used you dumb machine
            }
            else
            {
                _pass = 0;
            }
            int sendSize = (ppl * members[0].GetSize()) + 1 + 1 + MAX_ID_SIZE + 1 + 1 + 1 + _pass; // The data stuff
            byte[] send = new byte[sendSize];
            send[0] = SpicyNetwork.ROOM;
            send[1] = (byte)ppl;
            send[2] = NetUtils.ConvertBoolArrayToByte(theyReady);
            send[3] = theyHost;
            send[4] = (byte)_pass;
            byte[] _id = ASCIIEncoding.ASCII.GetBytes(roomID);
            byte[] _roompass = ASCIIEncoding.ASCII.GetBytes(roomPass);
            if (_id.Length < MAX_ID_SIZE)
            {
                throw new Exception("ID length must be equal to " + MAX_ID_SIZE + "!");
            }
            else
            {
                for (int i = 0; i < MAX_ID_SIZE; i++)
                {
                    if (i == _id.Length)
                    {
                        break;
                    }
                    else
                    {
                        send[5 + i] = _id[i];
                    }
                }
            }
            if (_pass != 0)
            {
                if (_roompass.Length > 128)
                {
                    throw new Exception("RoomPass cannot have a character length greater than 128!");
                }
                else
                {
                    for (int i = 0; i < 128; i++)
                    {
                        if (i == _roompass.Length)
                        {
                            break;
                        }
                        else
                        {
                            send[13 + i] = _roompass[i];
                        }
                    }
                }
            }
            for (int p = 0; p < ppl; p++)
            {
                byte[] temp = members[p].ToBytes();
                for (int i = 0; i < temp.Length; i++)
                {
                    send[_pass + 13 + i + (p * members[0].GetSize())] = temp[i];
                }
            }
            return send;
        }
        public string GetRoomPass()
        {
            return roomPass;
        }
        public int GetNumPlayers()
        {
            int sum = 0;
            for (int i = 0; i < 4; i++)
            {
                if (members[i] != null)
                {
                    sum++;
                }
            }
            return sum;
        }
        public void Join()
        {
            roomJoined = true;
        }
        public bool isHost()
        {
            return host;
        }
        public PID GetHost()
        {
            return members[theyHost];
        }
        public void AddMember(PID member)
        {
            members[numOfPlayers++] = member;
            SpicyNetwork.InviteFriend(member.GetID()); // Its started
        }
        public void AddMember(PID member, bool noprob)
        {
            if (numOfPlayers == 0)
                theyHost = 0;
            members[numOfPlayers++] = member;
        }
        public void RemoveMember(PID member)
        {
            PID[] temp = new PID[MAX_MEMBERS];
            int count = 0;
            for (int i = 0; i < numOfPlayers; i++)
            {
                if (members[i].GetID() != member.GetID())
                {
                    temp[count++] = members[i];
                }
            }
            members = temp;
        }
        public bool Kick(PID member)
        {
            if (host)
            {
                SpicyNetwork.KickPlayer(member.GetID()); // Its started
                return true;
            }
            return false;
        }
        public PID[] GetMembers()
        {
            return members;
        }
        public void LeaveRoom()
        {
            SpicyNetwork.LeaveRoom(); //Tells the server you are leaving
        }
        public void SetReady(bool b)
        {
            isReady = b;
            SpicyNetwork.SetReady(b); //DW Server will not mirror commands sent to you...
        }
        public void SetReady(bool b, PID player)
        {
            for (int i = 0; i < MAX_MEMBERS; i++)
            {
                if (player.GetID() == members[i].GetID())
                {
                    theyReady[i] = b;
                    break;
                }
            }
        }
        public string GetRoomID()
        {
            return roomID;
        }
        public void SendChat(string message)
        {
            SpicyNetwork.SendRoomChat(new Message(message, SpicyNetwork.player));
        }
    }
}
