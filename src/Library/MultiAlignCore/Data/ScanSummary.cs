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
        public double Bpi
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the base peak m/z value.
        /// </summary>
        public double BpiMz
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the MS level (1, 2, ..., n).
        /// </summary>
        public int MsLevel
        {
            get;
            set;
        }       
        public double PrecursorMz  
        {   
            get;
            set;
        }
        public CollisionType CollisionType
        {
            get;
            set;
        }       
        /// <summary>
        /// Gets or sets the scan time in seconds.
        /// </summary>
        public double Time
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the scan number.
        /// </summary>
        public int Scan
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the total ion current of the scan.
        /// </summary>
        public double TotalIonCurrent
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the number of peaks.
        /// </summary>
        public int NumberOfPeaks
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the number of deisotoped features.
        /// </summary>
        public int NumberOfDeisotoped
        {
            get;
            set;
        }
    }    
}
