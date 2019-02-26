#region

using System.Collections.Generic;
using FeatureAlignment.Algorithms.Alignment;
using FeatureAlignment.Algorithms.Alignment.LcmsWarp;
using FeatureAlignment.Data.Alignment;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.Features;

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

        public static IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>
            CreateDatasetAligner(FeatureAlignmentType type,
                LcmsWarpAlignmentOptions options,
                SpectralOptions spectralOptions)
        {
            IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData> aligner = null;
            switch (type)
            {
                case FeatureAlignmentType.LCMS_WARP:
                    aligner = new LcmsWarpFeatureAligner(options);
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

        public static IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData> CreateDatabaseAligner(
            FeatureAlignmentType type,
            LcmsWarpAlignmentOptions options,
            SpectralOptions spectralOptions)
        {
            IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData> aligner = null;

            switch (type)
            {
                case FeatureAlignmentType.LCMS_WARP:
                    aligner = new LcmsWarpFeatureAligner(options);
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