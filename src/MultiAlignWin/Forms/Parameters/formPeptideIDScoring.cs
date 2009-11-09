using System;
using System.Windows.Forms;
using System.ComponentModel;

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
                classSMARTOptions options = new classSMARTOptions();
                
                options.ApplyDynamicModification     = ApplyDynamicModification;
                options.ApplyModification            = ApplyModification;                
                options.IsDataPaired                 = IsDataPaired;                    
                options.UseFScores                   = UseFScores;                     
                options.MaxModifications             = MaxModifications;                
                options.MinTrypticState              = MinimumTrypticState;
                options.MinMSMSObs                   = MinimumMSMSObservations;                
                options.ModificationAA               = Convert.ToSByte(ModificationAA);
                options.MassTolerancePPM             = MassTolerancePPM;                
                options.MinPeptideProphetProbability = MinimumPeptideProphetProbability;                    
                options.ModMass                      = ModifiedMass;
                options.NETTolerance                 = NETTolerance;
                options.PairedMass                   = PairedMass;

                return options;
            }
            set
            {
                ApplyDynamicModification         = value.ApplyDynamicModification;
                ApplyModification                = value.ApplyModification;
                IsDataPaired                     = value.IsDataPaired;
                UseFScores                       = value.UseFScores;
                MaxModifications                 = value.MaxModifications;
                MinimumTrypticState              = value.MinTrypticState;
                MinimumMSMSObservations          = value.MinMSMSObs;                
                ModificationAA                   = Convert.ToString(value.ModificationAA);
                MassTolerancePPM                 = value.MassTolerancePPM;
                MinimumPeptideProphetProbability = value.MinPeptideProphetProbability;
                ModifiedMass                     = value.ModMass;
                NETTolerance                     = value.NETTolerance;
                PairedMass                       = value.PairedMass;                
            }
        }
        #endregion

        #region Char/String Properties
        /// <summary>
        /// Gets or sets the modification AA value.
        /// </summary>
        public string ModificationAA
        {
            get
            {
                return mtextBox_modificationChar.Text;
            }
            set
            {
                mtextBox_modificationChar.Text = value;
            }
        }
        #endregion

        #region Short/Integer Properties
        /// <summary>
        /// Gets or sets the maximum number of modifications.
        /// </summary>
        public short MaxModifications
        {
            get
            {
                return Convert.ToInt16(mnum_maxModifications.Value);
            }
            set
            {
                mnum_maxModifications.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets the minimum tryptic state
        /// </summary>
        public short MinimumTrypticState
        {
            get
            {
                return Convert.ToInt16(mnum_minimumTrypticState.Value);
            }
            set
            {
                mnum_minimumTrypticState.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets the minimum number of MS/MS Observations
        /// </summary>
        public int MinimumMSMSObservations
        {
            get
            {
                return Convert.ToInt32(mnum_minimumMSMSObservations.Value);
            }
            set
            {
                mnum_minimumMSMSObservations.Value = Convert.ToDecimal(value);
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
        /// Gets or sets the minimum peptide prophet probability.
        /// </summary>
        public double MinimumPeptideProphetProbability
        {
            get
            {
                return Convert.ToDouble(mnum_minimumPeptideProphetProbability.Value);
            }
            set
            {
                mnum_minimumPeptideProphetProbability.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets the modified mass value.
        /// </summary>
        public double ModifiedMass
        {
            get
            {
                return Convert.ToDouble(mnum_modifiedMass.Value);
            }
            set
            {
                mnum_modifiedMass.Value = Convert.ToDecimal(value);
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
        /// Gets or sets to check the apply dynamic modification checkbox.
        /// </summary>
        public bool ApplyDynamicModification
        {
            get
            {
                return mcheckBox_applyDynamicModification.Checked;
            }
            set
            {
                mcheckBox_applyDynamicModification.Checked = value;
            }
        }
        /// <summary>
        /// Gets or sets to check the apply modification checkbox.
        /// </summary>
        public bool ApplyModification
        {
            get
            {
                return mcheckBox_applyModification.Checked;
            }
            set
            {
                mcheckBox_applyModification.Checked = value;
            }
        }
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
        public bool UseFScores
        {
            get
            {
                return mcheckBox_useFScores.Checked;
            }
            set
            {
                mcheckBox_useFScores.Checked = value;
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
        #endregion
    }
}