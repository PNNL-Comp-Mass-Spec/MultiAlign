using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;
using System.IO;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{

    /// <summary>
    /// Writes data from an analysis for figure 1
    /// </summary>
    public class SpectralWriterFigureOne : PaperFileWriter, ISpectralAnalysisWriter
    {

        public SpectralWriterFigureOne(string name, string path):
            base(name, path)
        {
        }

        public void Write(SpectralAnalysis analysis)
        {
            Open();
            WriteLine(string.Format("NET\t{0}", analysis.Options.NetTolerance));
            WriteLine(string.Format("Mass\t{0}", analysis.Options.MzTolerance));

            Histogram falseMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                                                                                1,
                                                                                .05,
                                                                                analysis.Matches.Where(x => !x.IsValidMatch));

            Histogram trueMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                                                                                1,
                                                                                .05,
                                                                                analysis.Matches.Where(x => x.IsValidMatch));


            WriteLine("Value\t False Matches\t True Matches");
            for (int i = 0; i < falseMatches.Bins.Count; i++)
            {
                double score = falseMatches.Bins[i];
                WriteLine(string.Format("{0}\t{1}\t{2}", 
                                                 score,
                                                 falseMatches.Data[i],
                                                 trueMatches.Data[i]));
            }
            WriteLine();
            Close();
        }
    }

}
