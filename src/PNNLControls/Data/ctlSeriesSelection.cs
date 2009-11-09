using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlSeriesSelection.
	/// </summary>
	public class ctlSeriesSelection : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ComboBox mcmb_baseline;
		private System.Windows.Forms.Label label_baseline;
		private PNNLControls.ctlListSelection mctl_list_selection;
		private DataTable mtable_data ; 

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlSeriesSelection()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}
		public ctlSeriesSelection(DataTable table)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			SetData(table) ;
		}

		public void SetData(DataTable table)
		{
			mtable_data = table ;
			mctl_list_selection.SetData(table) ;
			for (int i = 0 ; i < table.Columns.Count ; i++)
			{
				mcmb_baseline.Items.Add(table.Columns[i].ColumnName) ; 
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel2 = new System.Windows.Forms.Panel();
			this.mcmb_baseline = new System.Windows.Forms.ComboBox();
			this.label_baseline = new System.Windows.Forms.Label();
			this.mctl_list_selection = new PNNLControls.ctlListSelection();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.mcmb_baseline);
			this.panel2.Controls.Add(this.label_baseline);
			this.panel2.Location = new System.Drawing.Point(12, 224);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(520, 40);
			this.panel2.TabIndex = 15;
			// 
			// mcmb_baseline
			// 
			this.mcmb_baseline.Location = new System.Drawing.Point(208, 8);
			this.mcmb_baseline.Name = "mcmb_baseline";
			this.mcmb_baseline.Size = new System.Drawing.Size(296, 21);
			this.mcmb_baseline.TabIndex = 1;
			// 
			// label_baseline
			// 
			this.label_baseline.Location = new System.Drawing.Point(24, 8);
			this.label_baseline.Name = "label_baseline";
			this.label_baseline.Size = new System.Drawing.Size(96, 24);
			this.label_baseline.TabIndex = 0;
			this.label_baseline.Text = "Baseline Column:";
			// 
			// mctl_list_selection
			// 
			this.mctl_list_selection.Location = new System.Drawing.Point(8, 0);
			this.mctl_list_selection.Name = "mctl_list_selection";
			this.mctl_list_selection.Size = new System.Drawing.Size(520, 216);
			this.mctl_list_selection.TabIndex = 16;
			// 
			// ctlSeriesSelection
			// 
			this.Controls.Add(this.mctl_list_selection);
			this.Controls.Add(this.panel2);
			this.Name = "ctlSeriesSelection";
			this.Size = new System.Drawing.Size(544, 272);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public string BaseLineColumnName
		{
			get
			{
				if (mcmb_baseline.SelectedIndex == -1)
					return null ; 
				return (string) mcmb_baseline.Items[mcmb_baseline.SelectedIndex] ; 
			}
		}

		public string [] SelectedColumns
		{
			get
			{
				return mctl_list_selection.SelectedColumns ; 
			}
		}
		public int [] SelectedColumnIndices
		{
			get
			{
				return mctl_list_selection.SelectedColumnIndices ; 
			}
		}

	
	}
}
