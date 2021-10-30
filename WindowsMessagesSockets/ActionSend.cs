using HelperSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsMessagesSockets
{
    public class ActionSend : Action
    {
        private string _message;
        public ActionSend(StateObject stateObject, Label label, ChangeHistoryLabel changeHistory, string message): base(stateObject, label, changeHistory)
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
                _label.Invoke(_changeHistory, string.Format("Sent {0} bytes to server.\n", bytesSent));

                // Signal that all bytes have been sent.  
                resetEvent.Set();
            }
            catch (Exception ex)
            {
                stateObject.errorMessage = ex.Message + "\n";
                resetEvent.Set();
            }
        }

        protected override bool RunAction()
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(_message);

            // Begin sending the data to the remote device.  
            _stateObject.workSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(Callback), _stateObject);

            return resetEvent.WaitOne(Timeout);
        }
    }
}
