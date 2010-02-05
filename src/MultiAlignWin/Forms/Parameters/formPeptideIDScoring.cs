using System;
using System.Windows.Forms;
using System.ComponentModel;

//using PNNLProteomics.DataSMART;
using PNNLProteomics.SMART;

namespace MultiAlignWin.Forms.Parameters
{   
    /// <summary>
    /// Class that displays the options for the SMART parameters.
    /// </summary>
    public partial class formPeptideIDScoring : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public formPeptideIDScoring()
        {
            InitializeComponent();
        }

        #region Options Property
        /// <summary>
        /// Gets or sets the SMART Options used.
        /// </summary>
        public classSMARTOptions Options
        {
            get
            {
                classSMARTOptions options            = new classSMARTOptions();                              
                options.IsDataPaired                 = IsDataPaired;                    
                options.UsePriorProbabilities        = UsePriorProbabilities;                                     
                options.MassTolerancePPM             = MassTolerancePPM;                                
                options.NETTolerance                 = NETTolerance;
                options.PairedMass                   = PairedMass;
                return options;
            }
            set
            {
                IsDataPaired                     = value.IsDataPaired;
                UsePriorProbabilities            = value.UsePriorProbabilities;                
                MassTolerancePPM                 = value.MassTolerancePPM;
                NETTolerance                     = value.NETTolerance;
                PairedMass                       = value.PairedMass;                
            }
        }
        #endregion

        #region Double Properties
        /// <summary>
        /// Gets or sets the mass tolerance in PPM
        /// </summary>
        public double MassTolerancePPM
        {
            get
            {
                return Convert.ToDouble(mnum_massTolerance.Value);
            }
            set
            {
                mnum_massTolerance.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets the NET tolerance value.
        /// </summary>
        public double NETTolerance
        {
            get
            {
                return Convert.ToDouble(mnum_netTolerance.Value);
            }
            set
            {
                mnum_netTolerance.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets the paired mass value.
        /// </summary>
        public double PairedMass
        {
            get
            {
                return Convert.ToDouble(mnum_pairedMass.Value);
            }
            set
            {
                mnum_pairedMass.Value = Convert.ToDecimal(value);
            }
        }
        #endregion

        #region Boolean Properties
        /// <summary>
        /// Gets or sets to check the is data paired checkbox.
        /// </summary>
        public bool IsDataPaired
        {
            get
            {
                return mcheckBox_isDataPaired.Checked;
            }
            set
            {
                mcheckBox_isDataPaired.Checked = value;
            }
        }
        /// <summary>
        /// Gets or sets to check the use F scores checkbox for SEQUEST id confidence measures.
        /// </summary>
        public bool UsePriorProbabilities
        {
            get
            {
                return mcheckBox_usePriorProbabilities.Checked;
            }
            set
            {
                mcheckBox_usePriorProbabilities.Checked = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Constructs a new options object, and sets the values via a call to the 
        /// options property with the new object's settings (default from the DLL).
        /// </summary>
        /// <param name="sender">Default Button.</param>
        /// <param name="e">Event arguments.</param>
        private void mbutton_defaults_Click(object sender, EventArgs e)
        {
            classSMARTOptions options = new classSMARTOptions();
            Options = options;
        }
        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SaveOptions();
            Hide();
        }
        public void SaveOptions()
        {
            Properties.Settings.Default.SMARTIsDataPaired           = IsDataPaired;
            Properties.Settings.Default.SMARTMassTolerance          = MassTolerancePPM;
            Properties.Settings.Default.SMARTNETTolerance           = NETTolerance;
            Properties.Settings.Default.SMARTUsePriorProbabilities  = UsePriorProbabilities;            
            Properties.Settings.Default.Save();
        }
        #endregion        
    }
}