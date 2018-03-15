using System;
using System.Threading;
using System.Text;
namespace NetworkManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Network.DataRecieved += OnDataRecieved;
            try
            {
                //Network.Connect("192.168.1.6", 12344,"epicknex","password"); // local
                //Network.Connect("69.113.198.118", 12344,"epicknex","password"); //external
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nPress Enter!");
                Console.ReadLine();
                Environment.Exit(1);
            }
            //Network.SendData(new byte[] { 0 });
            byte[] test = NetUtils.PieceCommand(new object[] { "Hello Whats up!", 1234, new Message("Hello whats up", new PID("12345678", "epicknex", true))});
            object[] test2 = NetUtils.FormCommand(test, new string[] { "s", "i", "m" });
            for (int i = 0; i < test2.Length; i++)
            {
                Console.WriteLine(test2[i]);
            }
            Console.ReadLine();
        }
        static void OnDataRecieved(object sender, DataRecievedArgs e)
        {
            byte command = e.RawResponse[0];
            byte error = e.RawResponse[1];
            if (error != Network.NO_ERROR)
            {
                ErrorArgs data = new ErrorArgs();
                data.errorCode = error;
                Network.OnError(data);
            }
            if (command == Network.HELLO)
            {
                Network.SendData(new byte[] { 0 }); // Send Hello command to server
            }
            Console.WriteLine("Got Response from server: "+command+"  "+error);
        }
    }
}
