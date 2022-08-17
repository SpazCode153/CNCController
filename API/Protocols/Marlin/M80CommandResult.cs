namespace API.Protocols.Marlin
{
    public class M80CommandResult: BaseGCodeCommandResult
    {
        public M80CommandResult(bool error,
            bool inProgress,
            bool success)
            : base(error, inProgress, success)
        {
        }
    }
}
