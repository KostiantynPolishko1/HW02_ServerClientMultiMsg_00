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
                { "getEnv", getEnvironment}
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

        public static bool getEnvMsg(in Socket socket)
        {
            byte[] bytes = new byte[32];
            socket.Receive(bytes);

            int size = BitConverter.ToInt32(bytes, 0);
            byte[] data = new byte[size];
            socket.Receive(data);

            if (EnvToFile(data)) { return true; }
            return false;
        }

        private static byte[] IntToBytes(string? msg)
        {
            byte[] bytes = BitConverter.GetBytes(Encoding.Unicode.GetByteCount(msg ??= "no msg"));

            if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }

            return bytes;
        }

        private static byte[] IntToBytes(int size)
        {           
            byte[] bytes = BitConverter.GetBytes(size);

            //if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }

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

        public static bool EnvToFile(in byte[]? data = null)
        {
            FileStream? fs = null;
            bool flag = false;
            Dictionary<string, string> dict = null;

            try
            {
                fs = new FileStream($"{fname}{i}.txt", FileMode.OpenOrCreate);

                if(data == null)
                {
                    dict = new Dictionary<string, string>(Environment.GetEnvironmentVariables().Count);
                    foreach (DictionaryEntry de in Environment.GetEnvironmentVariables()) { if (de.Value != null) { dict.TryAdd(de.Key.ToString(), de.Value.ToString()); } }
                }
                else
                {
                    Utf8JsonReader utf8Reader = new Utf8JsonReader(data);
                    dict = JsonSerializer.Deserialize<Dictionary<string, string>>(ref utf8Reader)!;
                }

                JsonSerializer.Serialize<Dictionary<string, string>>(fs, dict);
                flag = true;
            }
            catch (Exception ex)
            {
                File.AppendAllText($"{fname}{i}.txt", ex.Message);
            }
            finally { fs?.Close(); }

            return flag;
        }

        public static bool getEnvironment(Socket? socket = null)
        {
            FileStream fs = null;
            bool flag = false;

            try 
            {
                fs = new FileStream($"{fname}{i}.txt", FileMode.Open);

                Dictionary<string, string>? dict = JsonSerializer.Deserialize<Dictionary<string, string>?> (fs);
                byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(dict);

                socket?.Send(IntToBytes(Buffer.ByteLength(bytes)));
                socket?.Send(bytes);

                flag = true;
            }
            catch (Exception ex)
            {
                File.AppendAllText($"{fname}{i}.txt", ex.Message);
            }
            finally { fs?.Close(); }

            return flag;
        }
    }
}
