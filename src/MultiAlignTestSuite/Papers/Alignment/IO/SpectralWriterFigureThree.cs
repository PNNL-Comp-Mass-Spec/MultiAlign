using System.Linq;
using MultiAlignTestSuite.Papers.Alignment.SSM;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    class SpectralWriterFigureThree: PaperFileWriter, ISpectralAnalysisWriter
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
            foreach (string name in analysis.DatasetNames)
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

            Histogram allMatches = MatchCountHistogramBuilder.SimilarityScore(0, 1, .05, analysis.Matches);
            WriteLine("Value\t False Matches\t True Matches\t All");
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
            WriteLine();            
        }
    }
}
