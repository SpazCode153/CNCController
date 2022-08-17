using System.Reactive.Subjects;
using API.Communication.Messages;

namespace API.Communication
{
    public abstract class BaseCommunicationHandler<TMessage> : IDisposable
        where TMessage : BaseMessage
    {
        public IConnectableObservable<TMessage> MessageStream
        {
            get { return connectableMessageStream; }
        }

        protected IConnectableObservable<TMessage> connectableMessageStream;

        public Func<byte[], Tuple<bool, int>> ChunkPredicate { get; protected set; }

        protected List<byte> buffer = new List<byte>();

        protected bool disposedValue;

        public BaseCommunicationHandler()
        {

        }

        public abstract void Connect();

        public abstract bool IsConnected();

        public abstract void Disconnect();

        public abstract void SendMessage(TMessage message);

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
