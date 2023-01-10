using API.Machines;

namespace ConsoleCommand
{
    public class PositionSquare
    {
        public Position TopLeft { get; set; }

        public Position TopRight { get; set; }

        public Position BottomLeft { get; set; }

        public Position BottomRight { get; set; }

        public double GridSpacing { get; set; }

        private IEnumerable<Position> grid { get; set; }



        public PositionSquare(Position topLeft,
            Position bottomLeft,
            Position topRight,
            Position bottomRight,
            double gridSpacing)
        {
            TopLeft = topLeft;
            BottomLeft = bottomLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            GridSpacing = gridSpacing;
        }

        private void buildGrid()
        {
            List<Position> grid = new List<Position>();
            bool onLeftSquareLine = false;
            bool onRightSquareLine = false;

            for (double x = BottomLeft.X; x < Math.Round((TopRight.X + GridSpacing),2); x = Math.Round(x + GridSpacing,2))
            {
                (double x, double y) pos1;
                (double x, double y) pos2;

                if (x == BottomLeft.X)
                {
                    //This is projecting 3D space to 2D in order to determine the Z height at a position
                    //on a line moving in the Y axis there for the following transalations happen
                    //3D Y becomes 2D X and 3D Z becomes 2D Y
                    pos1 = (BottomLeft.Y, BottomLeft.Z);
                    pos2 = (TopLeft.Y, TopLeft.Z);
                }
                else if (x == TopRight.X)
                {
                    //This is projecting 3D space to 2D in order to determine the Z height at a position
                    //on a line moving in the Y axis there for the following transalations happen
                    //3D Y becomes 2D X and 3D Z becomes 2D Y
                    pos1 = (BottomRight.Y, BottomRight.Z);
                    pos2 = (TopRight.Y, TopRight.Z);
                }
                else
                {
                    //We are somewhere in the middle with no Z coordinates
                    //We have to calculate the Z coordinate for the top position and bottom position
                    //This is projecting 3D space to 2D in order to determine the Z height at a position
                    //on a line moving in the X axis there for the following transalations happen
                    //3D X becomes 2D X and 3D Z becomes 2D Y
                    double z1Pos = Math.Round(CalculateYValue(BottomRight.Z, BottomLeft.Z, BottomRight.X, BottomLeft.X, x), 2);
                    double z2Pos = Math.Round(CalculateYValue(TopRight.Z, TopLeft.Z, TopRight.X, TopLeft.X, x), 2);

                    pos1 = (BottomRight.Y, z1Pos);
                    pos2 = (TopRight.Y, z2Pos);
                }

                for (double y = BottomLeft.Y; y < Math.Round((TopRight.Y + GridSpacing),2); y = Math.Round(y + GridSpacing, 2))
                {
                    grid.Add(new Position()
                    {
                        X = x,
                        Y = y,
                        Z = Math.Round(CalculateYValue(pos2.y, pos1.y, pos2.x, pos1.x, y),2 )
                    });
                }
            }

            this.grid = grid;
        }



        public bool IsPositionInSquare(Position position)
        {
            if(position.X >= TopLeft.X && position.X <= TopRight.X)
            {
                if (position.Y >= BottomLeft.Y && position.Y <= TopLeft.Y)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsPositionInSquare(double x, double y)
        {
            if (x >= TopLeft.X && x <= TopRight.X)
            {
                if (y >= BottomLeft.Y && y <= TopLeft.Y)
                {
                    return true;
                }
            }

            return false;
        }

        public Position ApplyZHeight(Position position)
        {
            (double x, double y) pos1;
            (double x, double y) pos2;

            if (Math.Round(position.X, 2) == Math.Round(BottomLeft.X,2))
            {
                //This is projecting 3D space to 2D in order to determine the Z height at a position
                //on a line moving in the Y axis there for the following transalations happen
                //3D Y becomes 2D X and 3D Z becomes 2D Y
                pos1 = (BottomLeft.Y, BottomLeft.Z);
                pos2 = (TopLeft.Y, TopLeft.Z);
            }
            else if (Math.Round(position.X, 2) == Math.Round(TopRight.X,2))
            {
                //This is projecting 3D space to 2D in order to determine the Z height at a position
                //on a line moving in the Y axis there for the following transalations happen
                //3D Y becomes 2D X and 3D Z becomes 2D Y
                pos1 = (BottomRight.Y, BottomRight.Z);
                pos2 = (TopRight.Y, TopRight.Z);
            }
            else
            {
                //We are somewhere in the middle with no Z coordinates
                //We have to calculate the Z coordinate for the top position and bottom position
                //This is projecting 3D space to 2D in order to determine the Z height at a position
                //on a line moving in the X axis there for the following transalations happen
                //3D X becomes 2D X and 3D Z becomes 2D Y
                double z1Pos = Math.Round(CalculateYValue(BottomRight.Z, BottomLeft.Z, BottomRight.X, BottomLeft.X, position.X), 2);
                double z2Pos = Math.Round(CalculateYValue(TopRight.Z, TopLeft.Z, TopRight.X, TopLeft.X, position.X), 2);

                pos1 = (BottomRight.Y, z1Pos);
                pos2 = (TopRight.Y, z2Pos);
            }

            return new Position()
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z + Math.Round(CalculateYValue(pos2.y, pos1.y, pos2.x, pos1.x, position.Y), 2)
            };
        }







        private double CalculateSlope(double y2, double y1, double x2, double x1)
        {
            return ((y2 - y1) / (x2 - x1));
        }

        private double CalculateC(double rawMax, double slope, double euMax)
        {
            return rawMax - (slope * euMax);
        }

        public double CalculateXValue(double y2, double y1, double x2, double x1, double y)
        {
            double slope = CalculateSlope(y2, y1, x2, x1);

            double c = CalculateC(y2, slope, x2);

            //y = mx + c
            //y - c = mx
            //y - c / m = x
            return ((y - c) / slope);
        }

        public double CalculateYValue(double y2, double y1, double x2, double x1, double x)
        {
            double slope = CalculateSlope(y2, y1, x2, x1);

            double c = CalculateC(y2, slope, x2);

            //y = mx + c
            return (slope * x) + c;
        }
    }
}
