using System;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Data.Features;

    /// <summary>
    /// Object to contain the feature match data for LCMS Warp.
    /// Contains the feature index, baseline index, feature normalized elution time,
    /// baseline normalized elution time and the error for the mass, the net and the drift time
    /// </summary>
    public class LcmsWarpFeatureMatch : IComparable<LcmsWarpFeatureMatch>
    {
        /// <summary>
        /// Constructor, initializes the values to -1 to ensure ability to see
        /// when there hasn't been a match made
        /// </summary>
        public LcmsWarpFeatureMatch()
        {
            FeatureIndex = -1;
            BaselineFeatureIndex = -1;
            Net = -1;
            BaselineNet = -1;
        }

        /// <summary>
        /// Mass Error
        /// Equal to the difference between the feature Mass and the baseline Mass, in Da
        /// </summary>
        public double MassError { get; set; }

        /// <summary>
        /// PPM Mass Error
        /// Equal to the difference between the feature Mass and the baseline Mass, in ppm
        /// </summary>
        public double PpmMassError { get; set; }

        /// <summary>
        /// Normalized Elution Time error
        /// Equal to the difference between the feature NET and the baseline NET
        /// </summary>
        public double NetError { get; set; }

        /// <summary>
        /// Drift Time Error
        /// Equal to the difference between the feature Drift time and the baseline Drift time
        /// </summary>
        public double DriftError { get; set; }

        /// <summary>
        /// Normalized elution time of the feature
        /// Typically tracks scan number
        /// </summary>
        public double Net { get; set; }

        /// <summary>
        /// Normalized elution time of the baseline feature that this feature matches to.
        /// The baseline feature is either an AMT tag or a feature in a baseline dataset
        /// </summary>
        /// <remarks>
        /// If an AMT tag, this is a true NET value (typically 0 to 1)
        /// If a feature in a baseline dataset, this is ... TBD</remarks>
        public double BaselineNet { get; set; }

        //TODO: probably just point to the reference of the feature
        /// <summary>
        /// Index of the feature that this match corresponds to
        /// </summary>
        public int FeatureIndex { get; set; }

        public UMCLight AligneeFeature { get; set; }

        //TODO: probably just point to the reference of the feature
        /// <summary>
        /// Index of the baseline feature that this match corresponds to
        /// The baseline feature is either an AMT tag or a feature in a baseline dataset
        /// </summary>
        public int BaselineFeatureIndex { get; set; }

        public UMCLight BaselineFeature { get; set; }

        /// <summary>
        /// Compares two feature matches based on the Normalized elution time
        /// </summary>
        /// <param name="compareFeature"></param>
        /// <returns></returns>
        public int CompareTo(LcmsWarpFeatureMatch compareFeature)
        {
            if (compareFeature == null)
            {
                return 1;
            }
            return Net.CompareTo(compareFeature.Net);
        }

        public override string ToString()
        {
            return "Index=" + FeatureIndex + "; Index2=" + BaselineFeatureIndex + "; NET=" + Net.ToString("0.000") + "; NET2=" +
                   BaselineNet.ToString("0.000");
        }
    }
}