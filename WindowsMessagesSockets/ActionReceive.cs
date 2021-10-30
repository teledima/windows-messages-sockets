using HelperSockets;
using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace WindowsMessagesSockets
{
    public class ActionReceive : Action
    {
        private string _response;
        public ActionReceive(StateObject stateObject, Label label, ChangeHistoryLabel changeHistory) : base(stateObject, label, changeHistory)
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
                    resetEvent.Set();
                }
            }
            catch (Exception ex)
            {
                stateObject.errorMessage = ex.Message + "\n";
                resetEvent.Set();
            }
        }

        protected override bool RunAction()
        {
            // Begin receiving the data from the remote device.  
            _stateObject.workSocket.BeginReceive(_stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(Callback), _stateObject);
            bool _actionRes = resetEvent.WaitOne();
            if (_actionRes)
                _label.Invoke(_changeHistory, string.Format("Response received : {0}\n", _response));
            return _actionRes;
        }
    }
}
