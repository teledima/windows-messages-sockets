using System;
using HelperSockets;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace WindowsMessagesSockets
{
    public class ActionConnect : Action
    {
        private readonly IPEndPoint _endPoint;
        public ActionConnect(StateObject stateObject, Label label, ChangeHistoryLabel changeHistory,   IPEndPoint endPoint): base(stateObject, label, changeHistory) 
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

                _label.Invoke(_changeHistory, string.Format("Socket connected to {0}\n", client.RemoteEndPoint.ToString()));

                // Signal that the connection has been made.  
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
            _stateObject.workSocket.BeginConnect(_endPoint, new AsyncCallback(Callback), _stateObject);
            return resetEvent.WaitOne(Timeout);
        }
    }
}
