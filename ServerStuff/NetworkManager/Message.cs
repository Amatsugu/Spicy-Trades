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
        public Message(string message, PID pid,DateTime time)
        {
            this.message = message;
            this.pid = pid;
            this.time = time;
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
    }
}
