using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;

namespace HelperSockets
{
    public class SocketActionAccept : SocketAction
    {
        private RSACryptoServiceProvider _rsa;
        public SocketActionAccept(Socket handler, IDisplayMessage displayMessage) : base(handler, displayMessage)
        {
            _rsa = new RSACryptoServiceProvider(4096);
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            // Signal the main thread to continue.  
            _eventManual.Set();
            // Get the socket that handles the client request.
            Socket listener = (Socket)asyncResult.AsyncState;
            Socket handler = listener.EndAccept(asyncResult);

            // Send public key to client
            new SocketActionSend(
                handler,
                _displayMessage, "Send {0} bytes to client.", 
                Encoding.ASCII.GetBytes(_rsa.ToXmlString(false))
            ).Run();

            var aesService = new AesService();
            var result = new SocketActionReceive(handler, _displayMessage).Run();
            if (result.Success)
            {
                _displayMessage.Display(string.Format("Get key from client", result.Response.Length));
                aesService = AesService.FromBytes(_rsa.Decrypt(result.Response, false));
            }

            var sourceGames = new List<SourceGames>();
            while (true)
            {
                result = new SocketActionReceive(handler, _displayMessage).Run();
                if (result.Success)
                {
                    // Get all data from server.
                    _displayMessage.Display(string.Format("Get {0} bytes from client", result.Response.Length));

                    // Decrypt response
                    var data = aesService.Decrypt(result.Response).ToList().Where(symbol => symbol != 0).ToArray();

                    // check if the answer is a keyword, then exit the loop
                    if (Encoding.ASCII.GetString(data) == "<EOF>")
                        break;

                    sourceGames.AddRange(SourceGamesHelper.Parse(data).ToList());

                    _displayMessage.Display(string.Format("Process {0} bytes", data.Length));
                }
                else
                    break;
            };

            // Send response to client 
            new SocketActionSend(handler, _displayMessage, "All data gets.", Encoding.ASCII.GetBytes("Ok")).Run();

            try
            {
                // Export data
                SourceGamesHelper.ExportToPostgres(sourceGames);
            }
            catch (Exception e)
            {
                _displayMessage.Display(e.Message);
            }
        }

        protected override ResultAction RunAction()
        {
            // Start an asynchronous socket to listen for connections.  
            _displayMessage.Display("Waiting for a connection...");
            // Set the event to nonsignaled state.  
            _eventManual.Reset();
            _handler.BeginAccept(new AsyncCallback(Callback), _handler);

            // Wait until a connection is made before continuing.  
            return new ResultAction() { Success = _eventManual.WaitOne() };

        }

        ~SocketActionAccept()
        {
            _rsa.Clear();
        }
    }
}
