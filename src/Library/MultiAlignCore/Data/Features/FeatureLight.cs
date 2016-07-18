using System;
using PNNLOmics.Data.Constants.Libraries;

namespace MultiAlignCore.Data.Features
{
    using System.Collections.Generic;

    /// <summary>
    /// Basic feature class
    /// </summary>
    public class FeatureLight
    {
        public enum SeparationTypes
        {
            LC,
            DriftTime,
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FeatureLight()
        {
            Abundance = 0;
            ChargeState = 0;
            DriftTime = 0;
            Id = -1;
            MassMonoisotopic = 0;
            MassMonoisotopicAligned = 0;
            Net = 0;
            NetAligned = 0;
            Net = 0;
            MSnSpectra = new List<MSSpectra>();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="feature">Feature to copy data from.</param>
        public FeatureLight(FeatureLight feature)
        {

            Abundance = 0;
            ChargeState = 0;
            DriftTime = 0;
            Id = -1;
            MassMonoisotopic = 0;
            MassMonoisotopicAligned = 0;
            Net = 0;
            NetAligned = 0;
            Net = 0;

            Abundance = feature.Abundance;
            ChargeState = feature.ChargeState;
            DriftTime = feature.DriftTime;
            Id = feature.Id;
            MassMonoisotopic = feature.MassMonoisotopic;
            MassMonoisotopicAligned = feature.MassMonoisotopicAligned;
            Net = feature.Net;
            Score = feature.Score;
            Net = feature.Net;
            AmbiguityScore = double.MaxValue;
            MSnSpectra = new List<MSSpectra>();
        }

        public int Id { get; set; }
        public int Index { get; set; }
        public int IdentifiedSpectraCount { get; set; }

        public virtual List<MSSpectra> MSnSpectra { get; set; }

        /// <summary>
        /// Gets or sets the number of MSMS spectra
        /// </summary>
        public int MsMsCount { get; set; }

        /// <summary>
        /// Gets or sets the scan value.
        /// </summary>
        /// <remarks>For IMS data, stores the frame number</remarks>
        public int Scan { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance between this feature's elements and another
        /// </summary>
        public double MinimumCentroidDistance { get; set; }

        public double Mz { get; set; }

        /// <summary>
        /// Gets or sets the abundance.
        /// </summary>
        public double Abundance { get; set; }

        /// <summary>
        /// Gets or sets the monoisotopic mass of the feature.
        /// </summary>
        public double MassMonoisotopic { get; set; }

        /// <summary>
        /// Gets or sets the monoisotopic mass of the feature.
        /// </summary>
        public virtual double MassMonoisotopicAligned { get; set; }

        /// <summary>
        /// Gets or sets the retention time of a feature.
        /// </summary>
        public double Net { get; set; }

        /// <summary>
        /// Gets or sets the retention time of a feature.
        /// </summary>
        [Obsolete]
        public double RetentionTimexx { get; set; }

        /// <summary>
        /// Gets or sets the normalized retention time for this feature.
        /// </summary>
        [Obsolete]
        public double Netx { get; set; }

        /// <summary>
        /// Gets or sets the aligned NET value for this Feature
        /// </summary>
        public double NetAligned { get; set; }

        /// <summary>
        /// Gets or sets the first IMS scan this feature was seen in.
        /// </summary>
        public double ImsScanStart { get; set; }

        /// <summary>
        /// Gets or sets the first IMS scan this feature was seen in.
        /// </summary>
        public double ImsScanEnd { get; set; }

        /// <summary>
        /// Gets or sets the start drift time for this feature.
        /// </summary>
        public double DriftTimeStart { get; set; }

        /// <summary>
        /// Gets or sets the end drift time for this feature.
        /// </summary>
        public double DriftTimeEnd { get; set; }

        /// <summary>
        /// Gets or sets the drift time of a feature.
        /// </summary>
        /// <remarks>Typically an integer for the TOF scan number</remarks>
        public double DriftTime { get; set; }

        /// <summary>
        /// Gets or sets the drift time of a feature.
        /// </summary>
        public double DriftTimeAligned { get; set; }

        /// <summary>
        /// Gets or sets the charge state of a feature.
        /// </summary>
        public int ChargeState { get; set; }

        /// <summary>
        /// Gets or sets the score value for this feature.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the ambiguity score for a given cluster.
        /// </summary>
        public double AmbiguityScore { get; set; }
        /// <summary>
        /// Gets or sets the group id (e.g. dataset) this feature originated from.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Compares the aligned monoisotopic mass of two Features
        /// </summary>
        public static Comparison<FeatureLight> MassComparison = delegate(FeatureLight x, FeatureLight y)
        {
            return x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
        };

        /// <summary>
        /// Compares the aligned monoisotopic mass of two Features
        /// </summary>
        public static Comparison<FeatureLight> MassAlignedComparison = delegate(FeatureLight x, FeatureLight y)
        {
            return x.MassMonoisotopicAligned.CompareTo(y.MassMonoisotopicAligned);
        };

        public static double ComputeMonoisotopicMassFromMz(double mz, int chargeState)
        {
            var charge = Convert.ToDouble(chargeState);
            return (mz * charge) - (SubAtomicParticleLibrary.MASS_PROTON * charge);
        }

        public virtual double GetSeparationValue(SeparationTypes type)
        {
            double value = 0.0;
            switch (type)
            {
                case SeparationTypes.LC:
                    value = this.Net;
                    break;
                case SeparationTypes.DriftTime:
                    value = this.DriftTime;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return value;
        }

        public virtual void SetSeparationValue(SeparationTypes type, double value, bool aligned = false)
        {
            switch (type)
            {
                case SeparationTypes.LC:
                    if (aligned)
                    {
                        this.NetAligned = value;
                    }
                    else
                    {
                        this.Net = value;
                    }
                    break;
                case SeparationTypes.DriftTime:
                    if (aligned)
                    {
                        this.DriftTimeAligned = value;
                    }
                    else
                    {
                        this.DriftTime = value;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #region Public Utility Functions
        /// <summary>
        /// Computes the mass difference in parts per million (ppm) for two given masses.
        /// </summary>
        /// <param name="massX">Mass of feature X.</param>
        /// <param name="massY">Mass of feature Y.</param>
        /// <returns>Mass difference in parts per million (ppm).</returns>
        public static double ComputeMassPPMDifference(double massX, double massY)
        {
            return (massX - massY) * 1e6 / massX;
        }

        /// <summary>
        /// Computes the mass difference in parts per million (ppm) for two given masses.
        /// </summary>
        /// <param name="massX">Mass of feature X.</param>
        /// <param name="ppm"></param>
        /// <returns>Mass difference in parts per million (ppm).</returns>
        public static double ComputeDaDifferenceFromPPM(double massX, double ppm)
        {
            return massX - (ppm * 1e-6 * massX);
        }
        #endregion

        #region Overriden Base Methods
        /// <summary>
        /// Returns a basic string representation of the cluster.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Feature Light ID = " + Id +
                    " Mono Mass = " + MassMonoisotopic +
                    " Retention Time = " + Net +
                    " Drift Time = " + DriftTime;
        }
        /// <summary>
        /// Compares two objects' values.
        /// </summary>
        /// <param name="obj">Other to compare with.</param>
        /// <returns>True if values are the same, false if not.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as FeatureLight;
            if (other == null)
                return false;

            if (!Id.Equals(other.Id))
            {
                return false;
            }
            if (!Abundance.Equals(other.Abundance))
            {
                return false;
            }
            if (!ChargeState.Equals(other.ChargeState))
            {
                return false;
            }
            if (!DriftTime.Equals(other.DriftTime))
            {
                return false;
            }
            if (!MassMonoisotopic.Equals(other.MassMonoisotopic))
            {
                return false;
            }
            if (!Net.Equals(other.Net))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Generates a hash code.
        /// </summary>
        /// <returns>Hash code based on stored data.</returns>
        public override int GetHashCode()
        {
            var hashCode =
                Abundance.GetHashCode() ^
                ChargeState.GetHashCode() ^
                DriftTime.GetHashCode() ^
                Id.GetHashCode() ^
                Net.GetHashCode();
            return hashCode;
        }
        #endregion
    }
}
