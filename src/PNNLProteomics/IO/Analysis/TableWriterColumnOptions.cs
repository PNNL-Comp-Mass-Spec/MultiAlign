using System;

namespace PNNLProteomics.IO
{
    /// <summary>
    /// Holds flags indicating whether certain columns of data should be exported.
    /// </summary>
    public class TableWriterColumnOptions
    {
        #region Members
        /// <summary>
        /// Flag indicating whether to output the mass of the feature.
        /// </summary>
        private bool m_mass_colum;
        /// <summary>
        /// Flag indicating whether to output the calibrated mass of the feature.
        /// </summary>
        private bool m_calibrated_mass_colum;
        /// <summary>
        /// Flag indicating whether to output the scan of the feature.
        /// </summary>
        private bool m_scan_colum;
        /// <summary>
        /// Flag indicating whether to output the aligned scan of the feature.
        /// </summary>
        private bool m_aligned_scan_colum;
        /// <summary>
        /// Flag indicating whether to output the umc index of the feature.
        /// </summary>
        private bool m_umc_index_column;
        /// <summary>
        /// Flag indicating whether to output the drift time of the feature.
        /// </summary>
        private bool m_driftTime_column;
        /// <summary>
        /// Flag indicating whether to output the max abundance of the feature.
        /// </summary>
        private bool m_abundance_max;
        /// <summary>
        /// Flag indicating whether to output the summed abundance of the feature.
        /// </summary>
        private bool m_abundance_sum;
        /// <summary>
        /// Flag indicating whether to output the MS feature count.
        /// </summary>
        private bool m_msFeatureCount;
        /// <summary>
        /// Flag indicating whether to output charge mass class abundance values (abundances at each charge state).
        /// </summary>
        private bool m_cmc;
        /// <summary>
        /// Flag indicating whether to output the charge state for each UMC.
        /// </summary>
        private bool m_chargeState;
        #endregion

        #region Constructors
        public TableWriterColumnOptions()
        {
            Clear();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resets all the flags to their defaults.
        /// </summary>
        public void Clear()
        {
            m_mass_colum            = true;
            m_calibrated_mass_colum = true;
            m_scan_colum            = true;
            m_aligned_scan_colum    = true;
            m_umc_index_column      = true;
            m_driftTime_column      = true;
            m_abundance_max         = true;
            m_abundance_sum         = true;
            m_msFeatureCount        = true;
            m_cmc                   = false;
            m_chargeState           = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mass flag.
        /// </summary>
        public bool Mass
        {
            get
            {
                return m_mass_colum;
            }
            set
            {
                m_mass_colum = value;
            }
        }
        /// <summary>
        /// Gets or sets the calibrated mass flag.
        /// </summary>
        public bool MassCalibrated
        {
            get
            {
                return m_calibrated_mass_colum;
            }
            set
            {
                m_calibrated_mass_colum = value;
            }
        }
        /// <summary>
        /// Gets or sets the scan flag.
        /// </summary>
        public bool Scan
        {
            get
            {
                return m_scan_colum;
            }
            set
            {
                m_scan_colum = value;
            }
        }
        /// <summary>
        /// Gets or sets the scan aligned flag.
        /// </summary>
        public bool NET
        {
            get
            {
                return m_aligned_scan_colum;
            }
            set
            {
                m_aligned_scan_colum = value;
            }
        }
        /// <summary>
        /// Gets or sets the UMC index flag.
        /// </summary>
        public bool UMCIndex
        {
            get
            {
                return m_umc_index_column;
            }
            set
            {
                m_umc_index_column = value;
            }
        }
        /// <summary>
        /// Gets or sets the drift time flag.
        /// </summary>
        public bool DriftTime
        {
            get
            {
                return m_driftTime_column;
            }
            set
            {
                m_driftTime_column = value;
            }
        }
        /// <summary>
        /// Gets or sets the Abundance max flag.
        /// </summary>
        public bool AbundanceMax
        {
            get
            {
                return m_abundance_max;
            }
            set
            {
                m_abundance_max = value;
            }
        }
        /// <summary>
        /// Gets or sets the abundance sum flag.
        /// </summary>
        public bool AbundanceSum
        {
            get
            {
                return m_abundance_sum;
            }
            set
            {
                m_abundance_sum = value;
            }
        }
        /// <summary>
        /// Gets or sets the ms feature count flag.
        /// </summary>
        public bool MSFeatureCount
        {
            get
            {
                return m_msFeatureCount;
            }
            set
            {
                m_msFeatureCount= value;
            }
        }
        /// <summary>
        /// Gets or sets the charge mass class flag.
        /// </summary>
        public bool CMCAbundance
        {
            get
            {
                return m_cmc;
            }
            set
            {
                m_cmc = value;
            }
        }
        /// <summary>
        /// Gets or sets the charge state flag.
        /// </summary>
        public bool ChargeState
        {
            get
            {
                return m_chargeState;
            }
            set
            {
                m_chargeState = value;
            }
        }
        #endregion
    }

}
