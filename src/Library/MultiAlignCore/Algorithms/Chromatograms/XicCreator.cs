using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.RawData;
using SavitzkyGolaySmoother = MultiAlignCore.Algorithms.SpectralProcessing.SavitzkyGolaySmoother;

namespace MultiAlignCore.Algorithms.Chromatograms
{
    public class XicCreator
    {
        private const int CONST_POLYNOMIAL_ORDER = 3;

        public XicCreator()
        {
            ScanWindowSize = 100;
            FragmentationSizeWindow = .5;
            NumberOfPoints = 5;
            this.XicRefiner = new XicRefiner();
        }

        public XicRefiner XicRefiner { get; set; }

        public IEnumerable<UMCLight> CreateXicNew(List<UMCLight> features,
            double massError,
            InformedProteomicsReader provider,
            bool refine = true,
            IProgress<ProgressData> progress = null)
        {
            var progressData = new ProgressData(progress);
            int id = 0, count = 0;
            int msmsFeatureId = 0;
            var resultFeatures = new List<UMCLight> { Capacity = features.Count };
            var ipr = provider.GetReaderForGroup(0);

            ipr.HigherPrecursorChromatogramCacheSize = 2000;
            
            features.Sort((x,y) => x.Mz.CompareTo(y.Mz));

            // Iterate over XIC targets.
            foreach (var xicTarget in CreateXicTargetsYield(features, massError))
            {
                count++;
                // Read XIC
                var target = xicTarget.StartScan + ((xicTarget.EndScan - xicTarget.StartScan) / 2);
                var xic = ipr.GetPrecursorExtractedIonChromatogram(xicTarget.LowMz, xicTarget.HighMz, target);

                if (refine)
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
                xicTarget.Feature.MsFeatures.Clear();
                foreach (var point in xic)
                {
                    xicTarget.Feature.AddChildFeature(new MSFeatureLight
                    {
                        ChargeState = xicTarget.ChargeState,
                        Mz = xicTarget.Mz,
                        MassMonoisotopic = xicTarget.Feature.MassMonoisotopic,
                        Scan = point.ScanNum,
                        Abundance = Convert.ToInt64(point.Intensity),
                        Id = id++,
                        DriftTime = xicTarget.Feature.DriftTime,
                        Net = (ipr.GetElutionTime(point.ScanNum) - minEt) / diffEt,
                        GroupId = xicTarget.Feature.GroupId
                    });
                }

                // Associate MS/MS information.
                var ms2Scans = ipr.GetFragmentationSpectraScanNums(xicTarget.Feature.Mz).ToArray();
                int j = 0;
                for (int i = 0; i < xicTarget.Feature.MsFeatures.Count; i++)
                {
                    for (; j < ms2Scans.Length; j++)
                    {
                        // Scan below UMC feature scan range.
                        if (ms2Scans[j] < xicTarget.Feature.MsFeatures[i].Scan)
                        {
                            break;
                        }

                        // Haven't reached the last ms2 scan and ms2 scan is larger than next feature, could be associated with next feature
                        if (i < xicTarget.Feature.MsFeatures.Count - 1 && ms2Scans[j] > xicTarget.Feature.MsFeatures[i + 1].Scan)
                        {
                            break;
                        }

                        // We're on the last MSFeature - is the MS/MS scan actually for this feature?
                        if (i == xicTarget.Feature.MsFeatures.Count - 1 && 
                            ipr.GetPrevScanNum(ms2Scans[j], 1) != xicTarget.Feature.MsFeatures[i].Scan)
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
                                PrecursorMz = xicTarget.Feature.MsFeatures[i].Mz,
                            },
                            CollisionType = CollisionType.None,
                            Scan = ms2Scans[j],
                            PrecursorMz = xicTarget.Feature.MsFeatures[i].Mz
                        };
                        xicTarget.Feature.MsFeatures[i].MSnSpectra.Add(spectraData);
                    }
                }

                resultFeatures.Add(xicTarget.Feature);
                if (count%100 == 0 || count == features.Count - 1)
                {
                    progressData.Report(count, features.Count);
                }
            }

