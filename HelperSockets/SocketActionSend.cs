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
        private readonly byte[] _data;
        private readonly string _displayPattern;
        public SocketActionSend(StateObject stateObject, IDisplayMessage displayMessage, string displayPattern, byte[] data): base(stateObject, displayMessage)
        {
            _data = data;
            _displayPattern = displayPattern;
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
                _displayMessage.Display(string.Format(_displayPattern, _data.Length));

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
            // Begin sending the data to the remote device.  
            _stateObject.workSocket.BeginSend(_data, 0, _data.Length, 0, new AsyncCallback(Callback), _stateObject);

            return _eventManual.WaitOne(_timeout);
        }
    }
}
