using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Papers.Alignment.SSM;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public class MatchCountHistogramBuilder
    {
        public static Histogram SimilarityScore(double start, double stop, double step, IEnumerable<SpectralAnchorPointMatch> matches)
        {
            var bins    = new List<double>();
            var values  = new List<double>();

            for (double score = start; score <= stop; score += step)
            {
                int count = matches.Count(x => x.SimilarityScore < (score + step) && x.SimilarityScore >= score);
                bins.Add(score);
                values.Add(Convert.ToDouble(count));
            }

            return  new Histogram(step, "", bins, values);            
        }
        public static Histogram CreateResidualHistogram(double start, double stop, double step, IEnumerable<double> anchors)
        {
            var bins   = new List<double>();
            var values = new List<double>();

            for (double score = start; score <= stop; score += step)
            {
                int count = anchors.Count(x => x < (score + step) && x >= score);
                bins.Add(score);
                values.Add(Convert.ToDouble(count));
            }

            return new Histogram(step, "", bins, values);            
        }
    }    
}
