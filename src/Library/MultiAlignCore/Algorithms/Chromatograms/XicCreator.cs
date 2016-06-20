namespace MultiAlignCore.Algorithms.Chromatograms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using InformedProteomics.Backend.Data.Spectrometry;
    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.RawData;

    /// <summary>
    /// This class is a MultiAlign wrapper for the InformedProteomics XIC extractor.
    /// </summary>
    public class XicCreator : ISettingsContainer
    {
        /// <summary>
        /// Smoother to smooth Xics with.
        /// </summary>
        private SpectralProcessing.SavitzkyGolaySmoother smoother;

        /// <summary>
        /// Gets or sets the number scans to pad the feature scan range by when extracting the XIC.
        /// </summary>
        public int ScanTolerance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether refinement (smoothing and tail snipping)
        /// should be run on the XICs.
        /// </summary>
        public bool ShouldSmooth { get; set; }

        /// <summary>
        /// Gets or sets the polynomial order for Savitzky-Golay smoother.
        /// </summary>
        public int SmoothingPolynomialOrder { get; set; }

        /// <summary>
        /// Gets or sets the smoothing window size for Savitzky-Golay smoother.
        /// </summary>
        public int SmoothingWindowSize { get; set; }

        /// <summary>
        /// Gets or sets the intensity threshold relative to the highest peak to snip tails at.
        /// </summary>
        public double RelativeIntensityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the mass error in PPM.
        /// </summary>
        public double MassError { get; set; }

        /// <summary>
        /// Create a XICs for LCMS features.
        /// </summary>
        /// <param name="features">The features to create XICs for.</param>
        /// <param name="provider">The InformedProteomics XIC extractor.</param>
        /// <param name="progress"></param>
        /// <returns>The features with XICs filled in as MSFeatures.</returns>
        public IEnumerable<UMCLight> CreateXic(
            List<UMCLight> features,
            InformedProteomicsReader provider,
            IProgress<ProgressData> progress = null)
        {
            var progressData = new ProgressData(progress);
            int id = 0, count = 0;
            int msmsFeatureId = 0;
            var resultFeatures = new List<UMCLight> { Capacity = features.Count };
            var ipr = provider.LcMsRun;

            ipr.HigherPrecursorChromatogramCacheSize = 2000;
            
            features.Sort((x,y) => x.Mz.CompareTo(y.Mz));

            // Iterate over XIC targets.
            foreach (var feature in features)
            {
                count++;

                // Get target M/z and scan
                var targetScan = this.GetTargetScanNum(feature);
                var highMz = FeatureLight.ComputeDaDifferenceFromPPM(feature.Mz, -this.MassError);
                var lowMz = FeatureLight.ComputeDaDifferenceFromPPM(feature.Mz, this.MassError);

                // Read XIC
                var xic = ipr.GetPrecursorExtractedIonChromatogram(lowMz, highMz, targetScan, this.ScanTolerance);

                if (this.ShouldSmooth)
                {
                    xic = this.RefineXic(xic);
                }

                if (xic.Count < 3)
                {
                    continue;
                }

                var minEt = ipr.GetElutionTime(ipr.MinLcScan);
                var maxEt = ipr.GetElutionTime(ipr.MaxLcScan);
                var diffEt = maxEt - minEt;

                // Add xic points as MSFeatures.
                feature.MsFeatures.Clear();
                foreach (var point in xic)
                {
                    feature.AddChildFeature(new MSFeatureLight
                    {
                        ChargeState = feature.ChargeState,
                        Mz = feature.Mz,
                        MassMonoisotopic = feature.MassMonoisotopic,
                        Scan = point.ScanNum,
                        Abundance = Convert.ToInt64(point.Intensity),
                        Id = id++,
                        DriftTime = feature.DriftTime,
                        Net = (ipr.GetElutionTime(point.ScanNum) - minEt) / diffEt,
                        GroupId = feature.GroupId
                    });
                }

                // Associate MS/MS information.
                var ms2Scans = ipr.GetFragmentationSpectraScanNums(feature.Mz).ToArray();
                int j = 0;
                for (int i = 0; i < feature.MsFeatures.Count; i++)
                {
                    for (; j < ms2Scans.Length; j++)
                    {
                        // Scan below UMC feature scan range.
                        if (ms2Scans[j] < feature.MsFeatures[i].Scan)
                        {
                            break;
                        }

                        // Haven't reached the last ms2 scan and ms2 scan is larger than next feature, could be associated with next feature
                        if (i < feature.MsFeatures.Count - 1 && ms2Scans[j] > feature.MsFeatures[i + 1].Scan)
                        {
                            break;
                        }

                        // We're on the last MSFeature - is the MS/MS scan actually for this feature?
                        if (i == feature.MsFeatures.Count - 1 && 
                            ipr.GetPrevScanNum(ms2Scans[j], 1) != feature.MsFeatures[i].Scan)
                        {
                            continue;
                        }

                        // Otherwise this is a MS/MS we want to add!
                        var spectraData = new MSSpectra
                        {
                            Id = msmsFeatureId++,
                            ScanMetaData = new ScanSummary
                            {
                                MsLevel = 2,
                                Scan = ms2Scans[j],
                                PrecursorMz = feature.MsFeatures[i].Mz,
                            },
                            CollisionType = CollisionType.None,
                            Scan = ms2Scans[j],
                            PrecursorMz = feature.MsFeatures[i].Mz
                        };
                        feature.MsFeatures[i].MSnSpectra.Add(spectraData);
                    }
                }

                resultFeatures.Add(feature);
                if (count % 100 == 0 || count == features.Count - 1)
                {
                    progressData.Report(count, features.Count);
                }
            }

            return resultFeatures;
        }

        /// <summary>
        /// Gets the target scan number. We attempt to set the target at
        /// the apex of the peak.
        /// </summary>
        /// <param name="feature">The feature to find the target scan number for.</param>
        /// <returns>The target scan number.</returns>
        public int GetTargetScanNum(UMCLight feature)
        {
            int maxAbundance = 0;
            int targetScan = 0;
            foreach (var msFeature in feature.MsFeatures)
            {
                if (msFeature.Abundance >= maxAbundance)
                {
                    targetScan = msFeature.Scan;
                }
            }

            return targetScan;
        }

        /// <summary>
        /// Restore settings back to their default values.
        /// </summary>
        public void RestoreDefaults()
        {
            this.MassError = 10;
            this.ShouldSmooth = true;
            this.RelativeIntensityThreshold = 0;
            this.ScanTolerance = 3;
        }

        /// <summary>
        /// This method smooths the XIC using the Savitzky-Golay smoothing algorithm
        /// and snips the tails on the XIC based on where the relative intensity 
        /// falls below <see cref="RelativeIntensityThreshold" />.
        /// </summary>
        /// <param name="xic">The XIC to smooth.</param>
        /// <returns>The smoothed and snipped XIC.</returns>
        private Xic RefineXic(Xic xic)
        {
            if (xic.Count == 0)
            {
                return xic;
            }


            if (this.smoother == null)
            {   // Create the smoother if it hasn't been created yet.
                this.smoother = new SpectralProcessing.SavitzkyGolaySmoother(this.SmoothingWindowSize, this.SmoothingPolynomialOrder, false);
            }

            // Here we smooth the points...and remove any features with from and trailing zero points
            var unsmoothedPoints = xic.Select(xicp => new XYData(xicp.ScanNum, xicp.Intensity))
                                      .OrderBy(p => p.X).ToList();
            var points = this.smoother.Smooth(unsmoothedPoints);

            // Snip peak tails.
            if (this.RelativeIntensityThreshold > 0)
            {
                points = this.SnipPeakTails(points);
            }

            var refinedXic = new Xic();
            foreach (var point in points)
            {
                refinedXic.Add(new XicPoint((int)point.X, xic[0].Mz, point.Y));
            }

            return refinedXic;
        }

        private List<XYData> SnipPeakTails(List<XYData> xic)
        {
            // Find the biggest peak...
            var maxScanIndex = 0;
            double maxAbundance = 0;
            for (var i = 0; i < xic.Count; i++)
            {
                if (maxAbundance < xic[i].Y)
                {
                    maxScanIndex = i;
                    maxAbundance = xic[i].Y;
                }
            }

            // Then find when the feature goes to zero
            // Start from max to left                        
            var startIndex = maxScanIndex;

            // If we hit zero, then keep
            for (; startIndex > 0; startIndex--)
            {
                if ((xic[startIndex].Y / maxAbundance) < this.RelativeIntensityThreshold)
                    break;
            }

            // Start from max to right
            var stopIndex = maxScanIndex;
            for (; stopIndex < xic.Count - 1; stopIndex++)
            {
                if ((xic[stopIndex].Y / maxAbundance) < this.RelativeIntensityThreshold)
                    break;
            }

            // Add the features back
            var snippedXic = new List<XYData>();
            for (var i = startIndex; i <= stopIndex; i++)
            {
                snippedXic.Add(new XYData(Convert.ToInt32(xic[i].X), Convert.ToInt64(xic[i].Y)));
            }

            return snippedXic;
        } 
    }

}
