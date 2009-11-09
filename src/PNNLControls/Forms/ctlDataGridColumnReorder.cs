using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlDataGridColumnReorder.
	/// </summary>
	public class ctlDataGridColumnReorder : UserControl
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonMoveLeft;
		private System.Windows.Forms.Button buttonMoveRight;
		private System.Windows.Forms.Panel panelHolder;
		private System.Windows.Forms.DataGrid dataGridPreview;
		//private ECsoft.Windows.Forms.DataGridEx dataGridPreview;
		

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlDataGridColumnReorder(DataGrid grid)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
//	
//			dataGridPreview = new ECsoft.Windows.Forms.DataGridEx();
//			dataGridPreview.Dock = DockStyle.Fill;
//			panelHolder.Container.Add(dataGridPreview);
//			components.Add(dataGridPreview);

			Init(grid);			
		}

		public ctlDataGridColumnReorder()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			Init(null);
		}

		public DataGrid DataGrid
		{
			get 
			{
				return null; //dataGridPreview;
			}
			set { Init(value); }
		}
		
		private void Init(DataGrid grid)
		{			
//			if (grid == null)
//				return; 
//
//			DataTable table = grid.DataSource as DataTable;
//			if (table == null)
//				return; 
//
//			/// 
//			/// Load the data
//			/// 			
//			DataTable newTable = table.Copy();
//
//			DataGridTableStyle myGridStyle = new DataGridTableStyle();
//			myGridStyle.MappingName = table.TableName;
//			for (int colNum = 0 ; colNum < table.Columns.Count ; colNum++)
//			{
//				DataGridTextBoxColumn colStyle = new DataGridTextBoxColumn();				
//				colStyle.MappingName = table.Columns[colNum].ColumnName ;
//				colStyle.HeaderText  = table.Columns[colNum].ColumnName  ;
//				colStyle.NullText    = string.Empty ; 			
//				myGridStyle.GridColumnStyles.Add(colStyle);
//				myGridStyle.AllowSorting = false;				
//			}			
//			myGridStyle.SelectionBackColor = Color.Navy;
//			myGridStyle.SelectionForeColor = Color.White;
//
//			dataGridPreview.TableStyles.Clear();
//			dataGridPreview.TableStyles.Add(myGridStyle);
//			dataGridPreview.DataSource = newTable;
//			dataGridPreview.AllowSorting = false;
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonMoveRight = new System.Windows.Forms.Button();
			this.buttonMoveLeft = new System.Windows.Forms.Button();
			this.panelHolder = new System.Windows.Forms.Panel();
			this.dataGridPreview = new System.Windows.Forms.DataGrid();
			this.panel1.SuspendLayout();
			this.panelHolder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonMoveRight);
			this.panel1.Controls.Add(this.buttonMoveLeft);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 488);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(648, 32);
			this.panel1.TabIndex = 1;
			// 
			// buttonMoveRight
			// 
			this.buttonMoveRight.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonMoveRight.Location = new System.Drawing.Point(328, 4);
			this.buttonMoveRight.Name = "buttonMoveRight";
			this.buttonMoveRight.Size = new System.Drawing.Size(56, 24);
			this.buttonMoveRight.TabIndex = 3;
			this.buttonMoveRight.Text = ">>>";
			// 
			// buttonMoveLeft
			// 
			this.buttonMoveLeft.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonMoveLeft.Location = new System.Drawing.Point(264, 4);
			this.buttonMoveLeft.Name = "buttonMoveLeft";
			this.buttonMoveLeft.Size = new System.Drawing.Size(56, 24);
			this.buttonMoveLeft.TabIndex = 2;
			this.buttonMoveLeft.Text = "<<<";
			// 
			// panelHolder
			// 
			this.panelHolder.Controls.Add(this.dataGridPreview);
			this.panelHolder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelHolder.Location = new System.Drawing.Point(0, 0);
			this.panelHolder.Name = "panelHolder";
			this.panelHolder.Size = new System.Drawing.Size(648, 488);
			this.panelHolder.TabIndex = 2;
			// 
			// dataGridPreview
			// 
			this.dataGridPreview.DataMember = "";
			this.dataGridPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridPreview.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridPreview.Location = new System.Drawing.Point(0, 0);
			this.dataGridPreview.Name = "dataGridPreview";
			this.dataGridPreview.Size = new System.Drawing.Size(648, 488);
			this.dataGridPreview.TabIndex = 0;
			// 
			// ctlDataGridColumnReorder
			// 
			this.Controls.Add(this.panelHolder);
			this.Controls.Add(this.panel1);
			this.Name = "ctlDataGridColumnReorder";
			this.Size = new System.Drawing.Size(648, 520);
			this.panel1.ResumeLayout(false);
			this.panelHolder.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridPreview)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		

