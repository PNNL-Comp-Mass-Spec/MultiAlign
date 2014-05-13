#region

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    public class LcmsWarpFeatureAligner :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, classAlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, classAlignmentData>
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        public LcmsWarpFeatureAligner()
        {
            Options = new AlignmentOptions();
        }

        private void OnStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
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
        public AlignmentOptions Options { get; set; }

        /// <summary>
        ///     Aligns a dataset to a mass tag database.
        /// </summary>
        public classAlignmentData Align(MassTagDatabase massTagDatabase,
            IEnumerable<UMCLight> features)
        {
            var alignmentProcessor = new clsAlignmentProcessor
            {
                AlignmentOptions = AlignmentOptions.ConvertToEngine(Options)
            };


            var umcLights = features as IList<UMCLight> ?? features.ToList();
            var featureTest = umcLights.ToList().Find(x => x.DriftTime > 0);

            if (featureTest != null && !massTagDatabase.DoesContainDriftTime)
            {
                OnStatus("Warning! Data contains drift time information and the database does not.");
            }

            OnStatus("Configuring features as mass tags.");
            var tags = FeatureDataConverters.ConvertToMassTag(massTagDatabase.MassTags);

            OnStatus("Setting reference features using mass tags.");
            alignmentProcessor.SetReferenceDatasetFeatures(tags, true);
            var data = AlignFeatures(alignmentProcessor,
                umcLights,
                Options);

            alignmentProcessor.Dispose();
            return data;
        }

        /// <summary>
        ///     Aligns a dataset to a dataset
        /// </summary>
        public classAlignmentData Align(IEnumerable<UMCLight> baselineFeatures,
            IEnumerable<UMCLight> features)
        {
            var alignmentProcessor = new clsAlignmentProcessor
            {
                AlignmentOptions = AlignmentOptions.ConvertToEngine(Options)
            };
            OnStatus("Setting features from baseline dataset.");


            var umcLights = baselineFeatures as IList<UMCLight> ?? baselineFeatures.ToList();

            var filteredFeatures = FilterFeaturesByAbundance(umcLights.ToList(), Options);
            var convertedBaseLineFeatures = FeatureDataConverters.ConvertToUMC(filteredFeatures);
            alignmentProcessor.SetReferenceDatasetFeatures(convertedBaseLineFeatures);
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

            alignmentProcessor.Dispose();

            return alignmentData;
        }

        /// <summary>
        ///     Aligns the dataset to the data stored in the alignment processor.
        /// </summary>
        private classAlignmentData AlignFeatures(clsAlignmentProcessor alignmentProcessor,
            IEnumerable<UMCLight> features,
            AlignmentOptions alignmentOptions)
        {
            OnStatus("Starting alignment of features.");
            var alignmentFunctions = new List<clsAlignmentFunction>();
            var netErrorHistograms = new List<double[,]>();
            var massErrorHistograms = new List<double[,]>();
            var driftErrorHistograms = new List<double[,]>();
            var alignmentData = new List<classAlignmentData>();
            var heatScores = new List<float[,]>();
            var xIntervals = new List<float[]>();
            var yIntervals = new List<float[]>();

            var minMtdbnet = 0.0F;
            var maxMtdbnet = 1.0F;
            alignmentProcessor.GetReferenceNETRange(ref minMtdbnet, ref maxMtdbnet);

            var minScanBaseline = int.MaxValue;
            var maxScanBaseline = int.MinValue;

            // Max the split boundaries (m/z) at 2.            
            var totalBoundaries = 1;
            if (alignmentOptions.SplitAlignmentInMZ)
                totalBoundaries = 2;

            var umcLights = features as IList<UMCLight> ?? features.ToList();

            var filteredFeatures = FilterFeaturesByAbundance(umcLights.ToList(), alignmentOptions);

            // Conver the features, and make a map, so that we can re-adjust the aligned values later.
            var oldFeatures = FeatureDataConverters.ConvertToUMC(filteredFeatures);
            var transformedFeatures = FeatureDataConverters.ConvertToUMC(umcLights);
            var map = FeatureDataConverters.MapFeature(umcLights);

            for (var i = 0; i < totalBoundaries; i++)
            {
                // Set features                
                OnStatus("Setting alignee features.");
                alignmentProcessor.SetAligneeDatasetFeatures(oldFeatures, alignmentOptions.MZBoundaries[i]);

                // Find alignment 
                OnStatus("Performing alignment warping.");
                alignmentProcessor.PerformAlignmentToMSFeatures();

                // Extract alignment function
                var alignmentFunction = alignmentProcessor.GetAlignmentFunction();
                alignmentFunctions.Add(alignmentFunction);

                // Correct the features
                OnStatus("Applying alignment function to all features.");
                var newFeatures =
                    alignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref transformedFeatures);


                // Find min/max scan for meta-data
                var tempMinScanBaseline = int.MaxValue;
                var tempMaxScanBaseline = int.MinValue;

                //foreach (var feature in transformedFeatures)
                foreach (var feature in newFeatures)
                {
                    tempMaxScanBaseline = Math.Max(tempMaxScanBaseline, feature.Scan);
                    tempMinScanBaseline = Math.Min(tempMinScanBaseline, feature.Scan);
                    var featureId = feature.Id;
                    var isInMap = map.ContainsKey(featureId);
                    if (!isInMap) continue;

                    map[featureId].MassMonoisotopicAligned = feature.MassCalibrated;
                    map[featureId].NetAligned = feature.Net;
                    map[featureId].Net = feature.Net;
                    map[featureId].ScanAligned = feature.ScanAligned;
                }

                minScanBaseline = Math.Min(minScanBaseline, tempMinScanBaseline);
                maxScanBaseline = Math.Max(maxScanBaseline, tempMaxScanBaseline);

                // Pull out the heat maps...
                var heatScore = new float[1, 1];
                var xInterval = new float[1];
                var yInterval = new float[1];

                OnStatus("Retrieving alignment data.");
                alignmentProcessor.GetAlignmentHeatMap(ref heatScore, ref xInterval, ref yInterval);

                xIntervals.Add(xInterval);
                yIntervals.Add(yInterval);
                heatScores.Add(heatScore);

                // Mass and net error histograms!  
                var massErrorHistogram = new double[1, 1];
                var netErrorHistogram = new double[1, 1];
                var driftErrorHistogram = new double[1, 1];

                alignmentProcessor.GetErrorHistograms(alignmentOptions.MassBinSize,
                    alignmentOptions.NETBinSize,
                    alignmentOptions.DriftTimeBinSize,
                    ref massErrorHistogram,
                    ref netErrorHistogram,
                    ref driftErrorHistogram);
                massErrorHistograms.Add(massErrorHistogram);
                netErrorHistograms.Add(netErrorHistogram);
                driftErrorHistograms.Add(driftErrorHistogram);

                // Get the residual data from the warp.
                var residualData = alignmentProcessor.GetResidualData();

                // Set all of the data now 
                var data = new classAlignmentData
                {
                    massErrorHistogram = massErrorHistogram,
                    driftErrorHistogram = driftErrorHistogram,
                    netErrorHistogram = netErrorHistogram,
                    alignmentFunction = alignmentFunction,
                    heatScores = heatScore,
                    minScanBaseline = minScanBaseline,
                    maxScanBaseline = maxScanBaseline,
                    NETIntercept = alignmentProcessor.NETIntercept,
                    NETRsquared = alignmentProcessor.NETLinearRSquared,
                    NETSlope = alignmentProcessor.NETSlope,
                    ResidualData = residualData,
                    MassMean = alignmentProcessor.GetMassMean(),
                    MassStandardDeviation = alignmentProcessor.GetMassStandardDeviation(),
                    NETMean = alignmentProcessor.GetNETMean(),
                    NETStandardDeviation = alignmentProcessor.GetNETStandardDeviation()
                };

                // Find out the max scan or NET value to use for the range depending on what 
                // type of baseline dataset it was (MTDB or dataset).                 
                if (alignmentOptions.IsAlignmentBaselineAMasstagDB)
                {
                    data.minMTDBNET = minMtdbnet;
                    data.maxMTDBNET = maxMtdbnet;
                }

                alignmentData.Add(data);
            }


            OnStatus("Combining alignment residual and mass / net error data for split analysis.");
            var mergedData = new classAlignmentData();
            var mergedAlignmentFunction = alignmentFunctions[alignmentFunctions.Count - 1];


            // Merge the mass error histogram data.            
            var maxMassHistogramLength = 0;
            var maxNetHistogramLength = 0;
            var maxDriftHistogramLength = 0;
            foreach (var t in alignmentData)
            {
                maxMassHistogramLength = Math.Max(maxMassHistogramLength, t.massErrorHistogram.GetLength(0));
                maxNetHistogramLength = Math.Max(maxNetHistogramLength, t.netErrorHistogram.GetLength(0));
                maxDriftHistogramLength = Math.Max(maxDriftHistogramLength, t.driftErrorHistogram.GetLength(0));
            }

            var massErrorHistogramData = new double[maxMassHistogramLength, 2];
            MergeHistogramData(massErrorHistogramData, alignmentData[0].massErrorHistogram, false);


            // The residual arrays are the same size, here we start the process to count the 
            // size so we can merge all of the results back into one array.            
            var countMassResiduals = 0;
            var countNetResiduals = 0;

            for (var i = 0; i < alignmentData.Count; i++)
            {
                if (i > 0)
                    MergeHistogramData(massErrorHistogramData, alignmentData[i].massErrorHistogram, true);

                countMassResiduals += alignmentData[i].ResidualData.mz.Length;
                countNetResiduals += alignmentData[i].ResidualData.scans.Length;
            }


            // Merge:
            //     NET error histogram data
            //     Mass Residual Data                        
            var netErrorHistogramData = new double[maxNetHistogramLength, 2];
            MergeHistogramData(netErrorHistogramData, alignmentData[0].netErrorHistogram, false);

            mergedData.ResidualData = new classAlignmentResidualData
            {
                customNet = new float[countNetResiduals],
                linearCustomNet = new float[countNetResiduals],
                linearNet = new float[countNetResiduals],
                scans = new float[countNetResiduals],
                massError = new float[countMassResiduals],
                massErrorCorrected = new float[countMassResiduals],
                mz = new float[countMassResiduals],
                mzMassError = new float[countMassResiduals],
                mzMassErrorCorrected = new float[countMassResiduals]
            };


            var copyNetBlocks = 0;
            var copyMassBlocks = 0;

            for (var i = 0; i < alignmentData.Count; i++)
            {
                // Merge the residual data                
                alignmentData[i].ResidualData.customNet.CopyTo(mergedData.ResidualData.customNet, copyNetBlocks);
                alignmentData[i].ResidualData.linearCustomNet.CopyTo(mergedData.ResidualData.linearCustomNet,
                    copyNetBlocks);
                alignmentData[i].ResidualData.linearNet.CopyTo(mergedData.ResidualData.linearNet, copyNetBlocks);
                alignmentData[i].ResidualData.scans.CopyTo(mergedData.ResidualData.scans, copyNetBlocks);

                alignmentData[i].ResidualData.massError.CopyTo(mergedData.ResidualData.massError, copyMassBlocks);
                alignmentData[i].ResidualData.massErrorCorrected.CopyTo(mergedData.ResidualData.massErrorCorrected,
                    copyMassBlocks);
                alignmentData[i].ResidualData.mzMassError.CopyTo(mergedData.ResidualData.mzMassError, copyMassBlocks);
                alignmentData[i].ResidualData.mzMassErrorCorrected.CopyTo(mergedData.ResidualData.mzMassErrorCorrected,
                    copyMassBlocks);
                alignmentData[i].ResidualData.mz.CopyTo(mergedData.ResidualData.mz, copyMassBlocks);

                copyNetBlocks += alignmentData[i].ResidualData.scans.Length;
                copyMassBlocks += alignmentData[i].ResidualData.mz.Length;

                mergedData.MassMean = alignmentData[i].MassMean;
                mergedData.MassStandardDeviation = alignmentData[i].MassStandardDeviation;

                mergedData.NETIntercept = alignmentData[i].MassStandardDeviation;
                mergedData.NETMean = alignmentData[i].NETMean;
                mergedData.NETStandardDeviation = alignmentData[i].NETStandardDeviation;
                mergedData.NETRsquared = alignmentData[i].NETRsquared;
                mergedData.NETSlope = alignmentData[i].NETSlope;

                if (i > 0)
                {
                    MergeHistogramData(netErrorHistogramData, alignmentData[i].netErrorHistogram, true);
                }
            }

            // Grab the heat scores!
            mergedData.heatScores = alignmentData[alignmentData.Count - 1].heatScores;
            mergedData.massErrorHistogram = massErrorHistogramData;
            mergedData.netErrorHistogram = netErrorHistogramData;

            mergedData.driftErrorHistogram = driftErrorHistograms[0];
            mergedData.alignmentFunction = mergedAlignmentFunction;

            alignmentProcessor.Dispose();

            return mergedData;
        }

        private static IEnumerable<UMCLight> FilterFeaturesByAbundance(List<UMCLight> features,
            AlignmentOptions alignmentOptions)
        {
            features.Sort((x, y) => x.AbundanceSum.CompareTo(y.AbundanceSum));

            var percent = 1 - (alignmentOptions.TopFeatureAbundancePercent/100);
            var total = features.Count - Convert.ToInt32(features.Count*percent);
            var threshold = features[Math.Min(features.Count - 1, Math.Max(0, total))].AbundanceSum;

            // Filters features below a certain threshold.
            var filteredFeatures = features.FindAll(feature => feature.AbundanceSum >= threshold);
            return filteredFeatures;
        }

        /// <summary>
        ///     Merges the histogram data leaving the result in old.
        /// </summary>
        /// <param name="histogramDest">Data to retain merged data.</param>
        /// <param name="histogramSource">Data to copy.</param>
        /// <param name="checkClosestBin">
        ///     Flag indicating whether to use the closest bin or to just assume that the x values match
        ///     between dest and src.
        /// </param>
        private static void MergeHistogramData(double[,] histogramDest,
            double[,] histogramSource,
            bool checkClosestBin)
        {
            for (var i = 0; i < histogramSource.GetLength(0) && i < histogramDest.GetLength(0); i++)
            {
                var bestIndex = 0;
                var massDiff = double.MaxValue;

                if (checkClosestBin == false)
                {
                    bestIndex = i;
                    histogramDest[i, 0] = histogramSource[bestIndex, 0];
                }
                else
                {
                    var length = Math.Min(histogramDest.GetLength(0), histogramSource.GetLength(0));

                    // Find the best mass item if the previous mass items are skewed or changed                    
                    for (var j = 0; j < length; j++)
                    {
                        var diff = Math.Abs(histogramDest[j, 0] - histogramSource[j, 0]);
                        if (diff < massDiff)
                        {
                            bestIndex = j;
                            massDiff = diff;
                        }
                    }
                }
                histogramDest[i, 1] += histogramSource[bestIndex, 1];
            }
        }

        /// <summary>
        ///     Aligns the clusters to the mass tag database
        /// </summary>
        /// <param name="massTagDatabase">Mass tag databaset to align to.</param>
        /// <param name="clusters">Clusters to align.</param>
        /// <param name="options">Alignment options.</param>
        /// <returns>Alignment data for the clusters to mass tag database.</returns>
        public classAlignmentData AlignFeatures(MassTagDatabase massTagDatabase,
            List<clsCluster> clusters,
            AlignmentOptions options)
        {
            OnStatus("Starting alignment of clusters.");
            var alignmentProcessor = new clsAlignmentProcessor();
            var tags = massTagDatabase.MassTags.Select(tag => new clsMassTag
            {
                mintMassTagId = tag.Id,
                mintConformerID = tag.ConformationId,
                DriftTime = tag.DriftTime,
                mdblAvgGANET = tag.NetAverage,
                mdblMonoMass = tag.MassMonoisotopic
            }).ToList();

            alignmentProcessor.SetReferenceDatasetFeatures(tags, true);

            OnStatus("Setting alignee clusters.");
            alignmentProcessor.SetAligneeDatasetFeatures(clusters, options.MZBoundaries[0]);

            OnStatus("Performing alignment warping.");
            alignmentProcessor.PerformAlignmentToMassTagDatabase();

            OnStatus("Applying alignment function to all features.");
            alignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref clusters);
            var alignmentFunction = alignmentProcessor.GetAlignmentFunction();


            OnStatus("Retrieving alignment data.");
            // Heat maps
            var heatScores = new float[1, 1];
            var xIntervals = new float[1];
            var yIntervals = new float[1];
            alignmentProcessor.GetAlignmentHeatMap(ref heatScores, ref xIntervals, ref yIntervals);


            float minMtdbnet = 0.0F, maxMtdbnet = 1.0F;
            alignmentProcessor.GetReferenceNETRange(ref minMtdbnet, ref maxMtdbnet);


            // Residuals

            var residualData = alignmentProcessor.GetResidualData();

            // Get error histograms 
            var netErrorHistogram = new double[1, 1];
            var massErrorHistogram = new double[1, 1];
            var driftErrorHistogram = new double[1, 1];
            alignmentProcessor.GetErrorHistograms(options.MassBinSize,
                options.NETBinSize,
                options.DriftTimeBinSize,
                ref massErrorHistogram,
                ref netErrorHistogram,
                ref driftErrorHistogram);

            var clusterAlignmentData = new classAlignmentData
            {
                alignmentFunction = alignmentFunction,
                heatScores = heatScores,
                massErrorHistogram = massErrorHistogram,
                netErrorHistogram = netErrorHistogram,
                driftErrorHistogram = driftErrorHistogram,
                NETIntercept = alignmentProcessor.NETIntercept,
                NETRsquared = alignmentProcessor.NETLinearRSquared,
                NETSlope = alignmentProcessor.NETSlope,
                ResidualData = residualData,
                MassMean = alignmentProcessor.GetMassMean(),
                MassStandardDeviation = alignmentProcessor.GetMassMean(),
                NETMean = alignmentProcessor.GetNETMean(),
                NETStandardDeviation = alignmentProcessor.GetNETStandardDeviation()
            };

            return clusterAlignmentData;
        }
    }
}