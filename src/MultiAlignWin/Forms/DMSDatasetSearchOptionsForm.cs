using System;
using System.Windows.Forms;
using System.Collections.Generic;

using PNNLProteomics.Data;
using MultiAlignEngine;
using MultiAlignWin.IO;

namespace MultiAlignWin.Forms
{
    public partial class DMSDatasetSearchOptionsForm : Form
    {        
        public DMSDatasetSearchOptionsForm(DMSDatasetSearchOptions options)
        {
            InitializeComponent();

            m_datetime.MaxDate = DateTime.MaxValue;
            m_datetime.MinDate = DateTime.MinValue;

            UpdateUserInterface(options);
            FormClosing += new FormClosingEventHandler(DMSDatasetSearchOptionsForm_FormClosing);
        }

        void DMSDatasetSearchOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult = DialogResult.Cancel;
                e.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// Updates the user interface with the data provided.
        /// </summary>
        /// <param name="options"></param>
        private void UpdateUserInterface(DMSDatasetSearchOptions options)
        {

            m_datasetNameTextbox.Text    = options.DatasetName;
            m_instrumentNameTextbox.Text = options.InstrumentName;            
            m_datetime.Value             = options.DateTime;
            m_parameterFileTextbox.Text  = options.ParameterFileName;
            m_decon2lsCheckBox.Checked          = false;
            m_decon2lsAgilentCheckBox.Checked   = false;
            m_icr2lsCheckBox.Checked            = false;
            m_lcmsFeatureFinderCheckBox.Checked = false;
            m_ltqftpekCheckBox.Checked          = false;
            m_decon2lsVsCheckbox.Checked        = false;

            m_fileExtensionTextbox.Text = options.FileExtension;

            foreach (DeisotopingTool id in options.ToolIDs)
            {
                switch (id)
                {
                    case DeisotopingTool.Decon2ls:
                        m_decon2lsCheckBox.Checked = true;
                        break;
                    case DeisotopingTool.Decon2lsAgilent:
                        m_decon2lsAgilentCheckBox.Checked = true;
                        break;
                    case DeisotopingTool.ICR2ls:
                        m_icr2lsCheckBox.Checked = true;
                        break;
                    case DeisotopingTool.LCMSFeatureFinder:
                        m_lcmsFeatureFinderCheckBox.Checked = true;
                        break;
                    case DeisotopingTool.LTQ_FTPek:
                        m_ltqftpekCheckBox.Checked = true;
                        break;
                    case DeisotopingTool.Decon2ls_V2:
                        m_decon2lsVsCheckbox.Checked = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the search options from the user interface.
        /// </summary>
        public DMSDatasetSearchOptions Options
        {
            get
            {
                DMSDatasetSearchOptions options = new DMSDatasetSearchOptions();
                options.DatasetName             = m_datasetNameTextbox.Text;
                options.InstrumentName          = m_instrumentNameTextbox.Text;                
                options.DateTime                = m_datetime.Value;
                options.ParameterFileName       = m_parameterFileTextbox.Text;
                options.ToolIDs.Clear();

                if (m_decon2lsVsCheckbox.Checked == true)
                {
                    options.ToolIDs.Add(DeisotopingTool.Decon2ls_V2);
                }                
                if (m_decon2lsCheckBox.Checked == true)
                {
                    options.ToolIDs.Add(DeisotopingTool.Decon2ls);
                }                
                if (m_decon2lsAgilentCheckBox.Checked == true)
                {
                    options.ToolIDs.Add(DeisotopingTool.Decon2lsAgilent);
                }                
                if (m_icr2lsCheckBox.Checked == true)
                {
                    options.ToolIDs.Add(DeisotopingTool.ICR2ls);
                }                
                if (m_lcmsFeatureFinderCheckBox.Checked == true)
                {
                    options.ToolIDs.Add(DeisotopingTool.LCMSFeatureFinder);
                }                
                if (m_ltqftpekCheckBox.Checked == true)
                {
                    options.ToolIDs.Add(DeisotopingTool.LTQ_FTPek);
                }
                options.FileExtension = m_fileExtensionTextbox.Text;
                return options;
            }            
        }

        private void m_cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }
        private void m_okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateUserInterface(new DMSDatasetSearchOptions());
        }
    }
}