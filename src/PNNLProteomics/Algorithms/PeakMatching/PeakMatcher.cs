using System;
using System.Collections.Generic;

using PNNLProteomics.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;
using PNNLProteomics.SMART;

namespace PNNLProteomics.Algorithms.PeakMatching
{

    public class PeakMatcher: IPeakMatcher
    {
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        public clsPeakMatchingResults PerformPeakMatching(List<clsCluster> clusters,
                                                          clsMassTagDB massTagDatabase,
                                                          clsPeakMatchingOptions options,
                                                          double daltonShift)
        {
            clsPeakMatchingProcessor peakMatcher = new clsPeakMatchingProcessor();
            peakMatcher.MassTolerance            = options.MassTolerance;
            peakMatcher.NETTolerance             = options.NETTolerance;
            peakMatcher.DriftTimeTolerance       = options.DriftTimeTolerance;

                        
            clsPeakMatchingResults peakMatchingResults = peakMatcher.PerformPeakMatching(clusters,
                                                                                           massTagDatabase,
                                                                                           daltonShift);
            return peakMatchingResults;
        }
        /// <summary>
        /// Calculates the SMART Scores if matched to a AMTDB 
        /// for peptide identification.
        /// </summary>
        public classSMARTResults PerformSTAC(List<clsCluster> clusters,
                                                clsMassTagDB massTagDatabase,
                                                classSMARTOptions options)
        {
            classSMARTProcessor processor = new classSMARTProcessor();

            // Load the mass tags
            List<classSMARTMassTag> massTags = new List<classSMARTMassTag>();
            int totalTags = massTagDatabase.GetMassTagCount();
            for (int i = 0; i < totalTags; i++)
            {
                clsMassTag tag = massTagDatabase.GetMassTagFromIndex(i);

                classSMARTMassTag msFeature = new classSMARTMassTag();
                msFeature.mdouble_monoMass = tag.mdblMonoMass;
                msFeature.mdouble_NET = tag.NetAverage;
                msFeature.mint_ID = tag.mintMassTagId;
                msFeature.mint_count = tag.mintNumObsPassingFilter;

                msFeature.mdouble_probability = tag.HighPeptideProphetProbability;
                massTags.Add(msFeature);
            }

            // Load the clusters
            int totalClusters = clusters.Count;
            List<classSMARTUMC> smartFeatures = new List<classSMARTUMC>();
            for (int i = 0; i < totalClusters; i++)
            {
                clsCluster cluster = clusters[i];
                classSMARTUMC feature = new classSMARTUMC();
                //TODO: BLL Change to aligned feature data
                feature.mdouble_NET = cluster.Net;
                feature.mdouble_monoMass = cluster.Mass;
                feature.mint_id = i;
                smartFeatures.Add(feature);
            }

            classSMARTResults smartResults = processor.ScoreUMCMatches(massTags,
                                                                       smartFeatures,
                                                                       options);

            return smartResults;
        }
        /// <summary>
        /// Converts the SMART Results into peak matching results.
        /// </summary>
        /// <param name="smart">Results computed using SMART.</param>
        /// <returns>Peak matching results.</returns>
        public clsPeakMatchingResults ConvertSTACResultsToPeakResults(classSMARTResults smart,
                                                                       clsMassTagDB massTagDatabase,
                                                                       List<clsCluster> clusters)
        {
            /// 
            /// Create a new results object for holding peak matching data.
            /// 
            clsPeakMatchingResults results = new clsPeakMatchingResults();

            /// 
            /// Then, look through the UMC cluster keys, 
            /// and pull out the clusters and MTID's from each.
            /// 
            int[] smartKeys = smart.GetUMCMatchIndices();
            foreach (int key in smartKeys)
            {
                /// 
                /// Get the list of all the UMC' data that matched to MTID's
                /// 
                List<classSMARTProbabilityResult> umcMatches =
                    smart.GetResultFromUMCIndex(key);
                /// 
                /// Just in case, make sure we have result data here...
                /// 
                if (umcMatches != null)
                {
                    /// 
                    /// Then since we can have multiple MTID's 
                    /// match to UMC's,
                    /// pull out all possible probabilities and 
                    /// add to the result
                    /// matching data object to construct the 
                    /// protein to UMC - MTID relationships                    
                    /// 
                    /// First - we grab the cluster data, since 
                    /// it should be the same for all probability
                    /// matches
                    /// 
                    /// Second - we enumerate all the matches 
                    /// of umc cluster to mtid adding them
                    /// to the results object.
                    ///                     
                    clsCluster cluster = clusters[key];
                    foreach (classSMARTProbabilityResult probability in
                        umcMatches)
                    {
                        clsMassTag tag = massTagDatabase.GetMassTag(probability.MassTagID);
                        results.AddPeakMatchResult(tag, cluster, key);
                    }
                }
            }
            /// 
            /// Finally, now that we have matches, we pull
            /// out all of the protein
            /// information that was retrieved and stored 
            /// in the mass tag database object.            
            /// 
            results.ExtractProteinInformation(massTagDatabase);

            /// 
            /// And last, we finally tie together all of 
            /// the information.
            ///     MTID - Protein - UMC Cluster Index.  
            /// This method will generate the triplet 
            /// structures that are used to key off of each other.
            /// 
            results.UpdatePeakMatchArrays();
            return results;
        }        
    }
}
