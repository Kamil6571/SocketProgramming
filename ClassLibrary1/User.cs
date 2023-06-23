using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;


public class User
{
    public string Name { get; set; }
    public string Color { get; set; }
    public string IPAddress { get; set; }


    public User()
    {
        Console.Write("Enter your name: ");
        Name = Console.ReadLine();
        Console.Write("Enter your color: ");
        Color = Console.ReadLine();
        Console.Write("Enter your IP address: ");
        IPAddress = Console.ReadLine();
    }
}

public class ChatClient
{
    static User user;
    static TcpClient client;
    static NetworkStream stream;
    static string serverAddress;
    static int serverPort;


    static List<string> serverAddresses = new List<string>()
    {
        "127.0.0.1",
        "192.168.2.2",
        "localhost"
    };

    static void Main(string[] args)
    {

        Console.WriteLine("Choose a server address:");
        for (int i = 0; i < serverAddresses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {serverAddresses[i]}");
        }
        int choice = int.Parse(Console.ReadLine());
        serverAddress = serverAddresses[choice - 1];

        Console.Write("Enter the server port: ");
        serverPort = int.Parse(Console.ReadLine());

        Console.WriteLine("Connecting to server...");
        client = new TcpClient(serverAddress, serverPort);
        stream = client.GetStream();


        user = new User();


        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(user.Name);
        byte[] colorBytes = System.Text.Encoding.UTF8.GetBytes(user.Color);
        stream.Write(nameBytes, 0, nameBytes.Length);
        stream.Write(colorBytes, 0, colorBytes.Length);


        Thread thread = new Thread(new ThreadStart(ReceiveMessages));
        thread.Start();


        while (true)
        {
            string message = Console.ReadLine();
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
    }


    static void ReceiveMessages()
    {
        while (true)
        {
            byte[] messageBytes = new byte[4096];
            int bytesRead = stream.Read(messageBytes, 0, messageBytes.Length);
            string message = System.Text.Encoding.UTF8.GetString(messageBytes, 0, bytesRead);

            string[] parts = message.Split('|');
            string name = parts[0];
            string color = parts[1];
            string text = parts[2];

            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color);
            Console.WriteLine($"{name}: {text}");
            Console.ResetColor();
        }
    }
}