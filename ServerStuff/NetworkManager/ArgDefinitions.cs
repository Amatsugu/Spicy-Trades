using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    public class DataEventArgs : EventArgs
    {
        public string Response { get; set; }
        public int Errorcode { get; set; }
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
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class MessageArgs : EventArgs
    {
        public string Message { get; set; }
        public PID Player { get; set; }
    }
    public class BasicResponseArgs : EventArgs
    {
        public string Response { get; set; }
    }
    public class RoomUpdateArgs : EventArgs
    {
        public PID Player { get; set; }
        public Room Room { get; set; }
        public bool Ready { get; set; }
        public byte Command { get; set; }
    }
    public class RoomCountArgs : EventArgs
    {
        public int count { get; set; }
    }
    public class RoomListingArgs : EventArgs
    {
        public Room[] Rooms { get; set; }
    }
    public class FriendsArgs : EventArgs
    {
        public PID[] Players { get; set; }
    }
    public class FriendRequestArgs : EventArgs
    {
        public PID Player { get; set; }
    }
    public class RoomIsHostArgs : EventArgs
    {
        public bool host { get; set; }
        public Room Room { get; set; }
    }
    public class ResourceListArgs : EventArgs
    {
        //Need info from Kham
    }
    public class ChatDataArgs : EventArgs
    {
        public PID Speaker { get; set; }
        public int Flag { get; set; }
        public Room Room { get; set; }
    }
}
