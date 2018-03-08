using System;
using System.Threading;
namespace NetworkManager
{
    class Program
    {
        public static Client client;
        static void Main(string[] args)
        {
            try
            {
                client = Network.Connect("192.168.1.6", 12344);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nPress Enter!");
                Console.ReadLine();
                client = null;
                Environment.Exit(1);
            }
            client.Send(new byte[] {1,2,3,4,5,6,7,8,9,0,11,22,12,13,14,15,16});
            Thread t = new Thread(new ThreadStart(ThreadProc));
            // Receive the response from the remote device.  
            t.Start();
        }
        public static void ThreadProc()
        {
            client.Receive();
            client.sendDone.WaitOne();
            while (true)
            {
                //Keep the thread going
                Thread.Sleep(10);
            }
        }
    }
}
