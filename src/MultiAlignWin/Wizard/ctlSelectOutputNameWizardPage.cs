using System;
using System.IO;
using System.Windows.Forms;
using PNNLProteomics.Data;

namespace MultiAlignWin
{
    public class ctlSelectOutputNameWizardPage : UserControl, MultiAlignWin.Forms.Wizard.IWizardControl<PNNLProteomics.Data.MultiAlignAnalysis>
	{
        private System.Windows.Forms.Label labelSelect;
		private System.Windows.Forms.Label labelAnalysisPath;
        private System.Windows.Forms.TextBox textBoxProjectName;
		private System.Windows.Forms.TextBox textBoxFolder;
		private System.Windows.Forms.Button mbtnAnalysisPath;
		private System.ComponentModel.IContainer components = null;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button mbutton_pickFileName; 
		private string folderName;

        /// <summary>
        /// Analysis object that we are currently building.
        /// </summary>
        private MultiAlignAnalysis m_analysis;

        public ctlSelectOutputNameWizardPage( )
        {
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Set the help text description for the FolderBrowserDialog.
			this.folderBrowserDialog1 = new FolderBrowserDialog(); 
			this.folderBrowserDialog1.Description = 
				"Select the directory that you want to use for the analysis:";

			// Do not allow the user to create new files via the FolderBrowserDialog.
			this.folderBrowserDialog1.ShowNewFolderButton = true;

			// Default to the My Documents folder.
			this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            
            /// 
            /// If its not the users first time, then make the output folder be where they last 
            /// saved their data.  Otherwise, make it the my documents of the local user.
            /// 
            textBoxFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Properties.Settings.Default.MAFirstTime == false)
            {
                if (Directory.Exists(Properties.Settings.Default.UserOutputPath) == true)
                    textBoxFolder.Text = Properties.Settings.Default.UserOutputPath;
            }
            else
            {
                Properties.Settings.Default.MAFirstTime = false;
                Properties.Settings.Default.Save();
            }
		}
        public void SetAsActivePage()
        {
            if (m_analysis!= null)
            {
                if (!string.IsNullOrWhiteSpace(m_analysis.MetaData.AnalysisName))
                {
                    textBoxProjectName.Text = m_analysis.MetaData.AnalysisName;
                }
                if (!string.IsNullOrWhiteSpace(m_analysis.MetaData.AnalysisPath))
                {
                    textBoxFolder.Text = m_analysis.MetaData.AnalysisPath;
                }
            }
        }

