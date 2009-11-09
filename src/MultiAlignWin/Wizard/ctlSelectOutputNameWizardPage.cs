using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace MultiAlignWin
{
	public class ctlSelectOutputNameWizardPage : Wizard.UI.InternalWizardPage
	{
        private System.Windows.Forms.Label labelSelect;
		private System.Windows.Forms.Label labelAnalysisPath;
        private System.Windows.Forms.TextBox textBoxProjectName;
		private System.Windows.Forms.TextBox textBoxFolder;
		private System.Windows.Forms.Button mbtnAnalysisPath;
		private System.ComponentModel.IContainer components = null;
		private FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button mbtnSavePara;
        private Button mbutton_pickFileName; 
		private string folderName; 
        

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

            SetActive += new CancelEventHandler(ctlSelectOutputNameWizardPage_SetActive);

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

        void ctlSelectOutputNameWizardPage_SetActive(object sender, CancelEventArgs e)
        {
            NextButtonText = "Start";
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
            this.label1 = new System.Windows.Forms.Label();
            this.mbtnSavePara = new System.Windows.Forms.Button();
            this.mbutton_pickFileName = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Banner.Dock = System.Windows.Forms.DockStyle.None;
            this.Banner.Location = new System.Drawing.Point(140, 0);
            this.Banner.Size = new System.Drawing.Size(708, 64);
            this.Banner.Subtitle = "Select Project Name, Output folder where intermediate files will be stored";
            this.Banner.Title = "Step 4. Specify Output Folder and Names";
            // 
            // textBoxProjectName
            // 
            this.textBoxProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProjectName.Location = new System.Drawing.Point(329, 70);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.Size = new System.Drawing.Size(471, 20);
            this.textBoxProjectName.TabIndex = 1;
            this.textBoxProjectName.Text = "MultiAlignAnalysis";
            // 
            // labelSelect
            // 
            this.labelSelect.Location = new System.Drawing.Point(160, 67);
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
            this.mbtnAnalysisPath.Location = new System.Drawing.Point(806, 98);
            this.mbtnAnalysisPath.Name = "mbtnAnalysisPath";
            this.mbtnAnalysisPath.Size = new System.Drawing.Size(27, 24);
            this.mbtnAnalysisPath.TabIndex = 2;
            this.mbtnAnalysisPath.Text = "...";
            this.mbtnAnalysisPath.UseVisualStyleBackColor = false;
            this.mbtnAnalysisPath.Click += new System.EventHandler(this.mbtnAnalysisPath_Click);
            // 
            // labelAnalysisPath
            // 
            this.labelAnalysisPath.Location = new System.Drawing.Point(160, 97);
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
            this.textBoxFolder.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBoxFolder.Location = new System.Drawing.Point(329, 101);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(471, 20);
            this.textBoxFolder.TabIndex = 1;
            this.textBoxFolder.Text = "C:\\Data";
            // 
            // label1
            // 
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(160, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "Save Parameters to a File";
            // 
            // mbtnSavePara
            // 
            this.mbtnSavePara.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnSavePara.Enabled = false;
            this.mbtnSavePara.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnSavePara.Location = new System.Drawing.Point(329, 132);
            this.mbtnSavePara.Name = "mbtnSavePara";
            this.mbtnSavePara.Size = new System.Drawing.Size(29, 23);
            this.mbtnSavePara.TabIndex = 7;
            this.mbtnSavePara.Text = "...";
            this.mbtnSavePara.UseVisualStyleBackColor = false;
            this.mbtnSavePara.Click += new System.EventHandler(this.mbtnSaveParam_Click);
            // 
            // mbutton_pickFileName
            // 
            this.mbutton_pickFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_pickFileName.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_pickFileName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_pickFileName.Location = new System.Drawing.Point(806, 70);
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
            this.Controls.Add(this.mbtnSavePara);
            this.Controls.Add(this.labelSelect);
            this.Controls.Add(this.label1);
            this.Name = "ctlSelectOutputNameWizardPage";
            this.Size = new System.Drawing.Size(852, 407);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.labelSelect, 0);
            this.Controls.SetChildIndex(this.mbtnSavePara, 0);
            this.Controls.SetChildIndex(this.textBoxProjectName, 0);
            this.Controls.SetChildIndex(this.labelAnalysisPath, 0);
            this.Controls.SetChildIndex(this.mbtnAnalysisPath, 0);
            this.Controls.SetChildIndex(this.textBoxFolder, 0);
            this.Controls.SetChildIndex(this.mbutton_pickFileName, 0);
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
		private void mbtnSaveParam_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" ;
			saveFileDialog1.FilterIndex = 1 ;
			saveFileDialog1.RestoreDirectory = true ;
			saveFileDialog1.InitialDirectory = "c:\\";

			if(saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				// Code goes here
			}
		}	
		public string DestinationFolder
		{
			get
			{
				string path = textBoxFolder.Text ; 
				if (Directory.Exists(path))
				{
					while (path.LastIndexOf("\\") == path.Length-1)
						path = path.Substring(0,path.Length-1);				
				}
				else
					path = null;

                if (path != null)
                {
                    Properties.Settings.Default.UserOutputPath = path;
                    Properties.Settings.Default.Save();
                }
                return path;
			}
			set
			{
				textBoxFolder.Text = value ;
			}
		}		
		public string ProjectName
		{
			get
			{
				return textBoxProjectName.Text ;
			}
		}
		public string ProjectOutputFileName
		{
			get
			{
				return System.IO.Path.Combine(DestinationFolder, ProjectName) + ".mln" ; 
			}
		}
        /// <summary>
        /// Displays a open file dialog box allowing a user to find an existing file on disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_pickFileName_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select existing filename";
            openFileDialog.Filter = "MultiAlign Analysis Files (*.mln)|*.mln";

            if (Directory.Exists(textBoxFolder.Text) == true)
                openFileDialog.InitialDirectory = textBoxFolder.Text;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxProjectName.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            }
        }
    }
}

