using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.SequenceData;

    public class IdComparisonScorer : IFeatureComparisonScorer
    {
        private readonly IdentificationProviderCache identificationProviderCache;

        /// <summary>
        ///
        /// </summary>
        /// <param name="identificationProviderCache">The identification provider.</param>
        public IdComparisonScorer(IdentificationProviderCache identificationProviderCache)
        {
            this.identificationProviderCache = identificationProviderCache;
        }

        /// <summary>
        /// Score two features against each other by comparing their identifications.
        /// Each matching identification: +1
        /// Each non-matching identification: -1
        /// </summary>
        /// <param name="feature1">The first feature.</param>
        /// <param name="feature2">The second feature.</param>
        /// <returns>The score of the two features.</returns>
        public double ScoreComparison(FeatureLight feature1, FeatureLight feature2)
        {
            var leftProteins = this.GetIdentifications(feature1);
            var rightProteins = this.GetIdentifications(feature2);

            var intersect = leftProteins.Intersect(rightProteins).ToList();
            var leftOnly = leftProteins.Except(intersect);
            var rightOnly = rightProteins.Except(intersect);

            double score = 0.0;
            score += intersect.Count;
            score -= leftOnly.Count();
            score -= rightOnly.Count();

            return score;
        }

        /// <summary>
        /// Get the MS/MS identifications for the given feature.
        /// </summary>
        /// <param name="feature">The feature to get MS/MS identifications for.</param>
        /// <returns>The list of identifications.</returns>
        private List<Peptide> GetIdentifications(FeatureLight feature)
        {
            var peptides = new List<Peptide>();
            var provider = this.identificationProviderCache.GetProvider(feature.GroupId);
            var ids = provider.GetAllIdentifications();

            foreach (var msnSpectrum in feature.MSnSpectra)
            {
                if (ids.ContainsKey(msnSpectrum.Scan))
                {
                    peptides.AddRange(ids[msnSpectrum.Scan]);
                }
            }

            return peptides;
        }
    }
}
