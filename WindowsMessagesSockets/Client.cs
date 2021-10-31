#nullable enable

using HelperSockets;
using System;
using System.Net;
using System.Net.Sockets;

namespace WindowsMessagesSockets
{
    public class Client
    {
        private readonly IPEndPoint _endPoint;
        private readonly IDisplayMessage _displayMessage;

        public Client(IDisplayMessage displayMessage)
        {
            // Get host and port from settings
            string host = HelperSockets.Properties.Settings.Default["host"].ToString();
            int port = (int)HelperSockets.Properties.Settings.Default["port"];

            // Establish the local endpoint for the socket.
            _endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _displayMessage = displayMessage;
        }

        public void SendData()
        {
            try
            {
                // Create a TCP/IP socket. 
                using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                bool _actionResult = false;
                StateObject _stateObject = new()
                {
                    workSocket = client,
                };
                // Connect to the remote endpoint.  
                _actionResult = new SocketActionConnect(_stateObject, _displayMessage, _endPoint).Run();
                if (!_actionResult)
                    return;

                // Send test data to the remote device.  
                _actionResult = new SocketActionSend(_stateObject, _displayMessage, "This is a test<EOF>").Run();
                if (!_actionResult)
                    return;

                // Receive the response from the remote device.  
                _actionResult = new SocketActionReceive(_stateObject, _displayMessage).Run();
                if (!_actionResult)
                    return;
            }
            catch (Exception ex)
            {
                _displayMessage.Display(ex.ToString() + '\n');
            }
        }
    }
}
