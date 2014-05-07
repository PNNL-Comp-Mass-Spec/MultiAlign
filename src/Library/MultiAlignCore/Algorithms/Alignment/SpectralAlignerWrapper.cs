using MultiAlignCore.Data.Alignment;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;
using PNNLOmics.Data.Features;
using System.Collections.Generic;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment
{
    public class SpectralAlignerWrapper: SpectralAligner,
                IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, classAlignmentData>,
                IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, classAlignmentData>
    {
        public new classAlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee)
        {
            var matches = base.Align(baseline, alignee);
            var data = new classAlignmentData { Matches = matches };

            Matches = matches;

            return data;
        }
        public new classAlignmentData Align(MassTagDatabase database, IEnumerable<UMCLight> alignee)
        {
            var matches = base.Align(database, alignee);
            var data = new classAlignmentData {Matches = matches};
            
            Matches = matches;

            return data;

        }

        public IEnumerable<SpectralAnchorPointMatch> Matches { get; private set; }
    }
}
