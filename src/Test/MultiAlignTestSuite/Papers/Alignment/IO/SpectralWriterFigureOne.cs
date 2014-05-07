using MultiAlignTestSuite.Papers.Alignment.SSM;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{

    /// <summary>
    /// Writes data from an analysis for figure 1
    /// </summary>
    public class SpectralWriterFigureOne : PaperFileWriter, ISpectralAnalysisWriter
    {

        public SpectralWriterFigureOne(string name, string path):
            base(name, path, true)
        {
        }

        public void Write(SpectralAnalysis analysis)
        {
            Open();
            WriteLine("[Test]");
            WriteLine("[Datasets]");
            foreach (string name in analysis.DatasetNames)
            {
                WriteLine(name);
            }

            WriteLine("[Histogram]");
            Histogram falseMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                                                                                1,
                                                                                .05,
                                                                                analysis.Matches.Where(x => x.IsValidMatch == AnchorPointMatchType.FalseMatch));

            Histogram trueMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                                                                                1,
                                                                                .05,
                                                                                analysis.Matches.Where(x => x.IsValidMatch == AnchorPointMatchType.TrueMatch));

            Histogram allMatches = MatchCountHistogramBuilder.SimilarityScore(0, 1, .05, analysis.Matches);

            WriteLine("Value\t False Matches\t True Matches");
            for (int i = 0; i < falseMatches.Bins.Count; i++)
            {
                double score = falseMatches.Bins[i];
                WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", 
                                                 score,
                                                 falseMatches.Data[i],
                                                 trueMatches.Data[i],
                                                 allMatches.Data[i]));
            }
            // Determine how many matches we have
            WriteLine("[Matches]");
            Dictionary<int, List<SpectralAnchorPoint>> datasetX = new Dictionary<int, List<SpectralAnchorPoint>>();

            // Here we tally the number of matches
            foreach (var match in analysis.Matches)
            {         
                int scanX = match.AnchorPointX.Scan;
                if (!datasetX.ContainsKey(scanX))
                    datasetX.Add(scanX, new List<SpectralAnchorPoint>());

                // making sure to attribute true positive matches
                match.AnchorPointX.IsTrue = match.IsValidMatch == AnchorPointMatchType.TrueMatch;
                datasetX[scanX].Add(match.AnchorPointX);
            }

            int maxMatch = 0;
            Dictionary<int, int> matchCount   = new Dictionary<int, int>();
            Dictionary<int, int> multipleTrue = new Dictionary<int, int>();
            foreach (var points in datasetX.Values)
            {
                // Then we go through the list...
                // count the number of true matches
                int total       = points.Count;
                int totalTrue   = points.Count(x => x.IsTrue);

                // first time to see this match total...then make a new list
                maxMatch        = Math.Max(maxMatch, total);
                if (!matchCount.ContainsKey(total))                
                    matchCount.Add(total, 0);

                // if we have more than one entry for this spectrum
                // and we have more than one true positive...lets add him up
                if (total >= 1 && totalTrue > 0)
                {
                    if (!multipleTrue.ContainsKey(totalTrue))
                        multipleTrue.Add(totalTrue, 0);

                    multipleTrue[totalTrue]++;
                }

                matchCount[total]++;
            }

            WriteLine("Num Matches\tTotal\tTotal True");
            for (int i = 1; i < maxMatch; i++)
            {
                int totalTrue = 0;
                int total     = 0;
                if (multipleTrue.ContainsKey(i)) totalTrue  = multipleTrue[i];
                if (matchCount.ContainsKey(i)) total        = matchCount[i];
 
                WriteLine(string.Format("{0}\t{1}\t{2}", i, total, totalTrue));
            }
            WriteLine();

          
            Close();
        }
    }

}
