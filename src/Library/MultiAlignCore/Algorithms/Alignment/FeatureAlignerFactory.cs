using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.Alignment;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Alignment
{
    public static class FeatureAlignerFactory
    {
        static FeatureAlignerFactory()
        {
            Bandwidth = .25;
        }

        public static double Bandwidth { get; set; }

        public static IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, classAlignmentData> CreateDatasetAligner(FeatureAlignmentType type,
                                                                                                                             AlignmentOptions options,
                                                                                                                             SpectralOptions spectralOptions)
        {
            IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, classAlignmentData> aligner = null;
            switch (type)
            {
                case FeatureAlignmentType.LcmsWarp:
                    aligner = new LcmsWarpFeatureAligner{ Options = options };
                    break;
                case FeatureAlignmentType.DirectImsInfusion:
                    aligner = new DummyAlignment();
                    break;
                case FeatureAlignmentType.SpectralAlignment:
                    aligner = new SpectralAlignerWrapper { Options = spectralOptions, Bandwidth = Bandwidth };
                    break;
            }

            return aligner;
        }
        public static IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, classAlignmentData> CreateDatabaseAligner(FeatureAlignmentType type,
                                                                                                                        AlignmentOptions options,
                                                                                                                        SpectralOptions spectralOptions)
        {
            IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, classAlignmentData> aligner = null;

            switch (type)
            {
                case FeatureAlignmentType.LcmsWarp:
                    aligner = new LcmsWarpFeatureAligner{ Options = options };
                    break;
                case FeatureAlignmentType.DirectImsInfusion:
                    aligner = new DummyAlignment();
                    break;
                case FeatureAlignmentType.SpectralAlignment:
                    aligner = new SpectralAlignerWrapper{Options = spectralOptions};
                    break;
            }

            return aligner;
        }
    }
}
