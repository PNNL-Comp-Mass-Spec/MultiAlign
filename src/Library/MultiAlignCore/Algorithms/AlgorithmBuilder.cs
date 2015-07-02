#region

using System;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Algorithms.Options;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.FeatureMatcher.Data;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    ///     Builds the set of algorithms using the builder design pattern.
    /// </summary>
    public class AlgorithmBuilder
    {
        /// <summary>
        ///     Final provider.
        /// </summary>
        private readonly AlgorithmProvider m_provider;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public AlgorithmBuilder()
        {
            m_provider = new AlgorithmProvider();
        }

        /// <summary>
        ///     Builds the algorithm types.
        /// </summary>
        /// <param name="clusterType"></param>
        /// <returns></returns>
        public void BuildClusterer(LcmsFeatureClusteringAlgorithmType clusterType)
        {
            m_provider.Clusterer = ClusterFactory.Create(clusterType);
        }

        /// <summary>
        ///     Builds the feature aligner.
        /// </summary>
        public void BuildAligner(LcmsWarpAlignmentOptions options, SpectralOptions spectralOptions)
        {
            m_provider.DatasetAligner = FeatureAlignerFactory.CreateDatasetAligner(options.AlignmentAlgorithmType, options,
                spectralOptions);

            m_provider.DatabaseAligner = FeatureAlignerFactory.CreateDatabaseAligner(options.AlignmentAlgorithmType, options,
                spectralOptions);
        }

        /// <summary>
        ///     Builds a peak matcher object.
        /// </summary>
        public void BuildPeakMatcher(MultiAlignAnalysisOptions options)
        {
            var tolerances = new FeatureMatcherTolerances();
            var stanleyMatcher = new STACAdapter<UMCClusterLight>
            {
                Options =
                {
                    HistogramBinWidth = options.StacOptions.HistogramBinWidth,
                    HistogramMultiplier = options.StacOptions.HistogramMultiplier,
                    ShiftAmount = options.StacOptions.ShiftAmount,
                    ShouldCalculateHistogramFDR = options.StacOptions.ShouldCalculateHistogramFDR,
                    ShouldCalculateShiftFDR = options.StacOptions.ShouldCalculateShiftFDR,
                    ShouldCalculateSLiC = options.StacOptions.ShouldCalculateSLiC,
                    ShouldCalculateSTAC = options.StacOptions.ShouldCalculateSTAC,
                    UseDriftTime = options.StacOptions.UseDriftTime,
                    UseEllipsoid = options.StacOptions.UseEllipsoid,
                    UsePriors = options.StacOptions.UsePriors
                }
            };
            tolerances.DriftTimeTolerance = Convert.ToSingle(options.StacOptions.DriftTimeTolerance);
            tolerances.MassTolerancePPM = options.StacOptions.MassTolerancePPM;
            tolerances.NETTolerance = options.StacOptions.NETTolerance;
            tolerances.Refined = options.StacOptions.Refined;
            stanleyMatcher.Options.UserTolerances = tolerances;
            m_provider.PeakMatcher = stanleyMatcher;
        }

        /// <summary>
        ///     Returns the list of algorithms post build.
        /// </summary>
        /// <returns></returns>
        public AlgorithmProvider GetAlgorithmProvider(MultiAlignAnalysisOptions options)
        {
            BuildClusterer(LcmsFeatureClusteringAlgorithmType.AverageLinkage);
            BuildAligner(options.AlignmentOptions.LCMSWarpOptions, options.SpectralOptions);
            BuildPeakMatcher(options);
            return m_provider;
        }
    }
}