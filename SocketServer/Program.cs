using HelperSockets;
using System;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IDisplayMessage displayMessage = new DisplayConsole();
            new Server(displayMessage).StartListening();

            Console.Read();
        }
    }
}
