using System;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Chromatograms
{
    public sealed class XicFeature : Chromatogram, IComparable<XicFeature>
    {
        public double LowMz { get; set; }
        public double HighMz { get; set; }                
        public int Id { get; set; }
        public UMCLight Feature { get; set; }


        /// <summary>
        /// Compares this xic feature to another based on m/z
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(XicFeature other)
        {
            return Mz.CompareTo(other.Mz);
        }

    }
}