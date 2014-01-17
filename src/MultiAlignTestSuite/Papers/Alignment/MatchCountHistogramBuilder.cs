using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public class MatchCountHistogramBuilder
    {
        public static Histogram SimilarityScore(double start, double stop, double step, IEnumerable<AnchorPointMatch> matches)
        {
            List<double> bins   = new List<double>();
            List<double> values = new List<double>();
            
            
            for (double score = start; score <= stop; score += step)
            {
                int count = matches.Count(x => x.SimilarityScore < (stop - score));
                bins.Add(score);
                values.Add(Convert.ToDouble(count));
            }

            Histogram gram = new Histogram(step, "", bins, values);                                                
            return gram;
        }
    }    
}
