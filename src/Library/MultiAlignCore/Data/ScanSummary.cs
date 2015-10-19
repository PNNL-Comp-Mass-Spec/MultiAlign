namespace MultiAlignCore.Data
{    
    /// <summary>
    /// Encapsulates MS Spectrum summary information (e.g. BPI, # of peaks, TIC)
    /// </summary>
    public sealed class ScanSummary
    {
        /// <summary>
        /// Gets or sets the base peak intensity
        /// </summary>
        public double Bpi { get; set; }

        /// <summary>
        /// Gets or sets the base peak m/z value.
        /// </summary>
        public double BpiMz { get; set; }

        /// <summary>
        /// Gets or sets the MS level (1, 2, ..., n).
        /// </summary>
        public int MsLevel { get; set; }

        public double PrecursorMz { get; set; }

        public CollisionType CollisionType { get; set; }

        /// <summary>
        /// Gets or sets the NET value for this scan.
        /// </summary>
        public double Net { get; set; }

        /// <summary>
        /// Gets or sets the scan time in seconds.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Gets or sets the scan number.
        /// </summary>
        public int Scan { get; set; }

        /// <summary>
        /// Gets or sets the total ion current of the scan.
        /// </summary>
        public double TotalIonCurrent { get; set; }

        /// <summary>
        /// Gets or sets the number of peaks.
        /// </summary>
        public int NumberOfPeaks { get; set; }

        /// <summary>
        /// Gets or sets the number of deisotoped features.
        /// </summary>
        public int NumberOfDeisotoped { get; set; }

        /// <summary>
        /// The Dataset id number (for use in the database)
        /// </summary>
        public int DatasetId { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as ScanSummary;

            if (other == null)
            {
                return false;
            }
            if (DatasetId.Equals(other.DatasetId) && Scan.Equals(other.Scan))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash * 23 + DatasetId.GetHashCode();
            hash = hash * 23 + Scan.GetHashCode();

            return hash;
        }
    }    
}
