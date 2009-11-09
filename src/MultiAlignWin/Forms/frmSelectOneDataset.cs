using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	public enum enmSelectType {Decon2LS, ICR2LS, NEWEST} ;
	/// <summary>
	/// Summary description for frmSelectOneDataset.
	/// </summary>
	public class frmSelectOneDataset : System.Windows.Forms.Form
	{
		//public enum enmSelectType {Decon2LS, ICR2LS, NEWEST} ;

		private System.Windows.Forms.Panel panelOKCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		
		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxOutputs;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton mradioBtnICR;
		private System.Windows.Forms.RadioButton mradioBtnDecon;
		private System.Windows.Forms.RadioButton mradioBtnNewest;
		private System.Windows.Forms.CheckBox mchkBoxDefault;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		bool Decon2LSpresent = false ;
		private System.Windows.Forms.Label mlabelDecon;
		private System.Windows.Forms.Label mlabelICR;
		private System.Windows.Forms.Label mlabelNEW;
		bool ICR2LSpresent = false ;

		public frmSelectOneDataset()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public void SetDatasets(string []folder_names)
		{
			int numOptions = folder_names.Length ; 
			comboBoxOutputs.Items.Clear() ; 
			for (int optionNum = 0 ; optionNum < numOptions ; optionNum++)
			{
				comboBoxOutputs.Items.Add(folder_names[optionNum]) ; 
			}
		}

		public void SetDatasetNames(string []file_names)
		{
			int numOptions = file_names.Length ; 
			for (int optionNum = 0 ; optionNum < numOptions ; optionNum++)
			{
				if (file_names[optionNum].ToLower().IndexOf("_isos.csv") > 0 )
					Decon2LSpresent = true ;
				if (file_names[optionNum].ToLower().IndexOf(".pek") > 0 )
					ICR2LSpresent = true ;
			}
			if (!Decon2LSpresent)
			{
				mradioBtnDecon.Enabled = false ;
				mlabelDecon.Enabled = false ;
				mradioBtnNewest.Enabled = false ;
				mlabelNEW.Enabled = false ;
				
			}
			if (!ICR2LSpresent)
			{
				mradioBtnICR.Enabled = false ;
				mlabelICR.Enabled = false ;
				mradioBtnNewest.Enabled = false ;
				mlabelNEW.Enabled = false ;
			}
		}

		public int SelectedFileIndex
		{
			get
			{
				return comboBoxOutputs.SelectedIndex ; 
			}
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSelectOneDataset));
			this.panelOKCancel = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.panelMain = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.mlabelNEW = new System.Windows.Forms.Label();
			this.mlabelICR = new System.Windows.Forms.Label();
			this.mlabelDecon = new System.Windows.Forms.Label();
			this.mradioBtnDecon = new System.Windows.Forms.RadioButton();
			this.mradioBtnICR = new System.Windows.Forms.RadioButton();
			this.mradioBtnNewest = new System.Windows.Forms.RadioButton();
			this.mchkBoxDefault = new System.Windows.Forms.CheckBox();
			this.comboBoxOutputs = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panelOKCancel.SuspendLayout();
			this.panelMain.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelOKCancel
			// 
			this.panelOKCancel.Controls.Add(this.buttonCancel);
			this.panelOKCancel.Controls.Add(this.buttonOK);
			this.panelOKCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelOKCancel.Location = new System.Drawing.Point(0, 255);
			this.panelOKCancel.Name = "panelOKCancel";
			this.panelOKCancel.Size = new System.Drawing.Size(450, 40);
			this.panelOKCancel.TabIndex = 0;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(272, 8);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(96, 24);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(72, 8);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// panelMain
			// 
			this.panelMain.Controls.Add(this.groupBox1);
			this.panelMain.Controls.Add(this.mchkBoxDefault);
			this.panelMain.Controls.Add(this.comboBoxOutputs);
			this.panelMain.Controls.Add(this.label1);
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 0);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(450, 255);
			this.panelMain.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.mlabelNEW);
			this.groupBox1.Controls.Add(this.mlabelICR);
			this.groupBox1.Controls.Add(this.mlabelDecon);
			this.groupBox1.Controls.Add(this.mradioBtnDecon);
			this.groupBox1.Controls.Add(this.mradioBtnICR);
			this.groupBox1.Controls.Add(this.mradioBtnNewest);
			this.groupBox1.Location = new System.Drawing.Point(24, 56);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(400, 152);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Selection";
			// 
			// mlabelNEW
			// 
			this.mlabelNEW.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mlabelNEW.Location = new System.Drawing.Point(40, 128);
			this.mlabelNEW.Name = "mlabelNEW";
			this.mlabelNEW.Size = new System.Drawing.Size(288, 16);
			this.mlabelNEW.TabIndex = 9;
			this.mlabelNEW.Text = "(Selects the most recent from both Decon2LS and ICR2LS files)";
			// 
			// mlabelICR
			// 
			this.mlabelICR.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mlabelICR.Location = new System.Drawing.Point(40, 88);
			this.mlabelICR.Name = "mlabelICR";
			this.mlabelICR.Size = new System.Drawing.Size(176, 23);
			this.mlabelICR.TabIndex = 8;
			this.mlabelICR.Text = "(Selects the most recent if multiple files)";
			// 
			// mlabelDecon
			// 
			this.mlabelDecon.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mlabelDecon.Location = new System.Drawing.Point(40, 48);
			this.mlabelDecon.Name = "mlabelDecon";
			this.mlabelDecon.Size = new System.Drawing.Size(176, 23);
			this.mlabelDecon.TabIndex = 7;
			this.mlabelDecon.Text = "(Selects the most recent if multiple files)";
			// 
			// mradioBtnDecon
			// 
			this.mradioBtnDecon.Location = new System.Drawing.Point(24, 24);
			this.mradioBtnDecon.Name = "mradioBtnDecon";
			this.mradioBtnDecon.Size = new System.Drawing.Size(160, 24);
			this.mradioBtnDecon.TabIndex = 4;
			this.mradioBtnDecon.Text = "Select Decon2LS File (csv)";
			// 
			// mradioBtnICR
			// 
			this.mradioBtnICR.Location = new System.Drawing.Point(24, 64);
			this.mradioBtnICR.Name = "mradioBtnICR";
			this.mradioBtnICR.Size = new System.Drawing.Size(160, 24);
			this.mradioBtnICR.TabIndex = 5;
			this.mradioBtnICR.Text = "Select ICR2LS File (pek)";
			// 
			// mradioBtnNewest
			// 
			this.mradioBtnNewest.Location = new System.Drawing.Point(24, 104);
			this.mradioBtnNewest.Name = "mradioBtnNewest";
			this.mradioBtnNewest.Size = new System.Drawing.Size(160, 24);
			this.mradioBtnNewest.TabIndex = 6;
			this.mradioBtnNewest.Text = "Select The Most Recent";
			// 
			// mchkBoxDefault
			// 
			this.mchkBoxDefault.Location = new System.Drawing.Point(48, 208);
			this.mchkBoxDefault.Name = "mchkBoxDefault";
			this.mchkBoxDefault.Size = new System.Drawing.Size(352, 32);
			this.mchkBoxDefault.TabIndex = 7;
			this.mchkBoxDefault.Text = "Make This The Default Selection Choice For This Analysis";
			// 
			// comboBoxOutputs
			// 
			this.comboBoxOutputs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBoxOutputs.Location = new System.Drawing.Point(0, 24);
			this.comboBoxOutputs.Name = "comboBoxOutputs";
			this.comboBoxOutputs.Size = new System.Drawing.Size(450, 21);
			this.comboBoxOutputs.TabIndex = 1;
			this.comboBoxOutputs.Text = "Select One File";
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(450, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "    Multiple PEK files were generated for this dataset. Please select one output";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// frmSelectOneDataset
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(450, 295);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.panelOKCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSelectOneDataset";
			this.Text = "Select Dataset";
			this.panelOKCancel.ResumeLayout(false);
			this.panelMain.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if (!SelectionMade())
			{
				MessageBox.Show("Please select a file before clicking ok") ; 
				return ; 
			}
			this.DialogResult = DialogResult.OK ; 
			this.Hide() ; 
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel ; 
			this.Hide() ; 		
		}

		
		private bool SelectionMade()
		{
			if (comboBoxOutputs.SelectedIndex != -1 || mradioBtnDecon.Checked || mradioBtnICR.Checked)
				return true ;
			else
				return false ;
		}


		public string setDatasetID
		{
			set
			{
				this.label1.Text = "    Multiple PEK files were generated for the dataset " + value + ". Please select one output";
			}
		}

		public enmSelectType Selection
		{
			get
			{
				if (mradioBtnDecon.Checked)
					return enmSelectType.Decon2LS ;
				else if (mradioBtnICR.Checked)
					return enmSelectType.ICR2LS ;
				else
					return enmSelectType.NEWEST ;
			}
		}

		public bool DefaultChecked
		{
			get
			{
				return mchkBoxDefault.Checked ;
			}
		}
	}
}
