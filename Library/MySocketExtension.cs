using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibSC
{
    public static class MySocketExtension
    {
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
    }
}
