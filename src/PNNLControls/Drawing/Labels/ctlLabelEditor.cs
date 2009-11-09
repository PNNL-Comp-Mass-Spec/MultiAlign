using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;


namespace PNNLControls
{
	public class ctlLabelEditor : PNNLControls.ctlDataFrame
	{
		private System.ComponentModel.IContainer components = null;

		public ctlLabelEditor()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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

		private System.Windows.Forms.Panel pnlData;
		private PNNLControls.ctlListSelection selector;

		private System.Windows.Forms.GroupBox grpVert;
		private System.Windows.Forms.ComboBox cboVert;
		private System.Windows.Forms.Label lblCboVert;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;

		public string [] SelectedColumns
		{
			get{return selector.SelectedColumns;}
		}

		private DataTable mTable = null;
		public DataTable Table
		{
			get{return(this.mTable);}
			set{
				this.mTable = value;

				if(this.mTable==null) return;


				this.cboVert.Items.Clear();
		
				hLabelVert.Clear();
				hLabelHorz.Clear();
				clsLabelAttributes vRoot = hLabelVert.GetRoot();
				clsLabelAttributes hRoot = hLabelHorz.GetRoot();

				if (this.mTable!=null)
				{
					try
					{
						selector.SetData(value);

						DataGridTableStyle style = new DataGridTableStyle();
						style.MappingName = mTable.TableName;
						style.RowHeadersVisible=false;

						for (int i=0; i<mTable.Columns.Count; i++)
						{
							this.cboVert.Items.Add((mTable.Columns[i] as DataColumn).Caption);
						}
						this.cboVert.SelectedIndex=0;

						AutoLoadDoubleColumns();
					}
					catch(Exception e){System.Windows.Forms.MessageBox.Show(e.Message);}
				}
			}
		}

		private void AutoLoadDoubleColumns()
		{
			if (this.mTable==null) return;

			for (int i=0; i<mTable.Columns.Count; i++)
			{
				DataColumn d = mTable.Columns[i] as DataColumn;
				if (d.DataType == System.Type.GetType("System.Double"))
					selector.SelectColumn(d.Caption, true);

			}
		}

		private float[,] GetData(int[] indices, float nullReplacement)
		{
			try
			{
				int rows = mTable.Rows.Count;
				int cols = indices.Length;

				float [,] data = new float[rows, cols];
				for(int i=0; i<rows; i++)
				{
					DataRow d = mTable.Rows[i];
					for(int j=0; j<cols; j++)
					{
						int index = indices[j];

						object o = d.ItemArray[index];
			
						//if (Object.ReferenceEquals(o.GetType(), System.DBNull))
						if (o is System.DBNull)
							//data[i,j]=float.NaN;
							data[i,j]=nullReplacement;
						else
						{
							Double dd = (Double)o;
							data[i,j] = (float)dd;
						}

					}
				}

				return data;
			}
			catch{}

			return null;
		}

		public float[,] GetData(float nullReplacement)
		{
			int[] indices = hLabelHorz.DataTags;

			return (GetData(indices, nullReplacement));
		}

