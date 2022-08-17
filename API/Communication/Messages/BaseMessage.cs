namespace API.Communication.Messages
{
    public abstract class BaseMessage
    {
        public string Message { get; protected set; }

        public byte[] MessagePayload { get; protected set; }
    }
}
