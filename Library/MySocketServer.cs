using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibSC
{
    public class MySocketServer : Socket
    {
        private IPEndPoint ipEndP;

        public MySocketServer(IPEndPoint ipEndP) :
            base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) => this.ipEndP = ipEndP;

        public void startUp()
        {
            if (!getClientSocket(out Socket? socket)) { return; }

            try
            {
                while (socket.Connected)
                {
                    MySocketExtension.sentMsg(socket, out string? msg, "out");
                    Console.Clear();

                    if (msg != null && msg.Equals("exit"))
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        break;
                    }

                    MySocketExtension.getMsg(socket, out msg);
                    Console.WriteLine($"client msg: {msg}");
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"{se.ErrorCode} - {se.Message}");
            }
            finally
            {
                socket?.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                socket?.Close();
            }
        }

        private bool getClientSocket(out Socket? socket)
        {
            try
            {
                this.Bind(ipEndP);
                this.Listen();
                socket = this.Accept();

                return true;
            }
            catch
            {
                socket = null;
                return false;
            }
        }
    }
}
