using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment
{
    public class LCMSWarpFeatureAligner: IFeatureAligner, IProgressNotifer
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        clsAlignmentProcessor m_processor = new clsAlignmentProcessor();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LCMSWarpFeatureAligner()
        {
        }

        private void OnStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }
        /// <summary>
        /// Aligns a dataset to a mass tag database.
        /// </summary>
        /// <param name="massTagDatabase"></param>
        /// <param name="features"></param>
        /// <param name="alignmentOptions"></param>
        /// <param name="boundaries"></param>
        /// <returns></returns>
        public classAlignmentData AlignFeatures(MassTagDatabase                 massTagDatabase,
                                                List<UMCLight>                  features,
                                                AlignmentOptions                alignmentOptions,
                                                bool                            alignDriftTimes)
        {                        
            clsAlignmentProcessor alignmentProcessor    = new clsAlignmentProcessor();
            alignmentProcessor.AlignmentOptions         = AlignmentOptions.ConvertToEngine(alignmentOptions);
            

            UMCLight featureTest = features.Find(delegate(UMCLight x)
            {
                return x.DriftTime > 0;
            });
            
            if (featureTest != null && !massTagDatabase.DoesContainDriftTime)
            {
                OnStatus("Warning! Data contains drift time information and the database does not.");                
            }

            OnStatus("Configuring features as mass tags.");            
            List<clsMassTag> tags = FeatureDataConverters.ConvertToMassTag(massTagDatabase.MassTags);

            OnStatus("Setting reference features using mass tags.");
            alignmentProcessor.SetReferenceDatasetFeatures(tags, true);
            classAlignmentData data =  AlignFeatures(alignmentProcessor,
                                                     features,   
                                                     alignmentOptions);

            alignmentProcessor.Dispose();
            return data;
        }           
        /// <summary>
        /// Aligns a dataset to a dataset
        /// </summary>
        /// <param name="baselineFeatures"></param>
        /// <param name="features"></param>
        /// <param name="alignmentOptions"></param>
        /// <param name="boundaries"></param>
        /// <returns></returns>
        public classAlignmentData AlignFeatures(List<UMCLight>                  baselineFeatures,
                                                List<UMCLight>                  features,
                                                AlignmentOptions                alignmentOptions)
        {
            clsAlignmentProcessor alignmentProcessor    = new clsAlignmentProcessor();
            alignmentProcessor.AlignmentOptions         = AlignmentOptions.ConvertToEngine(alignmentOptions);


            OnStatus("Setting features from baseline dataset.");
            List<clsUMC> convertedBaseLineFeatures = FeatureDataConverters.ConvertToUMC(baselineFeatures);
            alignmentProcessor.SetReferenceDatasetFeatures(convertedBaseLineFeatures);
            classAlignmentData alignmentData            = AlignFeatures( alignmentProcessor,
                                                                        features,
                                                                        alignmentOptions);

            int minScanReference = int.MaxValue;
            int maxScanReference = int.MinValue;
            foreach (UMCLight feature in baselineFeatures)
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
        /// Aligns the dataset to the data stored in the alignment processor.
        /// </summary>
        private classAlignmentData AlignFeatures(clsAlignmentProcessor              alignmentProcessor,
                                                List<UMCLight>                      features,
                                                AlignmentOptions                    alignmentOptions)
        {

            OnStatus("Starting alignment of features.");
            List<clsAlignmentFunction> alignmentFunctions   = new List<clsAlignmentFunction>();
            List<double[,]> netErrorHistograms              = new List<double[,]>();
            List<double[,]> massErrorHistograms             = new List<double[,]>();
            List<double[,]> driftErrorHistograms             = new List<double[,]>();
            List<classAlignmentData> alignmentData          = new List<classAlignmentData>();
            List<float[,]> heatScores                       = new List<float[,]>();
            List<float[]> xIntervals                        = new List<float[]>();
            List<float[]> yIntervals                        = new List<float[]>();

            float minMTDBNET = 0.0F;
            float maxMTDBNET = 1.0F;
            alignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET);

            int minScanBaseline = int.MaxValue;
            int maxScanBaseline = int.MinValue;

            // Max the split boundaries (m/z) at 2.            
            int totalBoundaries = 1;
            if (alignmentOptions.SplitAlignmentInMZ == true)
            {
                totalBoundaries = 2;
            }

            // Conver the features, and make a map, so that we can re-adjust the aligned values later.
            List<clsUMC> oldFeatures        = FeatureDataConverters.ConvertToUMC(features);
            Dictionary<int, UMCLight> map   = FeatureDataConverters.MapFeature<UMCLight>(features);

            for (int i = 0; i < totalBoundaries; i++)
            {
                // Set features                
                OnStatus("Setting alignee features.");
                alignmentProcessor.SetAligneeDatasetFeatures(oldFeatures, alignmentOptions.MZBoundaries[i]);                

                // Find alignment 

                OnStatus("Performing alignment warping.");
                alignmentProcessor.PerformAlignmentToMSFeatures();

                // Extract alignment function
                clsAlignmentFunction alignmentFunction = alignmentProcessor.GetAlignmentFunction();
                alignmentFunctions.Add(alignmentFunction);

                // Correct the features
                OnStatus("Applying alignment function to all features.");
                alignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref oldFeatures);
                
                // Find min/max scan for meta-data
                int tempMinScanBaseline = int.MaxValue;
                int tempMaxScanBaseline = int.MinValue;
                foreach (clsUMC feature in oldFeatures)
                {
                    tempMaxScanBaseline     = Math.Max(tempMaxScanBaseline, feature.Scan);
                    tempMinScanBaseline     = Math.Min(tempMinScanBaseline, feature.Scan);
                    int featureID           = feature.Id;
                    bool isInMap            = map.ContainsKey(featureID);
                    if (isInMap)
                    {
                        map[featureID].MassMonoisotopicAligned = feature.MassCalibrated;
                        map[featureID].NET                     = feature.Net;
                        map[featureID].RetentionTime           = feature.Net;                                               
                    }
                }                
                minScanBaseline = Math.Min(minScanBaseline, tempMinScanBaseline);
                maxScanBaseline = Math.Max(maxScanBaseline, tempMaxScanBaseline);

                // Pull out the heat maps...
                float[,] heatScore = new float[1, 1];
                float[] xInterval = new float[1];
                float[] yInterval = new float[1];

                OnStatus("Retrieving alignment data.");
                alignmentProcessor.GetAlignmentHeatMap(ref heatScore, ref xInterval, ref yInterval);

                xIntervals.Add(xInterval);
                yIntervals.Add(yInterval);
                heatScores.Add(heatScore);

                // Mass and net error histograms!  
                double[,] massErrorHistogram = new double[1, 1];
                double[,] netErrorHistogram = new double[1, 1];
                double[,] driftErrorHistogram = new double[1, 1];

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
                float[,] linearNet              = new float[1, 1];
                float[,] customNet              = new float[1, 1];
                float[,] linearCustomNet        = new float[1, 1];
                float[,] massError              = new float[1, 1];
                float[,] massErrorCorrected     = new float[1, 1];
                float[,] mzMassError            = new float[1, 1];
                float[,] mzMassErrorCorrected   = new float[1, 1];
                classAlignmentResidualData residualData = alignmentProcessor.GetResidualData();
               
                // Set all of the data now 
                classAlignmentData data     = new classAlignmentData();
                data.massErrorHistogram     = massErrorHistogram;
                data.driftErrorHistogram    = driftErrorHistogram;
                data.netErrorHistogram      = netErrorHistogram;                
                data.alignmentFunction      = alignmentFunction;
                data.heatScores             = heatScore;
                data.minScanBaseline        = minScanBaseline;
                data.maxScanBaseline        = maxScanBaseline;
                data.NETIntercept           = alignmentProcessor.NETIntercept;
                data.NETRsquared            = alignmentProcessor.NETLinearRSquared;
                data.NETSlope               = alignmentProcessor.NETSlope;
                data.ResidualData           = residualData;
                data.MassMean               = alignmentProcessor.GetMassMean();
                data.MassStandardDeviation  = alignmentProcessor.GetMassStandardDeviation();
                data.NETMean                = alignmentProcessor.GetNETMean();
                data.NETStandardDeviation   = alignmentProcessor.GetNETStandardDeviation();

                // Find out the max scan or NET value to use for the range depending on what 
                // type of baseline dataset it was (MTDB or dataset).                 
                if (alignmentOptions.IsAlignmentBaselineAMasstagDB == true)
                {
                    data.minMTDBNET = minMTDBNET;
                    data.maxMTDBNET = maxMTDBNET;
                }

                alignmentData.Add(data);                
            }


            OnStatus("Combining alignment residual and mass / net error data for split analysis.");
            classAlignmentData mergedData                = new classAlignmentData();
            clsAlignmentFunction mergedAlignmentFunction = alignmentFunctions[alignmentFunctions.Count - 1];
            float[,] mergedHeatScores                    = new float[1, 1];

            /// ////////////////////////////////////////////////////////////
            /// Merge the mass error histogram data.
            /// ////////////////////////////////////////////////////////////
            int maxMassHistogramLength = 0;
            int maxNetHistogramLength = 0;
            int maxDriftHistogramLength = 0;
            for (int i = 0; i < alignmentData.Count; i++)
            {
                maxMassHistogramLength = Math.Max(maxMassHistogramLength, alignmentData[0].massErrorHistogram.GetLength(0));
                maxNetHistogramLength = Math.Max(maxNetHistogramLength, alignmentData[0].netErrorHistogram.GetLength(0));
                maxDriftHistogramLength = Math.Max(maxDriftHistogramLength, alignmentData[0].driftErrorHistogram.GetLength(0));
            }

            double[,] massErrorHistogramData = new double[maxMassHistogramLength, 2];
            MergeHistogramData(massErrorHistogramData, alignmentData[0].massErrorHistogram, false);

            /// 
            /// The residual arrays are the same size, here we start the process to count the 
            /// size so we can merge all of the results back into one array.
            /// 
            int countMassResiduals = 0;
            int countNETResiduals = 0;

            for (int i = 0; i < alignmentData.Count; i++)
            {
                if (i > 0)
                    MergeHistogramData(massErrorHistogramData, alignmentData[i].massErrorHistogram, true);

                countMassResiduals += alignmentData[i].ResidualData.mz.Length;
                countNETResiduals += alignmentData[i].ResidualData.scans.Length;
            }

            /// //////////////////////////////////////////////////////////// 
            /// Merge:
            ///     NET error histogram data
            ///     Mass Residual Data            
            /// ////////////////////////////////////////////////////////////
            double[,] netErrorHistogramData = new double[maxNetHistogramLength, 2];
            MergeHistogramData(netErrorHistogramData, alignmentData[0].netErrorHistogram, false);

            mergedData.ResidualData = new classAlignmentResidualData();
            mergedData.ResidualData.customNet = new float[countNETResiduals];
            mergedData.ResidualData.linearCustomNet = new float[countNETResiduals];
            mergedData.ResidualData.linearNet = new float[countNETResiduals];
            mergedData.ResidualData.scans = new float[countNETResiduals];
            mergedData.ResidualData.massError = new float[countMassResiduals];
            mergedData.ResidualData.massErrorCorrected = new float[countMassResiduals];
            mergedData.ResidualData.mz = new float[countMassResiduals];
            mergedData.ResidualData.mzMassError = new float[countMassResiduals];
            mergedData.ResidualData.mzMassErrorCorrected = new float[countMassResiduals];


            int copyNETBlocks = 0;
            int copyMassBlocks = 0;

            for (int i = 0; i < alignmentData.Count; i++)
            {
                /// 
                /// Merge the residual data
                /// 
                alignmentData[i].ResidualData.customNet.CopyTo(mergedData.ResidualData.customNet, copyNETBlocks);
                alignmentData[i].ResidualData.linearCustomNet.CopyTo(mergedData.ResidualData.linearCustomNet, copyNETBlocks);
                alignmentData[i].ResidualData.linearNet.CopyTo(mergedData.ResidualData.linearNet, copyNETBlocks);
                alignmentData[i].ResidualData.scans.CopyTo(mergedData.ResidualData.scans, copyNETBlocks);

                alignmentData[i].ResidualData.massError.CopyTo(mergedData.ResidualData.massError, copyMassBlocks);
                alignmentData[i].ResidualData.massErrorCorrected.CopyTo(mergedData.ResidualData.massErrorCorrected, copyMassBlocks);
                alignmentData[i].ResidualData.mzMassError.CopyTo(mergedData.ResidualData.mzMassError, copyMassBlocks);
                alignmentData[i].ResidualData.mzMassErrorCorrected.CopyTo(mergedData.ResidualData.mzMassErrorCorrected, copyMassBlocks);
                alignmentData[i].ResidualData.mz.CopyTo(mergedData.ResidualData.mz, copyMassBlocks);

                copyNETBlocks += alignmentData[i].ResidualData.scans.Length;
                copyMassBlocks += alignmentData[i].ResidualData.mz.Length;

                mergedData.MassMean                 = alignmentData[i].MassMean;
                mergedData.MassStandardDeviation    = alignmentData[i].MassStandardDeviation; 
                
                mergedData.NETIntercept         = alignmentData[i].MassStandardDeviation;
                mergedData.NETMean              = alignmentData[i].NETMean;
                mergedData.NETStandardDeviation = alignmentData[i].NETStandardDeviation;
                mergedData.NETRsquared          = alignmentData[i].NETRsquared;
                mergedData.NETSlope             = alignmentData[i].NETSlope;

                if (i > 0)
                {
                    MergeHistogramData(netErrorHistogramData, alignmentData[i].netErrorHistogram, true);
                }
            }
            
            // Grab the heat scores!
            mergedData.heatScores           = alignmentData[alignmentData.Count - 1].heatScores;
            mergedData.massErrorHistogram   = massErrorHistogramData;
            mergedData.netErrorHistogram    = netErrorHistogramData;
            
            mergedData.driftErrorHistogram  = driftErrorHistograms[0];
            mergedData.alignmentFunction    = mergedAlignmentFunction;

            alignmentProcessor.Dispose();

            return mergedData;
        }
        /// <summary>
        /// Merges the histogram data leaving the result in old.
        /// </summary>
        /// <param name="histogramOld">Data to retain merged data.</param>
        /// <param name="histogramNew">Data to copy.</param>
        /// <param name="checkClosestBin">Flag indicating whether to use the closest bin or to just assume that the x values match between dest and src.</param>
        private void MergeHistogramData(double[,] histogramDest, 
                                        double[,] histogramSource, 
                                        bool checkClosestBin)
        {
            for (int i = 0; i < histogramSource.GetLength(0) && i < histogramDest.GetLength(0); i++)
            {
                int bestIndex = 0;
                double massDiff = double.MaxValue;

                if (checkClosestBin == false)
                {
                    bestIndex = i;
                    histogramDest[i, 0] = histogramSource[bestIndex, 0];
                }
                else
                {
                    int length = Math.Min(histogramDest.GetLength(0), histogramSource.GetLength(0));

                    /// 
                    /// Find the best mass item if the previous mass items are skewed or changed
                    /// 
                    for (int j = 0; j < length; j++)
                    {
                        double diff = Math.Abs(histogramDest[j, 0] - histogramSource[j, 0]);
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
        /// Aligns the clusters to the mass tag database
        /// </summary>
        /// <param name="massTagDatabase">Mass tag databaset to align to.</param>
        /// <param name="clusters">Clusters to align.</param>
        /// <param name="options">Alignment options.</param>
        /// <returns>Alignment data for the clusters to mass tag database.</returns>
        public classAlignmentData AlignFeatures(MassTagDatabase         massTagDatabase,
                                                List<clsCluster>        clusters,
                                                AlignmentOptions     options)
        {

            OnStatus("Starting alignment of clusters.");
            clsAlignmentProcessor alignmentProcessor = new clsAlignmentProcessor();
            List<clsMassTag> tags                    = new List<clsMassTag>();

            foreach (MassTagLight tag in massTagDatabase.MassTags)
            {
                // mixed mode tag
                clsMassTag mmTag        = new clsMassTag();
                mmTag.mintMassTagId     = tag.ID;
                mmTag.mintConformerID   = tag.ConformationID;
                mmTag.DriftTime         = tag.DriftTime;
                mmTag.mdblAvgGANET      = tag.NETAverage;
                mmTag.mdblMonoMass      = tag.MassMonoisotopic;
                tags.Add(mmTag);
            }

            alignmentProcessor.SetReferenceDatasetFeatures(tags, true);

            OnStatus("Setting alignee clusters.");
            alignmentProcessor.SetAligneeDatasetFeatures(clusters, options.MZBoundaries[0]);

            OnStatus("Performing alignment warping.");
            alignmentProcessor.PerformAlignmentToMassTagDatabase();

            OnStatus("Applying alignment function to all features.");
            alignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref clusters); 
            clsAlignmentFunction alignmentFunction = alignmentProcessor.GetAlignmentFunction();


            OnStatus("Retrieving alignment data.");
            // Heat maps
            float[,] heatScores = new float[1, 1];
            float[] xIntervals  = new float[1];
            float[] yIntervals  = new float[1];
            alignmentProcessor.GetAlignmentHeatMap(ref heatScores, ref xIntervals, ref yIntervals);


            float minMTDBNET    = 0.0F, maxMTDBNET = 1.0F;
            alignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET);


            // Residuals
            float[,] linearNet = new float[1, 1];
            float[,] customNet = new float[1, 1];
            float[,] linearCustomNet        = new float[1, 1];
            float[,] massError              = new float[1, 1];
            float[,] massErrorCorrected     = new float[1, 1];
            float[,] mzMassError            = new float[1, 1];
            float[,] mzMassErrorCorrected   = new float[1, 1];
            classAlignmentResidualData residualData = alignmentProcessor.GetResidualData();

            // Get error histograms 
            double[,] netErrorHistogram     = new double[1, 1];
            double[,] massErrorHistogram    = new double[1, 1];
            double[,] driftErrorHistogram   = new double[1, 1];
            alignmentProcessor.GetErrorHistograms(options.MassBinSize,
                                                  options.NETBinSize,
                                                  options.DriftTimeBinSize,
                                                  ref massErrorHistogram,
                                                  ref netErrorHistogram,
                                                  ref driftErrorHistogram);

            classAlignmentData  clusterAlignmentData    = new classAlignmentData();
            clusterAlignmentData.alignmentFunction      = alignmentFunction;
            clusterAlignmentData.heatScores             = heatScores;
            clusterAlignmentData.massErrorHistogram     = massErrorHistogram;
            clusterAlignmentData.netErrorHistogram      = netErrorHistogram;
            clusterAlignmentData.driftErrorHistogram    = driftErrorHistogram;
            clusterAlignmentData.NETIntercept           = alignmentProcessor.NETIntercept;
            clusterAlignmentData.NETRsquared            = alignmentProcessor.NETLinearRSquared;
            clusterAlignmentData.NETSlope               = alignmentProcessor.NETSlope;
            clusterAlignmentData.ResidualData           = residualData;
            clusterAlignmentData.MassMean               = alignmentProcessor.GetMassMean();
            clusterAlignmentData.MassStandardDeviation  = alignmentProcessor.GetMassMean();
            clusterAlignmentData.NETMean                = alignmentProcessor.GetNETMean();
            clusterAlignmentData.NETStandardDeviation   = alignmentProcessor.GetNETStandardDeviation();

            return clusterAlignmentData;
        }

    }
}
