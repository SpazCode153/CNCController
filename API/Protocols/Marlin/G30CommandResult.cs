namespace API.Protocols.Marlin
{
    public class G30CommandResult : BaseGCodeCommandResult
    {
        public double[] Position { get; protected set; }

        public G30CommandResult(bool error,
            bool inProgress,
            bool success,
            double[] position)
            : base(error, inProgress, success)
        {
            Position = position;
        }
    }
}
