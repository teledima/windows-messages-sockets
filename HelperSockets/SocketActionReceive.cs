using HelperSockets;
using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace WindowsMessagesSockets
{
    public class SocketActionReceive : SocketAction
    {
        private string _response;
        public SocketActionReceive(StateObject stateObject, IDisplayMessage displayMessage) : base(stateObject, displayMessage)
        {

        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            // Retrieve the state object and the client socket
            // from the asynchronous state object.  
            StateObject stateObject = (StateObject)asyncResult.AsyncState;
            try
            {
                Socket client = stateObject.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(asyncResult);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    stateObject.sb.Append(Encoding.ASCII.GetString(stateObject.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(Callback), stateObject);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (stateObject.sb.Length > 1)
                    {
                        _response = stateObject.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    _eventManual.Set();
                }
            }
            catch (Exception ex)
            {
                stateObject.errorMessage = ex.Message + "\n";
                _eventManual.Set();
            }
        }

        protected override bool RunAction()
        {
            // Begin receiving the data from the remote device.  
            _stateObject.workSocket.BeginReceive(_stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(Callback), _stateObject);
            bool _actionRes = _eventManual.WaitOne();
            if (_actionRes)
                _displayMessage.Display(string.Format("Response received : {0}\n", _response));
            return _actionRes;
        }
    }
}
