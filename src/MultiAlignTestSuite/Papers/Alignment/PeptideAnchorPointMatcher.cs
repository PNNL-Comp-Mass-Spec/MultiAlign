using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using MultiAlignTestSuite.Papers.Alignment.SSM;

namespace MultiAlignTestSuite.Papers.Alignment
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
        public void Match(IEnumerable<AnchorPointMatch>   matches,             
                                                Dictionary<int, Peptide>        peptideMapX,
                                                Dictionary<int, Peptide>        peptideMapY,
                                                SpectralOptions                 options)
        {
            Peptide peptidex = null;
            Peptide peptidey = null;

            foreach (AnchorPointMatch match in matches)
            {
                int scanX    = match.AnchorPointX.Scan;
                int scanY    = match.AnchorPointY.Scan;
                // Assume the spectrum was not identified first...then prove a false match later
                AnchorMatch isMatch = AnchorMatch.PeptideFailed;

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

                peptidex = peptideMapX[scanX];
                peptidey = peptideMapY[scanY];
                if (peptidex == null || peptidey == null)
                {
                    match.IsValidMatch = isMatch;
                    continue;
                }

                peptidex.Sequence = PeptideUtility.CleanString(peptidex.Sequence);
                peptidey.Sequence = PeptideUtility.CleanString(peptidey.Sequence);

                // Make sure the peptides are equivalent.
                if (peptidex.Sequence.Equals(peptidey.Sequence) && !string.IsNullOrWhiteSpace(peptidey.Sequence))                
                    isMatch = AnchorMatch.TrueMatch;                
                else
                    isMatch = AnchorMatch.FalseMatch;

                // Then link as true positive.
                match.AnchorPointX.Peptide  = peptidex;
                match.AnchorPointY.Peptide  = peptidey;
                match.IsValidMatch          = isMatch;
            }            
        }
    }
}
