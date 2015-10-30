using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using MultiAlignCore.Data.Features;

    public class IdComparisonScorer : IFeatureComparisonScorer
    {
        public double ScoreComparison(FeatureLight feature1, FeatureLight feature2)
        {
            var proteinHash = new HashSet<string>();

            double score = 0.0;
            for (int i = 1; i < feature1.MSnSpectra.Count; i++)
            {
                var leftMsnSpectrum = feature1.MSnSpectra[i];
                for (int j = 0; j < feature2.MSnSpectra.Count; j++)
                {
                    var rightMsnSpectrum = feature2.MSnSpectra[j];
                    var rightPeptides = rightMsnSpectrum.Peptides;

                    foreach (var id in leftMsnSpectrum.Peptides)
                    {
                        foreach (var protein in id.ProteinList)
                        {
                            if (!proteinHash.Contains(protein.Name))
                            {
                                score += rightPeptides.Any(pep => pep.ProteinList.Any(prot => protein.Name == prot.Name)) ? 1.0 : -1.0;
                                proteinHash.Add(protein.Name);
                            }
                        }
                    }
                }
            }

            return score;
        }
    }
}
