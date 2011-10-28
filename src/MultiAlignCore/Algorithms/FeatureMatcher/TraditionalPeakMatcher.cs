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
    /// Performs traditional peak matching.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TraditionalPeakMatcher<T>  : IStatusProvider, IPeakMatcher<T> where T: UMCClusterLight
    {
        /// <summary>
        /// Fired when new status is available.
        /// </summary>
        public event MessageEventHandler Status;

        /// <summary>
        /// Gets or sets the peak matching options.
        /// </summary>
        public clsPeakMatchingOptions Options
        {
            get;
            set;
        }
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        public List<FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T>             clusters,
                                                                            MassTagDatabase     massTagDatabase)
        {
            if (Options == null)
            {
                throw new Exception("The peak matching options were not set.");
            }

            OnStatus("Performing traditional peak matching.");
            PeakMatcher<UMCCluster, MassTag> peakMatcher            = new PeakMatcher<UMCCluster, MassTag>();
            PeakMatcherOptions matchingOptions                      = new PeakMatcherOptions();
            matchingOptions.Tolerances.Mass                         = Options.MassTolerance;
            matchingOptions.Tolerances.RetentionTime                = Options.NETTolerance;
            matchingOptions.Tolerances.DriftTime                    = Options.DriftTimeTolerance;
            matchingOptions.DaltonShift                             = Options.DaltonShift;


            List<FeatureMatchLight<T, MassTagLight>> matches    = new List<FeatureMatchLight<T, MassTagLight>>();
            Dictionary<int, T> clusterMap                       = new Dictionary<int, T>();
            Dictionary<int, MassTagLight> tagMap                = new Dictionary<int, MassTagLight>();

            List<MassTag> massTags = new List<MassTag>();
            foreach (MassTagLight tag in massTagDatabase.MassTags)
            {
                MassTag mt = new MassTag();
                mt.Abundance = Convert.ToInt32(tag.Abundance);
                mt.ChargeState = tag.ChargeState;
                mt.CleavageState = tag.CleavageState;
                mt.ConformationID = tag.ConformationID;
                mt.ConformationObservationCount = tag.ConformationObservationCount;
                mt.DiscriminantMax = tag.DiscriminantMax;
                mt.DriftTime = Convert.ToSingle(tag.DriftTime);
                mt.DriftTimePredicted = tag.DriftTimePredicted;
                mt.ID = tag.ID;
                mt.MassMonoisotopic = tag.MassMonoisotopic;
                mt.ModificationCount = tag.ModificationCount;
                mt.Modifications = tag.Modifications;
                mt.Molecule = tag.Molecule;
                mt.MSGFSpecProbMax = tag.MSGFSpecProbMax;
                mt.NET = tag.NET;
                mt.NETAverage = tag.NETAverage;
                mt.NETPredicted = tag.NETPredicted;
                mt.NETStandardDeviation = tag.NETStandardDeviation;
                mt.ObservationCount = tag.ObservationCount;
                mt.PeptideSequence = tag.PeptideSequence;
                mt.PeptideSequenceEx = tag.PeptideSequenceEx;
                mt.PriorProbability = tag.PriorProbability;
                mt.QualityScore = tag.QualityScore;
                mt.XCorr = tag.XCorr;

                massTags.Add(mt);
                tagMap.Add(tag.ID, tag);
            }

            // convert data needed by the algorithm.
            List<UMCCluster> features = new List<UMCCluster>();
            foreach (T cluster in clusters)
            {
                UMCCluster feature = new UMCCluster();
                feature.ID = cluster.ID;
                feature.MassMonoisotopicAligned = cluster.MassMonoisotopic;
                feature.MassMonoisotopic = cluster.MassMonoisotopic;
                feature.NET = cluster.NET;
                feature.NETAligned = cluster.NET;
                feature.ChargeState = cluster.ChargeState;
                feature.DriftTime = Convert.ToSingle(cluster.DriftTime);
                feature.DriftTimeAligned = Convert.ToDouble(cluster.DriftTime);
                features.Add(feature);

                clusterMap.Add(cluster.ID, cluster);
            }

            List<FeatureMatch<UMCCluster, MassTag>> heavyMatches  = peakMatcher.MatchFeatures(   features,
                                                                                        massTags,
                                                                                        matchingOptions);

            foreach (FeatureMatch<UMCCluster, MassTag> match in heavyMatches)
            {
                FeatureMatchLight<UMCClusterLight, MassTagLight> matched = new FeatureMatchLight<UMCClusterLight, MassTagLight>();
                matched.Observed = clusterMap[match.ObservedFeature.ID];
                matched.Target = tagMap[match.TargetFeature.ID];
            }

            return matches;
        }       
        /// <summary>
        /// Handles propagating status messages to the observers.
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
