namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions
{
    /// <summary>
    /// Basis function for the LM Algorithm using Polynomials
    /// </summary>
    public class Cubic : BasisFunctionBase
    {

        /// <summary>
        /// Evaluates the cubic polynomials
        /// </summary>
        /// <param name="c">Set of coefficients</param>
        /// <param name="x">Input variables</param>
        /// <param name="functionResult">Returned sum value of your function</param>
        /// <param name="obj">?</param>
        public override void FunctionDelegate(double[] c, double[] x, ref double functionResult, object obj)
        {
            //Y=ax^3 + bx^2 + cx +d
            functionResult = 0;
            functionResult = (c[0] * x[0] * x[0] * x[0]) + (c[1] * x[0] * x[0]) + (c[2] * x[0]) + c[3];
        }
    }


}
