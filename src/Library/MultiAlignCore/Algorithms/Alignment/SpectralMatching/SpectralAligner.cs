using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.SpectralProcessing;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{
    public class SpectralAligner :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, IEnumerable<SpectralAnchorPointMatch>>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, IEnumerable<SpectralAnchorPointMatch>>
    {

        public event EventHandler<ProgressNotifierArgs> Progress;

        public SpectralAligner()
        {
            Options          = new SpectralOptions();
            Filter           = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);
            SpectralComparer = SpectralComparerFactory.CreateSpectraComparer(SpectralComparison.CosineDotProduct);
        }

        public SpectralOptions Options
        {
            get; set;
        }

        public double Bandwidth { get; set; }

        /// <summary>
        /// Gets or sets the baseline spectra provider
        /// </summary>
        public IScanSummaryProvider BaselineSpectraProvider { get; set; }
        /// <summary>
        /// Gets or sets the alignee spectra provider.
        /// </summary>
        public IScanSummaryProvider AligneeSpectraProvider { get; set; }
        /// <summary>
        /// Gets or sets the spectral comparer.
        /// </summary>
        public ISpectralComparer SpectralComparer
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the spectral filter
        /// </summary>
        public ISpectraFilter Filter { get; set; }


        private void OnProgress(string message)
        {
            Progress?.Invoke(this, new ProgressNotifierArgs(message));
        }

        /// <summary>
        /// Aligns to a mass tag database.  The baseline provider should be able to convert a mass tag sequence into a theoretical spectrum.
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="alignee"></param>
        /// <returns></returns>
        public IEnumerable<SpectralAnchorPointMatch> Align( MassTagDatabase baseline,
                                                            IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            OnProgress("Finding anchor point matches");
            var finder  = new SpectralAnchorPointFinder();
            var matches = finder.FindAnchorPoints(BaselineSpectraProvider as ISpectraProvider,
                                                    AligneeSpectraProvider as ISpectraProvider,
                                                    SpectralComparer,
                                                    Filter,
                                                    Options);

            OnProgress("Creating Alignment Functions");
            var aligner = new SpectralAnchorPointAligner();
            var spectralAnchorPointMatches = matches as SpectralAnchorPointMatch[] ?? matches.ToArray();
            //TODO: Brian modified to test alignment using LCMSWarp regression code.
            aligner.CreateAlignmentFunctions(spectralAnchorPointMatches);

            OnProgress("Transforming sub-features");
            foreach (var feature in alignee)
            {
                feature.NetAligned = feature.Net; // aligner.AlignNet(feature.Net);
            }

            return spectralAnchorPointMatches;
        }

        /// <summary>
        /// Aligns two feature datasets.
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="alignee"></param>
        /// <returns></returns>
        public IEnumerable<SpectralAnchorPointMatch> Align( IEnumerable<UMCLight> baseline,
                                                            IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            OnProgress("Finding anchor point matches");
            var finder  = new SpectralAnchorPointFinder();
            var matches = finder.FindAnchorPoints(BaselineSpectraProvider as ISpectraProvider,
                                                    AligneeSpectraProvider as ISpectraProvider,
                                                    SpectralComparer,
                                                    Filter,
                                                    Options);

            OnProgress("Creating Alignment Functions");
            var aligner                     = new SpectralAnchorPointAligner(Bandwidth);
            var spectralAnchorPointMatches  = matches as SpectralAnchorPointMatch[] ?? matches.ToArray();
            aligner.CreateAlignmentFunctions(spectralAnchorPointMatches);

            OnProgress("Transforming sub-features");
            foreach (var feature in alignee)
            {
                feature.NetAligned              = aligner.AlignNet(feature.Net);
                feature.Net           = feature.NetAligned;
                feature.MassMonoisotopicAligned    = aligner.AlignMass(feature.MassMonoisotopic);
            }

            return spectralAnchorPointMatches;
        }

    }
}
