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
                //Network.Connect("69.113.198.118", 12344,"epicknex","password"); //external
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nPress Enter!");
                Console.ReadLine();
                Environment.Exit(1);
            }
            byte[] test = NetUtils.PieceCommand(new object[] { new string[] { "test1","test2","test3"}, 12345 });

            object[] test2 = NetUtils.FormCommand(test,new string[] {"s[]","i" });
            var temp1 = (string[])test2[0];
            var temp2 = (int)test2[1];
            for(int i = 0; i < temp1.Length; i++)
            {
                Console.WriteLine(temp1[i]);
            }
            Console.WriteLine(temp2);
            Network.SendData(new byte[] { 0 });
            Console.ReadLine();
        }
    }
}
