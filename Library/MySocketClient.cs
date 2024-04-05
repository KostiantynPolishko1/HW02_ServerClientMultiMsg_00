﻿using System.Net;
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

                    if ((bool)MySocketExtension.methods?.ContainsKey(msg))
                    {
                        SocketMethods? method = null;
                        if((bool)MySocketExtension.methods?.TryGetValue(msg, out method))
                        {
                            if (msg == "getEnv")
                            {
                                method?.Invoke(this);
                            }
                            else if ((bool)method?.Invoke(this)) 
                            {
                                string fname = $"{MySocketExtension.fname}{MySocketExtension.i}.txt";
                                MySocketExtension.sentMsg(this, $"{msg} : {fname}"); 
                            }
                        }
                        else
                        {
                            MySocketExtension.sentMsg(this, $"fault: {msg}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"server msg: {msg}");
                        if (msg != null && msg.Equals("exit")) { break; }

                        MySocketExtension.sentMsg(this, out msg);
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
