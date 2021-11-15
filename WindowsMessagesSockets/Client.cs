#nullable enable

using HelperSockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;

namespace WindowsMessagesSockets
{
    public class Client
    {
        private readonly IPEndPoint _endPoint;
        private readonly IDisplayMessage _displayMessage;
        private readonly DesService _desService;

        public Client(IDisplayMessage displayMessage)
        {
            // Get host and port from settings
            string host = HelperSockets.Properties.Settings.Default["host"].ToString();
            int port = (int)HelperSockets.Properties.Settings.Default["port"];

            // Establish the local endpoint for the socket.
            _endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _displayMessage = displayMessage;

            // Initialize des service
            _desService = new();
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
                var rsaService = new RSACryptoServiceProvider();
                _actionResult = new SocketActionReceive(client, _displayMessage).Run();
                if (!_actionResult.Success)
                    return;
                else
                {
                    rsaService.FromXmlString(Encoding.ASCII.GetString(_actionResult.Response));
                    _displayMessage.Display("AES Key received\n");
                }
                // Send des key
                _actionResult = new SocketActionSend(client, _displayMessage, "Send DES key to server.\n", rsaService.Encrypt(_desService.Serialize(), false)).Run();
                if (!_actionResult.Success)
                    return;

                // Get list rows from source table
                foreach (var sourceGame in sourceGames)
                {
                    Thread.Sleep(100);
                    // Send encrypted row to server.  
                    _actionResult = new SocketActionSend(client, _displayMessage, "Send {0} bytes to server.\n", SourceGamesHelper.Encrypt(new[] { sourceGame }, _desService)).Run();
                    if (!_actionResult.Success)
                        return;
                }
                // Send stop keyword
                _actionResult = new SocketActionSend(client, _displayMessage, "Send stop keyword.\n", _desService.Encrypt(Encoding.ASCII.GetBytes("<EOF>"))).Run();

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
