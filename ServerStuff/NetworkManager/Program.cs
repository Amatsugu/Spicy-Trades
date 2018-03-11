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
            Network.DataRecieved += OnDataRecieved;
            client.Send("Hello Server!");
            Thread t = new Thread(new ThreadStart(ThreadProc));
            // Receive the response from the remote device.
            Console.WriteLine(Network.ERROR_CODES[0x01]);
            t.Start();
        }
        static void OnDataRecieved(object sender, DataEventArgs e)
        {
            Console.WriteLine("Got response form the server: {0}", e.Response);
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