		public float[,] GetData()
		{
			return (GetData(0f));
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlData = new System.Windows.Forms.Panel();
			this.selector = new PNNLControls.ctlListSelection();
			this.grpVert = new System.Windows.Forms.GroupBox();
			this.cboVert = new System.Windows.Forms.ComboBox();
			this.lblCboVert = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel1.SuspendLayout();
			this.pnlSE.SuspendLayout();
			this.pnlHeader.SuspendLayout();
			this.grpVert.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(608, 120);
			// 
			// hLabelHorz
			// 
			this.hLabelHorz.Location = new System.Drawing.Point(113, 0);
			this.hLabelHorz.Name = "hLabelHorz";
			this.hLabelHorz.Size = new System.Drawing.Size(495, 120);
			this.hLabelHorz.UpdateComplete = true;
			this.hLabelHorz.Align += new PNNLControls.ctlHierarchalLabel.AlignDelegate(this.hLabelHorz_Align);
			// 
			// hLabelVert
			// 
			this.hLabelVert.Location = new System.Drawing.Point(0, 96);
			this.hLabelVert.Name = "hLabelVert";
			this.hLabelVert.Size = new System.Drawing.Size(112, 384);
			this.hLabelVert.LabelChanged += new PNNLControls.clsLabelAttributes.ChangedDelegate(this.hLabelVert_LabelChanged);
			// 
			// legend
			// 
			this.legend.Location = new System.Drawing.Point(608, 96);
			this.legend.Name = "legend";
			this.legend.Size = new System.Drawing.Size(0, 384);
			this.legend.Visible = false;
			// 
			// pnlSE
			// 
			this.pnlSE.Location = new System.Drawing.Point(608, 0);
			this.pnlSE.Name = "pnlSE";
			this.pnlSE.Size = new System.Drawing.Size(0, 120);
			this.pnlSE.Visible = false;
			// 
			// pnlHeader
			// 
			this.pnlHeader.Controls.Add(this.selector);
			this.pnlHeader.Controls.Add(this.grpVert);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(608, 96);
			this.pnlHeader.Controls.SetChildIndex(this.grpVert, 0);
			this.pnlHeader.Controls.SetChildIndex(this.selector, 0);
			// 
			// picRight
			// 
			this.picRight.Location = new System.Drawing.Point(-40, 0);
			this.picRight.Name = "picRight";
			this.picRight.Visible = false;
			// 
			// picLeft
			// 
			this.picLeft.Name = "picLeft";
			this.picLeft.Size = new System.Drawing.Size(113, 120);
			// 
			// pnlData
			// 
			this.pnlData.BackColor = System.Drawing.SystemColors.Control;
			this.pnlData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlData.Location = new System.Drawing.Point(112, 96);
			this.pnlData.Name = "pnlData";
			this.pnlData.Size = new System.Drawing.Size(496, 384);
			this.pnlData.TabIndex = 7;
			// 
			// selector
			// 
			this.selector.BackColor = System.Drawing.SystemColors.Control;
			this.selector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.selector.DockPadding.All = 5;
			this.selector.Location = new System.Drawing.Point(112, 0);
			this.selector.Name = "selector";
			this.selector.Size = new System.Drawing.Size(494, 94);
			this.selector.TabIndex = 0;
			this.selector.SelectList += new PNNLControls.ctlListSelection.SelectListDelegate(this.selector_SelectList);
			// 
			// grpVert
			// 
			this.grpVert.Controls.Add(this.cboVert);
			this.grpVert.Controls.Add(this.lblCboVert);
			this.grpVert.Dock = System.Windows.Forms.DockStyle.Left;
			this.grpVert.Location = new System.Drawing.Point(0, 0);
			this.grpVert.Name = "grpVert";
			this.grpVert.Size = new System.Drawing.Size(112, 94);
			this.grpVert.TabIndex = 3;
			this.grpVert.TabStop = false;
			// 
			// cboVert
			// 
			this.cboVert.Dock = System.Windows.Forms.DockStyle.Top;
			this.cboVert.ItemHeight = 13;
			this.cboVert.Location = new System.Drawing.Point(3, 32);
			this.cboVert.Name = "cboVert";
			this.cboVert.Size = new System.Drawing.Size(106, 21);
			this.cboVert.TabIndex = 2;
			this.cboVert.SelectedIndexChanged += new System.EventHandler(this.cboVert_SelectedIndexChanged);
			// 
			// lblCboVert
			// 
			this.lblCboVert.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblCboVert.Location = new System.Drawing.Point(3, 16);
			this.lblCboVert.Name = "lblCboVert";
			this.lblCboVert.Size = new System.Drawing.Size(106, 16);
			this.lblCboVert.TabIndex = 3;
			this.lblCboVert.Text = "Vertical Labels";
			this.lblCboVert.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// splitter1
			// 
			this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitter1.Location = new System.Drawing.Point(112, 96);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 384);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// splitter2
			// 
			this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(115, 477);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(493, 3);
			this.splitter2.TabIndex = 9;
			this.splitter2.TabStop = false;
			// 
			// ctlLabelEditor
			// 
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.pnlData);
			this.Name = "ctlLabelEditor";
			this.Size = new System.Drawing.Size(608, 600);
			this.LabelsUpdated += new PNNLControls.ctlHierarchalLabel.LabelUpdateDelegate(this.ctlLabelEditor_LabelsUpdated);
			this.ClientSize += new PNNLControls.ctlDataFrame.ClientSizeDelegate(this.ctlLabelEditor_ClientSize);
			this.ClientLocation += new PNNLControls.ctlDataFrame.ClientLocationDelegate(this.ctlLabelEditor_ClientLocation);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.pnlHeader, 0);
			this.Controls.SetChildIndex(this.hLabelVert, 0);
			this.Controls.SetChildIndex(this.legend, 0);
			this.Controls.SetChildIndex(this.pnlData, 0);
			this.Controls.SetChildIndex(this.splitter1, 0);
			this.Controls.SetChildIndex(this.splitter2, 0);
			this.panel1.ResumeLayout(false);
			this.pnlSE.ResumeLayout(false);
			this.pnlHeader.ResumeLayout(false);
			this.grpVert.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ctlLabelEditor_LabelsUpdated()
		{
			//align column widths and heights to match hierarchal labels

//			try
//			{
//				mGrid.Left = -mGrid.DisplayRectangle.Left;
//				DataGridTableStyle a = mGrid.TableStyles[0] ; 
//				a.ColumnHeadersVisible = true;
//				a.RowHeaderWidth =0;
//
//				for (int i=0; i<this.hLabelHorz.Alignment.Length; i++)
//				{
//					int width = hLabelHorz.Alignment[i+1]-hLabelHorz.Alignment[i]-1;
//					DataGridColumnStyle c = a.GridColumnStyles[i] as DataGridColumnStyle;
//					c.Width = width;
//				}
//
//				for (int i=0; i<this.hLabelVert.Alignment.Length; i++)
//				{				
//				}
//			}
//			catch{}
			}

		private void hLabelHorz_Align(int[] positions)
		{
//			try
//			{
////				DataGridTableStyle a = mGrid.TableStyles[0] ; 
////				a.ColumnHeadersVisible = true;
////				a.RowHeaderWidth =0;
//
//				int [] dataTags = hLabelHorz.DataTags;
//
//				//get rid of existing column styles
////				a.GridColumnStyles.Clear();
//
//				for (int i=0; i<this.hLabelHorz.Alignment.Length-1; i++)
//				{
//					int width = hLabelHorz.Alignment[i+1]-hLabelHorz.Alignment[i];
//					if(i==hLabelHorz.Alignment.Length-2)
//						width-=2;
//
//					int j=dataTags[i];
//
//					//DataGridColumnStyle c = a.GridColumnStyles[j] as DataGridColumnStyle;
//					DataGridColumnStyle c =mColumns[j] as DataGridColumnStyle;
//					a.GridColumnStyles.Add(c);					
//					c.Width = width;
//				}
//			}
//			catch{}
		
		}

		public void LoadVerticalLabels()
		{
			int index = cboVert.SelectedIndex;

			if (mTable.Columns.Count<=index) 
				return;

			hLabelVert.Clear();
			//hLabelVert.MinLeafHeight = mGrid.TableStyles[0].PreferredRowHeight;
			clsLabelAttributes root = hLabelVert.GetRoot();

			int rows = mTable.Rows.Count;
			
			for(int i=0; i<rows; i++)
			{
				DataRow d = mTable.Rows[i];
				string id = d.ItemArray[index].ToString();
				clsLabelAttributes l = hLabelVert.AddBranch(root, id);	
				l.dataTag = i;
			}
			hLabelVert.Init();
			hLabelVert.LoadLabels(0, Math.Min(rows-1, 10));
		}

		private void ctlLabelEditor_ClientLocation(System.Drawing.Point location)
		{
//			mGrid.Dock = DockStyle.None;
//			pnlHeader.BringToFront();
//			mGrid.Location = location;
		}

		private void ctlLabelEditor_ClientSize(System.Drawing.Size size)
		{
			//mGrid.Size = size;
		}

		private void selector_SelectList(int[] positions)
		{

			DataColumn d = null;			

			try
			{
				clsLabelAttributes hRoot = hLabelHorz.GetRoot();
				hLabelHorz.Clear();
				for (int i=0; i<positions.Length; i++)
				{
					int j = positions[i];
					d = mTable.Columns[j] as DataColumn;
					clsLabelAttributes lbl = hLabelHorz.AddBranch(hRoot, d.ToString());
					lbl.dataTag = j;
				}

				hLabelHorz.Init();

				if (positions.Length>0)
					hLabelHorz.LoadLabels(0, positions.Length-1);
			}
			catch(Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void cboVert_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			LoadVerticalLabels();
		}

		private void hLabelVert_LabelChanged(PNNLControls.clsLabelAttributes newLbl, PNNLControls.clsLabelAttributes prevLbl)
		{
			//MessageBox.Show(newLbl.text);
		}
	}
}

