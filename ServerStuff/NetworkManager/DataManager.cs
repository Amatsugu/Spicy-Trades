using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkManager
{
    public static class DataManager
    {
        public static void INIT()
        {
            Network.DataRecieved += OnDataRecieved;
        }
        public static void OnDataRecieved(object sender, DataRecievedArgs e)
        {
            byte command = e.RawResponse[0];
            byte error = e.RawResponse[1];
            byte[] data = e.RawResponse.SubArray(2, e.RawResponse.Length-2);
            //If there is an error you'll get the command byte[0] error byte[1] and a string of what the error is
            if (error != Network.NO_ERROR)
            {
                object[] objects = NetUtils.FormCommand(data, new string[] { "s" });
                ErrorArgs dat = new ErrorArgs();
                dat.ErrorCode = error;
                dat.ErrorMessage = (string) objects[0];
                Network.OnError(dat);
            }
            switch (command)
            {
                case Network.HELLO:
                    Console.WriteLine("Got a hello from the server!");
                    Network.SendData(new byte[] { 0 });
                    break;
                case Network.LOGIN:
                    //Login was successful! Nothing is needed!
                    break;
                case Network.REGISTER:
                    //Registering was successful! Nothing is needed!
                    break;
                case Network.UPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.DOUPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
                    break;
                case Network.LISTR:
                    object[] objects = NetUtils.FormCommand(data, new string[] { "s" });
                    break;
                case Network.JROOM:
                    //
                    break;
                case Network.CHAT:
                    //
                    break;
                case Network.REQUEST:
                    //
                    break;
                case Network.LISTF:
                    //
                    break;
                case Network.LISTRF:
                    //
                    break;
                case Network.ADDF:
                    //
                    break;
                case Network.FORMR:
                    //
                    break;
                case Network.IHOST:
                    //
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
                case Network.LEAVER:
                    //
                    break;
                case Network.GRESORCE:
                    //
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
