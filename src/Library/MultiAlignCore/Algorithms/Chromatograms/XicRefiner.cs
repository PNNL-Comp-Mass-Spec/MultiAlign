namespace MultiAlignCore.Algorithms.Chromatograms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using InformedProteomics.Backend.Data.Spectrometry;
    using InformedProteomics.Backend.Utils;
    using MultiAlignCore.Data;

    /// <summary>
    /// This class performs refinement on XICs by smoothing and snipping peak tails.
    /// </summary>
    public class XicRefiner
    {
        /// <summary>
        /// Default polynomial order for SavitzkyGolay smoother.
        /// </summary>
        private const int ConstPolynomialOrder = 3;

        /// <summary>
        /// Default window size for SavitzkyGolay smoother.
        /// </summary>
        private const int NumberOfPoints = 5;

        /// <summary>
        /// Smoother to smooth Xics with.
        /// </summary>
        private readonly SpectralProcessing.SavitzkyGolaySmoother smoother;

        /// <summary>
        /// Initializes new instance of the <see cref="XicRefiner" /> class.
        /// </summary>
        /// <param name="relativeIntensityThreshold">The relative intensity threshold for snipping XIC tails</param>
        /// <param name="smoother">Savitzky-Golay smoother to use for smoothing the XICs.</param>
        public XicRefiner(double relativeIntensityThreshold = 0.05, SpectralProcessing.SavitzkyGolaySmoother smoother = null)
        {
            this.RelativeIntensityThreshold = relativeIntensityThreshold;
            this.smoother = smoother ?? new SpectralProcessing.SavitzkyGolaySmoother(NumberOfPoints, ConstPolynomialOrder, false);
        }

        /// <summary>
        /// Gets or sets the relative intensity threshold for snipping XIC tails.
        /// </summary>
        public double RelativeIntensityThreshold { get; set; }

        /// <summary>
        /// Refinement includes smoothing the XICs with a Savitzky-Golay smoother,
        /// and snipping points off of peaks that are less than a given percentage
        /// (<see cref="RelativeIntensityThreshold" />) of the peak apex.
        /// </summary>
        /// <param name="xics">The XICs to refine.</param>
        /// <param name="progress">Progress reporter object.</param>
        /// <returns>List of refined XICs.</returns>
        public List<Xic> RefineXics(IList<Xic> xics, IProgress<ProgressData> progress = null)
        {
            var progressData = new ProgressData(progress);
            var refinedXics = new List<Xic> { Capacity = xics.Count };
            for (int i = 0; i < xics.Count; i++)
            {
                refinedXics.Add(this.RefineXic(xics[i]));
                progressData.Report(i, xics.Count);
            }

            return refinedXics;
        }

        /// <summary>
        /// Smooth and snip tails off a single XIC.
        /// </summary>
        /// <param name="xic">The XIC to refine.</param>
        /// <returns>Refined XIC.</returns>
        public Xic RefineXic(Xic xic)
        {
            // Here we smooth the points...and remove any features with from and trailing zero points
            if (xic.Count == 0)
            {
                return xic;
            }

            var unsmoothedPoints = xic.Select(xicp => new XYData(xicp.ScanNum, xicp.Intensity))
                                      .OrderBy(p => p.X).ToList(); 
            var points = this.smoother.Smooth(unsmoothedPoints);

            // Find the biggest peak...
            var maxScanIndex = 0;
            double maxAbundance = 0;
            for (var i = 0; i < xic.Count; i++)
            {
                if (maxAbundance < xic[i].Intensity)
                {
                    maxScanIndex = i;
                    maxAbundance = xic[i].Intensity;
                }
            }

            // Then find when the feature goes to zero
            // Start from max to left                        
            var startIndex = maxScanIndex;

            // If we hit zero, then keep
            for (; startIndex > 0; startIndex--)
            {
                if ((xic[startIndex].Intensity / maxAbundance) < this.RelativeIntensityThreshold)
                    break;
            }

            // Start from max to right
            var stopIndex = maxScanIndex;
            for (; stopIndex < xic.Count - 1; stopIndex++)
            {
                if ((xic[stopIndex].Intensity / maxAbundance) < this.RelativeIntensityThreshold)
                    break;
            }

            // Add the features back
            for (var i = startIndex; i <= stopIndex; i++)
            {
                xic[i] = new XicPoint(Convert.ToInt32(xic[i].ScanNum), xic[i].Mz, Convert.ToInt64(points[i].Y));
            }

            return xic;
        }
    }
}