//		/// <summary>
//		///	Move the drag column from one location to another
//		/// </summary>
//		/// <param name="fromColumn">Source Column Index</param>
//		/// <param name="toColumn">Destination Column Index</param>
//		public void MoveColumn(int fromColumn, int toColumn) 
//		{ 
//			if(fromColumn == toColumn) return;
//			
//			DataGridTableStyle oldTS = dataGridPreview.TableStyles[0];
//			DataGridTableStyle newTS = new DataGridTableStyle();
//			newTS.MappingName = oldTS.MappingName;
//			CopyTableStyle(oldTS, newTS);	// Copy the old TableStyle to new TableStyle			
//			for(int i = 0; i < oldTS.GridColumnStyles.Count; i++) 
//			{
//				//oldTS.HeaderBackColor = Color.Gray;
//				
//				
//				if(i != fromColumn && fromColumn < toColumn)
//					newTS.GridColumnStyles.Add(oldTS.GridColumnStyles[i]); 
//			
//				if(i == toColumn) 
//				{
//					oldTS.GridColumnStyles[fromColumn].DataGridTableStyle.BackColor = Color.Blue;
//					oldTS.GridColumnStyles[fromColumn].DataGridTableStyle.HeaderBackColor = Color.Blue;					
//					oldTS.GridColumnStyles[fromColumn].HeaderText = "[" +  oldTS.GridColumnStyles[fromColumn].HeaderText + "]";
//					oldTS.GridColumnStyles[fromColumn].DataGridTableStyle.GridLineColor = Color.Red;
//					newTS.GridColumnStyles.Add(oldTS.GridColumnStyles[fromColumn]); 
//				}
//
//				if(i != fromColumn && fromColumn > toColumn) 
//					newTS.GridColumnStyles.Add(oldTS.GridColumnStyles[i]);      
//			} 
//			dataGridPreview.TableStyles.Remove(oldTS); 
//			dataGridPreview.TableStyles.Add(newTS); 
//		}
//		
//		/// <summary>
//		/// Copy the display-related properties of the given DataGridTableStyle
//		/// </summary>
//		/// <param name="oldTS">From TableStyle</param>
//		/// <param name="newTS">To TableStyle</param>
//		private void CopyTableStyle(DataGridTableStyle oldTS, DataGridTableStyle newTS)
//		{
//			newTS.AllowSorting = oldTS.AllowSorting;
//			newTS.AlternatingBackColor = oldTS.AlternatingBackColor;
//			newTS.BackColor = oldTS.BackColor;
//			newTS.ColumnHeadersVisible = oldTS.ColumnHeadersVisible;
//			newTS.ForeColor = oldTS.ForeColor;
//			newTS.GridLineColor = oldTS.GridLineColor;
//			newTS.GridLineStyle = oldTS.GridLineStyle;
//			newTS.HeaderBackColor = oldTS.HeaderBackColor;
//			newTS.HeaderFont = oldTS.HeaderFont;
//			newTS.HeaderForeColor = oldTS.HeaderForeColor;
//			newTS.LinkColor = oldTS.LinkColor;
//			newTS.PreferredColumnWidth = oldTS.PreferredColumnWidth;
//			newTS.PreferredRowHeight = oldTS.PreferredRowHeight;
//			newTS.ReadOnly = oldTS.ReadOnly;
//			newTS.RowHeadersVisible = oldTS.RowHeadersVisible;
//			newTS.RowHeaderWidth = oldTS.RowHeaderWidth;
//			newTS.SelectionBackColor = oldTS.SelectionBackColor;
//			newTS.SelectionForeColor = oldTS.SelectionForeColor;
//		}
//
//		private void buttonMoveRight_Click(object sender, System.EventArgs e)
//		{
//			int col = dataGridPreview.SelectedColumn;
//
//			DataTable table = dataGridPreview.DataSource as DataTable;
//			if (col + 1 >= table.Columns.Count || col < 0)
//			{				
//				return;
//			}
//			
//			dataGridPreview.HeaderBackColor = Color.Blue;
//			MoveColumn(col, col+1);
//			dataGridPreview.SelectedColumn = col + 1;
//		}
//
//		private void buttonMoveLeft_Click(object sender, System.EventArgs e)
//		{
//			int col = dataGridPreview.SelectedColumn;
//
//			DataTable table = dataGridPreview.DataSource as DataTable;
//			if (col <= 0 || col >= table.Columns.Count)
//			{
//				return;
//			}
//			MoveColumn(col, col-1);
//			dataGridPreview.SelectedColumn = col - 1;
//		}
	}
}
