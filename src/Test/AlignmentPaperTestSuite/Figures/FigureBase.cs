#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.IO.Features;
using MultiAlignTestSuite.Papers.Alignment;
using MultiAlignTestSuite.Papers.Alignment.IO;
using NUnit.Framework;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;
using PNNLOmics.Algorithms.Regression;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmicsIO.IO;

#endregion

namespace AlignmentPaperTestSuite.Figures
{
    /// <summary>
    ///     Creates all figures used in the alignment paper
    /// </summary>
    
    public class FigureBase
    {
        [SetUp]
        public void SetupFigureTests()
        {
            AlignmentAnalysisWriterFactory.BasePath = @"M:\doc\papers\paperAlignment\Data\figure1";
        }

        internal class PathCache
        {
            public string Cache { get; set; }
            public string Msgf { get; set; }
            public string Features { get; set; }
        }

        protected static SpectralAnalysis MatchDatasets(SpectralComparison comparerType,
            ISpectraProvider readerX,
            ISpectraProvider readerY,
            SpectralOptions options,
            AlignmentDataset datasetX,
            AlignmentDataset datasetY,
            List<string> names)
        {
            var peptideReader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            var finder = new SpectralAnchorPointFinder();
            var validator = new SpectralAnchorPointValidator();
            var comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType);
            var filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

            var matches = finder.FindAnchorPoints(readerX,
                readerY,
                comparer,
                filter,
                options);

            var peptidesX = peptideReader.Read(datasetX.PeptideFile);
            var peptidesY = peptideReader.Read(datasetY.PeptideFile);
            validator.ValidateMatches(matches,
                peptidesX,
                peptidesY,
                options);

            var analysis = new SpectralAnalysis
            {
                DatasetNames = names,
                Matches = matches,
                Options = options
            };
            return analysis;
        }


        protected static void AlignMatches(SpectralAnalysis analysis, ISpectralAnalysisWriter writer)
        {
            var netXvalues = new List<double>();
            var netYvalues = new List<double>();
            var massXvalues = new List<double>();
            var massYvalues = new List<double>();

            var matches =
                analysis.Matches.OrderBy(x => x.AnchorPointX.Net);

            // 1. Find the best matches
            // 2. Find only matches that have been made once.

            var bestMatches = new Dictionary<int, SpectralAnchorPointMatch>();
            foreach (var match in matches)
            {
                var scan = match.AnchorPointX.Scan;
                if (bestMatches.ContainsKey(scan))
                {
                    if (bestMatches[scan].SimilarityScore < match.SimilarityScore)
                    {
                        bestMatches[scan] = match;
                    }
                }
                else
                {
                    bestMatches.Add(scan, match);
                }
            }

            // 2. Find only those matched once
            var all = new Dictionary<int, SpectralAnchorPointMatch>();
            foreach (var match in bestMatches.Values)
            {
                var scan = match.AnchorPointY.Scan;
                if (all.ContainsKey(scan))
                {
                    if (all[scan].SimilarityScore < match.SimilarityScore)
                    {
                        all[scan] = match;
                    }
                }
                else
                {
                    all.Add(scan, match);
                }
            }
            // Write the analysis 
            writer.Write(analysis);

            // Then generate the NET Alignment using R1
            var anchorPoints =
                all.Values.OrderBy(x => x.AnchorPointX.Net).ToList();

            foreach (var match in anchorPoints)
            {
                netXvalues.Add(match.AnchorPointX.Net);
                netYvalues.Add(match.AnchorPointY.Net);
            }

            Func<double, double, double> netFunc = (x, y) => x - y;
            Func<double, double, double> massFunc = FeatureLight.ComputeMassPPMDifference;
            InterpolateDimension("NET-R1", writer, netXvalues, netYvalues, anchorPoints, netFunc);


            // Then generate the Mass Alignment using R1
            // We also have to resort the matches based on mass now too
            anchorPoints = all.Values.OrderBy(x => x.AnchorPointX.Mz).ToList();
            foreach (var match in anchorPoints)
            {
                massXvalues.Add(match.AnchorPointX.Mz);
                massYvalues.Add(match.AnchorPointY.Mz);
            }
            InterpolateDimension("Mass-R1", writer, massXvalues, massYvalues, anchorPoints, massFunc);
        }


        private static void InterpolateDimension(string name,
            ISpectralAnalysisWriter writer,
            List<double> xvalues,
            List<double> yvalues,
            List<SpectralAnchorPointMatch> anchorPoints,
            Func<double, double, double> func)
        {
            var interpolator = new LoessInterpolator();
            var pre = new List<double>();
            var post = new List<double>();

            var fit = interpolator.Smooth(xvalues, yvalues,
                FitFunctionFactory.Create(FitFunctionTypes.TriCubic));

            writer.WriteLine("");
            WriteMatches(name,
                writer,
                xvalues,
                yvalues,
                fit,
                interpolator,
                anchorPoints,
                pre,
                post,
                func);

            WriteErrorHistogram(name, pre, post, writer);
        }

        private static void WriteMatches(
            string name,
            ISpectralAnalysisWriter writer,
            List<double> xvalues,
            List<double> yvalues,
            IList<double> fit,
            LoessInterpolator interpolator,
            List<SpectralAnchorPointMatch> anchorPoints,
            List<double> preNet,
            List<double> postNet,
            Func<double, double, double> difference)
        {
            if (anchorPoints == null) throw new ArgumentNullException("anchorPoints");

            writer.WriteLine(string.Format(@"[{0}]", name));
            writer.WriteLine("x\ty\tfit\tpre-diff\tpost-diff\tsim score\tvalid");
            for (var index = 0; index < xvalues.Count; index++)
            {
                var x = xvalues[index];
                var y = yvalues[index];
                var value = interpolator.Predict(x);
                var preDiff = difference(x, y);
                var postDiff = difference(value, y);

                writer.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    x,
                    y,
                    fit[index],
                    preDiff,
                    postDiff,
                    anchorPoints[index].SimilarityScore,
                    anchorPoints[index].IsValidMatch));

                preNet.Add(preDiff);
                postNet.Add(postDiff);
            }
        }

        private static void WriteErrorHistogram(string message, IEnumerable<double> pre, IEnumerable<double> post,
            ISpectralAnalysisWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine(message);
            var preHist =
                MatchCountHistogramBuilder.CreateResidualHistogram(-.5, .5, .01, pre);
            var postHist =
                MatchCountHistogramBuilder.CreateResidualHistogram(-.5, .5, .01, post);

            writer.WriteLine("Value\t False Matches\t True Matches");
            for (var index = 0; index < preHist.Bins.Count; index++)
            {
                var preValue = preHist.Bins[index];
                writer.WriteLine(string.Format("{0}\t{1}\t{2}",
                    preValue,
                    preHist.Data[index],
                    postHist.Data[index]));
            }
        }
    }

}