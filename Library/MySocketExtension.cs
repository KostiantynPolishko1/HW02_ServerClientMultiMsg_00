using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace LibSC
{
    public delegate bool SocketMethods(Socket? socket);

    public static class MySocketExtension
    {
        public static string fname { get; private set; }
        public static int i { get; private set; }
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

        public static bool createFile(Socket? socket = null)
        {
            while (true)
            {
                if (!File.Exists($"{fname}{i}.txt")) 
                { 
                    using var f = File.Create($"{fname}{i}.txt");
                    break;
                }
                i += 1;
            }
            return EnvToFile();
        }

        public static bool deleteFile(Socket? socket = null)
        {
            while (true)
            {
                i -= 1;
                if (!File.Exists($"{fname}{i}.txt")) 
                {
                    File.Delete($"{fname}{i}.txt");
                    break;
                }
            }

            return true;
        }

        public static bool EnvToFile()
        {
            FileStream? fs = null;
            bool flag = false;

            try
            {
                fs = new FileStream($"{fname}{i}.txt", FileMode.Open);
                Dictionary<object, object> dict = new Dictionary<object, object>(Environment.GetEnvironmentVariables().Count);
                foreach (DictionaryEntry de in Environment.GetEnvironmentVariables()) { if (de.Value != null) { dict.TryAdd(de.Key, de.Value); } }

                JsonSerializer.Serialize<Dictionary<object, object>>(fs, dict);
                flag = true;
            }
            catch (Exception ex)
            {
                File.AppendAllText($"{fname}{i}.txt", ex.Message);
            }
            finally { fs?.Close(); }

            return flag;
        }

        public static bool getEnvironment(Socket? socket = null) => false;
    }
}
