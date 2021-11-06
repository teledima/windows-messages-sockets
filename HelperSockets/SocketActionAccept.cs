﻿using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace HelperSockets
{
    public class SocketActionAccept : SocketAction
    {
        private RSACryptoServiceProvider _rsa;
        public SocketActionAccept(StateObject stateObject, IDisplayMessage displayMessage) : base(null, displayMessage)
        {
            _stateObject = stateObject;
            _stateObject.typeAccept = TypeAccept.SendKey;
            _rsa = new RSACryptoServiceProvider();
        }
        protected override void Callback(IAsyncResult asyncResult)
        {
            // Signal the main thread to continue.  
            _eventManual.Set();
            // Get the socket that handles the client request.  
            var stateObject = (StateObject)asyncResult.AsyncState;
            Socket listener = stateObject.workSocket;
            Socket handler = listener.EndAccept(asyncResult);

            // Create the state object.  
            StateObject state = new()
            {
                workSocket = handler
            };

            state.typeAccept = TypeAccept.SendKey;
            new SocketActionSend(
                state,
                _displayMessage, "Send {0} bytes to client.", 
                Encoding.ASCII.GetBytes(_rsa.ToXmlString(false))
            ).Run();

            state.typeAccept = TypeAccept.ImportData;
            handler.BeginReceive(
                state.buffer, 0, StateObject.BufferSize, 0, 
                new AsyncCallback(ReadCallback), 
                state);
        }

        protected override bool RunAction()
        {
            // Start an asynchronous socket to listen for connections.  
            _displayMessage.Display("Waiting for a connection...");
            // Set the event to nonsignaled state.  
            _eventManual.Reset();
            _stateObject.typeAccept = TypeAccept.SendKey;
            _stateObject.workSocket.BeginAccept(new AsyncCallback(Callback), _stateObject);

            // Wait until a connection is made before continuing.  
            return _eventManual.WaitOne();

        }

        private void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            // Move buffer to data
            state.data.AddRange(state.buffer.ToList().GetRange(0, bytesRead));
            if (handler.Available > 0)
            {
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            }
            else if (bytesRead > 0)
            {
                // All the data has been read from the client.

                // Split content and key
                var key = _rsa.Decrypt(state.data.GetRange(state.data.Count - 256, 128).ToArray(), false);
                var iv = _rsa.Decrypt(state.data.GetRange(state.data.Count - 128, 128).ToArray(), false);


                var sourceGames = SourceGamesHelper.Decrypt(state.data.GetRange(0, state.data.Count - 256).ToArray(), key, iv);

                try
                {
                    // Export data
                    Task.Run(() => SourceGamesHelper.ExportToPostgres(sourceGames));
                }
                catch (Exception e)
                {
                    _displayMessage.Display(e.Message);
                }

                _displayMessage.Display(string.Format("Read {0} bytes from socket.", state.data.Count));


                new SocketActionSend(state, _displayMessage, "Send {0} bytes to client.", Encoding.ASCII.GetBytes(state.data.Count.ToString())).Run();
            }
        }

        ~SocketActionAccept()
        {
            _rsa.Clear();
        }
    }
}
