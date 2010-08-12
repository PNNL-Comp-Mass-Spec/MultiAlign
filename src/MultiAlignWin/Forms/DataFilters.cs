using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MultiAlignEngine.Features;
using PNNLProteomics.Filters;

namespace MultiAlignWin.Forms.Filters
{
    public partial class DataFilters : Form
    {
        /// <summary>
        /// Cluster Filters
        /// </summary>
        private UMCClusterFilters   m_clusterFilters;
        /// <summary>
        /// UMC Filters
        /// </summary>
        private UMCFilters          m_umcFilters;

        /// <summary>
        /// Constructor that initializes the filters.
        /// </summary>
        /// <param name="umcFilters"></param>
        /// <param name="clusterFilters"></param>
        public DataFilters(UMCFilters umcFilters, UMCClusterFilters clusterFilters)
        {
            InitializeComponent();

            m_clusterFilters    = clusterFilters;
            m_umcFilters        = umcFilters;

            UpdateUserInterface();
        }    
        
        /// <summary>
        /// Gets a list of the UMC Filters.
        /// </summary>
        /// <returns></returns>
        public UMCFilters UMC
        {
            get
            {
                return m_umcFilters;
            }
        }
        /// <summary>
        /// Gets a list of UMC Cluster Filters
        /// </summary>
        public UMCClusterFilters Clusters
        {
            get
            {
                return m_clusterFilters;
            }
        }       

