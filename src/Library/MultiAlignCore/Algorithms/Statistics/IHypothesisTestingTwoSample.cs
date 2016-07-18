using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Statistics
{
    public interface IHypothesisTestingTwoSample
    {
        HypothesisTestingData Test(List<double> dist1, List<double> dist2);
    }
}