        /// <summary>
        /// Gets the title for the wizard page.
        /// </summary>
        public string Title
        {
            get
            {
                return "Select Output";
            }
        }
        /// <summary>
        /// Gets or sets the analysis used for this page.
        /// </summary>
        public MultiAlignAnalysis Data
        {
            get
            {
                return m_analysis;
            }
            set
            {
                m_analysis = value;
            }
        }
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.labelSelect = new System.Windows.Forms.Label();
            this.mbtnAnalysisPath = new System.Windows.Forms.Button();
            this.labelAnalysisPath = new System.Windows.Forms.Label();
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.mbutton_pickFileName = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxProjectName
            // 
            this.textBoxProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProjectName.Location = new System.Drawing.Point(184, 18);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.Size = new System.Drawing.Size(595, 20);
            this.textBoxProjectName.TabIndex = 1;
            this.textBoxProjectName.Text = "MultiAlignAnalysis";
            // 
            // labelSelect
            // 
            this.labelSelect.Location = new System.Drawing.Point(15, 15);
            this.labelSelect.Name = "labelSelect";
            this.labelSelect.Size = new System.Drawing.Size(163, 24);
            this.labelSelect.TabIndex = 0;
            this.labelSelect.Text = "Analysis Name";
            this.labelSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mbtnAnalysisPath
            // 
            this.mbtnAnalysisPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbtnAnalysisPath.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnAnalysisPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnAnalysisPath.Location = new System.Drawing.Point(804, 46);
            this.mbtnAnalysisPath.Name = "mbtnAnalysisPath";
            this.mbtnAnalysisPath.Size = new System.Drawing.Size(27, 24);
            this.mbtnAnalysisPath.TabIndex = 2;
            this.mbtnAnalysisPath.Text = "...";
            this.mbtnAnalysisPath.UseVisualStyleBackColor = false;
            this.mbtnAnalysisPath.Click += new System.EventHandler(this.mbtnAnalysisPath_Click);
            // 
            // labelAnalysisPath
            // 
            this.labelAnalysisPath.Location = new System.Drawing.Point(15, 45);
            this.labelAnalysisPath.Name = "labelAnalysisPath";
            this.labelAnalysisPath.Size = new System.Drawing.Size(163, 24);
            this.labelAnalysisPath.TabIndex = 0;
            this.labelAnalysisPath.Text = "Select Analysis Path";
            this.labelAnalysisPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFolder.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxFolder.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.textBoxFolder.Location = new System.Drawing.Point(184, 49);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(595, 20);
            this.textBoxFolder.TabIndex = 1;
            this.textBoxFolder.Text = "C:\\Data";
            // 
            // mbutton_pickFileName
            // 
            this.mbutton_pickFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_pickFileName.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_pickFileName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_pickFileName.Location = new System.Drawing.Point(804, 18);
            this.mbutton_pickFileName.Name = "mbutton_pickFileName";
            this.mbutton_pickFileName.Size = new System.Drawing.Size(27, 24);
            this.mbutton_pickFileName.TabIndex = 8;
            this.mbutton_pickFileName.Text = "...";
            this.mbutton_pickFileName.UseVisualStyleBackColor = false;
            this.mbutton_pickFileName.Click += new System.EventHandler(this.mbutton_pickFileName_Click);
            // 
            // ctlSelectOutputNameWizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.mbutton_pickFileName);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.mbtnAnalysisPath);
            this.Controls.Add(this.labelAnalysisPath);
            this.Controls.Add(this.textBoxProjectName);
            this.Controls.Add(this.labelSelect);
            this.Name = "ctlSelectOutputNameWizardPage";
            this.Size = new System.Drawing.Size(852, 407);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	    private void mbtnAnalysisPath_Click(object sender, System.EventArgs e)
		{

            folderBrowserDialog1.ShowNewFolderButton = true;

            if (Directory.Exists(textBoxFolder.Text) == true)
                folderBrowserDialog1.SelectedPath = textBoxFolder.Text;

            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog1.ShowDialog();

			if( result == DialogResult.OK )
			{
				folderName = folderBrowserDialog1.SelectedPath;
				textBoxFolder.Text = folderName;
			}
		}		
        /// <summary>
        /// Displays a open file dialog box allowing a user to find an existing file on disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_pickFileName_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog   = new OpenFileDialog();
            openFileDialog.Title            = "Select existing filename";
            openFileDialog.Filter           = "MultiAlign Analysis Files (*.mln)|*.mln";

            if (Directory.Exists(textBoxFolder.Text) == true)
                openFileDialog.InitialDirectory = textBoxFolder.Text;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxProjectName.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            }
        }

        #region IWizardControl<MultiAlignAnalysis> Members
        /// <summary>
        /// Determines if the analysis can proceed.
        /// </summary>
        /// <returns></returns>
        public bool IsComplete()
        {
            bool exists = Directory.Exists(this.textBoxFolder.Text);
            if (!exists)
            {
                MessageBox.Show("The directory you have selected does not exist.");
                return false;
            }
            else
            {
                if (Directory.Exists(Path.Combine(this.textBoxFolder.Text, this.textBoxProjectName.Text)))
                {
                    
                    DialogResult result = MessageBox.Show("The path you have specified already exists.  Do you want to overwrite the previous version?", "Confirm Continue.", MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes)
                    {
                        return false;
                    }
                }
            }

            m_analysis.MetaData.AnalysisPath = PNNLProteomics.MultiAlign.AnalysisPathUtils.BuildAnalysisName(textBoxFolder.Text, textBoxProjectName.Text);
            m_analysis.MetaData.AnalysisName = textBoxProjectName.Text;

            Properties.Settings.Default.UserOutputPath = textBoxFolder.Text;
            Properties.Settings.Default.Save();

            return true;
        }
        #endregion
    }
}

