using System;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;

namespace HelperSockets
{
    public class SocketActionReceive : SocketAction
    {
        private List<byte> _response;
        private byte[] _buffer;
        private int _bufferSize;
        public int BufferSize
        {
            get { return _bufferSize; }
            set { if (value > 0) _bufferSize = value; }
        }
        public SocketActionReceive(Socket handler, IDisplayMessage displayMessage) : base(handler, displayMessage)
        {
            // Set default buffer size
            BufferSize = 256;
            _buffer = new byte[BufferSize];
            _response = new List<byte>();
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                Socket handler = (Socket)asyncResult.AsyncState;
                
                // Read data from the remote device.  
                int bytesRead = handler.EndReceive(asyncResult);

                // Move buffer to response.
                _response.AddRange(_buffer.ToList().GetRange(0, bytesRead));
                if (handler.Available > 0)
                {
                    if (handler.Available < BufferSize)
                        BufferSize = handler.Available;
                    // Get the rest of the data.  
                    handler.BeginReceive(_buffer, 0, BufferSize, 0,
                        new AsyncCallback(Callback), handler);
                }
                else
                {
                    // Signal that all bytes have been received.  
                    _eventManual.Set();
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message + "\n";
                _eventManual.Set();
            }
        }

        protected override ResultAction RunAction()
        {
            // Begin receiving the data from the remote device.  
            _handler.BeginReceive(_buffer, 0, BufferSize, 0, new AsyncCallback(Callback), _handler);
            return new ResultAction() { Success = _eventManual.WaitOne(/*_timeout*/), Response = _response.ToArray() };
        }
    }
}
