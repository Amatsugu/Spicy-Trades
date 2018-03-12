using System;
using System.Threading;
using System.Text;
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
            //client.Send(new byte[] {0});
            Thread t = new Thread(new ThreadStart(ThreadProc));
            // Receive the response from the remote device.
            t.Start();
            //PID testPid = new PID("12345678", "epicknex", false);
            //Message testMsg = new Message("Hello how are you doing?", testPid, DateTime.Now);
            //byte[] test = testMsg.ToBytes();
            //for(int i=0; i < test.Length; i++)
            //{
            //    Console.Write((char)test[i]);
            //}
        }
        static void OnDataRecieved(object sender, DataEventArgs e)
        {
            byte command = e.RawResponse[0];
            byte error = e.RawResponse[1];
            if (error != Network.NO_ERROR)
            {
                DataEventArgs data = new DataEventArgs();
                data.errorcode = error;
                Network.OnError(data);
            }
            if (command == Network.HELLO)
            {
                client.Send(new byte[] { 0 }); // Send Hello command to server
            }
            Console.WriteLine("Got Response from server: "+command+"  "+error);
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
