#region

using System;
using System.Collections.Generic;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    public class SpectralAlignerWrapper : SpectralAligner,
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData>
    {
        public new AlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            var matches = base.Align(baseline, alignee);
            var data = new AlignmentData {Matches = matches};

            Matches = matches;

            return data;
        }

        public new AlignmentData Align(MassTagDatabase database, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            var matches = base.Align(database, alignee);
            var data = new AlignmentData {Matches = matches};

            Matches = matches;

            return data;
        }

        public IEnumerable<SpectralAnchorPointMatch> Matches { get; private set; }
    }
}