using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibSC
{
    public sealed class MyTcpServer : TcpListener, IDisposable
    {
        public byte[] recieve { get; private set; } = new byte[256];
        public byte[] send { get; private set; } = new byte[] { };
        public int bytes { get; private set; } = 0;

        void IDisposable.Dispose() { }
        public MyTcpServer(in IPEndPoint ipEndPoint) : base(ipEndPoint) { }

        public async void launch()
        {
            try
            {
                this.Start();
                Console.WriteLine("Server is waiting...");

                using (var tcpClient = await AcceptTcpClientAsync())
                {
                    Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} is connected");

                    NetworkStream streamClient = tcpClient.GetStream();

                    while (true)
                    {
                        Console.Write("enter msg -> ");
                        string msg = string.Empty;
                        send = Encoding.UTF8.GetBytes(msg = Console.ReadLine() != null ? msg : string.Empty);
                        streamClient.Write(send);
                    }
                }
            }
            catch ( SocketException se )
            {
                Console.WriteLine($"{se.ErrorCode} - {se.Message}");
            }
            finally
            {
                Console.WriteLine("Server stop!");
                this.Stop();
            }
        }

        public void Dispose() { }


    }
}