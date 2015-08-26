using System;
using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions
{
    /// <summary>
    /// Basis function for the LM Algorithm using First Order Chebyshev
    /// </summary>
    public class Chebyshev : BasisFunctionBase
    {

        private double m_min;
        private double m_max;

        public override void Scale(List<double> x)
        {
            m_min = double.MaxValue;
            m_max = double.MinValue;
            
            foreach (var d in x)
            {
                m_max = Math.Max(m_max, d);
                m_min = Math.Min(m_min, d);
            }
        }

        /// <summary>
        /// Evalutates the second order chebyshev polynomials
        /// </summary>
        /// <param name="c">Set of coefficients</param>
        /// <param name="x">Input variables</param>
        /// <param name="functionResult">Returned sum value of your function</param>
        /// <param name="obj">?</param>
        public override void FunctionDelegate(double[] c, double[] x, ref double functionResult, object obj)
        {

            var tx      = (x[0] - m_min) / (m_max - m_min);
            tx             -= 1.0; // This scales the value between -1 and 1
            
            var t0   = c[0];
            var t1   = c[1] * tx;
            var sum  = t0 + t1;
            var prev = t1;            
            double n    = 2;
            var acos = Math.Acos(tx);

            for (var i = 2; i < c.Length; i++)
            {
                var value = (2 * Math.Cos(n * acos) * c[i]) - Math.Cos( (n - 1) * acos);
                prev = value;
                sum += value;
                n++;
            }
            functionResult = sum;
        }
    }
}
