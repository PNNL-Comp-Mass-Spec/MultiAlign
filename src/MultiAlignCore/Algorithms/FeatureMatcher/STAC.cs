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
using PNNLOmics.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Algorithms.PeakMatching;   

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    /// Adapts the STAC computation code from PNNL OMICS into the MultiAlign workflow.
    /// </summary>
    public class STACAdapter<T>: IPeakMatcher<T> where T: UMCClusterLight
    {
        /// <summary>
        /// Gets the peak matching object.
        /// </summary>
        public FeatureMatcher<UMCCluster, MassTag> Matcher
        {
            get;
            private set;
        }
        /// <summary>
        /// Fired when new status messages are available.
        /// </summary>
        public event PNNLOmics.Utilities.MessageEventHandler Status;

        /// <summary>
        /// Gets or sets the feature matching parameters.
        /// </summary>
        public FeatureMatcherParameters Options
        {
            get;
            set;
        }

        /// <summary>
        /// Performs STAC against the mass tag database.
        /// </summary>
        /// <param name="umcs"></param>
        /// <param name="massTags"></param>
        public List<FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T> clusters, MassTagDatabase database)
        {
            List<FeatureMatchLight<T, MassTagLight>> matches    = new List<FeatureMatchLight<T, MassTagLight>>();
            Dictionary<int, T> clusterMap                       = new Dictionary<int,T>();
            Dictionary<int, MassTagLight> tagMap                = new Dictionary<int,MassTagLight>();

            List<MassTag> massTags = new List<MassTag>();
            foreach(MassTagLight tag in database.MassTags)
            {
                MassTag mt = new MassTag();
                mt.Abundance                     = Convert.ToInt32(tag.Abundance);
                mt.ChargeState                   = tag.ChargeState;
                mt.CleavageState                 = tag.CleavageState;
                mt.ConformationID                = tag.ConformationID;
                mt.ConformationObservationCount  = tag.ConformationObservationCount;
                mt.DiscriminantMax               = tag.DiscriminantMax;
                mt.DriftTime                     = Convert.ToSingle(tag.DriftTime);
                mt.DriftTimePredicted            = tag.DriftTimePredicted;                
                mt.ID                            = tag.ID;
                mt.MassMonoisotopic              = tag.MassMonoisotopic;
                mt.ModificationCount             = tag.ModificationCount;
                mt.Modifications                 = tag.Modifications;
                mt.Molecule                      = tag.Molecule;
                mt.MSGFSpecProbMax               = tag.MSGFSpecProbMax;
                mt.NET                           = tag.NET;
                mt.NETAverage                    = tag.NETAverage;
                mt.NETPredicted                  = tag.NETPredicted;
                mt.NETStandardDeviation          = tag.NETStandardDeviation;
                mt.ObservationCount              = tag.ObservationCount;
                mt.PeptideSequence               = tag.PeptideSequence;
                mt.PeptideSequenceEx             = tag.PeptideSequenceEx;
                mt.PriorProbability              = tag.PriorProbability;
                mt.QualityScore                  = tag.QualityScore;
                mt.XCorr                         = tag.XCorr;

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

            // create a stac manager and run.
            FeatureMatcher<UMCCluster, MassTag> matcher = new FeatureMatcher<UMCCluster, MassTag>(features, massTags, Options);

            matcher.MessageEvent            += new PNNLOmics.Utilities.MessageEventHandler(StatusHandler);
            matcher.ProcessingCompleteEvent += new PNNLOmics.Utilities.MessageEventHandler(StatusHandler);
            matcher.ErrorEvent              += new PNNLOmics.Utilities.MessageEventHandler(StatusHandler);
            matcher.MatchFeatures();
            Matcher = matcher;

            foreach (FeatureMatch<UMCCluster, MassTag> match in matcher.MatchList)
            {
                FeatureMatchLight<UMCClusterLight, MassTagLight> matched = new FeatureMatchLight<UMCClusterLight, MassTagLight>();
                matched.Observed                                         = clusterMap[match.ObservedFeature.ID];
                matched.Target                                           = tagMap[match.TargetFeature.ID];
            }


            matcher.MessageEvent            -= StatusHandler;
            matcher.ProcessingCompleteEvent -= StatusHandler;
            matcher.ErrorEvent              -= StatusHandler;

            return matches;
        }

        /// <summary>
        /// Handles status messages from the attributes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StatusHandler(object sender, PNNLOmics.Utilities.MessageEventArgs e)
        {
            if (Status != null)
            {
                Status(this, e);
            }
        }
    }
}
