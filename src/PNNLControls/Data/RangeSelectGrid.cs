using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for RangeSelectGrid.
	/// </summary>
	public class RangeSelectGrid : System.Windows.Forms.DataGrid
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private SelectionRangeCollection mobj_range_collection = null; 
		private bool mbln_stop_mouse_messages = false ; 
		private int mint_default_column_width = 50 ; 
		private int mint_default_cell_height = 24 ; 
		private Color mclr_default_selection_border_color ; 
		public RangeSelectGrid()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			mobj_range_collection =new SelectionRangeCollection() ; 
			this.ColumnHeadersVisible = true ; 
			this.RowHeadersVisible = true ; 
			this.HeaderBackColor = Color.White ;
			this.BackgroundColor = Color.White ; 
			mclr_default_selection_border_color = Color.Black ; 
			this.AllowSorting = false ; 
			StopMouseMessages = false ; 

		}

		public bool StopMouseMessages
		{
			get
			{
				return this.mbln_stop_mouse_messages ;
			}
			set
			{
				this.mbln_stop_mouse_messages = value ; 
			}
		}

		public void SetViewColumns(ArrayList column_names) 
		{
			DataGridTableStyle table_style = this.TableStyles[0] ; 
			for (int i = 0 ; i < table_style.GridColumnStyles.Count ; i++)
			{
				string column_header = table_style.GridColumnStyles[i].HeaderText ; 
				table_style.GridColumnStyles[i].MappingName = null ; 
				for (int j = 0 ; j < column_names.Count ; j++)
				{
					if ((string)column_names[j] == column_header)
					{
						table_style.GridColumnStyles[i].MappingName = column_header ; 
						break ;
					}
				}
			}
		}

		public int DefaultCellHeight
		{
			get
			{
				return this.mint_default_cell_height ; 
			}
			set
			{
				this.mint_default_cell_height = value ; 
			}
		}
//		public Color SelectionBorderColor
//		{
//			get
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				if (table == null)
//					return mclr_default_selection_border_color ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[0] ;
//				return column_style.SelectionBorderColor ; 
//			}
//			set
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				if (table == null)
//				{
//					mclr_default_selection_border_color = value ; 
//					return ; 
//				}
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				for (int i = 0 ; i < table_style.GridColumnStyles.Count - 1 ; i++)
//				{
//					RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[i] ;
//					column_style.SelectionBorderColor = value ;
//				}
//			}
//		}      
//		public Color SelectionInternalBorderColor
//		{
//			get
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[0] ;
//				return column_style.SelectionInternalBorderColor ; 
//			}
//			set
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				for (int i = 0 ; i < table_style.GridColumnStyles.Count - 1 ; i++)
//				{
//					RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[i] ;
//					column_style.SelectionInternalBorderColor = value ;
//				}
//			}
//		}      
//		public float SelectionInternalBorderWidth
//		{
//			get
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[0] ;
//				return column_style.SelectionInternalBorderWidth ; 
//			}
//			set
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				for (int i = 0 ; i < table_style.GridColumnStyles.Count - 1 ; i++)
//				{
//					RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[i] ;
//					column_style.SelectionInternalBorderWidth = value ;
//				}
//			}
//		}      
//
//		public float SelectionBorderWidth
//		{
//			get
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[0] ;
//				return column_style.SelectionBorderWidth ; 
//			}
//			set
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				for (int i = 0 ; i < table_style.GridColumnStyles.Count - 1 ; i++)
//				{
//					RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[i] ;
//					column_style.SelectionBorderWidth = value ;
//				}
//			}
//		}      
    
//		public new Color SelectionForeColor
//		{
//			get
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[0] ;
//				return column_style.SelectionForeColor ; 
//			}
//			set
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				for (int i = 0 ; i < table_style.GridColumnStyles.Count - 1 ; i++)
//				{
//					RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[i] ;
//					column_style.SelectionForeColor = value ;
//					base.SelectionForeColor = value ; 
//				}
//			}
//		}

