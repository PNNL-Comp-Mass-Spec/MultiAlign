using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.IO.TextFiles
{
    public class DeconToolsIsosFilterOptions
    {

        private double mMaximumIsotopicFit;
        private double mAbundanceMinimum;
        private double mAbundanceMaximum;
        private int mMaximumDataPoints;
        private int mLCScanStart;
        private int mLCScanEnd;

        /// <summary>
        /// Maximum isotopic fit value to allow
        /// </summary>
        /// <remarks>If 0 or negative, isotopic fit filtering is not applied</remarks>
        public double MaximumIsotopicFit
        {
            get
            {
                return mMaximumIsotopicFit;
            }
            set
            {
                mMaximumIsotopicFit = value;
                UpdateFilters();
            }
        }

        /// <summary>
        /// Minimum abundance value
        /// </summary>
        public double AbundanceMinimum
        {
            get
            {
                return mAbundanceMinimum;
            }
            set
            {
                mAbundanceMinimum = value;
                UpdateFilters();
            }

        }

        /// <summary>
        /// Maximum abundance value
        /// </summary>
        /// <remarks>If 0 or negative, abundance filtering is not applied.  Filtering is also skipped if AbundanceMinimum > AbundanceMaximum</remarks>
        public double AbundanceMaximum
        {
            get
            {
                return mAbundanceMaximum;
            }
            set
            {
                mAbundanceMaximum = value;
                UpdateFilters();
            }

        }

        /// <summary>
        /// Maximum number of data points to load (lower abundance data is discarded)
        /// </summary>
        public int MaximumDataPoints
        {
            get
            {
                return mMaximumDataPoints;
            }
            set
            {
                mMaximumDataPoints = value;
                UpdateFilters();
            }
        }

        /// <summary>
        /// Minimum scan number to retain
        /// </summary>
        public int LCScanStart
        {
            get
            {
                return mLCScanStart;
            }
            set
            {
                mLCScanStart = value;
                UpdateFilters();
            }
        }

        /// <summary>
        /// Maximum scan number to retain
        /// </summary>
        /// <remarks>If 0 or negative, scan filtering is not applied.  Filtering is also skipped if scanStart > ScanEnd</remarks>
        public int LCScanEnd
        {
            get
            {
                return mLCScanEnd;
            }
            set
            {
                mLCScanEnd = value;
                UpdateFilters();
            }
        }

        /// <summary>
        /// True when the scan filter should be used because ScanStart is less than or equal to ScanEnd and ScanEnd is greater than 0
        /// </summary>
        public bool UseScanFilter { get; private set; }

        /// <summary>
        /// True when the abundance filter should be used because AbundanceMinimum is less than AbundanceMaximum and AbundanceMaximum is greater than 0
        /// </summary>
        public bool UseAbundanceFilter { get; private set; }

        /// <summary>
        /// True when the isotopic fit filter should be used because MaximumIsotopicFit is greater than 0
        /// </summary>
        public bool UseIsotopicFitFilter { get; private set; }

        /// <summary>
        /// True when the data count filter should be used because MaximumDataPoints is greater than 0
        /// </summary>
        public bool UseDataCountFilter { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DeconToolsIsosFilterOptions()
        {
            MaximumIsotopicFit = 0;
            AbundanceMinimum = 0;
            AbundanceMaximum = 0;
            LCScanStart = 0;
            LCScanEnd = 0;
            MaximumDataPoints = 0;

            UpdateFilters();
        }

        private void UpdateFilters()
        {

            UseScanFilter = (LCScanStart <= LCScanEnd && LCScanEnd > 0);

            UseAbundanceFilter = (AbundanceMinimum < AbundanceMaximum && AbundanceMaximum > 0);

            UseIsotopicFitFilter = (MaximumIsotopicFit > 0);

            UseDataCountFilter = (MaximumDataPoints > 0);

        }
    }
}
