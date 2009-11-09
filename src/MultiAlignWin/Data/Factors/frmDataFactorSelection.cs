using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PNNLControls;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmFactorAssignment.
	/// </summary>
	public class frmDataFactorSelection : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listFactors;
		private System.Windows.Forms.Panel panelBottom;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.StatusBar status;
		private clsMultiAlignAnalysis mobj_analysis;

		public frmDataFactorSelection(clsMultiAlignAnalysis analysis)
		{
			InitializeComponent();
			mobj_analysis = analysis;
			listFactors.SelectedIndexChanged += new EventHandler(listFactors_SelectedIndexChanged);	
			foreach (clsFactor factor in this.mobj_analysis.FactorTree.Factors)
			{
				listFactors.Items.Add(factor.Name);
			}
			DialogResult = DialogResult.None;
			status.Text = "Double-click a factor to modify.";
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
                this.mobj_analysis = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataFactorSelection));
            this.listFactors = new System.Windows.Forms.ListBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.StatusBar();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // listFactors
            // 
            this.listFactors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFactors.Location = new System.Drawing.Point(0, 0);
            this.listFactors.Name = "listFactors";
            this.listFactors.Size = new System.Drawing.Size(480, 147);
            this.listFactors.TabIndex = 0;
            this.listFactors.DoubleClick += new System.EventHandler(this.listFactors_DoubleClick);
            this.listFactors.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listFactors_KeyUp);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.btnOk);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 149);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(480, 48);
            this.panelBottom.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(192, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 32);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Close";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(0, 197);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(480, 16);
            this.status.TabIndex = 4;
            this.status.Text = "Double-click a factor to modify";
            // 
            // frmDataFactorSelection
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(480, 213);
            this.Controls.Add(this.listFactors);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.status);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDataFactorSelection";
            this.Text = "Select Factor";
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void listFactors_SelectedIndexChanged(object sender, EventArgs e)
		{
			System.Console.WriteLine("click");
		}

		private void EditSelectedFactor()
		{
			if (mobj_analysis.Files.Count <= 0)
				return;
			if (listFactors.SelectedItem == null)
				return;

			frmDataFactorAssignment factorForm = new frmDataFactorAssignment();
			string factorName = (string)(listFactors.SelectedItem);
			PNNLControls.clsFactorTree tree = mobj_analysis.FactorTree;

			/// 
			/// This is an ugly way to do it because if the data has not been assigned 
			///    it wont be in the factor tree.
			/// 
			foreach (clsFactor factor in tree.Factors)
			{
				if (factor.Name == factorName)
				{
					foreach(string factorValue in factor.Values.Keys)
					{
						factorForm.AddFactorValue(factorValue);
					}
					foreach(MultiAlign.clsDatasetInfo data in this.mobj_analysis.Files)
					{
						bool found = false;
						int i = 0;
						foreach(MultiAlign.clsFactorInfo fact in data.Factors)
						{
							if (fact.mstrFactor == factorName)
							{
								found = true;
								break;
							}
							i++;
						}
						if (found == true)
						{
							string factorValue = data.AssignedFactorValues[i] as String;
							factorForm.AddData(data.mstrDatasetName, factorValue);
						}
					}
					break;
				}
			}
			DialogResult result = factorForm.ShowDialog();
			if (result == DialogResult.OK)
			{

				if (this.mobj_analysis.Files.Count <= 0)
					return;

				// i = index of factor in array.
				int indexOf = 0;
				MultiAlign.clsDatasetInfo datasetInfo = mobj_analysis.Files[0] as MultiAlign.clsDatasetInfo;
				for(int i = 0; i < datasetInfo.Factors.Count; i++)
				{
					MultiAlign.clsFactorInfo fact = datasetInfo.Factors[i] as MultiAlign.clsFactorInfo;
					if (fact.mstrFactor == factorName)
					{
						indexOf = i;
						break;
					}
				}


				///  
				///  Tediuos but...we need to update in two places.
				///  
				foreach(MultiAlign.clsDatasetInfo data in mobj_analysis.Files)
				{
					data.AssignedFactorValues[indexOf] = factorForm.GetDatasetFactorValue(data.mstrDatasetName); 
				}

				mobj_analysis.BuildFactorTree();
//				foreach(clsFactorDataset data in tree.Data)
//				{
//					string factorValueName = factorForm.GetDatasetFactorValue(data.Name);
//					data.Values[factorName] = factorValueName;
//				}
			}
		}

		private void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditSelectedFactor();
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			mobj_analysis.BuildFactorTree();
			DialogResult = DialogResult.OK;
		}

		private void listFactors_DoubleClick(object sender, System.EventArgs e)
		{
			EditSelectedFactor();
		}

		private void listFactors_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditSelectedFactor();
		}
	}
}
