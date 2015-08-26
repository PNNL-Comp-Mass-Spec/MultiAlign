#region

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignTestSuite.Papers.Alignment.SSM;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment
{
    public class MatchCountHistogramBuilder
    {
        public static Histogram SimilarityScore(double start, double stop, double step,
            IEnumerable<SpectralAnchorPointMatch> matches)
        {
            var bins = new List<double>();
            var values = new List<double>();

            for (var score = start; score <= stop; score += step)
            {
                var count = matches.Count(x => x.SimilarityScore < (score + step) && x.SimilarityScore >= score);
                bins.Add(score);
                values.Add(Convert.ToDouble(count));
            }

            return new Histogram(step, "", bins, values);
        }

        public static Histogram CreateResidualHistogram(double start, double stop, double step,
            IEnumerable<double> anchors)
        {
            var bins = new List<double>();
            var values = new List<double>();

            for (var score = start; score <= stop; score += step)
            {
                var count = anchors.Count(x => x < (score + step) && x >= score);
                bins.Add(score);
                values.Add(Convert.ToDouble(count));
            }

            return new Histogram(step, "", bins, values);
        }
    }
}