using System.Text;

namespace API.Communication.Messages
{
    public class SerialMessage: BaseMessage
    {
        public SerialMessage(string message)
        {
            Message = message;
            MessagePayload = Encoding.UTF8.GetBytes(message);
        }

        public SerialMessage(byte[] message)
        {
            MessagePayload = message;
            Message = Encoding.UTF8.GetString(message);
        }
    }
}
