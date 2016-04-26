using System;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Chromatograms
{
    public sealed class XicFeature : Chromatogram, IComparable<XicFeature>
    {
        public XicFeature(UMCLight feature, double massError)
        {
            var targetScan = 0;
            var maxAbundance = 0;



            this.HighMz = FeatureLight.ComputeDaDifferenceFromPPM(feature.Mz, -massError);
            this.LowMz = FeatureLight.ComputeDaDifferenceFromPPM(feature.Mz, massError);
            this.Mz = feature.Mz;
            this.Feature = feature;
            this.Id = feature.Id;
            this.EndScan = targetScan;
            this.StartScan = targetScan;
            this.ChargeState = feature.ChargeState;
        }


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