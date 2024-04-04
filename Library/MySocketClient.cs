using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibSC
{
    public class MySocketClient : Socket
    {
        private IPEndPoint ipEndP;

        public MySocketClient(IPEndPoint ipEndP) :
            base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) => this.ipEndP = ipEndP;

        public void startUp()
        {
            try
            {
                this.Connect(ipEndP);

                while (this.Connected)
                {
                    MySocketExtension.getMsg(this, out string? msg);
                    Console.Clear();
                    Console.WriteLine($"server msg: {msg}");

                    if (msg != null && msg.Equals("exit")) { break; }

                    MySocketExtension.sentMsg(this, out msg);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
