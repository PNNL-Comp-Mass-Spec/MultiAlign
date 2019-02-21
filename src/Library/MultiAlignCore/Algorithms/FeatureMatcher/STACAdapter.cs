#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    ///     Adapts the STAC computation code from PNNL OMICS into the MultiAlign workflow.
    /// </summary>
    public class STACAdapter<T> : IProgressNotifer, IPeakMatcher<T>
        where T : UMCClusterLight
    {
        public event EventHandler<ProgressNotifierArgs> Progress;


        public STACAdapter()
        {
            Options = new FeatureMatcherParameters();
        }

        /// <summary>
        ///     Gets the peak matching object.
        /// </summary>
        public FeatureMatcher<UMCClusterLight, MassTagLight> Matcher { get; private set; }


        /// <summary>
        ///     Gets or sets the feature matching parameters.
        /// </summary>
        public FeatureMatcherParameters Options { get; set; }

        /// <summary>
        ///     Performs STAC against the mass tag database.
        /// </summary>
        public List<MultiAlignCore.Data.MassTags.FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T> clusters, MassTagDatabase database)
        {
            var clusterMap = new Dictionary<int, T>();
            var tagMap = new Dictionary<int, Dictionary<int, MassTagLight>>();

            var massTags = new List<MassTagLight>();
            var i = 0;
            foreach (var tag in database.MassTags)
            {
                var mt = new MassTagLight
                {
                    Abundance = Convert.ToInt32(tag.Abundance),
                    ChargeState = tag.ChargeState,
                    CleavageState = tag.CleavageState,
                    ConformationId = tag.ConformationId,
                    ConformationObservationCount = tag.ConformationObservationCount,
                    DiscriminantMax = tag.DiscriminantMax,
                    DriftTime = Convert.ToSingle(tag.DriftTime),
                    DriftTimePredicted = tag.DriftTimePredicted,
                    Id = tag.Id,
                    MassMonoisotopic = tag.MassMonoisotopic,
                    ModificationCount = tag.ModificationCount,
                    Modifications = tag.Modifications,
                    Molecule = tag.Molecule,
                    MsgfSpecProbMax = tag.MsgfSpecProbMax,
                    Net = tag.NetAverage,
                    NetAverage = tag.NetAverage,
                    NetPredicted = tag.NetPredicted,
                    NetStandardDeviation = tag.NetStandardDeviation,
                    ObservationCount = tag.ObservationCount,
                    PeptideSequence = tag.PeptideSequence,
                    PeptideSequenceEx = tag.PeptideSequenceEx,
                    PriorProbability = tag.PriorProbability,
                    QualityScore = tag.QualityScore,
                    XCorr = tag.XCorr
                };
                mt.Index = i++;
                massTags.Add(mt);
                if (!tagMap.ContainsKey(tag.Id))
                {
                    tagMap.Add(tag.Id, new Dictionary<int, MassTagLight>());
                }
                tagMap[tag.Id].Add(tag.ConformationId, tag);
            }

            // convert data needed by the algorithm.
            var features = new List<UMCClusterLight>();
            foreach (var cluster in clusters)
            {
                var feature = new UMCClusterLight
                {
                    Id = cluster.Id,
                    MassMonoisotopicAligned = cluster.MassMonoisotopic,
                    MassMonoisotopic = cluster.MassMonoisotopic
                };
                feature.Net = cluster.Net;
                feature.NetAligned = cluster.Net;
                feature.ChargeState = cluster.ChargeState;
                feature.DriftTime = Convert.ToSingle(cluster.DriftTime);
                feature.DriftTimeAligned = Convert.ToDouble(cluster.DriftTime);
                features.Add(feature);

                clusterMap.Add(cluster.Id, cluster);
            }

            // create a stac manager and run.
            var matcher = new FeatureMatcher<UMCClusterLight, MassTagLight>(features, massTags, Options);

            matcher.MessageEvent += StatusHandler;
            matcher.ProcessingCompleteEvent += StatusHandler;
            matcher.MatchFeatures();
            Matcher = matcher;


            Matcher.PopulateStacfdrTable(matcher.MatchList);

            var matches = new List<MultiAlignCore.Data.MassTags.FeatureMatchLight<T, MassTagLight>>();
            foreach (var match in matcher.MatchList)
            {
                var matched = new MultiAlignCore.Data.MassTags.FeatureMatchLight<T, MassTagLight>
                {
                    Observed = clusterMap[match.ObservedFeature.Id],
                    Target = tagMap[match.TargetFeature.Id][match.TargetFeature.ConformationId],
                    Confidence = match.STACScore,
                    Uniqueness = match.STACSpecificity
                };
                matches.Add(matched);
            }


            matcher.MessageEvent -= StatusHandler;
            matcher.ProcessingCompleteEvent -= StatusHandler;

            return matches;
        }

        /// <summary>
        ///     Handles status messages from the attributes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusHandler(object sender, ProgressNotifierArgs e)
        {
            Progress?.Invoke(this, e);
        }
    }
}