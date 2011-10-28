using System;
using System.Collections.Generic;

using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;
using PNNLProteomics.SMART;
using MultiAlignCore.Data.MassTags;

using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms.FeatureMatcher;
using PNNLOmics.Utilities;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    /// Perform STAC
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SMART<T> : IStatusProvider, IPeakMatcher<T> where T : UMCClusterLight
    {
        /// <summary>
        /// Fired when new status is available.
        /// </summary>
        public event MessageEventHandler Status;

        /// <summary>
        /// Gets or sets the peak matching options.
        /// </summary>
        public classSMARTOptions Options
        {
            get;
            set;
        }
        public classSMARTResults Results
        {
            get;
            set;
        }
        /// <summary>
        /// Calculates the SMART Scores if matched to a AMTDB 
        /// for peptide identification.
        /// </summary>
        public List<FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T> clusters,
                                                                             MassTagDatabase massTagDatabase)
        {
            List<FeatureMatchLight<T, MassTagLight>> matches = new List<FeatureMatchLight<T, MassTagLight>>();

            OnStatus("Performing first version of STAC.");
            classSMARTProcessor processor = new classSMARTProcessor();

            // Load the mass tags
            List<classSMARTMassTag> massTags    = new List<classSMARTMassTag>();
            int totalTags                       = massTagDatabase.MassTags.Count;


            OnStatus("Converting data structures.");
            foreach (MassTagLight tag in massTagDatabase.MassTags)
            {
                classSMARTMassTag msFeature     = new classSMARTMassTag();
                msFeature.mdouble_monoMass      = tag.MassMonoisotopic;
                msFeature.mdouble_NET           = tag.NETAverage;
                msFeature.mint_ID               = tag.ID;
                msFeature.mint_count            = tag.ObservationCount;
                msFeature.mdouble_probability   = tag.PriorProbability;
                massTags.Add(msFeature);
            }

            // Load the clusters
            int totalClusters = clusters.Count;
            List<classSMARTUMC> smartFeatures = new List<classSMARTUMC>();
            foreach (T cluster in clusters)
            {
                classSMARTUMC feature       = new classSMARTUMC();
                feature.mdouble_NET         = cluster.NET;
                feature.mdouble_monoMass    = cluster.MassMonoisotopic;
                feature.mint_id             = cluster.ID;
                smartFeatures.Add(feature);
            }

            OnStatus("Performing first version of STAC.");
            classSMARTResults smartResults = processor.ScoreUMCMatches(massTags,
                                                                       smartFeatures,
                                                                       Options);

            return ConvertSTACResultsToPeakResults(smartResults, massTagDatabase, clusters);
        }
        /// <summary>
        /// Converts the SMART Results into peak matching results.
        /// </summary>
        /// <param name="smart">Results computed using SMART.</param>
        /// <returns>Peak matching results.</returns>
        private List<FeatureMatchLight<T, MassTagLight>> ConvertSTACResultsToPeakResults(classSMARTResults smart,
                                                                                           MassTagDatabase massTagDatabase,
                                                                                           List<T> clusters)
        {
            OnStatus("Converting STAC result tables.");
            List<FeatureMatchLight<T, MassTagLight>> matches    = new List<FeatureMatchLight<T, MassTagLight>>();
            Dictionary<int, MassTagLight> tagMap                = new Dictionary<int, MassTagLight>();
            Dictionary<int, T> featureMap                       = new Dictionary<int, T>();

            foreach (T feature in clusters)
            {
                featureMap.Add(feature.ID, feature);
            }
            foreach (MassTagLight tag in massTagDatabase.MassTags)
            {
                tagMap.Add(tag.ID, tag);
            }

            OnStatus("Mapping STAC data structres.");
            // Then, look through the UMC cluster keys, 
            // and pull out the clusters and MTID's from each.             
            int[] smartKeys = smart.GetUMCMatchIndices();
            foreach (int key in smartKeys)
            {
                // Get the list of all the UMC' data that matched to MTID's
                List<classSMARTProbabilityResult> umcMatches = smart.GetResultFromUMCIndex(key);
                // Just in case, make sure we have result data here...
                if (umcMatches != null)
                {
                    // Then since we can have multiple MTID's 
                    // match to UMC's,
                    // pull out all possible probabilities and 
                    // add to the result
                    // matching data object to construct the 
                    // protein to UMC - MTID relationships                    
                    // First - we grab the cluster data, since 
                    // it should be the same for all probability
                    // matches                    
                    // Second - we enumerate all the matches 
                    // of umc cluster to mtid adding them
                    // to the results object.
                    T feature = featureMap[key];

                    foreach (classSMARTProbabilityResult probability in umcMatches)
                    {
                        bool containsTag = tagMap.ContainsKey(probability.MassTagID);
                        if (containsTag)
                        {
                            FeatureMatchLight<T, MassTagLight> match    = new FeatureMatchLight<T, MassTagLight>();
                            match.Target                                = tagMap[probability.MassTagID];
                            match.Observed                              = feature;
                            matches.Add(match);
                        }
                    }
                }
            }

            return matches;
        }
        /// <summary>
        /// Handles calling the status event for observers.
        /// </summary>
        /// <param name="message"></param>
        private void OnStatus(string message)
        {
            if (Status != null)
            {
                Status(this, new MessageEventArgs(message));
            }
        }        
    }
}
