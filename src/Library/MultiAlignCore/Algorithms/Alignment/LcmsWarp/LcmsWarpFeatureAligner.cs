#region

using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    public class LcmsWarpFeatureAligner :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<IEnumerable<MassTagLight>, IEnumerable<UMCLight>, AlignmentData>
    {
        private readonly LcmsWarpAlignmentOptions _options;

        public event EventHandler<ProgressNotifierArgs> Progress;

        public LcmsWarpFeatureAligner(LcmsWarpAlignmentOptions options)
        {
            _options = options;
        }

        private void OnStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }

        private void OnStatus(ProgressNotifierArgs e)
        {
            if (Progress != null)
            {
                Progress(this, e);
            }
        }

        private void AlignmentProcessor_Progress(object sender, ProgressNotifierArgs e)
        {
            OnStatus(e);
        }

        /// <summary>
        ///     Gets or sets the baseline spectra provider
        /// </summary>
        public ISpectraProvider BaselineSpectraProvider { get; set; }

        /// <summary>
        ///     Gets or sets the alignee spectra provider.
        /// </summary>
        public ISpectraProvider AligneeSpectraProvider { get; set; }

        /// <summary>
        /// Aligns a dataset to a mass tag database
        /// </summary>
        /// <param name="massTagDatabase"></param>
        /// <param name="features"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public AlignmentData Align(MassTagDatabase massTagDatabase,
            IEnumerable<UMCLight> features,
            IProgress<ProgressData> progress = null)
        {
            var progData = new ProgressData(progress);
            var alignmentProcessor = new LcmsWarpAlignmentProcessor(_options);

            alignmentProcessor.Progress += AlignmentProcessor_Progress;
            alignmentProcessor.Progress += (o, e) =>
            {
                progData.Status = e.Message;
                progData.Report(e.PercentComplete);
            };

            var umcLights = features as IList<UMCLight> ?? features.ToList();
            var featureTest = umcLights.ToList().Find(x => x.DriftTime > 0);

            if (featureTest != null && !massTagDatabase.DoesContainDriftTime)
            {
                OnStatus("Warning! Data contains drift time information and the database does not.");
            }

            OnStatus("Configuring features as mass tags.");

            OnStatus("Setting reference features using mass tags.");
            alignmentProcessor.SetReferenceDatasetFeatures(massTagDatabase.MassTags);
            var data = AlignFeatures(alignmentProcessor,
                umcLights,
                _options);

            return data;
        }

        /// <summary>
        /// Align a dataset to a baseline dataset
        /// </summary>
        /// <param name="baselineFeatures">baseline dataset features</param>
        /// <param name="features">alignee dataset features</param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public AlignmentData Align(IEnumerable<UMCLight> baselineFeatures,
            IEnumerable<UMCLight> features,
            IProgress<ProgressData> progress = null)
        {
            var progData = new ProgressData(progress);
            var alignmentProcessor = new LcmsWarpAlignmentProcessor(_options);

            alignmentProcessor.Progress += AlignmentProcessor_Progress;
            alignmentProcessor.Progress += (o, e) =>
            {
                progData.Status = e.Message;
                progData.Report(e.PercentComplete);
            };

            OnStatus("Setting features from baseline dataset.");

            var umcLights = baselineFeatures as List<UMCLight> ?? baselineFeatures.ToList();

            var filteredFeatures = FilterFeaturesByAbundance(umcLights, _options);

            alignmentProcessor.SetReferenceDatasetFeatures(filteredFeatures);
            var alignmentData = AlignFeatures(alignmentProcessor,
                features,
                _options);

            var minScanReference = int.MaxValue;
            var maxScanReference = int.MinValue;
            foreach (var feature in umcLights)
            {
                minScanReference = Math.Min(minScanReference, feature.Scan);
                maxScanReference = Math.Max(maxScanReference, feature.Scan);
            }

            alignmentData.MaxMTDBNET = maxScanReference;
            alignmentData.MinMTDBNET = minScanReference;

            return alignmentData;
        }

        /// <summary>
        /// Align a UMCLight Enumerable to a MassTagLight enumerable.
        /// The MassTagLight Enumerable is used as the baseline to align the
        /// UMCLight enumerable.
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="features"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public AlignmentData Align(IEnumerable<MassTagLight> baseline, IEnumerable<UMCLight> features,
            IProgress<ProgressData> progress = null)
        {
            var alignmentProcessor = new LcmsWarpAlignmentProcessor(_options);
            var baselineMassTags = baseline as List<MassTagLight> ?? baseline.ToList();
            var aligneeFeatures = features as List<UMCLight> ?? features.ToList();

            var featureTest = aligneeFeatures.Find(x => x.DriftTime > 0);
            var massTagTest = baselineMassTags.Find(x => x.DriftTime > 0);

            if (featureTest != null && massTagTest == null)
            {
                Console.WriteLine("Warning! Data has drift time info, but the mass tags do not.");
            }

            alignmentProcessor.SetReferenceDatasetFeatures(baselineMassTags);

            var data = AlignFeatures(alignmentProcessor, aligneeFeatures, _options);

            return data;
        }

        /// <summary>
        ///     Aligns the dataset to the data stored in the alignment processor.
        /// </summary>
        private AlignmentData AlignFeatures(LcmsWarpAlignmentProcessor alignmentProcessor,
            IEnumerable<UMCLight> features,
            LcmsWarpAlignmentOptions alignmentOptions)
        {
            var alignmentData = new AlignmentData();
            OnStatus("Starting alignment of features.");
            //var alignmentFunctions = new List<LcmsWarpAlignmentFunction>();
            //var netErrorHistograms = new List<Dictionary<double, int>>();
            //var massErrorHistograms = new List<Dictionary<double, int>>();
            //var driftErrorHistograms = new List<Dictionary<double, int>>();
            //var heatScores = new List<double[,]>();
            //var xIntervals = new List<double[]>();
            //var yIntervals = new List<double[]>();

            // Set minMtdbnet and maxMtdbnet to 0 when aligning against AMT tags from a database
            // The values will be updated later
            //var minMtdbNet = alignmentProcessor.MinReferenceNet;
            //var maxMtdbNet = alignmentProcessor.MaxReferenceNet;

            // Find out the max scan or NET value to use for the range depending on what 
            // type of baseline dataset it was (MTDB or dataset).                 
            if (alignmentOptions.AlignToMassTagDatabase)
            {
                alignmentData.MinMTDBNET = (float)alignmentProcessor.MinReferenceNet;
                alignmentData.MaxMTDBNET = (float)alignmentProcessor.MaxReferenceNet;
            }

            var umcLights = features as List<UMCLight> ?? features.ToList();

            var filteredFeatures = FilterFeaturesByAbundance(umcLights, alignmentOptions);

            // Convert the features, and make a map, so that we can re-adjust the aligned values later.                        
            var map = FeatureDataConverters.MapFeature(umcLights);

            // Set features                
            OnStatus("Setting alignee features.");
            alignmentProcessor.SetAligneeDatasetFeatures(filteredFeatures);

            // Find alignment 
            OnStatus("Performing alignment warping.");
            alignmentProcessor.PerformAlignmentToMsFeatures();

            // Extract alignment function
            //var alignmentFunction = alignmentProcessor.GetAlignmentFunction();
            //alignmentFunctions.Add(alignmentFunction);
            alignmentData.AlignmentFunction = alignmentProcessor.GetAlignmentFunction();

            // Correct the features (updates MassMonoisotopicAligned)
            OnStatus("Applying alignment function to all features.");
            alignmentProcessor.ApplyNetMassFunctionToAligneeDatasetFeatures(umcLights);

            // Find min/max scan for meta-data
            var minScanBaseline = int.MaxValue;
            var maxScanBaseline = int.MinValue;

            foreach (var feature in umcLights)
            {
                maxScanBaseline = Math.Max(maxScanBaseline, feature.Scan);
                minScanBaseline = Math.Min(minScanBaseline, feature.Scan);
                var featureId = feature.Id;
                var isInMap = map.ContainsKey(featureId);
                if (!isInMap)
                    continue;

                map[featureId].MassMonoisotopicAligned = feature.MassMonoisotopicAligned;
                map[featureId].NetAligned = feature.NetAligned;
                map[featureId].Net = feature.Net;
                map[featureId].NetStart = feature.NetStart;
                map[featureId].NetEnd = feature.NetEnd;
                map[featureId].ScanAligned = feature.ScanAligned;
            }

            alignmentData.MinScanBaseline = minScanBaseline;
            alignmentData.MaxScanBaseline = maxScanBaseline;

            // Pull out the heat maps...
            //double[,] heatScore;
            //double[] xInterval;
            //double[] yInterval;

            OnStatus("Retrieving alignment data.");
            //alignmentProcessor.GetAlignmentHeatMap(out heatScore, out xInterval, out yInterval);

            alignmentData.HeatScores = alignmentProcessor.GetAlignmentHeatMap();

            //xIntervals.Add(xInterval);
            //yIntervals.Add(yInterval);
            //heatScores.Add(heatScore);

            // Mass and net error histograms!
            //Dictionary<double, int> massErrorHistogram;
            //Dictionary<double, int> netErrorHistogram;
            //Dictionary<double, int> driftErrorHistogram;

            //alignmentProcessor.GetErrorHistograms(alignmentOptions.MassBinSize, alignmentOptions.NetBinSize,
            //    alignmentOptions.DriftTimeBinSize, out massErrorHistogram, out netErrorHistogram, out driftErrorHistogram);
            //massErrorHistograms.Add(massErrorHistogram);
            //netErrorHistograms.Add(netErrorHistogram);
            //driftErrorHistograms.Add(driftErrorHistogram);
            alignmentData.MassErrorHistogram = alignmentProcessor.GetMassErrorHistogram(alignmentOptions.MassBinSize);
            alignmentData.NetErrorHistogram = alignmentProcessor.GetNetErrorHistogram(alignmentOptions.NetBinSize);
            alignmentData.DriftErrorHistogram = alignmentProcessor.GetDriftErrorHistogram(alignmentOptions.DriftTimeBinSize);

            // Get the residual data from the warp.
            //var residualData = alignmentProcessor.GetResidualData();
            alignmentData.ResidualData = alignmentProcessor.GetResidualData();

            alignmentData.NETIntercept = alignmentProcessor.NetIntercept;
            alignmentData.NETRsquared = alignmentProcessor.NetRsquared;
            alignmentData.NETSlope = alignmentProcessor.NetSlope;
            alignmentData.MassMean = alignmentProcessor.MassMu;
            alignmentData.MassStandardDeviation = alignmentProcessor.MassStd;
            alignmentData.NETMean = alignmentProcessor.NetMu;
            alignmentData.NETStandardDeviation = alignmentProcessor.NetStd;
            alignmentData.BaselineIsAmtDB = _options.AlignToMassTagDatabase;

            // Set all of the data now
            //var alignmentData = new AlignmentData
            //{
            //    MassErrorHistogram = massErrorHistogram,
            //    DriftErrorHistogram = driftErrorHistogram,
            //    NetErrorHistogram = netErrorHistogram,
            //    AlignmentFunction = alignmentFunction,
            //    HeatScores = heatScore,
            //    MinScanBaseline = minScanBaseline,
            //    MaxScanBaseline = maxScanBaseline,
            //    NETIntercept = alignmentProcessor.NetIntercept,
            //    NETRsquared = alignmentProcessor.NetRsquared,
            //    NETSlope = alignmentProcessor.NetSlope,
            //    ResidualData = residualData,
            //    MassMean = alignmentProcessor.MassMu,
            //    MassStandardDeviation = alignmentProcessor.MassStd,
            //    NETMean = alignmentProcessor.NetMu,
            //    NETStandardDeviation = alignmentProcessor.NetStd,
            //    BaselineIsAmtDB = _options.AlignToMassTagDatabase
            //};

            // Find out the max scan or NET value to use for the range depending on what 
            // type of baseline dataset it was (MTDB or dataset).                 
            //if (alignmentOptions.AlignToMassTagDatabase)
            //{
            //    alignmentData.MinMTDBNET = (float)minMtdbNet;
            //    alignmentData.MaxMTDBNET = (float)maxMtdbNet;
            //}

            return alignmentData;
        }

        private static List<UMCLight> FilterFeaturesByAbundance(List<UMCLight> features,
            LcmsWarpAlignmentOptions alignmentOptions)
        {
            // Sort by abundance to ease filtering process. Options look at the percentage of abundance
            // so threshhold needs to be converted to what the abundance sum would be. 
            features.Sort((x, y) => x.AbundanceSum.CompareTo(y.AbundanceSum));

            if (alignmentOptions.TopFeatureAbundancePercent <= 0)
            {
                return features;
            }
            
            var percent = 1 - (alignmentOptions.TopFeatureAbundancePercent / 100);
            var total = features.Count - Convert.ToInt32(features.Count * percent);
            var threshold = features[Math.Min(features.Count - 1, Math.Max(0, total))].AbundanceSum;

            // Re-sort with regards to monoisotopic mass for accurate application of NET-Mass function
            // to the dataset we need to align.
            features.Sort((x, y) => x.MassMonoisotopic.CompareTo(y.MassMonoisotopic));

            if (threshold <= 0)
                return features;

            // Filters features below a certain threshold.
            var filteredFeatures = features.Where(feature => feature.AbundanceSum >= threshold);
            return filteredFeatures.ToList();
        }

    }
}