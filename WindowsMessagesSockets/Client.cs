#nullable enable

using HelperSockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

        public void SendData(IEnumerable<SourceGames> sourceGames)
        {
            try
            {
                // Create a TCP/IP socket. 
                using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                ResultAction _actionResult;
                // Connect to the remote endpoint.  
                _actionResult = new SocketActionConnect(client, _displayMessage, _endPoint).Run();
                if (!_actionResult.Success)
                    return;

                // Receive rsa public key 
                _actionResult = new SocketActionReceive(client, _displayMessage).Run();
                if (!_actionResult.Success)
                    return;
                else
                    _displayMessage.Display("Key received\n");

                // Send test data to the remote device.  
                _actionResult = new SocketActionSend(client, _displayMessage, "Send {0} bytes to server.\n", SourceGamesHelper.Encrypt(sourceGames, _actionResult.Response)).Run();
                if (!_actionResult.Success)
                    return;

                // Receive the response from the remote device.
                _actionResult = new SocketActionReceive(client, _displayMessage).Run();
                if (!_actionResult.Success)
                    return;
                else
                    _displayMessage.Display(string.Format("Response received : {0}\n", Encoding.ASCII.GetString(_actionResult.Response)));

                // Clearing the original data after successfully completing all steps
                Task.Run(() => SourceGamesHelper.ClearSourceGames(Properties.Settings.Default["source_filepath"].ToString()));
            }
            catch (Exception ex)
            {
                _displayMessage.Display(ex.ToString() + '\n');
            }
        }
    }
}
