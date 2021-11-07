using System;
using System.Net.Sockets;
namespace HelperSockets
{
    public class SocketActionSend : SocketAction
    {
        private readonly byte[] _data;
        private readonly string _displayPattern;
        public SocketActionSend(Socket handler, IDisplayMessage displayMessage, string displayPattern, byte[] data): base(handler, displayMessage)
        {
            _data = data;
            _displayPattern = displayPattern;
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)asyncResult.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(asyncResult);
                _displayMessage.Display(string.Format(_displayPattern, _data.Length));

                // Signal that all bytes have been sent.  
                _eventManual.Set();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message + "\n";
                _eventManual.Set();
            }
        }

        protected override ResultAction RunAction()
        {
            // Begin sending the data to the remote device.  
            _handler.BeginSend(_data, 0, _data.Length, 0, new AsyncCallback(Callback), _handler);

            return new ResultAction() { Success = _eventManual.WaitOne(_timeout) };
        }
    }
}
