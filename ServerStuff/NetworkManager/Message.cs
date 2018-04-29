using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace NetworkManager
{
    public class Message
    {
        private string message;
        private PID pid;
        private DateTime time;
        private int MESSAGE_BYTE_SIZE;
        public Message(string message, PID pid) //When data comes from the client
        {
            this.message = message;
            this.pid = pid;
            time = DateTime.Now;
        }
        public Message(string message) //When data comes from the client
        {
            this.message = message;
            pid = Network.player;
            time = DateTime.Now;
        }
        /*
         * So the Message object has this byte format:
         * 2-bytes Message Size
         * 3-bytes Time-Stamp
         * 138-bytes PID (This may change so i use getSize() on the pid data)
         * VSar-Size Message
         */
        public Message(byte[] messagedata) //When data comes from the server
        {
            //BitConverter.ToInt32(b, 0)
            byte Type = messagedata[0];
            if (Type == Network.MESSAGE) // Make sure the datatype is correct
            {
                int mSize = BitConverter.ToInt16(messagedata.SubArray(1, 2),0);
                int hour = messagedata[3];
                int minute = messagedata[4];
                int second = messagedata[5];
                CultureInfo MyCultureInfo = CultureInfo.CurrentCulture;//This may not work for non USA time... But for now we dont need to worry about that... I could use ticks as well but meh
                string MyString = "12 July 2004 "+hour+":"+minute+":"+second;
                time = DateTime.Parse(MyString, MyCultureInfo);
                pid = new PID(messagedata.SubArray(6, PID.PID_SIZE));
                message = NetUtils.ConvertByteToString(messagedata.SubArray(PID.PID_SIZE+6, mSize));
            }
            else
            {
                throw new ArgumentException("Attempt to form an object (Message) from non Message data! TYPECODE: " + (int)Type);
            }
        }
        public String GetMessage()
        {
            return message;
        }
        public PID GetPID()
        {
            return pid;
        }
        public DateTime GetTime()
        {
            return time;
        }
        public int GetSize()
        {
            int pidsize = pid.GetSize();
            int mSize = message.Length;
            return mSize + pidsize + 6;
        }
        public byte[] ToBytes()
        {
            int pidsize = pid.GetSize();
            int mSize = message.Length;
            byte[] send = new byte[mSize + pidsize + 6];
            if (mSize > 65535)
            {
                throw new ArgumentException("Message size is too large Limit messages to 65535 characters or less!");
            }
            else
            {
                byte[] msg = Encoding.ASCII.GetBytes(message);
                byte[] size = BitConverter.GetBytes((Int16)mSize);
                byte[] pidB = pid.ToBytes();
                byte hour = (byte)time.Hour;
                byte minute = (byte)time.Minute;
                byte second = (byte)time.Second;
                send[0] = Network.MESSAGE;
                send[1] = size[0];
                send[2] = size[1];
                send[3] = hour;
                send[4] = minute;
                send[5] = second;
                for (int i = 0; i < pidsize; i++)
                {
                    send[6 + i] = pidB[i];
                }
                for (int i = 0; i < mSize; i++)
                {
                    send[6 + pidsize + i] = msg[i];
                }
            }
            return send;
        }
    }
}
