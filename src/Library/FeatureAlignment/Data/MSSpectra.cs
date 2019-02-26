using System;
using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using FeatureAlignment.Data.Peaks;

namespace FeatureAlignment.Data
{
    /// <summary>
    /// Contains MSn data for a given parent m/z.
    /// </summary>
    public class MSSpectra: IDisposable
    {
        /// <summary>
        /// The default MSn level (MS/MS).
        /// </summary>
        private const int CONST_DEFAULT_MS_LEVEL = 1;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MSSpectra ()
        {

            ScanMetaData            = new ScanSummary();
            MsLevel                 = CONST_DEFAULT_MS_LEVEL;
            CollisionType           = CollisionType.None;
            Scan                    = 0;
            TotalIonCurrent         = 0;
            PrecursorMz             = 0;
            GroupId                 = -1;
            PrecursorChargeState    = -1;
            Id                      = -1;
            Peaks                   = new List<XYData>();
            PeaksProcessed          = new List<ProcessedPeak>();
            PrecursorPeak           = null;
            ParentSpectra           = null;
            Peptides                = new List<Peptide>();
        }

        #region Properties
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the retention time.
        /// </summary>
        public double RetentionTime { get; set; }
        /// <summary>
        /// Gets or sets the normalized elution time of this spectrum.
        /// </summary>
        public double Net { get; set; }

        /// <summary>
        /// Gets or sets the scan value for this spectra.
        /// </summary>
        public int Scan
        {
            get;
            set;
        }

        public MSFeatureLight ParentFeature { get; set; }

        /// <summary>
        /// Gets or sets what group (or dataset) this spectra came from.
        /// </summary>
        public int GroupId
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the charge state for this spectra.
        /// </summary>
        public int PrecursorChargeState
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the MS Level.
        /// </summary>
        public int MsLevel
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the base peak.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public ProcessedPeak BasePeak
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the spectra for this MS level as x,y data points.
        /// </summary>
        public List<XYData> Peaks
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets processed peaks asspcoated with this spectra.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public List<ProcessedPeak> PeaksProcessed
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets any n + 1 level MSn child spectra.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public List<MSSpectra> ChildSpectra
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the parent spectra if MSLevel > 2.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public MSSpectra ParentSpectra { get; set; }
        /// <summary>
        /// Gets or sets the collision type.
        /// </summary>
        public CollisionType CollisionType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the total ion current.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public double TotalIonCurrent
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the parent precursor M/Z for this MSn spectra.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public double PrecursorMz
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the more accurate parent precursor M/Z for this MSn spectra.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public ProcessedPeak PrecursorPeak
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets information about the scan from the instrument vendor's file.
        /// </summary>
        public ScanSummary ScanMetaData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the peptide was associated with.
        /// </summary>
        public List<Peptide> Peptides
        {
            get;
            set;
        }
        #endregion


        #region Overriden Base Methods
        /// <summary>
        /// Returns a basic string representation of the cluster.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ID = {0} Scan = {1} Precursor = {2}  Charge {3} Group = {4}",
                                Id,
                                Scan,
                                PrecursorMz,
                                PrecursorChargeState,
                                GroupId);

        }
        /// <summary>
        /// Compares two objects' values.
        /// </summary>
        /// <param name="obj">Other to compare with.</param>
        /// <returns>True if values are the same, false if not.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as MSSpectra;
            if (other == null)
                return false;

            if (!GroupId.Equals(other.GroupId))
            {
                return false;
            }
            if (!MsLevel.Equals(other.MsLevel))
            {
                return false;
            }
            if (!PrecursorChargeState.Equals(other.PrecursorChargeState))
            {
                return false;
            }
            if (!PrecursorMz.Equals(other.PrecursorMz))
            {
                return false;
            }
            if (!Net.Equals(other.Net))
            {
                return false;
            }
            if (!Scan.Equals(other.Scan))
            {
                return false;
            }
            if (!TotalIonCurrent.Equals(other.TotalIonCurrent))
            {
                return false;
            }
            if (!CollisionType.Equals(other.CollisionType))
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
                PrecursorMz.GetHashCode() ^
                PrecursorChargeState.GetHashCode() ^
                Scan.GetHashCode() ^
                Id.GetHashCode() ^
                GroupId.GetHashCode() ^
                TotalIonCurrent.GetHashCode() ^
                RetentionTime.GetHashCode();
            return hashCode;
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Peaks          = null;
            PeaksProcessed = null;
            PrecursorPeak  = null;
            ParentSpectra  = null;


            if (ChildSpectra == null)
                return;

            foreach (var spectra in ChildSpectra)
            {
                spectra.Dispose();
            }
            ChildSpectra = null;
        }

        #endregion
    }
}
