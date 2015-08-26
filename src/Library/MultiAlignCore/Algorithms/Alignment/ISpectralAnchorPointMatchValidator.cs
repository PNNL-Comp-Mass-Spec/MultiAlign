using System.Collections.Generic;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment
{
    public interface ISpectralAnchorPointMatchValidator
    {
        void ValidateMatches(IEnumerable<SpectralAnchorPointMatch> matches,
                             IEnumerable<Peptide> peptidesA,
                             IEnumerable<Peptide> peptidesB,
                             SpectralOptions options);

    }
}
