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
        private int MAX_USERNAME_SIZE=128;
        private int MAX_ID_SIZE = 8;
        private int UID_BYTE_SIZE; // see constructor for how it is modified
        public static int PID_SIZE;
        //Max Username 128 characters
        //Id size 8 characters
        //ToBytes() will be used 
        //Format of pid bytes
        /*
         * byte[0] Type
         * byte[1] isFriend
         * byte[2-129] username
         * byte[130-137] id
         */
        public PID(string id,string name,bool isFriend)
        {
            this.id = id;
            this.name = name;
            this.isFriend = isFriend;
            UID_BYTE_SIZE = 2 + MAX_ID_SIZE + MAX_USERNAME_SIZE;
            PID_SIZE = UID_BYTE_SIZE;
        }
        public PID(byte[] piddata)
        {
            byte Type = piddata[0];
            if (Type == Network.PID) // Make sure the datatype is correct
            {
                isFriend = piddata[1] != 0x00;// Assign the isFriend bool
                byte[] _name = piddata.SubArray(2,MAX_USERNAME_SIZE);
                name = NetUtils.ConvertByteToString(_name);
                byte[] _id = piddata.SubArray(2+MAX_USERNAME_SIZE, MAX_ID_SIZE);
                id = NetUtils.ConvertByteToString(_id);
            } else
            {
                throw new ArgumentException("Attempt to form an object (PID) from non PID data! TYPECODE: "+(int)Type);
            }
            //Get data from byte and create the object
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
        public byte[] ToBytes()
        {
            byte[] send = new byte[UID_BYTE_SIZE];
            byte[] _name = ASCIIEncoding.ASCII.GetBytes(name);
            byte[] _id = ASCIIEncoding.ASCII.GetBytes(id);
            send[0] = (byte)Network.PID;
            if (isFriend)
            {
                send[1] = (byte)255; // true
            } else {
                send[1] = (byte)0; // false
            }
            if (_name.Length > MAX_USERNAME_SIZE)
            {
                throw new Exception("Username has a character length of greater than " + MAX_USERNAME_SIZE + "!");
            } else
            {
                for (int i=0; i < MAX_USERNAME_SIZE; i++)
                {
                    if (i == _name.Length)
                    {
                        break;
                    }
                    else
                    {
                        send[2 + i] = _name[i];
                    }
                }
            }
            if (_id.Length < MAX_ID_SIZE)
            {
                throw new Exception("ID length must be equal to "+MAX_ID_SIZE+"!");
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
                        send[2 + MAX_USERNAME_SIZE + i] = _id[i];
                    }
                }
            }
            return send;
        }
        public int GetSize()
        {
            return UID_BYTE_SIZE;
        }
    }
}
