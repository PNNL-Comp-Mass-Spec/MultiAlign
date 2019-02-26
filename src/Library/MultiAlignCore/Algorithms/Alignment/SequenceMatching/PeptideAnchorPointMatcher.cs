using System.Collections.Generic;
using FeatureAlignment.Algorithms.Alignment;
using FeatureAlignment.Algorithms.Alignment.SpectralMatching;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Utilities;

namespace MultiAlignCore.Algorithms.Alignment.SequenceMatching
{
    public class PeptideAnchorPointMatcher
    {
        /// <summary>
        /// Matches anchor points to peptide data.
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="peptideMapX"></param>
        /// <param name="peptideMapY"></param>
        /// <param name="options"></param>
        public void Match(IEnumerable<SpectralAnchorPointMatch> matches,
                            Dictionary<int, Peptide>        peptideMapX,
                            Dictionary<int, Peptide>        peptideMapY,
                            SpectralOptions                 options)
        {
            foreach (var match in matches)
            {
                var scanX    = match.AnchorPointX.Scan;
                var scanY    = match.AnchorPointY.Scan;
                // Assume the spectrum was not identified first...then prove a false match later
                var isMatch = AnchorPointMatchType.PeptideFailed;

                if (!peptideMapX.ContainsKey(scanX))
                {
                    match.IsValidMatch = isMatch;
                    continue;
                }
                if (!peptideMapY.ContainsKey(scanY))
                {
                    match.IsValidMatch = isMatch;
                    continue;
                }

                var peptidex = peptideMapX[scanX];
                var peptidey = peptideMapY[scanY];
                if (peptidex == null || peptidey == null)
                {
                    match.IsValidMatch = isMatch;
                    continue;
                }

                peptidex.Sequence = PeptideUtility.CleanString(peptidex.Sequence);
                peptidey.Sequence = PeptideUtility.CleanString(peptidey.Sequence);

                // Make sure the peptides are equivalent.
                if (peptidex.Sequence.Equals(peptidey.Sequence) && !string.IsNullOrWhiteSpace(peptidey.Sequence))
                    isMatch = AnchorPointMatchType.TrueMatch;
                else
                    isMatch = AnchorPointMatchType.FalseMatch;

                // Then link as true positive.
                match.AnchorPointX.Peptide  = peptidex;
                match.AnchorPointY.Peptide  = peptidey;
                match.IsValidMatch          = isMatch;
            }
        }
    }
}
