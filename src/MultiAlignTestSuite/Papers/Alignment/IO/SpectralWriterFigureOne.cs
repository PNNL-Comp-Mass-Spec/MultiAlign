using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{

    /// <summary>
    /// Writes data from an analysis for figure 1
    /// </summary>
    public class SpectralWriterFigureOne : ISpectralAnalysisWriter
    {
        public void Write(SpectralAnalysis analysis)
        {
            Console.WriteLine("NET,{0}", analysis.Options.NetTolerance);
            Console.WriteLine("Mass,{0}", analysis.Options.MzTolerance);

            Histogram falseMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                                                                                1,
                                                                                .05,
                                                                                analysis.Matches.Where(x => !x.IsValidMatch));

            Histogram trueMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                                                                                1,
                                                                                .05,
                                                                                analysis.Matches.Where(x => x.IsValidMatch));


            Console.WriteLine("Value, Matches, Non-Matches");
            for (int i = 0; i < falseMatches.Bins.Count; i++)
            {
                double score = falseMatches.Bins[i];
                Console.WriteLine("{0},{1},{2}", score,
                                                 falseMatches.Data[i],
                                                 trueMatches.Data[i]);
            }
            Console.WriteLine();
        }
    }
}
