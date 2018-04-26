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
            bool conn = Network.Connect("192.168.1.6", 12344, "epicknex", "password");
            if (!conn)
            {
                Console.WriteLine("Could not connect!");
            }
            Network.Error += OnError;
            Network.LoggedIn += OnLoggedIn;
            //byte [] data = NetUtils.PieceCommand(new object[] { "epicknex", "password" });
            //for (int i = 0; i < data.Length; i++)
            //{
            //    Console.Write(data[i] + ".");
            //}
            //Console.WriteLine("");
            //object[] temp = NetUtils.FormCommand(data,new string[] {"s","s" });
            //Console.WriteLine((string)temp[0]);
            //Console.WriteLine((string)temp[1]);
            //Console.ReadLine();
        }
        public static void OnError(object sender, ErrorArgs e)
        {
            Console.WriteLine("We have an error: "+e.ErrorCode+" Error Msg: "+e.ErrorMessage);
        }
        public static void OnLoggedIn(object sender, LoginEventArgs e)
        {
            Console.WriteLine("We are logged it!");
            PID test = Network.GetPID("12345678");
            Console.WriteLine("Got PID");
            Console.WriteLine(test.GetName());
        }
    }
}