            return resultFeatures;
        }

        private IEnumerable<XicFeature> CreateXicTargetsYield(IEnumerable<UMCLight> features, double massError)
        {
            int id = 0;
            foreach (var feature in features)
            {
                int minScan = Int32.MaxValue;
                int maxScan = 0;
                foreach (var msFeature in feature.MsFeatures)
                {
                    minScan = Math.Min(minScan, msFeature.Scan);
                    maxScan = Math.Max(maxScan, msFeature.Scan);
                }

                yield return new XicFeature
                {
                    HighMz = FeatureLight.ComputeDaDifferenceFromPPM(feature.Mz, -massError),
                    LowMz = FeatureLight.ComputeDaDifferenceFromPPM(feature.Mz, massError),
                    Mz = feature.Mz,
                    Feature = feature,
                    Id = id++,
                    EndScan = minScan + ScanWindowSize,
                    StartScan = maxScan - ScanWindowSize,
                    ChargeState = feature.ChargeState
                };
            }
        }

        private IEnumerable<UMCLight> RefineFeatureXics(IList<UMCLight> features)
        {
            // Here we smooth the points...and remove any features with from and trailing zero points
            var numberOfPoints = NumberOfPoints;
            var smoother = new SavitzkyGolaySmoother(numberOfPoints, CONST_POLYNOMIAL_ORDER, false);

            foreach (var feature in features)
            {
                var map = feature.CreateChargeMap();

                // Clear the MS Feature List 
                // Because we're going to refine each charge state then fix the length of the feature
                // from it's known max abundance value.                
                feature.MsFeatures.Clear();


                // Work on a single charge state since XIC's have different m/z values
                foreach (var chargeFeatures in map.Values)
                {
                    var xic = new List<XYData>();
                    var msFeatures = chargeFeatures.Where(x => x.Abundance > 0).OrderBy(x => x.Scan).ToList();
                    msFeatures.ForEach(x => xic.Add(new XYData(x.Scan, x.Abundance)));

                    var points = smoother.Smooth(xic);
                    if (msFeatures.Count <= 0)
                        continue;

                    // Find the biggest peak...
                    var maxScanIndex = 0;
                    double maxAbundance = 0;
                    for (var i = 0; i < msFeatures.Count; i++)
                    {
                        msFeatures[i].Abundance = Convert.ToInt64(points[i].Y);

                        if (maxAbundance < msFeatures[i].Abundance)
                        {
                            maxScanIndex = i;
                            maxAbundance = msFeatures[i].Abundance;
                        }
                    }

                    // Then find when the feature goes to zero
                    // Start from max to left                        
                    var startIndex = maxScanIndex;

                    // If we hit zero, then keep
                    for (; startIndex > 0; startIndex--)
                    {
                        if (msFeatures[startIndex].Abundance < 1)
                            break;
                    }

                    // Start from max to right
                    var stopIndex = maxScanIndex;
                    for (; stopIndex < msFeatures.Count - 1; stopIndex++)
                    {
                        if (msFeatures[stopIndex].Abundance < 1)
                            break;
                    }

                    // Add the features back
                    for (var i = startIndex; i <= stopIndex; i++)
                    {
                        msFeatures[i].Abundance = Convert.ToInt64(points[i].Y);
                        feature.AddChildFeature(msFeatures[i]);
                    }
                }

                // Clean up 
            }
            return features.Where(x => x.MsFeatures.Count > 0).ToList();
        }

        /// <summary>
        /// Creates XIC Targets from a list of UMC Features
        /// </summary>
        /// <param name="features"></param>
        /// <param name="massError"></param>
        /// <returns></returns>
        private List<XicFeature> CreateXicTargets(IEnumerable<UMCLight> features, double massError)
        {
            var allFeatures = new List<XicFeature>();

            // Create XIC Features
            var id = 0;
            // Then for each feature turn it into a new feature
            foreach (var feature in features)
            {
                // Build XIC features from each
                var x = feature.CreateChargeMap();
                foreach (var charge in x.Keys)
                {
                    double maxIntensity = 0;
                    double mz = 0;
                    var min = double.MaxValue;
                    var max = double.MinValue;

                    var scanStart = int.MaxValue;
                    var scanEnd = 0;

                    foreach (var chargeFeature in x[charge])
                    {
                        min = Math.Min(min, chargeFeature.Mz);
                        max = Math.Max(max, chargeFeature.Mz);
                        scanStart = Math.Min(scanStart, chargeFeature.Scan);
                        scanEnd = Math.Min(scanStart, chargeFeature.Scan);

                        if (chargeFeature.Abundance > maxIntensity)
                        {
                            maxIntensity = chargeFeature.Abundance;
                            mz = chargeFeature.Mz;
                        }
                    }

                    // Clear the ms feature list...because later we will populate it
                    feature.MsFeatures.Clear();

                    var xicFeature = new XicFeature
                    {
                        HighMz = FeatureLight.ComputeDaDifferenceFromPPM(mz, -massError),
                        LowMz = FeatureLight.ComputeDaDifferenceFromPPM(mz, massError),
                        Mz = mz,
                        Feature = feature,
                        Id = id++,
                        EndScan = scanEnd + ScanWindowSize,
                        StartScan = scanStart - ScanWindowSize,
                        ChargeState = charge
                    };

                    allFeatures.Add(xicFeature);
                }
            }

            return allFeatures;
        }

        /// <summary>
        /// Gets or sets how many scans to add before and after an initial XIC target
        /// </summary>
        public int ScanWindowSize { get; set; }
        /// <summary>
        /// Gets or sets the size of the m/z window to use when linking MS Features to MS/MS spectra
        /// </summary>
        public double FragmentationSizeWindow { get; set; }


        public int NumberOfPoints { get; set; }
    }

}
