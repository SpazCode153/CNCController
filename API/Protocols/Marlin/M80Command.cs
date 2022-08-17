namespace API.Protocols.Marlin
{
    public class M80Command: BaseGcodeCommand<M80CommandResult>
    {
        public M80Command()
        {
            Command = "M80";
            expectOk = true;
            expectResult = false;
        }

        public override M80CommandResult ValidateResponse(string response)
        {
            if (response.Contains("echo:busy: processing"))
            {
                return new M80CommandResult(false, true, false);
            }

            if (response.Contains("ok"))
            {
                return new M80CommandResult(false, false, true);
            }

            return new M80CommandResult(true, false, false);
        }
    }
}
