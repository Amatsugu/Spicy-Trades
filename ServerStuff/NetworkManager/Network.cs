using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace NetworkManager
{
    static class Network
    {
        public static Client Connect(string ip, int port)
        {
            IPHostEntry ipHostInfo;
            IPAddress ipAddress;
            try
            {
                ipHostInfo = Dns.GetHostEntry(ip);
                ipAddress = ipHostInfo.AddressList[0];
            } catch {
                // Looks like we could not connect!
                throw new Exception("Unable to connect to the server! Unknown Hostname: "+ip);
            }
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                return new Client(client, remoteEP, ipHostInfo, ipAddress, ip, port);
            } catch {
                throw new Exception("Unable to connect to the server! Is the server running? " + ip +":"+port);
            }
        }
        public static void OnDataRecieved(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = DataRecieved;
            handler(null,e);
        }
        public static event EventHandler<DataEventArgs> DataRecieved;
    }
    public class DataEventArgs: EventArgs
    {
        public string Response { get; set; }
    }
}
