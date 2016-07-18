using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms.Alignment.SequenceMatching;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Utilities;

namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{
    /// <summary>
    /// Assesses the validity of anchor point matches given a set of peptide sequences.
    /// </summary>
    public class SpectralAnchorPointValidator
    {
        ///// <summary>
        ///// Matches two datasets based on spectral similarity.
        ///// </summary>
        ///// <param name="readerX"></param>
        ///// <param name="readerY"></param>
        ///// <param name="options"></param>
        ///// <returns></returns>
        //public  MatchDatasets(ISpectraProvider readerX,
        //                                                        ISpectraProvider readerY,
        //                                                        SpectralOptions options)
        //{
        //    // This helps us compare various comparison calculation methods
        //    var comparer    = SpectralComparerFactory.CreateSpectraComparer(options.ComparerType);

        //    // This guy filters the spectra, so that we only keep the N most intense ions for comparison
        //    var filter      = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

        //    // Here we find all the matches
        //    var finder      = new SpectralAnchorPointFinderOriginal();
        //    return finder.FindAnchorPoints(readerX,
        //                                          readerY,
        //                                          comparer,
        //                                          filter,
        //                                          options);
        //}

        public void ValidateMatches( IEnumerable<SpectralAnchorPointMatch> matches,
                                                                    IEnumerable<Peptide> peptidesA,
                                                                    IEnumerable<Peptide> peptidesB,
                                                                    SpectralOptions options)
        {
            IEnumerable<SpectralAnchorPointMatch> anchorPointMatches = matches as SpectralAnchorPointMatch[] ?? matches.ToArray();

            // If the list has peptides...then we should validate matches
            var enumerable      = peptidesB as Peptide[] ?? peptidesB.ToArray();
            var peptides        = peptidesA as Peptide[] ?? peptidesA.ToArray();
            var matchPeptides   = (peptides.Any() && enumerable.Any());

            if (matchPeptides)
            {
                peptidesA = peptides.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();
                peptidesB = enumerable.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();

                var peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
                var peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

                // Then map the peptide sequences to identify True Positive and False Positives
                var matcher = new PeptideAnchorPointMatcher();
                matcher.Match(anchorPointMatches,
                                peptideMapX,
                                peptideMapY,
                                options);
            }
        }
    }
}
