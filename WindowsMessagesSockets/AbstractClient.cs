using HelperSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsMessagesSockets
{
    public abstract class AbstractClient
    {
        protected IDisplayMessage displayMessage;
        public AbstractClient(IDisplayMessage displayMessage) 
        {
            this.displayMessage = displayMessage;
        }

        public abstract void SendData(IEnumerable<SourceGames> sourceGames);
    }
}
