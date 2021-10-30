#nullable enable

using HelperSockets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsMessagesSockets
{
    public class Client
    {
        private readonly IPEndPoint EndPoint;
        private readonly Label LabelHistory;
        private readonly ChangeHistoryLabel ChangeHistory;

        public Client(Label labelHistory, ChangeHistoryLabel changeHistory)
        {
            // Get host and port from settings
            string host = Properties.Settings.Default["host"].ToString();
            int port = (int)Properties.Settings.Default["port"];

            // Establish the local endpoint for the socket.
            EndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            ChangeHistory = changeHistory;
            LabelHistory = labelHistory;
        }

        public void SendData()
        {
            try
            {
                // Create a TCP/IP socket. 
                using Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                bool _actionResult = false;
                StateObject _stateObject = new()
                {
                    workSocket = client,
                };
                // Connect to the remote endpoint.  
                _actionResult = new ActionConnect(_stateObject, LabelHistory, ChangeHistory, EndPoint).Run();
                if (!_actionResult)
                    return;

                // Send test data to the remote device.  
                _actionResult = new ActionSend(_stateObject, LabelHistory, ChangeHistory, "This is a test<EOF>").Run();
                if (!_actionResult)
                    return;

                // Receive the response from the remote device.  
                _actionResult = new ActionReceive(_stateObject, LabelHistory, ChangeHistory).Run();
                if (!_actionResult)
                    return;
            }
            catch (Exception ex)
            {
                LabelHistory.Invoke(ChangeHistory, ex.ToString() + '\n');
            }
        }
    }
}
