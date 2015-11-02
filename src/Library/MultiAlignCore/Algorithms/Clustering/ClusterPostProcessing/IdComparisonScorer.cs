using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using MultiAlignCore.Data.Features;

    public class IdComparisonScorer : IFeatureComparisonScorer
    {
        public double ScoreComparison(FeatureLight feature1, FeatureLight feature2)
        {
            var leftProteins = feature1.MSnSpectra.SelectMany(msn => msn.Peptides.SelectMany(pep => pep.ProteinList)).ToList();
            var rightProteins = feature2.MSnSpectra.SelectMany(msn => msn.Peptides.SelectMany(pep => pep.ProteinList)).ToList();

            var intersect = leftProteins.Intersect(rightProteins).ToList();
            var leftOnly = leftProteins.Except(intersect);
            var rightOnly = rightProteins.Except(intersect);

            double score = 0.0;
            score += intersect.Count;
            score -= leftOnly.Count();
            score -= rightOnly.Count();

            return score;
        }
    }
}
