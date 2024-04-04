using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibSC
{
    public sealed class MyTcpClient : TcpClient, IDisposable
    {
        public byte[] recieve { get; private set; } = new byte[256];
        public byte[] send { get; private set; } = new byte[] { };
        public int bytes { get; private set; } = 0;
        public readonly List<string>? functions;
        public readonly IPEndPoint ipEndPoint;
        void IDisposable.Dispose() { }

        public MyTcpClient(in IPEndPoint ipEndPoint) : base()
        {
            this.functions = new List<string>() { "createFile", "deleteFile", "writeFile", "getEnviroment" };
            this.ipEndPoint = ipEndPoint;
        }

        public async void launch()
        {
            try
            {
                await this.ConnectAsync(ipEndPoint);
                NetworkStream stream = this.GetStream();
                Console.WriteLine($"Client {this.Client.RemoteEndPoint} is connected");

                StringBuilder sb = new StringBuilder();

                do
                {
                    bytes = await stream.ReadAsync(recieve);
                    sb.Append(Encoding.UTF8.GetString(recieve, 0, bytes));

                } while (bytes > 0);
            }
            catch (SocketException se)
            {
                Console.WriteLine($"{se.ErrorCode} - {se.Message}");
            }
            finally
            {
                Console.WriteLine("Server stop!");
                this.Client.Close();
            }
        }

        public void Dispose() { }
    }
}
