using System;

namespace BlockPartyShared
{
    [Serializable]
    public class NetworkMessage
    {
        public enum MessageType
        {
            ServerGameState,
            ServerGameResults,
            ClientGameResults
        }

        public MessageType Type;
        public object Content;

        public NetworkMessage()
        {
        }

        public override string ToString()
        {
            return string.Format("[NetworkMessage: Type={0}, Content={1}]", Type.ToString(), Content.ToString());
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public NetworkMessage Message { get; set; }

        public string Sender { get; set; }
    }
}

