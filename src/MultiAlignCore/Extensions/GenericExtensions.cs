﻿using System;
using System.Collections.Generic;
using MathNet.Numerics.Statistics;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class GenericExtensions
    {
        public static Dictionary<int, int> CreateHistogram(this IEnumerable<int> values, int start, int end)
        {
            var histogram = new Dictionary<int, int>();
            for (var i = start; i < end; i++)            
                histogram.Add(i, 0);

            foreach (var chargeDouble in values)
            {                               
                if (!histogram.ContainsKey(chargeDouble))
                    histogram.Add(chargeDouble, 0);
                histogram[chargeDouble] = histogram[chargeDouble] + 1;                                    
            }
            return histogram;
        }
    }
}