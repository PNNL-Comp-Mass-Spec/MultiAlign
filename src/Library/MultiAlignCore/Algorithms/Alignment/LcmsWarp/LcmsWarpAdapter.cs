using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{

    /// <summary>
    /// Object which performs feature alignment through LCMSWarp
    /// </summary>
    public sealed class LcmsWarpAdapter :  
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, LcmsWarpAlignmentData>,
        IFeatureAligner<IEnumerable<MassTagLight>, IEnumerable<UMCLight>, LcmsWarpAlignmentData>
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        private readonly LcmsWarpAlignmentOptions m_options;

        public LcmsWarpAdapter()
            : this(new LcmsWarpAlignmentOptions())
        {
        }

        public LcmsWarpAdapter(LcmsWarpAlignmentOptions options)
        {
            m_options = options;
        }

        /// <summary>
        /// Gets or sets the baseline spectra provider
        /// </summary>
        public ISpectraProvider BaselineSpectraProvider { get; set; }
        /// <summary>
        /// Gets or sets the alignee spectra provider.
        /// </summary>
        public ISpectraProvider AligneeSpectraProvider { get; set; }

        /// <summary>
        /// Method to align two UMCLight Enumerables, the first parameter
        /// used as the baseline to align the second parameter
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        public LcmsWarpAlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> features, IProgress<ProgressData> progress = null)
        {

            return AlignFeatures(baseline as List<UMCLight>, features as List<UMCLight>, m_options);
        }

        /// <summary>
        /// Method to align a UMCLight Enumerable to a MassTagLight enumberable.
        /// The MassTagLight Enumberable is used as the baseline to align the
        /// UMCLight enumberable.
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        public LcmsWarpAlignmentData Align(IEnumerable<MassTagLight> baseline, IEnumerable<UMCLight> features, IProgress<ProgressData> progress = null)
        {
            return AlignFeatures(baseline as List<MassTagLight>, features as List<UMCLight>, m_options);
        }
    
        /// <summary>
        /// Uses a list of MassTagLights as a baseline for aligning a list of UMCLights to it, using
        /// the passed in options for processor options as well as a boolean for whether to align based on
        /// Drift Times as well.
        /// Creates an LCMS Alignment processor to do the alignment, does the alignment, and then passes
        /// back the alignment data to the caller.
        /// </summary>
        /// <param name="massTags"></param>
        /// <param name="aligneeFeatures"></param>
        /// <param name="options"></param>        
        /// <returns></returns>
        private LcmsWarpAlignmentData AlignFeatures(
            List<MassTagLight> massTags, 
            List<UMCLight> aligneeFeatures,
            LcmsWarpAlignmentOptions options)
        {
            var alignmentProcessor = new LcmsWarpAlignmentProcessor
            {
                Options = options
            };
            alignmentProcessor.ApplyAlignmentOptions();
            alignmentProcessor.Progress += alignmentProcessor_Progress;

            var featureTest = aligneeFeatures.Find(x => x.DriftTime > 0);
            var massTagTest = massTags.Find(x => x.DriftTime > 0);

            if (featureTest != null && massTagTest == null)
            {
                Console.WriteLine("Warning! Data has drift time info, but the mass tags do not.");
            }

            alignmentProcessor.SetReferenceDatasetFeatures(massTags);

            var data = AlignFeatures(alignmentProcessor, aligneeFeatures, options);

            return data;
        }

        void alignmentProcessor_Progress(object sender, ProgressNotifierArgs e)
        {
            OnProgress(e);
        }

        private void OnProgress(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }

        private void OnProgress(ProgressNotifierArgs e)
        {
            if (Progress != null)
            {
                Progress(this, e);
            }
        }

        /// <summary>
        /// Uses a list of UMCLights as a baseline for aligning a second list of UMCLights to it, using
        /// the passed in options for processor options
        /// Creates an LCMS Alignment processor to do the alignment, does the alignment, and then passes
        /// back the alignment data to the caller.
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="aligneeFeatures"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private LcmsWarpAlignmentData AlignFeatures(List<UMCLight> baseline, List<UMCLight> aligneeFeatures, LcmsWarpAlignmentOptions options)
        {
            var alignmentProcessor = new LcmsWarpAlignmentProcessor
            {
                Options = options
            };
            alignmentProcessor.ApplyAlignmentOptions();
            alignmentProcessor.Progress += alignmentProcessor_Progress;

            var filteredBaselineFeatures = FilterFeaturesByAbundance(baseline, options) as List<UMCLight>;

            alignmentProcessor.SetReferenceDatasetFeatures(filteredBaselineFeatures);

            return AlignFeatures(alignmentProcessor, aligneeFeatures, options);            
        }

        private LcmsWarpAlignmentData AlignFeatures(LcmsWarpAlignmentProcessor processor, List<UMCLight> aligneeFeatures, LcmsWarpAlignmentOptions options)
        {
            var alignmentFunctions = new List<LcmsWarpAlignmentFunction>();
            
            var heatScores = new List<double[,]>();
            var xIntervals = new List<double[]>();
            var yIntervals = new List<double[]>();

            double minMtdbNet;
            double maxMtdbNet;
            processor.GetReferenceNetRange(out minMtdbNet, out maxMtdbNet);

            var filteredFeatures = FilterFeaturesByAbundance(aligneeFeatures, options).ToList();            
                       
            // Set features
            processor.SetAligneeDatasetFeatures(filteredFeatures);

            // Find alignment
            processor.PerformAlignmentToMsFeatures();

            // Extract alignment function
            var alignmentFunction = processor.GetAlignmentFunction();
            alignmentFunctions.Add(alignmentFunction);

            // Correct the features
            processor.ApplyNetMassFunctionToAligneeDatasetFeatures(ref aligneeFeatures);
                        
            // Get the heat maps
            double[,] heatScore;
            double[] xInterval;
            double[] yInterval;

            processor.GetAlignmentHeatMap(out heatScore, out xInterval, out yInterval);
            xIntervals.Add(xInterval);
            yIntervals.Add(yInterval);
            heatScores.Add(heatScore);

            // Get the histograms
            double[,] massErrorHistogram;
            double[,] netErrorHistogram;
            double[,] driftErrorHistogram;

            processor.GetErrorHistograms(options.MassBinSize, options.NetBinSize, options.DriftTimeBinSize,
                out massErrorHistogram, out netErrorHistogram, out driftErrorHistogram);
            
            // Get the residual data
            var residualData = processor.GetResidualData();

            var data = new LcmsWarpAlignmentData
            {
                MassErrorHistogram      = massErrorHistogram,
                DriftErrorHistogram     = driftErrorHistogram,
                NetErrorHistogram       = netErrorHistogram,
                AlignmentFunction       = alignmentFunction,
                HeatScores              = heatScore,
                NetIntercept            = processor.NetIntercept,
                NetRsquared             = processor.NetRsquared,
                NetSlope                = processor.NetSlope,
                ResidualData            = residualData,
                MassMean                = processor.MassMu,
                MassStandardDeviation   = processor.MassStd,
                NetMean                 = processor.NetMu,
                NetStandardDeviation    = processor.NetStd
            };
            
            return data;
        }

        private static IEnumerable<UMCLight> FilterFeaturesByAbundance(List<UMCLight> features, LcmsWarpAlignmentOptions options)
        {
            // Sort by abundance to ease filtering process. Options look at the percentage of abundance
            // so threshhold needs to be converted to what the abundance sum would be. 
            features.Sort((x, y) => x.AbundanceSum.CompareTo(y.AbundanceSum));

            var percent = 1 - (options.TopFeatureAbundancePercent / 100);
            var total = features.Count - Convert.ToInt32(features.Count * percent);
            var threshhold = features[Math.Min(features.Count - 1, Math.Max(0, total))].AbundanceSum;
            
            // Re-sort with regards to monoisotopic mass for accurate application of NET-Mass function
            // to the dataset we need to align.
            features.Sort((x, y) => x.MassMonoisotopic.CompareTo(y.MassMonoisotopic));

            if (threshhold <= 0)
                return features;

            //Filters features below the threshold
            var filteredFeatures = features.Where(feature => feature.AbundanceSum >= threshhold);
            return filteredFeatures;
        }
    }
}
