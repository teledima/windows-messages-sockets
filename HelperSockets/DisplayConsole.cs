using System;

namespace HelperSockets
{
    public class DisplayConsole : IDisplayMessage
    {
        public void Display(string message)
        {
            Console.WriteLine(message);
        }
    }
}
