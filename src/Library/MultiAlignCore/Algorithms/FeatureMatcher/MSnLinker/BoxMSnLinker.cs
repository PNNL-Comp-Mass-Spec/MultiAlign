using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data.Constants.Libraries;

namespace MultiAlignCore.Algorithms.FeatureMatcher.MSnLinker
{
    /// <summary>
    /// Maps MS/MS data to MS features.
    /// </summary>
    public class BoxMSnLinker: IMSnLinker
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BoxMSnLinker()
        {
            Tolerances      = new FeatureTolerances();
            Tolerances.Mass = .5;
            AdductMass      = SubAtomicParticleLibrary.MASS_PROTON;
        }
        /// <summary>
        /// Gets or sets the feature tolerances to use.
        /// </summary>
        public FeatureTolerances Tolerances
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the adduct mass (e.g. proton, H+)
        /// </summary>
        public double AdductMass
        {
            get;
            set;
        }

        public Dictionary<int, int> LinkMSFeaturesToMSn(List<MSFeatureLight> features,
            List<MSSpectra> fragmentSpectra,
            IScanSummaryProvider provider)
        {
            return LinkMSFeaturesToMSn(features, fragmentSpectra);
        }
        /// <summary>
        /// Links MS features to MSMS (or MSn) data.
        /// </summary>
        /// <param name="features">Features to link.</param>
        /// <param name="spectra">Spectra to link to.</param>
        /// <param name="rawSpectraProvider">Provides access to raw scans if more data is required.</param>
        /// <returns>A map of a ms spectra id to the number of times it was mapped.</returns>
        public Dictionary<int, int>  LinkMSFeaturesToMSn(List<MSFeatureLight> features,
            List<MSSpectra> fragmentSpectra)
        {
            // First  - Sort the MSn features based on scan
            // Second - map all the features to a scan number for MS Features
            //          and a parent scan number of MSn features.
            //          The MSSpectra list should have missing scan numbers so when sorted
            //          will be monotonically increasing with 1, 2, 3, 5, 6, 7, 9, 10, 11, 13
            //          This indicates the parent scan these features were fragmented from.
            // Third  - Once these spectra have been mapped, then we can do a quicker search
            var spectra = new List<MSSpectra>();
            spectra.AddRange(fragmentSpectra);
            spectra.OrderBy(x => x.Scan);

            // Map the scans
            var featureMap    = new Dictionary<int, List<MSFeatureLight>>();
            var spectraMap         = new Dictionary<int, List<MSSpectra>>();
            var mappedMSSpectra                = new Dictionary<int,int>();
            foreach(var feature in features)
            {
                var scan                = feature.Scan;
                var containsFeature    = featureMap.ContainsKey(scan);
                if (!containsFeature)
                {
                    featureMap.Add(scan, new List<MSFeatureLight>());
                }
                featureMap[scan].Add(feature);
            }

            var totalSpectra        = spectra.Count;
            var scans   = new List<MSSpectra>();
            var parentScan          = spectra[0].Scan - 1;
            for(var i = 1; i < totalSpectra; i++)
            {
                var prevScan    = spectra[i - 1].Scan;
                var currentScan = spectra[i].Scan;
                if (currentScan - prevScan > 1)
                {
                    // Copy current data to the map.
                    var tempSpectra = new List<MSSpectra>();
                    tempSpectra.AddRange(scans);
                    spectraMap.Add(parentScan, tempSpectra);

                    // Get ready for next MSMS scan data.
                    parentScan = currentScan - 1;
                    scans.Clear();
                }
                scans.Add(spectra[i]);
            }

            // then we search for links
            var ppmRange     = Tolerances.Mass;
            var protonMass   = AdductMass;

            // Go through each scan, and see if there is a corresponding
            // MSMS scan range of spectra associated, some may not have it.
            foreach(var scan in  featureMap.Keys)
            {
                var containsMSMSFragments = spectraMap.ContainsKey(scan);
                if (containsMSMSFragments)
                {
                    // If MSMS exists there, then search all features in that mass spectra window for
                    // close spectra.
                    var suspectSpectra = spectraMap[scan];
                    foreach (var feature in featureMap[scan])
                    {
                        // Use the most abundant mass because it had a higher chance of being fragmented.
                        var mass = feature.Mz;

                        var matching = suspectSpectra.Where(x => Math.Abs(x.PrecursorMz - mass) <= ppmRange);

                        // Finally link!
                        foreach (var spectrum in matching)
                        {
                            var spectrumID      = spectrum.Id;
                            var hasBeenMapped  = mappedMSSpectra.ContainsKey(spectrumID);
                            if (!hasBeenMapped)
                            {
                                mappedMSSpectra[spectrumID] = 0;
                            }
                            mappedMSSpectra[spectrumID]++;
                            feature.MSnSpectra.Add(spectrum);
                            feature.MsMsCount++;
                            spectrum.ParentFeature = feature;
                        }
                    }
                }
            }
            return mappedMSSpectra;
        }
    }
}
