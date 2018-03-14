using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkManager
{
    class Client
    {
        private Socket client;
        private IPEndPoint remoteEP;
        private IPHostEntry ipHostInfo;
        private IPAddress ipAddress;
        private String response = String.Empty;
        private string ip;
        private int port;
        private ManualResetEvent connectDone =
        new ManualResetEvent(false);
        public ManualResetEvent sendDone =
            new ManualResetEvent(false);
        public ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        public Client(Socket client, IPEndPoint remoteEP,IPHostEntry ipHostInfo,IPAddress ipAddress,string ip,int port)
        {
            this.client = client;
            this.remoteEP = remoteEP;
            this.ipHostInfo = ipHostInfo;
            this.ipAddress = ipAddress;
            this.ip = ip;
            this.port = port;
            client.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();
        }
        public void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }
        public void Send(byte[] byteData)
        {
            // Convert the string data to byte data using ASCII encoding.  
            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }
        public void Receive()
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far. 
                    string test = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    state.sb.Append(test);
                    DataRecievedArgs data = new DataRecievedArgs();
                    data.Response = test;
                    data.RawResponse = state.buffer;
                    Network.OnDataRecieved(data);
                    //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    //    new AsyncCallback(ReceiveCallback), state);
                }
                //else
                //{
                //    // All the data has arrived; put it in response.  
                //    if (state.sb.Length > 1)
                //    {
                //        response = state.sb.ToString();
                //    }
                //    // Signal that all bytes have been received.  
                //    receiveDone.Set();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public string GetIp()
        {
            return ip;
        }
        public int GetPort()
        {
            return port;
        }
    }
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
}
