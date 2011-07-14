using System;
using System.Collections.Generic;

using PNNLProteomics.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;
using PNNLProteomics.SMART;
using PNNLProteomics.Data.MassTags;

using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms.FeatureMatcher;

namespace PNNLProteomics.Algorithms.PeakMatching
{

    public class PeakMatcher<T>  : IPeakMatcher<T> where T: UMCClusterLight
    {
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        public List<FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T>     clusters,
                                                                              MassTagDatabase           massTagDatabase,
                                                                              clsPeakMatchingOptions    options,
                                                                              double                    daltonShift)
        {

            FeatureMatcherLight<T, MassTagLight> peakMatcher         = new FeatureMatcherLight<T,MassTagLight>();

            FeatureMatcherLightOptions matchingOptions  = new FeatureMatcherLightOptions();
            matchingOptions.Tolerances.Mass             = options.MassTolerance;
            matchingOptions.Tolerances.RetentionTime    = options.NETTolerance;
            matchingOptions.Tolerances.DriftTime        = options.DriftTimeTolerance;
            matchingOptions.DaltonShift                 = daltonShift;
            List<FeatureMatchLight<T, MassTagLight>> matches        = peakMatcher.MatchFeatures(clusters,
                                                                                                   massTagDatabase.MassTags,
                                                                                                   matchingOptions);
            return matches;
        }
        /// <summary>
        /// Calculates the SMART Scores if matched to a AMTDB 
        /// for peptide identification.
        /// </summary>
        public classSMARTResults PerformSTAC(   List<T>   clusters,
                                                MassTagDatabase         massTagDatabase,
                                                classSMARTOptions       options)
        {
            classSMARTProcessor processor = new classSMARTProcessor();

            // Load the mass tags
            List<classSMARTMassTag> massTags = new List<classSMARTMassTag>();
            int totalTags = massTagDatabase.MassTags.Count;

            foreach(MassTagLight tag in massTagDatabase.MassTags)
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
            foreach(T cluster in clusters)
            {
                classSMARTUMC feature       = new classSMARTUMC();                
                feature.mdouble_NET         = cluster.NET;
                feature.mdouble_monoMass    = cluster.MassMonoisotopic;
                feature.mint_id             = cluster.ID;
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
        public List<FeatureMatchLight<T, MassTagLight>> ConvertSTACResultsToPeakResults(classSMARTResults smart,
                                                                           MassTagDatabase  massTagDatabase,
                                                                           List<T> clusters)
        {

            List<FeatureMatchLight<T, MassTagLight>> matches = new List<FeatureMatchLight<T, MassTagLight>>();
            Dictionary<int, MassTagLight> tagMap             = new Dictionary<int,MassTagLight>();
            Dictionary<int, T> featureMap                    = new Dictionary<int,T>();

            foreach(T feature in clusters)
            {
                featureMap.Add(feature.ID, feature);
            }
            foreach(MassTagLight tag in massTagDatabase.MassTags)
            {
                tagMap.Add(tag.ID, tag);
            }

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
                            FeatureMatchLight<T, MassTagLight> match = new FeatureMatchLight<T, MassTagLight>();
                            match.Target                             = tagMap[probability.MassTagID];
                            match.Observed                           = feature;
                            matches.Add(match);
                        }
                    }
                }
            }
            
            return matches;
        }        
    }
}
