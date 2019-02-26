using System.Collections.Generic;
using FeatureAlignment.Algorithms.Alignment.SpectralMatching;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;

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
