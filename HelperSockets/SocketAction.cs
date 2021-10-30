using System;
using System.Threading;

namespace HelperSockets
{
    public abstract class SocketAction
    {
        protected StateObject _stateObject;
        protected IDisplayMessage _displayMessage;
        protected ManualResetEvent _eventManual = new ManualResetEvent(false);
        protected int _timeout = (int)Properties.Settings.Default["timeout"];
        public SocketAction(StateObject stateObject, IDisplayMessage displayMessage)
        {
            _stateObject = stateObject;
            _displayMessage = displayMessage;
        }

        public bool Run()
        {
            bool res = RunAction();
            if (!res || !string.IsNullOrEmpty(_stateObject.errorMessage))
            {
                _displayMessage.Display(_stateObject.errorMessage);
                return false;
            }
            return true;
        }

        protected abstract bool RunAction();
        protected abstract void Callback(IAsyncResult asyncResult);
    }
}
