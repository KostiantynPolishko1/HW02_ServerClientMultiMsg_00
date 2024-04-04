using LibSC;
using System.Net;

namespace Server
{
    internal class ProgramServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Server!");

            using (MySocketServer server = new MySocketServer(new ConnectionData().getIpeP()))
            {
                server.startUp();
            }

            Console.Read();
            //end
        }
    }
}