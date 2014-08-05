using System;

namespace BlockPartyShared
{
    [Serializable]
    public class NetworkMessage
    {
        public enum MessageType
        {
            GameState,
            RoundResults
        }

        public MessageType Type;
        public object Content;

        public NetworkMessage()
        {
        }
    }
}

