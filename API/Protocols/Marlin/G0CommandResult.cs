namespace API.Protocols.Marlin
{
    public class G0CommandResult : BaseGCodeCommandResult
    {
        public G0CommandResult(bool error,
            bool inProgress,
            bool success)
            : base(error, inProgress, success)
        {
        }
    }
}
