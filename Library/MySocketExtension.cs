using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibSC
{
    public delegate void SocketMethods(Socket? socket);

    public static class MySocketExtension
    {
        private static string fname;
        private static int i;
        public static Dictionary<string, SocketMethods>? methods { get; private set; }

        static MySocketExtension()
        {
            fname = "myfile";
            i = 0;

            methods = new Dictionary<string, SocketMethods>()
            {
                { "createFile", createFile},
                { "deleteFile", deleteFile},
                { "getEnvironment", getEnvironment}
            };
        }

        public static void sentMsg(in Socket socket, string msg)
        {
            socket.Send(IntToBytes(msg));
            socket.Send(Encoding.Unicode.GetBytes(msg ??= "no msg"));
        }

        public static void sentMsg(in Socket socket, out string? msg, string? txt = default)
        {
            Console.Write($"Enter msg {(txt != null ? "(| exit | - " + txt + ")" : string.Empty)}: ");
            msg = Console.ReadLine();

            socket.Send(IntToBytes(msg));
            socket.Send(Encoding.Unicode.GetBytes(msg ??= "no msg"));
        }

        public static void getMsg(in Socket socket, out string msg)
        {
            byte[] bytes = new byte[32];
            socket.Receive(bytes);

            byte[] data = new byte[BitConverter.ToInt32(bytes, 0)];
            int bytesRead = socket.Receive(data);
            msg = Encoding.Unicode.GetString(data, 0, bytesRead);
        }

        private static byte[] IntToBytes(string? msg)
        {
            byte[] bytes = BitConverter.GetBytes(Encoding.Unicode.GetByteCount(msg ??= "no msg"));

            if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }

            return bytes;
        }

        public static void createFile(Socket? socket = null)
        {
            if (!File.Exists($"{fname}{i}.txt")) { File.Create($"{fname}{i}.txt"); }
            i +=1;
        }

        public static void deleteFile(Socket? socket = null)
        {
            i -=1;
            if (!File.Exists($"{fname}{i}.txt")) { File.Delete($"{fname}{i}.txt"); }
        }

        public static void getEnvironment(Socket? socket = null)
        {
            Console.WriteLine("call method getEnvironment");
        }
    }
}
