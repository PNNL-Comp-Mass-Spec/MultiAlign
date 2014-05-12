#region

using System.Linq;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    internal class SpectralWriterFigureThree : PaperFileWriter, ISpectralAnalysisWriter
    {
        public SpectralWriterFigureThree(string name, string path) :
            base(name, path, true)
        {
        }

        public override void WriteLine(string data)
        {
            Open();
            base.WriteLine(data);
        }

        public void Write(SpectralAnalysis analysis)
        {
            Open();
            WriteLine("[Test]");
            WriteLine("[Datasets]");
            foreach (var name in analysis.DatasetNames)
            {
                WriteLine(name);
            }

            WriteLine("[Histogram]");
            var falseMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                1,
                .05,
                analysis.Matches.Where(x => x.IsValidMatch == AnchorPointMatchType.FalseMatch));

            var trueMatches = MatchCountHistogramBuilder.SimilarityScore(0,
                1,
                .05,
                analysis.Matches.Where(x => x.IsValidMatch == AnchorPointMatchType.TrueMatch));

            var allMatches = MatchCountHistogramBuilder.SimilarityScore(0, 1, .05, analysis.Matches);
            WriteLine("Value\t False Matches\t True Matches\t All");
            for (var i = 0; i < falseMatches.Bins.Count; i++)
            {
                var score = falseMatches.Bins[i];
                WriteLine(string.Format("{0}\t{1}\t{2}\t{3}",
                    score,
                    falseMatches.Data[i],
                    trueMatches.Data[i],
                    allMatches.Data[i]));
            }
            // Determine how many matches we have
            WriteLine();
        }
    }
}