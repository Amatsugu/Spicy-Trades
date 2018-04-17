using System;
using System.Threading;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace NetworkManager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Network.Connect("192.168.1.6", 12344,"epicknex","password"); // local
                //Network.Connect("69.113.198.118", 12344,"epicknex","password"); //external
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message + "\nPress Enter!");
                //Console.ReadLine();
                //Environment.Exit(1);
            }
            // Network.SendData(new byte[] { 0 });
            string randomid = Guid.NewGuid().ToString("N");
            Console.WriteLine(randomid.Substring(0,8));
            Console.ReadLine();
        }
    }
}
