namespace API.Protocols
{
    public abstract class BaseGCodeCommandResult
    {
        public bool Error { get; protected set; }

        public bool InProgress { get; protected set; }

        public bool Success { get; protected set; }

        public BaseGCodeCommandResult(bool error, bool inProgress, bool success)
        {
            Error = error;
            InProgress = inProgress;
            Success = success;
        }
    }
}
