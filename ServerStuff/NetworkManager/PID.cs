using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    class PID
    {
        private string id;
        private string name;
        private bool isFriend;

        public PID(string id,string name,bool isFriend)
        {
            this.id = id;
            this.name = name;
            this.isFriend = isFriend;
        }
        public string GetID()
        {
            return id;
        }
        public string GetName()
        {
            return name;
        }
        public bool isAFriend()
        {
            return isFriend;
        }
        public void isAFriend(bool b)
        {
            isFriend = b;
        }
    }
}
