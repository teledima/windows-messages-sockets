using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace HelperSockets
{
    public class SocketActionAccept : SocketAction
    {
        private RSACryptoServiceProvider _rsa;
        public SocketActionAccept(Socket handler, IDisplayMessage displayMessage) : base(handler, displayMessage)
        {
            _rsa = new RSACryptoServiceProvider();
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

            var result = new SocketActionReceive(handler, _displayMessage).Run();
            if (result.Success)
            {
                // Get all data from server.
                _displayMessage.Display(string.Format("Get {0} from client", result.Response.Length));
                // Decrypt and export.

                var data = result.Response.ToList();
                // Extract DES key and initialization vector
                var key = _rsa.Decrypt(data.GetRange(data.Count - 256, 128).ToArray(), false);
                var iv = _rsa.Decrypt(data.GetRange(data.Count - 128, 128).ToArray(), false);

                // Decrypt main content
                var sourceGames = SourceGamesHelper.Decrypt(data.GetRange(0, data.Count - 256).ToArray(), key, iv);

                try
                {
                    // Export data
                    Task.Run(() => SourceGamesHelper.ExportToPostgres(sourceGames));
                }
                catch (Exception e)
                {
                    _displayMessage.Display(e.Message);
                }

                var message = string.Format("Process {0} bytes", data.Count);

                // Send response to client 
                new SocketActionSend(handler, _displayMessage, "Send {0} bytes to client.", Encoding.ASCII.GetBytes(message)).Run();
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
            return new ResultAction() { Success = _eventManual.WaitOne(_timeout) };

        }

        ~SocketActionAccept()
        {
            _rsa.Clear();
        }
    }
}
