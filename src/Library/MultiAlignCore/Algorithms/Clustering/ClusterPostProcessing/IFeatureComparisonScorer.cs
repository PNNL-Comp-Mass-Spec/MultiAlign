using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using MultiAlignCore.Data.Features;

    public interface IFeatureComparisonScorer
    {
        double ScoreComparison(FeatureLight feature1, FeatureLight feature2);
    }
}
