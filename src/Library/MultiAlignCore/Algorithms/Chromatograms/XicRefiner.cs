using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Chromatograms
{
    public class XicRefiner
    {
        /// <summary>
        /// Default polynomial order for SavitzkyGolay smoother.
        /// </summary>
        private const int CONST_POLYNOMIAL_ORDER = 3;

        /// <summary>
        /// Default window size for SavitzkyGolay smoother.
        /// </summary>
        private const int NUMBER_OF_POINTS = 5;

        /// <summary>
        /// Smoother to smooth Xics with.
        /// </summary>
        private readonly SavitzkyGolaySmoother smoother;

        /// <summary>
        /// Relative intensity threshold for snipping XICs.
        /// </summary>
        private double relativeIntensityThreshold;

        public XicRefiner(double relativeIntensityThreshold = 0.05, SavitzkyGolaySmoother smoother = null)
        {
            this.relativeIntensityThreshold = relativeIntensityThreshold;
            this.smoother = smoother ?? new SavitzkyGolaySmoother(CONST_POLYNOMIAL_ORDER, NUMBER_OF_POINTS);
        }

        /// <summary>
        /// Smooth and snip tails off of a collection of XICs.
        /// </summary>
        /// <param name="xics">The XICs to refine.</param>
        /// <param name="progress">Progress reporter object.</param>
        /// <returns>List of refined XICs.</returns>
        public List<Xic> RefineXics(IList<Xic> xics, IProgress<ProgressData> progress = null)
        {
            progress = progress ?? new Progress<ProgressData>();
            var progressData = new ProgressData();
            var refinedXics = new List<Xic> { Capacity = xics.Count };
            for (int i = 0; i < xics.Count; i++)
            {
                refinedXics.Add(RefineXic(xics[i]));
                progressData.Percent = (i / xics.Count) * 100;
                progress.Report(progressData);
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
                return xic;

            var unsmoothedPoints = xic.Select(x => x.Intensity).ToArray(); 
            var points = smoother.Smooth(unsmoothedPoints);

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
                if ((xic[startIndex].Intensity / maxAbundance) < this.relativeIntensityThreshold)
                    break;
            }

            // Start from max to right
            var stopIndex = maxScanIndex;
            for (; stopIndex < xic.Count - 1; stopIndex++)
            {
                if ((xic[stopIndex].Intensity / maxAbundance) < this.relativeIntensityThreshold)
                    break;
            }

            // Add the features back
            for (var i = startIndex; i <= stopIndex; i++)
            {
                points[i] = Convert.ToInt64(points[i]);
                xic[i] = new XicPoint(Convert.ToInt32(xic[i].ScanNum), xic[i].Mz, points[i]);
            }

            return xic;
        }
    }
}
