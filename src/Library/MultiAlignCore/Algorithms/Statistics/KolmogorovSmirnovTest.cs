using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class KolmogorovSmirnovTest : IHypothesisTestingTwoSample
    {

        #region IHypothesisTestingTwoSample Members

        public HypothesisTestingData Test(List<double> dist1, List<double> dist2)
        {
            var one = new double[dist1.Count];
            dist1.CopyTo(one);

            var two = new double[dist2.Count];
            dist1.CopyTo(two);

            var pValue = double.MaxValue;
            // TwoSampleKolmogorovSmirnovTest tester = new TwoSampleKolmogorovSmirnovTest(one, two, TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal);
            // pValue = testc.PValue;

            var data = new HypothesisTestingData(pValue, pValue, pValue);
            return data;
        }

        #endregion
    }
}