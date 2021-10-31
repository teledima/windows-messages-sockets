using System;
using System.Net;
using System.Net.Sockets;
using HelperSockets;

namespace SocketServer
{
    public class Server
    {
        private readonly IPEndPoint _endPoint;
        private readonly IDisplayMessage _displayMessage;
        public Server(IDisplayMessage displayMessage)
        {
            string host = HelperSockets.Properties.Settings.Default["host"].ToString();
            int port = (int)HelperSockets.Properties.Settings.Default["port"];
            _endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _displayMessage = displayMessage;
        }

        public void StartListening()
        {
            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(_endPoint);
                listener.Listen(100);

                while (true)
                {
                    new SocketActionAccept(new StateObject { workSocket = listener }, _displayMessage).Run();
                }

            }
            catch (Exception e)
            {
                _displayMessage.Display(e.Message);
            }

            _displayMessage.Display("\nPress ENTER to continue...");
        }
    }
}
