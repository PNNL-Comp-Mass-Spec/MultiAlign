using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlListSelection.
	/// </summary>
	public class ctlListSelection : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ListBox mlst_selected_columns;
		private System.Windows.Forms.ListBox mlst_columns;
		private System.Windows.Forms.Panel mpnl_selected_columns;
		private System.Windows.Forms.Panel mpnl_columns;

		private DataTable mtbl_data ;
		private System.Windows.Forms.Button mbtn_remove_current;
		private System.Windows.Forms.Button mbtn_remove_all;
		private System.Windows.Forms.Button mbtn_move_current;
		private System.Windows.Forms.Button mbtn_move_all;
		private System.Windows.Forms.Panel mpnl_buttons; 

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlListSelection()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary>
		/// event handler for the selection event.  
		/// provides an array of selected data columns.
		/// </summary>
		public delegate void SelectListDelegate (int [] positions);
		public event SelectListDelegate SelectList = null;

		public void Clear()
		{
			mlst_selected_columns.Items.Clear() ;
			mlst_columns.Items.Clear() ; 
		}
		public void SetData(DataTable table)
		{
			mtbl_data = table ; 
			mlst_columns.Items.Clear() ;
			mlst_selected_columns.Items.Clear() ;
			for (int i = 0 ; i < table.Columns.Count ; i++)
			{
				mlst_columns.Items.Add(table.Columns[i].ColumnName) ; 
			}
		}

		public void SetData(string [] names)
		{
			mlst_columns.Items.Clear() ;
			mlst_selected_columns.Items.Clear() ;
			for (int i = 0 ; i < names.Length ; i++)
			{
				mlst_columns.Items.Add(names[i]) ; 
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
			this.mpnl_selected_columns = new System.Windows.Forms.Panel();
			this.mlst_selected_columns = new System.Windows.Forms.ListBox();
			this.mpnl_columns = new System.Windows.Forms.Panel();
			this.mlst_columns = new System.Windows.Forms.ListBox();
			this.mpnl_buttons = new System.Windows.Forms.Panel();
			this.mbtn_move_all = new System.Windows.Forms.Button();
			this.mbtn_move_current = new System.Windows.Forms.Button();
			this.mbtn_remove_all = new System.Windows.Forms.Button();
			this.mbtn_remove_current = new System.Windows.Forms.Button();
			this.mpnl_selected_columns.SuspendLayout();
			this.mpnl_columns.SuspendLayout();
			this.mpnl_buttons.SuspendLayout();
			this.SuspendLayout();
			// 
			// mpnl_selected_columns
			// 
			this.mpnl_selected_columns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mpnl_selected_columns.Controls.Add(this.mlst_selected_columns);
			this.mpnl_selected_columns.Dock = System.Windows.Forms.DockStyle.Right;
			this.mpnl_selected_columns.DockPadding.All = 5;
			this.mpnl_selected_columns.Location = new System.Drawing.Point(299, 5);
			this.mpnl_selected_columns.Name = "mpnl_selected_columns";
			this.mpnl_selected_columns.Size = new System.Drawing.Size(216, 190);
			this.mpnl_selected_columns.TabIndex = 15;
			// 
			// mlst_selected_columns
			// 
			this.mlst_selected_columns.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mlst_selected_columns.Location = new System.Drawing.Point(5, 5);
			this.mlst_selected_columns.Name = "mlst_selected_columns";
			this.mlst_selected_columns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.mlst_selected_columns.Size = new System.Drawing.Size(204, 173);
			this.mlst_selected_columns.TabIndex = 0;
			// 
			// mpnl_columns
			// 
			this.mpnl_columns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mpnl_columns.Controls.Add(this.mlst_columns);
			this.mpnl_columns.Dock = System.Windows.Forms.DockStyle.Left;
			this.mpnl_columns.DockPadding.All = 5;
			this.mpnl_columns.Location = new System.Drawing.Point(5, 5);
			this.mpnl_columns.Name = "mpnl_columns";
			this.mpnl_columns.Size = new System.Drawing.Size(224, 190);
			this.mpnl_columns.TabIndex = 14;
			// 
			// mlst_columns
			// 
			this.mlst_columns.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mlst_columns.Location = new System.Drawing.Point(5, 5);
			this.mlst_columns.Name = "mlst_columns";
			this.mlst_columns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.mlst_columns.Size = new System.Drawing.Size(212, 173);
			this.mlst_columns.TabIndex = 0;
			this.mlst_columns.SelectedIndexChanged += new System.EventHandler(this.mlst_columns_SelectedIndexChanged);
			// 
			// mpnl_buttons
			// 
			this.mpnl_buttons.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.mpnl_buttons.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(255)));
			this.mpnl_buttons.Controls.Add(this.mbtn_move_all);
			this.mpnl_buttons.Controls.Add(this.mbtn_move_current);
			this.mpnl_buttons.Controls.Add(this.mbtn_remove_all);
			this.mpnl_buttons.Controls.Add(this.mbtn_remove_current);
			this.mpnl_buttons.Location = new System.Drawing.Point(244, 8);
			this.mpnl_buttons.Name = "mpnl_buttons";
			this.mpnl_buttons.Size = new System.Drawing.Size(32, 80);
			this.mpnl_buttons.TabIndex = 20;
			// 
			// mbtn_move_all
			// 
			this.mbtn_move_all.BackColor = System.Drawing.SystemColors.Control;
			this.mbtn_move_all.Dock = System.Windows.Forms.DockStyle.Top;
			this.mbtn_move_all.Location = new System.Drawing.Point(0, 60);
			this.mbtn_move_all.Name = "mbtn_move_all";
			this.mbtn_move_all.Size = new System.Drawing.Size(32, 20);
			this.mbtn_move_all.TabIndex = 20;
			this.mbtn_move_all.Text = ">>";
			this.mbtn_move_all.Click += new System.EventHandler(this.mbtn_move_all_Click);
			// 
			// mbtn_move_current
			// 
			this.mbtn_move_current.BackColor = System.Drawing.SystemColors.Control;
			this.mbtn_move_current.Dock = System.Windows.Forms.DockStyle.Top;
			this.mbtn_move_current.Enabled = false;
			this.mbtn_move_current.Location = new System.Drawing.Point(0, 40);
			this.mbtn_move_current.Name = "mbtn_move_current";
			this.mbtn_move_current.Size = new System.Drawing.Size(32, 20);
			this.mbtn_move_current.TabIndex = 21;
			this.mbtn_move_current.Text = ">";
			this.mbtn_move_current.Click += new System.EventHandler(this.mbtn_move_current_Click);
			// 
			// mbtn_remove_all
			// 
			this.mbtn_remove_all.BackColor = System.Drawing.SystemColors.Control;
			this.mbtn_remove_all.Dock = System.Windows.Forms.DockStyle.Top;
			this.mbtn_remove_all.Enabled = false;
			this.mbtn_remove_all.Location = new System.Drawing.Point(0, 20);
			this.mbtn_remove_all.Name = "mbtn_remove_all";
			this.mbtn_remove_all.Size = new System.Drawing.Size(32, 20);
			this.mbtn_remove_all.TabIndex = 22;
			this.mbtn_remove_all.Text = "<<";
			this.mbtn_remove_all.Click += new System.EventHandler(this.mbtn_remove_all_Click);
			// 
			// mbtn_remove_current
			// 
			this.mbtn_remove_current.BackColor = System.Drawing.SystemColors.Control;
			this.mbtn_remove_current.Dock = System.Windows.Forms.DockStyle.Top;
			this.mbtn_remove_current.Enabled = false;
			this.mbtn_remove_current.Location = new System.Drawing.Point(0, 0);
			this.mbtn_remove_current.Name = "mbtn_remove_current";
			this.mbtn_remove_current.Size = new System.Drawing.Size(32, 20);
			this.mbtn_remove_current.TabIndex = 23;
			this.mbtn_remove_current.Text = "<";
			this.mbtn_remove_current.Click += new System.EventHandler(this.mbtn_remove_current_Click);
			// 
			// ctlListSelection
			// 
			this.Controls.Add(this.mpnl_selected_columns);
			this.Controls.Add(this.mpnl_columns);
			this.Controls.Add(this.mpnl_buttons);
			this.DockPadding.All = 5;
			this.Name = "ctlListSelection";
			this.Size = new System.Drawing.Size(520, 200);
			this.mpnl_selected_columns.ResumeLayout(false);
			this.mpnl_columns.ResumeLayout(false);
			this.mpnl_buttons.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void mbtn_move_all_Click(object sender, System.EventArgs e)
		{
			for (int col_num = 0 ; col_num < mlst_columns.Items.Count ; col_num++)
			{
				mlst_selected_columns.Items.Add(mlst_columns.Items[col_num]) ; 
			}
			mlst_columns.Items.Clear() ; 
			mbtn_move_all.Enabled = false ; 
			mbtn_move_current.Enabled = false ; 
			mbtn_remove_all.Enabled = true ; 
			mbtn_remove_current.Enabled = true ; 

			if (this.SelectList!=null)
				this.SelectList(this.SelectedColumnIndices);
		}

		private void mbtn_remove_all_Click(object sender, System.EventArgs e)
		{
			for (int col_num = 0 ; col_num < mlst_selected_columns.Items.Count ; col_num++)
			{
				mlst_columns.Items.Add(mlst_selected_columns.Items[col_num]) ; 
			}
			mlst_selected_columns.Items.Clear() ; 
			mbtn_remove_all.Enabled = false ; 
			mbtn_remove_current.Enabled = false ; 
			mbtn_move_all.Enabled = true ; 
			mbtn_move_current.Enabled = true ; 

			if (this.SelectList!=null)
				this.SelectList(this.SelectedColumnIndices);
		}

		private void mbtn_move_current_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (mlst_columns.SelectedIndices.Count== 0)
					return ; 
				ArrayList remove_items = new ArrayList() ; 

				for (int i = 0 ; i < mlst_columns.SelectedIndices.Count ; i++)
				{
					int selected_index = mlst_columns.SelectedIndices[i] ; 
					mlst_selected_columns.Items.Add(mlst_columns.Items[selected_index]) ; 
					remove_items.Add(mlst_columns.Items[selected_index]) ; 
				}
				for (int i = 0 ; i < remove_items.Count ; i++)
				{
					mlst_columns.Items.Remove(remove_items[i]) ; 
				}

				if (mlst_columns.Items.Count == 0)
				{
					mbtn_move_all.Enabled = false ; 
					mbtn_move_current.Enabled = false ; 
				}

				if (this.SelectList!=null)
					this.SelectList(this.SelectedColumnIndices);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace + " " + ex.Message)  ;
			}
		}

		private void mbtn_remove_current_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (mlst_selected_columns.SelectedIndices.Count== 0)
					return ; 
				ArrayList remove_items = new ArrayList() ; 

				for (int i = 0 ; i < mlst_selected_columns.SelectedIndices.Count ; i++)
				{
					int selected_index = mlst_selected_columns.SelectedIndices[i] ; 
					mlst_columns.Items.Add(mlst_selected_columns.Items[selected_index]) ; 
					remove_items.Add(mlst_selected_columns.Items[selected_index]) ; 
				}
				for (int i = 0 ; i < remove_items.Count ; i++)
				{
					mlst_selected_columns.Items.Remove(remove_items[i]) ; 
				}

				if (mlst_selected_columns.Items.Count == 0)
				{
					mbtn_remove_all.Enabled = false ; 
					mbtn_remove_current.Enabled = false ; 
				}

				if (this.SelectList!=null)
					this.SelectList(this.SelectedColumnIndices);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace + " " + ex.Message)  ;
			}		
		}

		private void mlst_columns_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (mlst_selected_columns.Items.Count > 0)
			{
				mbtn_remove_all.Enabled = true ; 
				mbtn_remove_current.Enabled = true ; 
			}
			if (mlst_columns.Items.Count > 0)
			{
				mbtn_move_all.Enabled = true ; 
				mbtn_move_current.Enabled = true ; 
			}
		}

		public string [] SelectedColumns
		{
			get
			{
				string [] selected_columns = new string [mlst_selected_columns.Items.Count] ; 
				for (int selected_column_num = 0 ; selected_column_num < mlst_selected_columns.Items.Count ; selected_column_num++)
				{
					string selected_column = (string)mlst_selected_columns.Items[selected_column_num] ; 
					selected_columns[selected_column_num] = selected_column ; 
				}
				return selected_columns ; 
			}
		}

		public int [] SelectedColumnIndices
		{
			get
			{
				// look through the table for the columns names
				string [] column_names = SelectedColumns ; 
				int [] column_indices = new int [column_names.Length] ; 
				for (int col_num = 0 ; col_num < column_names.Length ; col_num++)
				{
					string col_name = column_names[col_num] ; 
					int table_col_index = mtbl_data.Columns.IndexOf(col_name) ; 
					column_indices[col_num] = table_col_index ; 
				}
				return column_indices ; 
			}
		}

		public void SelectColumn(string name, bool select)
		{
			for (int i=0; i<mlst_columns.Items.Count; i++)
				if (((mlst_columns.Items[i]) as String) == name)
					mlst_columns.SetSelected(i, select);
		}


		protected override void OnResize(EventArgs e)
		{
			mbtn_move_all.Left = this.Width/2 - mbtn_move_all.Width/2 ; 
			mbtn_move_current.Left = this.Width/2 - mbtn_move_current.Width/2 ; 
			mbtn_remove_all.Left = this.Width/2 - mbtn_remove_all.Width/2 ; 
			mbtn_remove_current.Left = this.Width/2 - mbtn_remove_current.Width /2 ; 
			mpnl_selected_columns.Width = this.Width/2 - mbtn_remove_current.Width ; 
			mpnl_columns.Width = this.Width/2 - mbtn_remove_current.Width ; 

			base.OnResize (e);
		}
	}
}
