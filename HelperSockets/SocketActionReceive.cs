using HelperSockets;
using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace HelperSockets
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

                // Put buffer to response
                var buffer = Encoding.ASCII.GetString(stateObject.buffer, 0, bytesRead);

                if (_stateObject.typeAccept == TypeAccept.ImportData)
                    _response += buffer;
                else
                    _stateObject.key += buffer;
                if (client.Available > 0)
                {
                    // Get the rest of the data.  
                    client.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(Callback), stateObject);
                }
                else
                {
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
                if (_stateObject.typeAccept == TypeAccept.ImportData)
                    _displayMessage.Display(string.Format("Response received : {0}\n", _response));
                else if (_stateObject.typeAccept == TypeAccept.SendKey)
                    _displayMessage.Display("Key received\n");
            return _actionRes;
        }
    }
}
