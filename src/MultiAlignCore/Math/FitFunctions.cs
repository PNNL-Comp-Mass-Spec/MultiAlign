using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.MathUtilities
{
    public enum FitFunctionTypes
    {
        Sin,
        Cubic,
        Quadratic,
        Linear,
        TriCubic
    }
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
            double tmp = 1 - x * x * x;
            return tmp * tmp * tmp;
        }
    }
    public class FitFunctionFactory
    {
        public static Func<double, double> Create(FitFunctionTypes type)
        {
            Func<double, double> func = null;
            switch (type)
            {
                case FitFunctionTypes.Sin:
                    func = FitFunctions.Sin;
                    break;
                case FitFunctionTypes.Linear:
                    func = FitFunctions.Linear;
                    break;
                case FitFunctionTypes.Quadratic:
                    func = FitFunctions.Quadratic;
                    break;
                case FitFunctionTypes.Cubic:
                    func = FitFunctions.Cubic;
                    break;
                case FitFunctionTypes.TriCubic:
                    func = FitFunctions.Tricube;
                    break;
                default:
                    break;
            }
            return func;
        }
    }
}
