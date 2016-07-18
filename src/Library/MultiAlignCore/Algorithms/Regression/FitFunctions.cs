namespace MultiAlignCore.Algorithms.Regression
{
    public static class FitFunctions
    {
        public static double Sin(double x)
        {
            return System.Math.Sin(x);
        }
        public static double Linear(double x)
        {
            return x;
        }
        public static double Quadratic(double x)
        {
            return x * x * x;
        }
        public static double Cubic(double x)
        {
            return x * x * x;
        }
        public static double Tricube(double x)
        {
            var tmp = 1 - x * x * x;
            return tmp * tmp * tmp;
        }
    }
}
