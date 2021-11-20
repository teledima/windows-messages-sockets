using System;
using System.Net.Sockets;
namespace HelperSockets
{
    public class SocketActionSend : SocketAction
    {
        private int _flag;
        private readonly byte[] _data;
        private readonly string _displayPattern;
        public SocketActionSend(Socket handler, IDisplayMessage displayMessage, string displayPattern, byte[] data): base(handler, displayMessage)
        {
            _data = data;
            _displayPattern = displayPattern;
            _flag = 0;
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)asyncResult.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(asyncResult);
                if (_flag == 1)
                    _displayMessage.Display(string.Format(_displayPattern, bytesSent));

                // Signal that all bytes have been sent.  
                _eventManual.Set();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message + "\n";
                _eventManual.Set();
            }
        }

        protected override ResultAction RunAction()
        {
            // Sending data size
            _handler.BeginSend(BitConverter.GetBytes(_data.Length), 0, sizeof(int), 0, new AsyncCallback(Callback), _handler);
            if (_eventManual.WaitOne())
            {
                _eventManual.Reset();
                _flag = 1;
                // Begin sending the data to the remote device.  
                _handler.BeginSend(_data, 0, _data.Length, 0, new AsyncCallback(Callback), _handler);
                return new ResultAction() { Success = _eventManual.WaitOne(/*_timeout*/) };
            }
            else
                return new ResultAction() { Success = false };
        }
    }
}
