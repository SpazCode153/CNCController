namespace API.Protocols.Marlin
{
    public class G28Command: BaseGcodeCommand<G28CommandResult>
    {
        public G28Command(bool x, bool y, bool z)
        {
            Command = "G28";

            if(!x || !y || !z)
            {
                if(x)
                {
                    Command = $"{Command} X";
                }
                if (y)
                {
                    Command = $"{Command} Y";
                }
                if (z)
                {
                    Command = $"{Command} Z";
                }
            }

            expectOk = false;
            expectResult = true;
        }

        public override G28CommandResult ValidateResponse(string response)
        {
            if(response.Contains("echo:busy: processing"))
            {
                return new G28CommandResult(false, true, false, new double[] { 0, 0, 0 });
            }

            if(response.Contains('X') && response.Contains('Y') && response.Contains('Z'))
            {
                string line = response.Substring(2, (response.Length - 2));
                string xStr = line.Substring(0, line.IndexOf(" "));

                line = response.Substring(line.IndexOf(" ") + 3, (response.Length - 3));
                string yStr = line.Substring(0, line.IndexOf(" "));

                line = response.Substring(line.IndexOf(" ") + 3, (response.Length - 3));
                string zStr = line.Substring(0, line.Length);

                double x = 0;
                double y = 0;
                double z = 0;

                try
                {
                    x = double.Parse(xStr);
                }
                catch(Exception)
                {
                    throw new InvalidDataException($"X coordiante in response from machine is invalid: {response}");
                }

                try
                {
                    y = double.Parse(xStr);
                }
                catch (Exception)
                {
                    throw new InvalidDataException($"Y coordiante in response from machine is invalid: {response}");
                }

                try
                {
                    z = double.Parse(xStr);
                }
                catch (Exception)
                {
                    throw new InvalidDataException($"Z coordiante in response from machine is invalid: {response}");
                }

                return new G28CommandResult(false, false, true, new double[] { x, y, z });
            }

            throw new InvalidDataException($"Response received from machine is invalid: {response}");
        }
    }
}
