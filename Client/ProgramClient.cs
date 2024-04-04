using LibSC;
using System.Net;

namespace Client
{
    internal class ProgramClient
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Client!");

            using (MySocketClient client = new MySocketClient(new ConnectionData().getIpeP()))
            {
                client.startUp();
            }

            Console.Read();
        }
    }
}