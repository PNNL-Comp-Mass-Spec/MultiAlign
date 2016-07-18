using System;

namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions
{
    class AsymmetricGaussian: BasisFunctionBase
    {
        /*
         denom = math.sqrt(2 * math.log(2)) * (hp + hm)
         a = 2 * hp * hm / denom
         b = (hp - hm) / denom
         sigma = a + b * (x - x0)
         return A * math.exp(-0.5 * ((x - x0) / sigma)**2)
         */
        public override void FunctionDelegate(double[] c, double[] xv, ref double functionResult, object obj)
        {
            var x     = xv[0];
            var x0    = c[0];
            var A     = c[1];
            var hm    = c[2];
            var hp    = c[3];
            var denom = Math.Sqrt(2 * Math.Log(2))  * (hp + hm);
            var a     = 2 * hp * hm / denom;
            var b     = (hp - hm) / denom;

            var sigma    = a + b * (x - x0);
            functionResult = A * Math.Exp(-.5 * Math.Pow(((x - x0) / sigma), 2));
        }
    }
}
