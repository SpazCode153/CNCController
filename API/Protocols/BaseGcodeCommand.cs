namespace API.Protocols
{
    public abstract class BaseGcodeCommand<TCommandResult>
        where TCommandResult : BaseGCodeCommandResult
    {
        public string Command { get; protected set; }

        protected bool expectOk;

        protected bool expectResult;

        public abstract TCommandResult ValidateResponse(string response);
    }
}
