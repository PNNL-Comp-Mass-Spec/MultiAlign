#region

using System;
using System.Collections.Generic;
using FeatureAlignment.Algorithms.Alignment;
using FeatureAlignment.Algorithms.Alignment.SpectralMatching;
using FeatureAlignment.Data.Alignment;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    public class SpectralAlignerWrapper : SpectralAligner,
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData>
    {
        public new AlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee, IProgress<PRISM.ProgressData> progress = null)
        {
            var matches = base.Align(baseline, alignee);
            var data = new AlignmentData {Matches = matches};

            Matches = matches;

            return data;
        }

        public new AlignmentData Align(MassTagDatabase database, IEnumerable<UMCLight> alignee, IProgress<PRISM.ProgressData> progress = null)
        {
            var matches = base.Align(database, alignee);
            var data = new AlignmentData {Matches = matches};

            Matches = matches;

            return data;
        }

        public IEnumerable<SpectralAnchorPointMatch> Matches { get; private set; }
    }
}