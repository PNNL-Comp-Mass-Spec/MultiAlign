#region

using System;
using System.Collections.Generic;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.RawData;
using NHibernate.Linq;

#endregion

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    ///     Finds UMC features based on m/z and uses a tree approach
    /// </summary>
    public class UmcTreeFeatureFinder : IFeatureFinder
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        public UmcTreeFeatureFinder()
        {
            FilteringOptions = new LcmsFeatureFilteringOptions();
        }

        private void OnStatus(string message)
        {
            Progress?.Invoke(this, new ProgressNotifierArgs(message));
        }

        public LcmsFeatureFilteringOptions FilteringOptions { get; set; }

        /// <summary>
        ///     Finds features
        /// </summary>
        /// <returns></returns>
        public List<UMCLight> FindFeatures(List<MSFeatureLight> msFeatures,
            LcmsFeatureFindingOptions options, IScanSummaryProvider provider,
            IProgress<ProgressData> progress = null)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            var tolerances = new FeatureTolerances
            {
                Mass = options.InstrumentTolerances.Mass,
                Net = options.MaximumNetRange
            };

            var clusterer = new MsToLcmsFeatures(provider, options);

            // MultiAlignCore.Algorithms.FeatureClustering.MsFeatureTreeClusterer
            //var clusterer = new MsFeatureTreeClusterer<MSFeatureLight, UMCLight>
            //{
            //    Tolerances =
            //        new FeatureTolerances
            //        {
            //            Mass = options.InstrumentTolerances.Mass,
            //            Net = options.MaximumNetRange
            //        },
            //    ScanTolerance = options.MaximumScanRange,
            //    SpectraProvider = (InformedProteomicsReader) provider
            //    //TODO: Make sure we have a mass range for XIC's too....
            //};

            //clusterer.SpectraProvider = (InformedProteomicsReader) provider;

            //OnStatus("Starting cluster definition");
            //clusterer.Progress += (sender, args) => OnStatus(args.Message);

            var features = clusterer.Convert(msFeatures, progress);

            var minScan = int.MaxValue;
            var maxScan = int.MinValue;
            foreach (var feature in msFeatures)
            {
                minScan = Math.Min(feature.Scan, minScan);
                maxScan = Math.Max(feature.Scan, maxScan);
            }



            var minScanTime = provider.GetScanSummary(minScan).Time;
            var maxScanTime = provider.GetScanSummary(maxScan).Time;
            var id = 0;
            var newFeatures = new List<UMCLight>();
            foreach (var feature in features)
            {
                if (feature.MsFeatures.Count < 1)
                    continue;
                feature.Net = (provider.GetScanSummary(feature.Scan).Time - minScanTime) /
                              (maxScanTime - minScanTime);
                feature.CalculateStatistics();
                feature.Id = id++;
                newFeatures.Add(feature);
                //Sets the width of the feature to be the width of the peak, not the width of the tails
                var maxAbundance = double.MinValue;
                var maxAbundanceIndex = 0;
                for (var msFeatureIndex = 0; msFeatureIndex < feature.MsFeatures.Count - 1; msFeatureIndex++)
                {
                    var msFeature = feature.MsFeatures[msFeatureIndex];
                    if (msFeature.Abundance > maxAbundance)
                    {
                        maxAbundance = msFeature.Abundance;
                        maxAbundanceIndex = msFeatureIndex;
                    }
                }
                for (var msFeatureIndex = maxAbundanceIndex; msFeatureIndex > 0; msFeatureIndex--)
                {
                    if (feature.MsFeatures[msFeatureIndex].Abundance / maxAbundance <= 0.05)
                    {
                        feature.ScanStart = feature.MsFeatures[msFeatureIndex].Scan;
                        break;
                    }
                }
                for (var msFeatureIndex = maxAbundanceIndex; msFeatureIndex < feature.MsFeatures.Count - 1; msFeatureIndex++)
                {
                    if (feature.MsFeatures[msFeatureIndex].Abundance / maxAbundance <= 0.05)
                    {
                        feature.ScanEnd = feature.MsFeatures[msFeatureIndex].Scan;
                        break;
                    }
                }
            }
            return features;
        }

        /// <summary>
        ///     Gets or sets the maximum NET values any two XIC's (within the same monoisotopic mass window) can be before they are
        ///     not considered to be from the same feature.
        /// </summary>
        public double MaximumNet { get; set; }

        /// <summary>
        ///     Gets or sets the maximum scan values any two deisotoped peaks can be before they are not considered to be from the
        ///     same feature.
        /// </summary>
        public int MaximumScan { get; set; }
    }
}