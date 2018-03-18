using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    public static class ServerDataManager
    {
        //Unlike the client data will live in this class
        private static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();
        private static Dictionary<string, PID> PIDs = new Dictionary<string, PID>();
        public static void INIT()
        {
            Network.DataRecieved += OnDataRecieved;
        }
        public static void OnDataRecieved(object sender, DataRecievedArgs e)
        {
            byte command = e.RawResponse[0];
            Network.Retrieve(command);
            byte[] data = e.RawResponse.SubArray(1, e.RawResponse.Length-2);
            object[] objects;
            string pid;
            switch (command)
            {
                case Network.HELLO:
                    Console.WriteLine("Got a hello from the client!");
                    //Handle data for this
                    break;
                case Network.LOGIN:
                    //
                    break;
                case Network.REGISTER:
                    //
                    break;
                case Network.UPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.DOUPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.LISTR: //Gets the roomids
                    //
                    break;
                case Network.JROOM:
                    //
                    break;
                case Network.CHAT:
                    //
                    break;
                case Network.REQUEST:
                    // Nothing is needed here
                    break;
                case Network.LISTF:
                    //
                    break;
                case Network.LISTRF:
                    //
                    break;
                case Network.ADDF:
                    //Not needed
                    break;
                case Network.FORMR:
                    //
                    break;
                case Network.IHOST:
                    //Command not needed, server handles and sends this data over
                    break;
                case Network.KICK:
                    //
                    break;
                case Network.INVITEF:
                    //
                    break;
                case Network.READY:
                    //
                    break;
                case Network.LEAVER: // only sent if you are in the room
                    //
                    break;
                case Network.GRESORCE:
                    // Need more data from kham
                    break;
                case Network.INIT:
                    //
                    break;
                case Network.CHATDM:
                    //
                    break;
                case Network.CHATRM:
                    //
                    break;
                case Network.ROOMS:
                    //
                    break;
                case Network.GROOM:
                    //
                    break;
                case Network.GPID:
                    //
                    break;
                default:
                    Console.WriteLine("Unknown command!");
                    break;
            }
        }
    }
}
