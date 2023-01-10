namespace API.Protocols.Marlin
{
    public class G30Command : BaseGcodeCommand<G30CommandResult>
    {
        public G30Command(double? x, double? y)
        {
            Command = $"G30";

            if (x != null)
            {
                Command = $"{Command} X{x}";
            }

            if (y != null)
            {
                Command = $"{Command} Y{y}";
            }

            expectOk = true;
            expectResult = false;
        }

        public override G30CommandResult ValidateResponse(string response)
        {
            if (response == "echo:busy: processing\n")
            {
                return new G30CommandResult(false, true, false, new double[] { 0, 0, 0 });
            }

            //TODO: This is output is actually from machines that have debug enabled
            if (response.Contains("measured_z:"))
            {
                return new G30CommandResult(false, true, false, new double[] { 0, 0, 0 });
            }

            if (response.Contains("Bed X: "))
            {
                string line = response.Substring(7, (response.Length - 8));
                string xStr = line.Substring(0, line.IndexOf(" "));

                line = line.Substring(line.IndexOf(" ") + 4, (line.Length - (line.IndexOf(" ") + 4)));
                string yStr = line.Substring(0, line.IndexOf(" "));

                line = line.Substring(line.IndexOf(" ") + 4, (line.Length - (line.IndexOf(" ") + 4)));
                string zStr = line.Substring(0, line.Length);

                double x = 0;
                double y = 0;
                double z = 0;

                try
                {
                    x = double.Parse(xStr);
                }
                catch (Exception)
                {
                    throw new InvalidDataException($"X coordiante in response from machine is invalid: {response}");
                }

                try
                {
                    y = double.Parse(yStr);
                }
                catch (Exception)
                {
                    throw new InvalidDataException($"Y coordiante in response from machine is invalid: {response}");
                }

                try
                {
                    z = double.Parse(zStr);
                }
                catch (Exception)
                {
                    throw new InvalidDataException($"Z coordiante in response from machine is invalid: {response}");
                }

                return new G30CommandResult(false, false, true, new double[] { x, y, z });
            }

            throw new InvalidDataException($"Response received from machine is invalid: {response}");
        }
    }
}
