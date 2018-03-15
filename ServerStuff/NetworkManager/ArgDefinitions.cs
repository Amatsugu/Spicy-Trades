using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    public class DataEventArgs : EventArgs
    {
        public string Response { get; set; }
        public int errorcode { get; set; }
        public byte[] RawResponse { get; set; }
        public object ObjectRef { get; set; }
    }
    public class DataRecievedArgs : EventArgs
    {
        public string Response { get; set; }
        public byte[] RawResponse { get; set; }
    }
    public class ErrorArgs : EventArgs
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class MessageArgs : EventArgs
    {
        public string Message { get; set; }
        public PID Player { get; set; }
    }
}
