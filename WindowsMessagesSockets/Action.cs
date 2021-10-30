using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelperSockets;

namespace WindowsMessagesSockets
{
    public abstract class Action
    {
        protected readonly Label _label;
        protected readonly ChangeHistoryLabel _changeHistory;
        protected readonly ManualResetEvent resetEvent = new ManualResetEvent(false);
        protected readonly StateObject _stateObject = new StateObject();
        protected readonly int Timeout;
        public Action(StateObject stateObject, Label label, ChangeHistoryLabel changeHistory)
        {
            _stateObject = stateObject;
            _label = label;
            _changeHistory = changeHistory;
            Timeout = (int)Properties.Settings.Default["timeout"];
        }
        public bool Run()
        {
            bool res = RunAction();
            if (!res || !string.IsNullOrEmpty(_stateObject.errorMessage))
            {
                _label.Invoke(_changeHistory, _stateObject.errorMessage);
                return false;
            }
            return true;
        }

        protected abstract bool RunAction();
        protected abstract void Callback(IAsyncResult asyncResult);
    }
}
