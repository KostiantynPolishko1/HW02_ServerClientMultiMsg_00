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
        public static void getMsg(in Socket socket, out string msg)
        {
            byte[] bytes = new byte[1024];
            int bytesRead = socket.Receive(bytes);
            msg = Encoding.Unicode.GetString(bytes, 0, bytesRead);
        }

        public static void sentMsg(in Socket socket, out string? msg, string? txt = default)
        {
            Console.Write($"Enter msg {(txt != null ? "(| exit | - " + txt + ")" : string.Empty)}: ");
            msg = Console.ReadLine();
            socket.Send(Encoding.Unicode.GetBytes(msg ??= "no msg"));
        }
    }
}
