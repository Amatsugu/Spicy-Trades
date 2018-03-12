using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    class Message
    {
        private string message;
        private PID pid;
        private DateTime time;
        public Message(string message, PID pid,DateTime time) //When data comes from the client
        {
            this.message = message;
            this.pid = pid;
            this.time = time;
        }
        public Message(byte[] messagedata) //When data comes from the server
        {
            //BitConverter.ToInt32(b, 0)
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
        /*
         * So the Message object has this byte format:
         * 2-bytes Message Size
         * 3-bytes Time-Stamp
         * VSar-Size Message
         */
        public byte[] ToBytes()
        {
            int pidsize = pid.GetSize();
            int mSize = message.Length;
            byte[] send = new byte[mSize + pidsize + 5];
            if (mSize > 65535)
            {
                DataEventArgs data = new DataEventArgs();
                data.Response = "Message size is too large Limit messages to 65535 characters or less!";
                data.errorcode = Network.MESSAGE_TO_LARGE;
                data.ObjectRef = this;
                Network.OnError(data);
            }
            else
            {
                byte[] msg = Encoding.ASCII.GetBytes(message);
                byte[] size = BitConverter.GetBytes((Int16)mSize);
                byte[] pidB = pid.ToBytes();
                byte hour = (byte)time.Hour;
                byte minute = (byte)time.Minute;
                byte second = (byte)time.Second;
                send[0] = size[0];
                send[1] = size[1];
                send[2] = hour;
                send[3] = minute;
                send[4] = second;
                for (int i = 0; i < pidsize; i++)
                {
                    send[5 + i] = pidB[i];
                }
                for (int i = 0; i < mSize; i++)
                {
                    send[5 + pidsize + i] = msg[i];
                }
            }
            return send;
        }
    }
}
