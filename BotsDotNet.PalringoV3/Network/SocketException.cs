using System;

namespace BotsDotNet.PalringoV3.Network
{
    public class SocketException : Exception
    {
        public object ReturnData { get; set; }

        public SocketException(object returnData, string message) : base(message)
        {
            ReturnData = returnData;
        }

        public SocketException(object returnData, string message, Exception inner) : base(message, inner)
        {
            ReturnData = returnData;
        }
    }
}
