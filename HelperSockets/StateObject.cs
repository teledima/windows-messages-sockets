using System.Collections.Generic;
using System.Net.Sockets;

namespace HelperSockets
{
    public enum TypeAccept { SendKey, ImportData };
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public List<byte> data = new();

        public TypeAccept typeAccept;
        // Error state
        public string errorMessage = string.Empty;
        // public rsa key
        public string key = string.Empty;
    }
}
