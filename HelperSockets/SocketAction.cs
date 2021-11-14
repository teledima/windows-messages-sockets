using System;
using System.Net.Sockets;
using System.Threading;

namespace HelperSockets
{
    public abstract class SocketAction
    {
        protected Socket _handler;
        protected IDisplayMessage _displayMessage;
        protected string _errorMessage = string.Empty;

        protected ManualResetEvent _eventManual = new(false);
        protected int _timeout = (int)Properties.Settings.Default["timeout"];
        
        public SocketAction(Socket handler, IDisplayMessage displayMessage)
        {
            _handler = handler;
            _displayMessage = displayMessage;
        }

        /// <summary>
        ///  Etrypoint for socket action
        /// </summary>
        /// <returns></returns>
        public ResultAction Run()
        {
            var res = new ResultAction();
            try
            {
                res = RunAction();
            }
            catch
            {
                res.Success = false;
            }
            if (!string.IsNullOrEmpty(_errorMessage))
                _displayMessage.Display(_errorMessage);
            return res;
        }

        protected abstract ResultAction RunAction();
        protected abstract void Callback(IAsyncResult asyncResult);
    }
}
