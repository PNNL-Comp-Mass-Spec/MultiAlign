using System;

namespace MultiAlign.ViewModels.Charting
{
    public class PositionArgs: EventArgs
    {

        public PositionArgs(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
    }
    
}
