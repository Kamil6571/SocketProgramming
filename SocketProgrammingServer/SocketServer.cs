using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketProgrammingServer
{
    internal class SocketServer
    {
        public SocketServer()
        {
            //new MultiThreading().StartThreading();
            StartServer();
        }

        /// <summary>
        /// Creates socket that only listens and binds it with the server endpoint
        ///If a connection have been made with the listener socket,
        ///a new thread is created that handles the traffic, therefore not blocking 
        /// </summary>
        void StartServer()
        {
            Socket listener = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            IPEndPoint iPEndPoint = CreateEndPoint();
            listener.Bind(iPEndPoint);
            listener.Listen(10);
            Console.WriteLine("Server listens on " + iPEndPoint);

            Thread clientThread = new(new ThreadStart(new SocketClient.SocketClient().StartClient));
            clientThread.Start();

            while (true)
            {
                Socket handler = listener.Accept();
                Thread thread = new(new ThreadStart(() => ConnectToClient(handler)));
                thread.Start();
            }
        }

        /// <summary>
        /// Creates an endpoint by selecting the PC's hostname and getting the 
        ///ipv4 network interfaces ip address list
        /// </summary>
        /// <param name="addressList"></param>
        /// <returns>IPEndPoint for server</returns>
        IPEndPoint CreateEndPoint()
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName(), AddressFamily.InterNetwork);

            int i = 0;
            foreach (var item in iPHostEntry.AddressList)
                Console.WriteLine($"[{i++}] {item}");

            int j;
            do { Console.Write("Input number of Ipaddress: "); }
            while (!int.TryParse(Console.ReadLine(), out j)
                || j < 0 || j >= iPHostEntry.AddressList.Length);

            IPAddress iPAddress = iPHostEntry.AddressList[j];
            //Or convert a string to type of IpAddress
            //IPAddress iPAddress2 = IPAddress.Parse("192.168.1.2");
            return new IPEndPoint(iPAddress, 22222);
        }

        /// <summary>
        /// Accepts connection from client, Recieves message 
        /// and returns message to client
        /// This method will be a thread by itself, and keeps the 
        /// connection open with the client socket, thus being
        /// able to recieve messages without interuption.
        /// </summary>
        /// <param name="listener"></param>
        void ConnectToClient(Socket handler)
        {
            Console.WriteLine("Connect to: " + handler.RemoteEndPoint);
            while (handler.Connected)
            {
                string? data = ClassLibrary1.Class1.GetMessage(handler);
                byte[] returnMsg = Encoding.Unicode.GetBytes("Server received msg<EOM>");
                if (handler.Connected) handler.Send(returnMsg);
                //TODO show message with username and color
                ClassLibrary1.Class1.ShowMessage(data);
                //Console.WriteLine(data + $" ({handler.RemoteEndPoint})");
            }
            Console.WriteLine("Disconnected with client " + handler.RemoteEndPoint);
        }
    }
}