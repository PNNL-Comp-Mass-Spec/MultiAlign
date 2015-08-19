#region

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    public class LcmsWarpFeatureAligner :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData>
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        public LcmsWarpFeatureAligner()
        {
            Options = new LcmsWarpAlignmentOptions();
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

        /// <summary>
        ///     Gets or sets the baseline spectra provider
        /// </summary>
        public ISpectraProvider BaselineSpectraProvider { get; set; }

        /// <summary>
        ///     Gets or sets the alignee spectra provider.
        /// </summary>
        public ISpectraProvider AligneeSpectraProvider { get; set; }

        /// <summary>
        ///     Gets or sets the alignment options
        /// </summary>
        public LcmsWarpAlignmentOptions Options { get; set; }

        /// <summary>
        ///     Aligns a dataset to a mass tag database.
        /// </summary>
        public AlignmentData Align(MassTagDatabase massTagDatabase,
            IEnumerable<UMCLight> features)
        {

            var alignmentProcessor = new LcmsWarpAlignmentProcessor
            {
                Options = Options
            };

            alignmentProcessor.Progress += alignmentProcessor_Progress;

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
                Options);

            return data;
        }

        /// <summary>
        ///     Aligns a dataset to a dataset
        /// </summary>
        public AlignmentData Align(IEnumerable<UMCLight> baselineFeatures,
            IEnumerable<UMCLight> features)
        {
            var alignmentProcessor = new LcmsWarpAlignmentProcessor
            {
                Options = Options
            };


            OnStatus("Setting features from baseline dataset.");


            var umcLights = baselineFeatures as List<UMCLight> ?? baselineFeatures.ToList();

            var filteredFeatures = FilterFeaturesByAbundance(umcLights, Options);

            alignmentProcessor.SetReferenceDatasetFeatures(filteredFeatures);
            var alignmentData = AlignFeatures(alignmentProcessor,
                features,
                Options);

            var minScanReference = int.MaxValue;
            var maxScanReference = int.MinValue;
            foreach (var feature in umcLights)
            {
                minScanReference = Math.Min(minScanReference, feature.Scan);
                maxScanReference = Math.Max(maxScanReference, feature.Scan);
            }

            alignmentData.maxMTDBNET = maxScanReference;
            alignmentData.minMTDBNET = minScanReference;

            return alignmentData;
        }

        /// <summary>
        ///     Aligns the dataset to the data stored in the alignment processor.
        /// </summary>
        private AlignmentData AlignFeatures(LcmsWarpAlignmentProcessor alignmentProcessor,
            IEnumerable<UMCLight> features,
            LcmsWarpAlignmentOptions alignmentOptions)
        {
            OnStatus("Starting alignment of features.");
            var alignmentFunctions = new List<LcmsWarpAlignmentFunction>();
            var netErrorHistograms = new List<double[,]>();
            var massErrorHistograms = new List<double[,]>();
            var driftErrorHistograms = new List<double[,]>();
            var heatScores = new List<double[,]>();
            var xIntervals = new List<double[]>();
            var yIntervals = new List<double[]>();

            double minMtdbnet;
            double maxMtdbnet;
            alignmentProcessor.GetReferenceNetRange(out minMtdbnet, out maxMtdbnet);

            var minScanBaseline = int.MaxValue;
            var maxScanBaseline = int.MinValue;

            var umcLights = features as List<UMCLight> ?? features.ToList();

            var filteredFeatures = FilterFeaturesByAbundance(umcLights.ToList(), alignmentOptions);

            // Conver the features, and make a map, so that we can re-adjust the aligned values later.                        
            var map = FeatureDataConverters.MapFeature(umcLights);


            // Set features                
            OnStatus("Setting alignee features.");
            alignmentProcessor.SetAligneeDatasetFeatures(filteredFeatures);

            // Find alignment 
            OnStatus("Performing alignment warping.");
            alignmentProcessor.PerformAlignmentToMsFeatures();

            // Extract alignment function
            var alignmentFunction = alignmentProcessor.GetAlignmentFunction();
            alignmentFunctions.Add(alignmentFunction);

            // Correct the features (updates MassMonoisotopicAligned)
            OnStatus("Applying alignment function to all features.");
            alignmentProcessor.ApplyNetMassFunctionToAligneeDatasetFeatures(ref umcLights);


            // Find min/max scan for meta-data
            var tempMinScanBaseline = int.MaxValue;
            var tempMaxScanBaseline = int.MinValue;

            foreach (var feature in umcLights)
            {
                tempMaxScanBaseline = Math.Max(tempMaxScanBaseline, feature.Scan);
                tempMinScanBaseline = Math.Min(tempMinScanBaseline, feature.Scan);
                var featureId = feature.Id;
                var isInMap = map.ContainsKey(featureId);
                if (!isInMap)
                    continue;


                map[featureId].MassMonoisotopicAligned = feature.MassMonoisotopicAligned;
                map[featureId].NetAligned = feature.NetAligned;
                map[featureId].Net = feature.Net;
                map[featureId].ScanAligned = feature.ScanAligned;
            }

            minScanBaseline = Math.Min(minScanBaseline, tempMinScanBaseline);
            maxScanBaseline = Math.Max(maxScanBaseline, tempMaxScanBaseline);

            // Pull out the heat maps...
            double[,] heatScore;
            double[] xInterval;
            double[] yInterval;

            OnStatus("Retrieving alignment data.");
            alignmentProcessor.GetAlignmentHeatMap(out heatScore, out xInterval, out yInterval);

            xIntervals.Add(xInterval);
            yIntervals.Add(yInterval);
            heatScores.Add(heatScore);

            // Mass and net error histograms!  
            double[,] massErrorHistogram;
            double[,] netErrorHistogram;
            double[,] driftErrorHistogram;

            alignmentProcessor.GetErrorHistograms(alignmentOptions.MassBinSize,
                alignmentOptions.NetBinSize,
                alignmentOptions.DriftTimeBinSize,
                out massErrorHistogram,
                out netErrorHistogram,
                out driftErrorHistogram);
            massErrorHistograms.Add(massErrorHistogram);
            netErrorHistograms.Add(netErrorHistogram);
            driftErrorHistograms.Add(driftErrorHistogram);

            // Get the residual data from the warp.
            var residualData = alignmentProcessor.GetResidualData();

            // Set all of the data now 
            var alignmentData = new AlignmentData
            {
                massErrorHistogram = massErrorHistogram,
                driftErrorHistogram = driftErrorHistogram,
                netErrorHistogram = netErrorHistogram,
                alignmentFunction = alignmentFunction,
                heatScores = heatScore,
                minScanBaseline = minScanBaseline,
                maxScanBaseline = maxScanBaseline,
                NETIntercept = alignmentProcessor.NetIntercept,
                NETRsquared = alignmentProcessor.NetRsquared,
                NETSlope = alignmentProcessor.NetSlope,
                ResidualData = residualData,
                MassMean = alignmentProcessor.MassMu,
                MassStandardDeviation = alignmentProcessor.MassStd,
                NETMean = alignmentProcessor.NetMu,
                NETStandardDeviation = alignmentProcessor.NetStd
            };

            // Find out the max scan or NET value to use for the range depending on what 
            // type of baseline dataset it was (MTDB or dataset).                 
            if (alignmentOptions.AlignToMassTagDatabase)
            {
                alignmentData.minMTDBNET = (float)minMtdbnet;
                alignmentData.maxMTDBNET = (float)maxMtdbnet;
            }

            return alignmentData;
        }

        void alignmentProcessor_Progress(object sender, ProgressNotifierArgs e)
        {
            // OnStatus(e.Message + ", " + e.PercentComplete.ToString("0.0") + "% complete");
            OnStatus(e);
        }

        private static List<UMCLight> FilterFeaturesByAbundance(List<UMCLight> features,
            LcmsWarpAlignmentOptions alignmentOptions)
        {
            features.Sort((x, y) => x.AbundanceSum.CompareTo(y.AbundanceSum));

            var percent = 1 - (alignmentOptions.TopFeatureAbundancePercent / 100);
            var total = features.Count - Convert.ToInt32(features.Count * percent);
            var threshold = features[Math.Min(features.Count - 1, Math.Max(0, total))].AbundanceSum;

            // Filters features below a certain threshold.
            var filteredFeatures = features.FindAll(feature => feature.AbundanceSum >= threshold);
            return filteredFeatures;
        }

    }
}