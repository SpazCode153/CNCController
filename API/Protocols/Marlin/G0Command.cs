namespace API.Protocols.Marlin
{
    public class G0Command : BaseGcodeCommand<G0CommandResult>
    {
        public G0Command(double? x, double? y, double? z)
        {
            Command = $"G0";

            if(x == null && y == null && z == null)
            {
                throw new InvalidOperationException("Cannot call a G0 command wihtout any coordinates");
            }

            if(x != null)
            {
                Command = $"{Command} X{x}";
            }

            if(y != null)
            {
                Command = $"{Command} Y{y}";
            }

            if (z != null)
            {
                Command = $"{Command} Z{z}";
            }

            expectOk = true;
            expectResult = false;
        }

        public override G0CommandResult ValidateResponse(string response)
        {
            if (response == "echo:busy: processing\n")
            {
                return new G0CommandResult(false, true, false);
            }

            if (response == "ok\n")
            {
                return new G0CommandResult(false, false, true);
            }

            throw new InvalidDataException($"Response received from machine is invalid: {response}");
        }
    }
}
