namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions
{
    /// <summary>
    /// Basis function for the LM Algorithm using Quadratic Peak Shapes
    /// </summary>
    public class Quadratic : BasisFunctionBase
    {
        /// <summary>
        /// Evalutates the second order chebyshev polynomials
        /// </summary>
        /// <param name="c">Set of coefficients</param>
        /// <param name="x">Input variables</param>
        /// <param name="functionResult">Returned sum value of your function</param>
        /// <param name="obj">?</param>
        public override void FunctionDelegate(double[] c, double[] x, ref double functionResult, object obj)
        {
            //Y=ax^2 + bx + c
            functionResult = 0;
            functionResult = (c[0] * x[0] * x[0]) + (c[1] * x[0]) + c[2];
        }
    }
}