        #region Form Event Handlers 
        void DataFilters_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = false;
                Hide();
            }
        }     
        #endregion
        /// <summary>
        /// Retrieves the settings from the user interface and stores them in the filter objects.
        /// </summary>
        public void UpdateUserInterface()
        {
            /// 
            /// Features
            ///             
            m_maxUMCScore.Value             = Convert.ToDecimal(UMC.ScoreFilter.Maximum);                
            m_minUMCScore.Value             = Convert.ToDecimal(UMC.ScoreFilter.Minimum);                
            scoreCheckbox.Checked           = UMC.ScoreFilter.Active;

            m_maxUMCScan.Value              = Convert.ToDecimal(UMC.ScanFilter.Maximum);                
            m_minUMCScan.Value              = Convert.ToDecimal(UMC.ScanFilter.Minimum);                 
            scanCheckbox.Checked            = UMC.ScanFilter.Active;

            m_maxUMCNET.Value               = Convert.ToDecimal(UMC.NetFilter.Maximum); 
            m_minUMCNET.Value               = Convert.ToDecimal(UMC.NetFilter.Minimum);         
            netAlignedCheckbox.Checked      = UMC.NetFilter.Active;

            m_maxUMCMass.Value              = Convert.ToDecimal(UMC.MassFilter.Maximum);                
            m_minUMCMass.Value              = Convert.ToDecimal(UMC.MassFilter.Minimum);                 
            UMC.MassFilter.Active           = massCheckbox.Checked;

            m_maxUMCMassCalibrated.Value    = Convert.ToDecimal(UMC.MassCalibratedFilter.Maximum);                  
            m_minUMCMassCalibrated.Value    = Convert.ToDecimal(UMC.MassCalibratedFilter.Minimum);                  
            UMC.MassCalibratedFilter.Active = massCalibratedCheckbox.Checked;

            m_maxUMCMSFeatureCount.Value    = Convert.ToDecimal(UMC.FeatureCountFilter.Maximum);                
            m_minUMCMSFeatureCount.Value    = Convert.ToDecimal(UMC.FeatureCountFilter.Minimum);                  
            msFeatureCountCheckbox.Checked  = UMC.FeatureCountFilter.Active;

            m_maxUMCDriftTime.Value         = Convert.ToDecimal(UMC.DriftTimeFilter.Maximum);                
            m_minUMCDriftTime.Value         = Convert.ToDecimal(UMC.DriftTimeFilter.Minimum);                  
            driftTimeCheckbox.Checked       = UMC.DriftTimeFilter.Active;

            m_maxUMCCharge.Value            = Convert.ToDecimal(UMC.ChargeStateFilter.Maximum);  
            m_minUMCChargeState.Value       = Convert.ToDecimal(UMC.ChargeStateFilter.Minimum);            
            chargeStateCheckbox.Checked     = UMC.ChargeStateFilter.Active;

            m_maxUMCAbundanceMax.Value      = Convert.ToDecimal(UMC.AbundanceMaxFilter.Maximum);                
            m_minUMCAbundanceMax.Value      = Convert.ToDecimal(UMC.AbundanceMaxFilter.Minimum); 
            abundanceMaxCheckbox.Checked    = UMC.AbundanceMaxFilter.Active;                 

            m_maxUMCAbundanceSum.Value      = Convert.ToDecimal(UMC.AbundanceSumFilter.Maximum);
            m_minUMCAbundanceSum.Value      = Convert.ToDecimal(UMC.AbundanceSumFilter.Minimum);                 
            abundanceSumCheckbox.Checked    = UMC.AbundanceSumFilter.Active;                

            /// 
            /// Clusters
            ///             
            m_minClusterDriftTime.Value         = Convert.ToDecimal(Clusters.DriftTime.Minimum);
            m_maxClusterDriftTime.Value         = Convert.ToDecimal(Clusters.DriftTime.Maximum);            
            clusterDriftTimeCheckbox.Checked    = Clusters.DriftTime.Active;  

            m_minClusterMass.Value              = Convert.ToDecimal(Clusters.Mass.Minimum);                 
            m_maxClusterMass.Value              = Convert.ToDecimal(Clusters.Mass.Maximum);
            clusterMassCheckbox.Checked         = Clusters.Mass.Active;
            
            m_minClusterSize.Value              = Convert.ToDecimal(Clusters.MemberCount.Minimum);            
            m_maxClusterSize.Value              = Convert.ToDecimal(Clusters.MemberCount.Maximum);                                 
            clusterSizeCheckbox.Checked         = Clusters.MemberCount.Active; 

            m_minClusterNET.Value               = Convert.ToDecimal(Clusters.NET.Minimum);
            m_maxClusterNET.Value               = Convert.ToDecimal(Clusters.NET.Maximum);                
            clusterNETCheckbox.Checked          = Clusters.NET.Active; 
                
            m_minClusterScoreMean.Value         = Convert.ToDecimal(Clusters.ScoreMean.Minimum);                 
            m_maxClusterScoreMean.Value         = Convert.ToDecimal(Clusters.ScoreMean.Maximum);                
            clusterScoreMean.Checked            = Clusters.ScoreMean.Active;

            m_minClusterScoreMedian.Value       = Convert.ToDecimal(Clusters.ScoreMedian.Minimum);
            m_maxClusterScoreMedian.Value       = Convert.ToDecimal(Clusters.ScoreMedian.Maximum);                
            clusterScoreMedian.Checked          = Clusters.ScoreMedian.Active;
                
        }

        /// <summary>
        /// Grabs all the data from the user interface and updates each filter.
        /// </summary>
        private void UpdateFilters()
        {

            /// 
            /// Features
            ///             
            UMC.ScoreFilter.Maximum = Convert.ToDouble(m_maxUMCScore.Value);
            UMC.ScoreFilter.Minimum = Convert.ToDouble(m_minUMCScore.Value);
            UMC.ScoreFilter.Active  = scoreCheckbox.Checked;


            UMC.ScanFilter.Maximum  = Convert.ToInt32(m_maxUMCScan.Value);
            UMC.ScanFilter.Minimum  = Convert.ToInt32(m_minUMCScan.Value);
            UMC.ScanFilter.Active   = scanCheckbox.Checked;

            UMC.NetFilter.Maximum   = Convert.ToDouble(m_maxUMCNET.Value);
            UMC.NetFilter.Minimum   = Convert.ToDouble(m_minUMCNET.Value);
            UMC.NetFilter.Active    = netAlignedCheckbox.Checked;

            UMC.MassFilter.Maximum  = Convert.ToDouble(m_maxUMCMass.Value);
            UMC.MassFilter.Minimum  = Convert.ToDouble(m_minUMCMass.Value);
            UMC.MassFilter.Active   = massCheckbox.Checked;

            UMC.MassCalibratedFilter.Maximum    = Convert.ToDouble(m_maxUMCMassCalibrated.Value);
            UMC.MassCalibratedFilter.Minimum    = Convert.ToDouble(m_minUMCMassCalibrated.Value);
            UMC.MassCalibratedFilter.Active     = massCalibratedCheckbox.Checked;

            UMC.FeatureCountFilter.Maximum  = Convert.ToInt32(m_maxUMCMSFeatureCount.Value);
            UMC.FeatureCountFilter.Minimum  = Convert.ToInt32(m_minUMCMSFeatureCount.Value);
            UMC.FeatureCountFilter.Active   = msFeatureCountCheckbox.Checked;

            UMC.DriftTimeFilter.Maximum     = Convert.ToDouble(m_maxUMCDriftTime.Value);
            UMC.DriftTimeFilter.Minimum     = Convert.ToDouble(m_minUMCDriftTime.Value);
            UMC.DriftTimeFilter.Active      = driftTimeCheckbox.Checked;

            UMC.ChargeStateFilter.Maximum   = Convert.ToInt32(m_maxUMCCharge.Value);
            UMC.ChargeStateFilter.Minimum   = Convert.ToInt32(m_minUMCChargeState.Value);
            UMC.ChargeStateFilter.Active    = chargeStateCheckbox.Checked;

            UMC.AbundanceMaxFilter.Maximum  = Convert.ToDouble(m_maxUMCAbundanceMax.Value);
            UMC.AbundanceMaxFilter.Minimum  = Convert.ToDouble(m_minUMCAbundanceMax.Value);
            UMC.AbundanceMaxFilter.Active   = abundanceMaxCheckbox.Checked;

            UMC.AbundanceSumFilter.Maximum  = Convert.ToDouble(m_maxUMCAbundanceSum.Value);
            UMC.AbundanceSumFilter.Minimum  = Convert.ToDouble(m_minUMCAbundanceSum.Value);
            UMC.AbundanceSumFilter.Active   = abundanceSumCheckbox.Checked;

            /// 
            /// Clusters
            /// 
            Clusters.DriftTime.Minimum  = Convert.ToDouble(m_minClusterDriftTime.Value);
            Clusters.DriftTime.Maximum  = Convert.ToDouble(m_maxClusterDriftTime.Value);
            Clusters.DriftTime.Active   = clusterDriftTimeCheckbox.Checked;

            Clusters.Mass.Minimum   = Convert.ToDouble(m_minClusterMass.Value);
            Clusters.Mass.Maximum   = Convert.ToDouble(m_maxClusterMass.Value);
            Clusters.Mass.Active    = clusterMassCheckbox.Checked;

            Clusters.MemberCount.Minimum    = Convert.ToInt32(m_minClusterSize.Value);
            Clusters.MemberCount.Maximum    = Convert.ToInt32(m_maxClusterSize.Value);
            Clusters.MemberCount.Active     = clusterSizeCheckbox.Checked;

            Clusters.NET.Minimum    = Convert.ToDouble(m_minClusterNET.Value);
            Clusters.NET.Maximum    = Convert.ToDouble(m_maxClusterNET.Value);
            Clusters.NET.Active     = clusterNETCheckbox.Checked;

            Clusters.ScoreMean.Minimum  = Convert.ToDouble(m_minClusterScoreMean.Value);
            Clusters.ScoreMean.Maximum  = Convert.ToDouble(m_maxClusterScoreMean.Value);
            Clusters.ScoreMean.Active   = clusterScoreMean.Checked;

            Clusters.ScoreMedian.Minimum    = Convert.ToDouble(m_minClusterScoreMean.Value);
            Clusters.ScoreMedian.Maximum    = Convert.ToDouble(m_maxClusterScoreMean.Value);
            Clusters.ScoreMedian.Active     = clusterScoreMedian.Checked;
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            UpdateFilters();
            Hide();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}