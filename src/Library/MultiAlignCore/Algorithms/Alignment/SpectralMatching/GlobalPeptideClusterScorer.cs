using System.Collections.Generic;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Extensions;

namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{
    /// <summary>
    /// Analyzes a set of clusters, and counts the number of times a peptide shows up in multiple or single clusters.
    /// </summary>
    public class GlobalPeptideClusterScorer
    {

        /// <summary>
        /// Extracts the peptides from the given features
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        private IEnumerable<Peptide> ExtractPeptides(IEnumerable<UMCLight> features)
        {
            var peptides = new List<Peptide>();
            foreach (var feature in features)
            {
                foreach (var msFeature in feature.MsFeatures)
                {
                    foreach (var spectrum in msFeature.MSnSpectra)
                    {
                        peptides.AddRange(spectrum.Peptides);
                    }
                }
            }
            return peptides;
        }
        /// <summary>
        /// Calculates the cluster score metrics.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public GlobalClusterPeptideStatistics Score(IEnumerable<UMCClusterLight> clusters)
        {
            var peptides = new Dictionary<string, List<Peptide>>();
            foreach (var cluster in clusters)
            {
                var clusterPeptides = ExtractPeptides(cluster.Features);
                foreach (var peptide in clusterPeptides)
                {
                    if (!peptides.ContainsKey(peptide.Sequence))
                    {
                        peptides.Add(peptide.Sequence, new List<Peptide>());
                    }
                    peptides[peptide.Sequence].Add(peptide);
                }
            }

            // analyze now...
            var matches = new GlobalClusterPeptideStatistics();

            foreach (var sequence in peptides.Keys)
            {
                var map = new Dictionary<int, UMCClusterLight>();
                foreach (var peptide in peptides[sequence])
                {
                    var parent = peptide.GetParentUmc();
                    if (parent?.UmcCluster != null)
                    {
                        var id = parent.UmcCluster.Id;
                        if (!map.ContainsKey(id))
                        {
                            map.Add(id, parent.UmcCluster);
                        }
                    }
                }

               if (map.Count > 1)
               {
                  // Determine if we need to fix the cluster assignments.



                  matches.DifferentCluster++;
               }
               else
                  matches.SameCluster++;

               matches.Maps.Add(sequence, map);
            }
            return matches;
        }
    }
}
