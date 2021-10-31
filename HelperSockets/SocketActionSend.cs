using HelperSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelperSockets
{
    public class SocketActionSend : SocketAction
    {
        private string _message;
        public SocketActionSend(StateObject stateObject, IDisplayMessage displayMessage, string message): base(stateObject, displayMessage)
        {
            _message = message;
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            StateObject stateObject = (StateObject)asyncResult.AsyncState;
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = stateObject.workSocket;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(asyncResult);
                _displayMessage.Display(string.Format("Sent {0} bytes to server.\n", bytesSent));

                // Signal that all bytes have been sent.  
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
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(_message);

            // Begin sending the data to the remote device.  
            _stateObject.workSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(Callback), _stateObject);

            return _eventManual.WaitOne(_timeout);
        }
    }
}
