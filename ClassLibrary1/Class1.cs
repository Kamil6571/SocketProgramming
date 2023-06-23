using System.Net.Sockets;
using System.Text;

namespace ClassLibrary1
{
    public class Class1
    {

        public static string? GetMessage(Socket socket)
        {
            string? data = null;
            while (socket.Connected && (data == null || !data.Contains("<EOM>")))
            {
                try
                {
                    byte[] bytes = new byte[4096];
                    int bytesRec = socket.Receive(bytes);
                    data += Encoding.Unicode.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("Bytes recieved: "+bytesRec);
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection suddenly lost!");
                    return null;
                }

            }
            return data;
        }

        /// <summary>
        /// Part one of the split is the message
        /// Part two is username
        /// Part three is color
        /// Part four is end of message tag
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg)
        {
            string[] s = msg.Split('|');
            Console.ForegroundColor = (ConsoleColor)int.Parse(s[2]);
            Console.WriteLine(s[1] + " : " + s[0]);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

}
