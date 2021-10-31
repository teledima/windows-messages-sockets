using System;
using HelperSockets;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace HelperSockets
{
    public class SocketActionConnect : SocketAction
    {
        private readonly IPEndPoint _endPoint;
        public SocketActionConnect(StateObject stateObject, IDisplayMessage displayMessage, IPEndPoint endPoint): base(stateObject, displayMessage) 
        {
            _endPoint = endPoint;
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            StateObject stateObject = (StateObject)asyncResult.AsyncState;
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = stateObject.workSocket;

                // Complete the connection.  
                client.EndConnect(asyncResult);
                _displayMessage.Display(string.Format("Socket connected to {0}\n", client.RemoteEndPoint.ToString()));

                // Signal that the connection has been made.  
                _eventManual.Set();
            }
            catch (Exception ex)
            {
                stateObject.errorMessage = ex.Message + "\n";
                _eventManual.Set();
            }
        }

        protected override bool RunAction()
        {
            _stateObject.workSocket.BeginConnect(_endPoint, new AsyncCallback(Callback), _stateObject);
            return _eventManual.WaitOne(_timeout);
        }
    }
}
