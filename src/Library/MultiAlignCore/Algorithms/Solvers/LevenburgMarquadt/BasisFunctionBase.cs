using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt
{
    /// <summary>
    /// interface for strategy design pattern
    /// </summary>
    public abstract class BasisFunctionBase
    {
        public virtual void Scale(List<double> x)
        {

        }

        /// <summary>
        /// Coefficients found via solving.
        /// </summary>
        public double[] Coefficients { get; set; }
        /// <summary>
        /// Evaluates the function.
        /// </summary>
        /// <param name="cofficients"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public virtual double Evaluate(double[] coefficients, double x)
        {
            double result = 0;
            FunctionDelegate(coefficients, new[] {x}, ref result, null);
            return result;
        }
        /// <summary>
        /// Evaluates the function.
        /// </summary>
        /// <param name="coefficients"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public virtual double Evaluate(double[] coefficients, double[] x)
        {
            double result = 0;
            FunctionDelegate(coefficients, x, ref result, null);
            return result;
        }
        /// <summary>
        /// Delegate definition for the LM solver.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="functionResult"></param>
        /// <param name="obj"></param>
        public abstract void FunctionDelegate(double[] c, double[] x, ref double functionResult, object obj);
    }
}
