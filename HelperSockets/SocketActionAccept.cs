using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace HelperSockets
{
    public class SocketActionAccept : SocketAction
    {
        public SocketActionAccept(StateObject stateObject, IDisplayMessage displayMessage) : base(null, displayMessage)
        {
            _stateObject = stateObject;
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            // Signal the main thread to continue.  
            _eventManual.Set();
            // Get the socket that handles the client request.  
            Socket listener = (Socket)asyncResult.AsyncState;
            Socket handler = listener.EndAccept(asyncResult);
            // Create the state object.  
            StateObject state = new StateObject
            {
                workSocket = handler
            };
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        protected override bool RunAction()
        {
            // Start an asynchronous socket to listen for connections.  
            _displayMessage.Display("Waiting for a connection...");
            // Set the event to nonsignaled state.  
            _eventManual.Reset();

            _stateObject.workSocket.BeginAccept(new AsyncCallback(Callback), _stateObject.workSocket);

            // Wait until a connection is made before continuing.  
            return _eventManual.WaitOne();

        }

        private void ReadCallback(IAsyncResult ar)
        {
            string content = string.Empty;
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the
                    // client. Display it.  
                    _displayMessage.Display(string.Format("Read {0} bytes from socket. \n Data : {1}", content.Length, content));

                    Thread.Sleep(1000 * 6);
                    // Echo the data back to the client. 
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                _displayMessage.Display(string.Format("Sent {0} bytes to client.", bytesSent));

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                _displayMessage.Display(e.Message);
            }
        }
    }
}
