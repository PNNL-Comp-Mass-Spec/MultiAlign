namespace MultiAlignCore.Algorithms.Chromatograms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        /// Initializes a new instance of the <see cref="XicCreator" /> class.
        /// </summary>
        /// <param name="xicRefiner">Refiner to use.</param>
        public XicCreator(XicRefiner xicRefiner = null)
        {
            this.XicRefiner = xicRefiner ?? new XicRefiner();
        }

        /// <summary>
        /// Gets or sets a value indicating whether refinement (smoothing and tail snipping)
        /// should be run on the XICs.
        /// </summary>
        public bool ShouldRefine { get; set; }

        /// <summary>
        /// Gets or sets the mass error in PPM for the 
        /// </summary>
        public double MassError { get; set; }

        /// <summary>
        /// Gets the XIC refiner.
        /// </summary>
        public XicRefiner XicRefiner { get; private set; }

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
                var xic = ipr.GetPrecursorExtractedIonChromatogram(lowMz, highMz, targetScan);

                if (this.ShouldRefine)
                {
                    var xicRefiner = this.XicRefiner ?? new XicRefiner();
                    xic = xicRefiner.RefineXic(xic);
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
            this.ShouldRefine = true;
            this.XicRefiner.RelativeIntensityThreshold = 0.1;
        }
    }

}
