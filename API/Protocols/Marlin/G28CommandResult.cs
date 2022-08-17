namespace API.Protocols.Marlin
{
    public class G28CommandResult: BaseGCodeCommandResult
    {
        public double[] Position { get; protected set; }

        public G28CommandResult(bool error, 
            bool inProgress,
            bool success,
            double[] position)
            : base(error, inProgress, success)
        {
            Position = position;
        }
    }
}
