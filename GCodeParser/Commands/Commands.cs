namespace GCodeParser.Commands
{
    public static class Commands
    {
        public static BaseCommand GetCommand(string command, string line)
        {
            BaseCommand cmd = getCommand(command, line);

            if(cmd != null)
            {
                return cmd;
            }

            throw new KeyNotFoundException("Invalid or unknown command");
        }

        private static BaseCommand getCommand(string command, string line)
        {
            switch(command)
            {
                case "G0":
                    {
                        return new G0Command(line);
                    }
                case "G00":
                    {
                        return new G0Command(line);
                    }
                case "G1":
                    {
                        return new G1Command(line);
                    }
                case "G01":
                    {
                        return new G1Command(line);
                    }
                case "G4":
                    {
                        return new G4Command(line);
                    }
                case "G04":
                    {
                        return new G4Command(line);
                    }
                case "G21":
                    {
                        return new G21Command(line);
                    }
                case "G90":
                    {
                        return new G90Command(line);
                    }
                case "M3":
                    {
                        return new M3Command(line);
                    }
                case "M03":
                    {
                        return new M3Command(line);
                    }
                case "M5":
                    {
                        return new M5Command(line);
                    }
                case "M05":
                    {
                        return new M5Command(line);
                    }
            }

            return null;
        }
    }
}
