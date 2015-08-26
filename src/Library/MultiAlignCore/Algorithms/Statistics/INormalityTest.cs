using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Statistics
{
    public interface INormalityTest
    {
        HypothesisTestingData Test(List<double> dist1);
    }
}
