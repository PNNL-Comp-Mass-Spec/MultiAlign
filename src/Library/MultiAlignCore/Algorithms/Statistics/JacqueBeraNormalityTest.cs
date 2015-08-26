using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Statistics
{
    public class JacqueBeraNormalityTest: INormalityTest
    {
        /// <summary>
        /// Computes a p-value for a given distribution.
        /// </summary>
        /// <param name="distribution">Values used to compute the p-value.</param>
        /// <returns>Level of significance (p-value)</returns>
        public HypothesisTestingData Test(List<double> dist1)
        {
            var x = new double[dist1.Count];
            var n = dist1.Count;            
            dist1.CopyTo(x);

            var pValue = double.MaxValue;

            alglib.jarqueberatest(x, n, out pValue);
            var t = new HypothesisTestingData(pValue);

            return t;
        }    
    }


    public class KSNormalityTest : INormalityTest
    {
        /// <summary>
        /// Computes a p-value for a given distribution.
        /// </summary>
        /// <param name="distribution">Values used to compute the p-value.</param>
        /// <returns>Level of significance (p-value)</returns>
        public HypothesisTestingData Test(List<double> dist1)
        {
            var x = new double[dist1.Count];
            var n = dist1.Count;
            dist1.CopyTo(x);

            var pValue = double.MaxValue;

            //alglib.(x, n, out pValue);
            var t = new HypothesisTestingData(pValue);

            return t;
        }
    }
}
