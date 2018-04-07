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
                Network.Connect("69.113.198.118", 12344,"epicknex","password"); //external
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nPress Enter!");
                Console.ReadLine();
                Environment.Exit(1);
            }
            // Network.SendData(new byte[] { 0 });
            uint test = 234523;
            string hexValue = test.ToString("X8");
            Console.WriteLine(hexValue);
            Console.ReadLine();
        }
    }
}
