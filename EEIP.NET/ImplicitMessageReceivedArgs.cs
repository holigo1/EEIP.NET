using System;

namespace Sres.Net.EEIP
{
    public class ImplicitMessageReceivedArgs : EventArgs
    {
        public uint ConnectionId { get; }

        public ImplicitMessageReceivedArgs(uint connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}