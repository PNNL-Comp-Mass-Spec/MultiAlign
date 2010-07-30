using System;
using System.Windows.Forms;

using MultiAlignWin.IO;

namespace MultiAlignWin.Forms
{
    public partial class TableWriterColumnOptionsForm : Form
    {
        /// <summary>
        /// Options for selecting column output in table form.
        /// </summary>
        private TableWriterColumnOptions m_options;

        public TableWriterColumnOptionsForm()
        {
            InitializeComponent();

            FormClosing += new FormClosingEventHandler(TableWriterColumnOptionsForm_FormClosing);
            m_options = new TableWriterColumnOptions();            
        }


        /// <summary>
        /// Gets or sets the table writer column selection options.
        /// </summary>
        public TableWriterColumnOptions Options
        {
            get
            {                
                m_options.Mass              = massCheckbox.Checked;
                m_options.MassCalibrated    = massCalibratedCheckbox.Checked;
                m_options.Scan              = scanCheckbox.Checked;                 
                m_options.NET               = netAlignedCheckbox.Checked;                 
                m_options.DriftTime         = driftTimeCheckbox.Checked;                 
                m_options.AbundanceMax      = abundanceMaxCheckbox.Checked;                 
                m_options.AbundanceSum      = abundanceSumCheckbox.Checked;             
                m_options.UMCIndex          = umcIndexCheckbox.Checked;   
                m_options.MSFeatureCount    = msFeatureCountCheckbox.Checked;
                m_options.CMCAbundance      = cmcCheckbox.Checked;
                m_options.ChargeState       = chargeStateCheckbox.Checked;
                return m_options;
            }
            set
            {
                m_options = value;
                UpdateUserInterface();
            }
        }

        private void UpdateUserInterface()
        {
            massCheckbox.Checked            = m_options.Mass;
            massCalibratedCheckbox.Checked  = m_options.MassCalibrated;
            scanCheckbox.Checked            = m_options.Scan;
            netAlignedCheckbox.Checked      = m_options.NET;
            driftTimeCheckbox.Checked       = m_options.DriftTime;
            abundanceMaxCheckbox.Checked    = m_options.AbundanceMax;
            abundanceSumCheckbox.Checked    = m_options.AbundanceSum;
            umcIndexCheckbox.Checked        = m_options.UMCIndex;
            msFeatureCountCheckbox.Checked  = m_options.MSFeatureCount;
            cmcCheckbox.Checked             = m_options.CMCAbundance;
            chargeStateCheckbox.Checked     = m_options.ChargeState;
        }

        #region Form Event Handlers 
        private void m_setToDefaultsButton_Click(object sender, EventArgs e)
        {
            m_options.Clear();
            Options = m_options;
        }

        private void m_okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }
        void TableWriterColumnOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        #endregion
    }
}