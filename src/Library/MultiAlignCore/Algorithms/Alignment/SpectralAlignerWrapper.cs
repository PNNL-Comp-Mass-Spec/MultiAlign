#region

using System.Collections.Generic;
using MultiAlignCore.Data.Alignment;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    public class SpectralAlignerWrapper : SpectralAligner,
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData>
    {
        public new AlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee)
        {
            var matches = base.Align(baseline, alignee);
            var data = new AlignmentData {Matches = matches};

            Matches = matches;

            return data;
        }

        public new AlignmentData Align(MassTagDatabase database, IEnumerable<UMCLight> alignee)
        {
            var matches = base.Align(database, alignee);
            var data = new AlignmentData {Matches = matches};

            Matches = matches;

            return data;
        }

        public IEnumerable<SpectralAnchorPointMatch> Matches { get; private set; }
    }
}