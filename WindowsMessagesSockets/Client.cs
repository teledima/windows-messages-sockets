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
    public class Client: AbstractClient
    {
        private readonly IPEndPoint _endPoint;
        private readonly AesService _aesService;

        public Client(IDisplayMessage displayMessage):base(displayMessage)
        {
            // Get host and port from settings
            string host = HelperSockets.Properties.Settings.Default["host"].ToString();
            int port = (int)HelperSockets.Properties.Settings.Default["port"];

            // Establish the local endpoint for the socket.
            _endPoint = new IPEndPoint(IPAddress.Parse(host), port);

            // Initialize des service
            _aesService = new();
        }

        public override void SendData(IEnumerable<SourceGames> sourceGames)
        {
            try
            {
                // Create a TCP/IP socket. 
                using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                ResultAction _actionResult;
                // Connect to the remote endpoint.  
                _actionResult = new SocketActionConnect(client, this.displayMessage, _endPoint).Run();
                if (!_actionResult.Success)
                    return;

                // Receive rsa public key 
                var rsaService = new RSACryptoServiceProvider();
                _actionResult = new SocketActionReceive(client, this.displayMessage).Run();
                if (!_actionResult.Success)
                    return;
                else
                {
                    rsaService.FromXmlString(Encoding.ASCII.GetString(_actionResult.Response));
                    this.displayMessage.Display("AES Key received\n");
                }
                // Send aes key
                _actionResult = new SocketActionSend(client, this.displayMessage, "Send AES key to server.\n", rsaService.Encrypt(_aesService.Serialize(), false)).Run();
                if (!_actionResult.Success)
                    return;

                // Get list rows from source table
                foreach (var sourceGame in sourceGames)
                {
                    // Encrypt row
                    var data = SourceGamesHelper.Encrypt(new[] { sourceGame }, _aesService);
                    // Send data  
                    _actionResult = new SocketActionSend(client, this.displayMessage, "Send {0} bytes to server.\n", data).Run();
                    if (!_actionResult.Success)
                        return;
                }
                // Send stop keyword
                _actionResult = new SocketActionSend(client, this.displayMessage, "Send stop keyword.\n", _aesService.Encrypt(Encoding.ASCII.GetBytes("<EOF>"))).Run();

                // Receive the response from the remote device.
                _actionResult = new SocketActionReceive(client, this.displayMessage).Run();
                if (!_actionResult.Success)
                    return;
                else
                    this.displayMessage.Display(string.Format("Response received : {0}\n", Encoding.ASCII.GetString(_actionResult.Response)));

                // Clearing the original data after successfully completing all steps
                Task.Run(() => SourceGamesHelper.ClearSourceGames(Properties.Settings.Default["source_filepath"].ToString()));
            }
            catch (Exception ex)
            {
                this.displayMessage.Display(ex.ToString() + '\n');
            }
        }
    }
}
