using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    class Room
    {
        private int MAX_MEMBERS = 4; //Max members that can be on a server
        private string roomID;
        private string roomPass;
        private PID[] members;
        private bool host;
        private Client client;
        private bool isReady;
        private bool roomJoined=false;

        public Room(string roomID, string roomPass, bool host,Client client)
        {
            this.roomID = roomID;
            this.roomPass = roomPass;
            this.host = host;
            this.client = client;
            members = new PID[MAX_MEMBERS];
        }
        public void Join()
        {
            roomJoined = true;
        }
        public bool isHost()
        {
            return host;
        }
        public void AddMember(PID member)
        {
            // TODO Add member to the room, change local data and send command to the server
        }
        public bool Kick(PID member)
        {
            // TODO Kick Member from the room... Should this be host only or anyone...
            return false;
        }
        public void LeaveRoom()
        {
            // TODO Leave the room
        }
        public void SetReady(bool b)
        {
            isReady = b;
        }
        public void SendChat(string message)
        {
            // TODO add chatting support in rooms
        }
    }
}
