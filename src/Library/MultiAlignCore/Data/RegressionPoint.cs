using System;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Object used for the LCMS Warp regressions, has a Net (or MZ), Mass error and Net Error
    /// </summary>
    public sealed class RegressionPoint : IComparable, IEquatable<RegressionPoint>
    {

        public RegressionPoint(double x, double y)
            : this(x, y, 0, 0)
        {

        }

        public RegressionPoint(double x, double y, double netError, double massError)
        {
            X = x;
            Y = y;
            MassError = massError;
            NetError = netError;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double NetError { get; set; }

        // Mass error in ppm
        public double MassError { get; set; }

        public int CompareTo(Object obj)
        {
            var rp = obj as RegressionPoint;
            return (rp == null ? 1 : X.CompareTo(rp.X));
        }

        public bool Equals(RegressionPoint other)
        {
            return (Math.Abs(X - other.X) < double.Epsilon);
        }

        public void Set(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Set(double x, double massError, double netError)
        {
            X = x;
            MassError = massError;
            NetError = netError;
        }

        public override string ToString()
        {
            return String.Format("{0,12:F4}, {1,9:F4}, MassError {2,8:F3}, NetError {3,8:F3}", X, Y, MassError, NetError);
        }
    }
}
