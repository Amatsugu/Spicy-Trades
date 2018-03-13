using System;
using System.Threading;
using System.Text;
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
            Network.DataRecieved += OnDataRecieved;
            Network.SendData(new byte[] { 0 });
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
                Network.SendData(new byte[] { 0 }); // Send Hello command to server
            }
            Console.WriteLine("Got Response from server: "+command+"  "+error);
        }
    }
}
