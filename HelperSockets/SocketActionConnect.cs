using System;
using System.Net;
using System.Net.Sockets;

namespace HelperSockets
{
    public class SocketActionConnect : SocketAction
    {
        private readonly IPEndPoint _endPoint;
        public SocketActionConnect(Socket handler, IDisplayMessage displayMessage, IPEndPoint endPoint): base(handler, displayMessage) 
        {
            _endPoint = endPoint;
            _handler = handler;
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)asyncResult.AsyncState;

                // Complete the connection.  
                handler.EndConnect(asyncResult);
                _displayMessage.Display(string.Format("Socket connected to {0}\n", handler.RemoteEndPoint.ToString()));

                // Signal that the connection has been made.  
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
            _handler.BeginConnect(_endPoint, new AsyncCallback(Callback), _handler);
            return new ResultAction() { Success = _eventManual.WaitOne(_timeout) };
        }
    }
}
