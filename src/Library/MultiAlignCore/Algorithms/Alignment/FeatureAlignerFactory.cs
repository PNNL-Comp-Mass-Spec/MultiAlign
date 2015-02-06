#region

using System.Collections.Generic;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.Alignment;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    public static class FeatureAlignerFactory
    {
        static FeatureAlignerFactory()
        {
            Bandwidth = .25;
        }

        public static double Bandwidth { get; set; }

        public static IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, classAlignmentData>
            CreateDatasetAligner(FeatureAlignmentType type,
                LcmsWarpAlignmentOptions options,
                SpectralOptions spectralOptions)
        {
            IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, classAlignmentData> aligner = null;
            switch (type)
            {
                case FeatureAlignmentType.LCMS_WARP:
                    aligner = new LcmsWarpFeatureAligner {Options = options};
                    break;
                case FeatureAlignmentType.DIRECT_IMS_INFUSION:
                    aligner = new DummyAlignment();
                    break;
                case FeatureAlignmentType.SPECTRAL_ALIGNMENT:
                    aligner = new SpectralAlignerWrapper {Options = spectralOptions, Bandwidth = Bandwidth};
                    break;
            }

            return aligner;
        }

        public static IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, classAlignmentData> CreateDatabaseAligner(
            FeatureAlignmentType type,
            LcmsWarpAlignmentOptions options,
            SpectralOptions spectralOptions)
        {
            IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, classAlignmentData> aligner = null;

            switch (type)
            {
                case FeatureAlignmentType.LCMS_WARP:
                    aligner = new LcmsWarpFeatureAligner { Options = options };
                    break;
                case FeatureAlignmentType.DIRECT_IMS_INFUSION:
                    aligner = new DummyAlignment();
                    break;
                case FeatureAlignmentType.SPECTRAL_ALIGNMENT:
                    aligner = new SpectralAlignerWrapper {Options = spectralOptions};
                    break;
            }

            return aligner;
        }
    }
}