//		public Font SelectionFont
//		{
//			get
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[0] ;
//				return column_style.SelectionFont ; 
//			}
//			set
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				for (int i = 0 ; i < table_style.GridColumnStyles.Count - 1 ; i++)
//				{
//					RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[i] ;
//					column_style.SelectionFont = value ;
//				}
//			}
//		}
//
//		public new Color SelectionBackColor
//		{
//			get
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[0] ;
//				return column_style.SelectionBackColor ; 
//			}
//			set
//			{
//				DataTable table = (DataTable) this.DataSource ; 
//				DataGridTableStyle table_style = this.TableStyles[table.TableName] ; 
//				for (int i = 0 ; i < table_style.GridColumnStyles.Count - 1 ; i++)
//				{
//					RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle) table_style.GridColumnStyles[i] ;
//					column_style.SelectionBackColor = value ;
//					base.SelectionBackColor = value ; 
//				}
//			}
//		}

		public SelectionRangeCollection RangeCollection
		{
			get { return this.mobj_range_collection ; }
		}

		// Add a table with all visible columns
		public void CreateTableStyleCollectionForTable(DataTable table)
		{
			try
			{
				string []visible_columns = new string [table.Columns.Count] ; 
				for (int i = 0 ; i < visible_columns.Length ; i++)
				{
					visible_columns[i] = table.Columns[i].ColumnName ; 
				}
				CreateTableStyleCollectionForTable(table, visible_columns, null, this.mint_default_cell_height) ; 
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(this, e.Message, "Error in CreateTableStyleCollectionForTable") ; 
			}
		}

		public void CreateTableStyleCollectionForTable(DataTable table, string []visible_columns, int []suggested_widths, int cell_height)
		{
			try
			{
				DataGridTableStyle new_table_style = new DataGridTableStyle() ;
				new_table_style.MappingName = table.TableName ; 
				new_table_style.SelectionBackColor = Color.FromArgb(175,185,220) ; 
				new_table_style.AlternatingBackColor = Color.WhiteSmoke  ; 
				new_table_style.BackColor = Color.White  ; 
				new_table_style.PreferredRowHeight = cell_height ; 
				new_table_style.RowHeaderWidth = 30 ;      
				new_table_style.ColumnHeadersVisible = true ; 
				new_table_style.GridLineColor = Color.SlateGray ; 
				new_table_style.AllowSorting = false ; 
				new_table_style.HeaderFont = new Font("Microsoft Sans Serif", 10) ;
				base.SelectionBackColor = new_table_style.SelectionBackColor ; 
				base.SelectionForeColor = new_table_style.SelectionForeColor ; 

				GridColumnStylesCollection column_style_collection = new_table_style.GridColumnStyles ; 

				for (int i = 0 ; i < visible_columns.Length ; i++)
				{
					RangeSelectDataGridTextBoxColumnStyle column_style = new RangeSelectDataGridTextBoxColumnStyle(i) ;
					column_style.MappingName = visible_columns[i] ; 
					column_style.RangeCollection = RangeCollection ; 
					column_style.HeaderText = visible_columns[i] ; 
					if (suggested_widths != null)
						column_style.Width = suggested_widths[i] ; 
					else
						column_style.Width = this.mint_default_column_width ; 
					column_style_collection.Add(column_style) ; 
				}
				this.mobj_range_collection.NumColumns = table.Columns.Count ; 
				this.TableStyles.Add(new_table_style) ; 
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(this, e.Message, "Error in CreateTableStyleCollectionForTable") ; 
			}
		}

		public void UnSelectRow(int row)
		{
			UnSelect(row) ; 
			this.mobj_range_collection.UnSelectRow(row) ;
			Rectangle last_cell = this.GetCellBounds(row, 0) ; 
			this.Invalidate(new Rectangle(0, last_cell.Y, this.Bounds.Width, this.Bounds.Height)) ; 
		}

		public bool IsSelectedRow(int row)
		{
			return this.mobj_range_collection.IsSelected(row,-1) ; 
		}

		private void ClearLastSelectedRangeArea()
		{
			Range last_range = this.mobj_range_collection.LastRange ;
			if (last_range == null)
				return ; 
			ClearRowArea(last_range.StartRow,  last_range.EndRow) ; 
		}

		private void ClearRowArea(int start_row, int end_row)
		{
			try
			{
				if (start_row < 0)
					start_row = 0 ; 
				if (end_row < 0)
					end_row = 0 ; 


				Rectangle bounds_min = this.GetCellBounds(start_row, 0) ; 
				Rectangle bounds_max = this.GetCellBounds(end_row,0) ; 
				Rectangle invalidate_rect = new Rectangle(this.Bounds.X, bounds_min.Y, this.Bounds.Width, bounds_max.Bottom - bounds_min.Top) ; 
				this.Invalidate(invalidate_rect) ; 
			}
			catch(System.Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
		

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				base.OnMouseMove(e) ;
				return ;
			}
			HitTestInfo hit_info = HitTest(e.X, e.Y) ;
			int last_row = this.mobj_range_collection.LastClickedRow ; 
			int last_column = this.mobj_range_collection.LastClickedColumn ; 

			if (hit_info.Row < 0)
				return ; 

			this.ClearLastSelectedRangeArea() ; 
			this.mobj_range_collection.AddActionMouseMove(hit_info.Row, hit_info.Column, e) ; 
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e) ; 
			if (this.TableStyles == null || this.TableStyles.Count == 0)
				return ; 

			// Take the current TableStyle and resize the columns in proportion.
			DataGridTableStyle table_style = this.TableStyles[0] ; 
			int total = 0 ; 
			for (int i = 0 ; i < table_style.GridColumnStyles.Count ; i++)
			{
				if (table_style.GridColumnStyles[i].MappingName != null &&
					table_style.GridColumnStyles[i].MappingName != "")
				{
					total += table_style.GridColumnStyles[i].Width ; 
				}
			}

			for (int i = 0 ; i < table_style.GridColumnStyles.Count ; i++)
			{
				if (table_style.GridColumnStyles[i].MappingName != null &&
					table_style.GridColumnStyles[i].MappingName != "")
				{
					table_style.GridColumnStyles[i].Width = (table_style.GridColumnStyles[i].Width * (this.Width - this.RowHeaderWidth)) / total ; 
				}
			}
		}


		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				base.OnMouseDown(e) ; 
				return ; 
			}

			DataTable table = (DataTable) this.DataSource ; 
			if (table == null || table.Rows.Count == 0)
			{
				return ; 
			}

			HitTestInfo hit_info = HitTest(e.X, e.Y) ; 
			bool is_last_selected = false ; 
			base.OnMouseDown(e) ;


			this.ClearLastSelectedRangeArea() ; 

			if(!(hit_info.Equals (DataGrid.HitTestInfo.Nowhere)) && !hit_info.Equals (HitTestType.ColumnHeader)) 
			{
				is_last_selected = this.mobj_range_collection.IsLastClickedElement(hit_info.Row, hit_info.Column) ; 
				this.mobj_range_collection.AddActionClick(hit_info.Row, hit_info.Column) ; 
				if (!is_last_selected)
				{
					if (hit_info.Column != -1 && hit_info.Column != -1)
					{
						if (this.TableStyles[table.TableName].GridColumnStyles[hit_info.Column].GetType() == typeof(RangeSelectDataGridTextBoxColumnStyle))
						{
							RangeSelectDataGridTextBoxColumnStyle column_style = (RangeSelectDataGridTextBoxColumnStyle)this.TableStyles[table.TableName].GridColumnStyles[hit_info.Column] ; 
							column_style.HideTextBox() ;              
						}
						this.ClearLastSelectedRangeArea() ; 
					}
				}
			}
		}
      
		protected override void OnClick(EventArgs e)
		{
			return ; 
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
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}
		#endregion
	}
}
