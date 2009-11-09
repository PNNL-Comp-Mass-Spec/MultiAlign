using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlGroupMembership.
	/// </summary>
	public class ctlGroupMembership : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox mtxt_num_groups;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.DataGrid mdataGrid_grouping;
		private int mint_num_groups = 0 ; 
		private string [] marr_group_names = null ; 
		private string mstr_columnname_col = "Dataset" ; 
		private string mstr_defaultgroupname_col = "Group #" ; 
		private System.Windows.Forms.Panel mpanel_groups; 
		private int mint_start_column = 0 ; 

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlGroupMembership()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			Init() ;

		}

		private void Init()
		{
			DataTable table = new DataTable("Datasets") ; 
			table.Columns.Clear() ; 
			table.Columns.Add(mstr_columnname_col) ;
			mdataGrid_grouping.DataSource = table ; 

			DataGridTableStyle dgt = new DataGridTableStyle();
			dgt.MappingName = table.TableName ;
			mdataGrid_grouping.TableStyles.Add(dgt) ; 

		}

		/// <summary>
		/// Sets the ColumnNames of provided table as the dataset names
		/// (i.e. row entries) of the data grid. 
		/// </summary>
		/// <param name="table"></param>
		private void AddDatasetNames(DataTable table, int start_column)
		{
			DataTable current_table = (DataTable) mdataGrid_grouping.DataSource ; 
			current_table.Rows.Clear() ;
			for (int column_num = start_column ; column_num < table.Columns.Count ; column_num++)
			{
				string col_name = table.Columns[column_num].ColumnName ; 
				DataRow row = current_table.NewRow() ; 
				row[0] = col_name ; 
				if (mint_num_groups > 0)
					row[1] = true ; 
				for (int group_num = 2 ; group_num <= mint_num_groups ; group_num++)
				{
					row[group_num] = false ; 
				}
				current_table.Rows.Add(row) ; 
			}
		}

		/// <summary>
		/// Sets the number of groups. If the provided number of groups is 
		/// less than the current number of groups, then the last few groups 
		/// are deleted. The memberships of people set to those groups 
		/// is also changed. If its greater than the current number of groups
		/// then new ones are added and the names of the current groups is 
		/// not changed.
		/// </summary>
		/// <param name="num_groups"></param>
		private void SetNumGroups(int num_groups)
		{
			DataTable myTable = (DataTable) mdataGrid_grouping.DataSource ;

			if (num_groups < 2)
			{
				throw (new System.Exception("Number of groups has to be greater than 2")) ;
			}
			if (num_groups > mint_num_groups)
			{
				string [] temp_group_names = new string [num_groups] ; 
				DataGridTableStyle dgt = mdataGrid_grouping.TableStyles[0] ; 
				for (int group_num = 0 ; group_num < mint_num_groups ; group_num++)
				{
					temp_group_names[group_num] = marr_group_names[group_num] ; 
				}
				for (int group_num = mint_num_groups ; group_num < num_groups ; group_num++)
				{
					string group_name = mstr_defaultgroupname_col + Convert.ToString(group_num+1) ; 
					temp_group_names[group_num] = group_name ; 
					myTable.Columns.Add(new DataColumn(group_name, typeof(bool)));

					DataGridBoolColumn dgbc = new DataGridBoolColumn();
					dgbc.MappingName = group_name ;
					dgbc.HeaderText= group_name ;
					dgbc.AllowNull = false ; 
					dgbc.TrueValueChanged +=new EventHandler(dgbc_TrueValueChanged);
					dgt.GridColumnStyles.Add(dgbc);
				}
				marr_group_names = temp_group_names ; 
				mint_num_groups = num_groups ; 
			}
		}

		public void SetOptions(int num_groups, DataTable source_table, int start_column)
		{
			mint_start_column = start_column ; 
			SetNumGroups(num_groups) ;
			AddDatasetNames(source_table, start_column) ;  
			mtxt_num_groups.Text = Convert.ToString(num_groups) ; 
		}
		public bool NumGroupsEditable
		{
			set
			{
				mpanel_groups.Enabled = value ; 
			}
		}

		public int [] GroupIndices
		{
			get
			{
				DataTable table = (DataTable)mdataGrid_grouping.DataSource ;
				int num_rows = table.Rows.Count ; 
				int [] group_indices = new int[num_rows+mint_start_column] ; 
				for (int i = 0 ; i < num_rows+mint_start_column ; i++)
				{
					group_indices[i] = -1 ; 
				}

				for (int row_num = 0 ; row_num < num_rows ; row_num++)
				{
					DataRow row = table.Rows[row_num] ; 
					for (int col_num = 1 ; col_num <= mint_num_groups ; col_num++)
					{
						bool val = (bool) row[col_num] ; 
						if (val)
						{
							group_indices[row_num+mint_start_column] = col_num-1 ; 
							break ; 
						}
					}
				}
				return group_indices ; 
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
			this.mpanel_groups = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.mtxt_num_groups = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.mdataGrid_grouping = new System.Windows.Forms.DataGrid();
			this.mpanel_groups.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mdataGrid_grouping)).BeginInit();
			this.SuspendLayout();
			// 
			// mpanel_groups
			// 
			this.mpanel_groups.Controls.Add(this.mtxt_num_groups);
			this.mpanel_groups.Controls.Add(this.label1);
			this.mpanel_groups.Dock = System.Windows.Forms.DockStyle.Top;
			this.mpanel_groups.DockPadding.All = 10;
			this.mpanel_groups.Location = new System.Drawing.Point(0, 0);
			this.mpanel_groups.Name = "mpanel_groups";
			this.mpanel_groups.Size = new System.Drawing.Size(280, 40);
			this.mpanel_groups.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of Groups ?";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mtxt_num_groups
			// 
			this.mtxt_num_groups.Dock = System.Windows.Forms.DockStyle.Left;
			this.mtxt_num_groups.Location = new System.Drawing.Point(162, 10);
			this.mtxt_num_groups.Name = "mtxt_num_groups";
			this.mtxt_num_groups.Size = new System.Drawing.Size(40, 20);
			this.mtxt_num_groups.TabIndex = 1;
			this.mtxt_num_groups.Text = "0";
			this.mtxt_num_groups.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.mdataGrid_grouping);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.DockPadding.All = 4;
			this.panel2.Location = new System.Drawing.Point(0, 40);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(280, 192);
			this.panel2.TabIndex = 1;
			// 
			// mdataGrid_grouping
			// 
			this.mdataGrid_grouping.DataMember = "";
			this.mdataGrid_grouping.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mdataGrid_grouping.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.mdataGrid_grouping.Location = new System.Drawing.Point(4, 4);
			this.mdataGrid_grouping.Name = "mdataGrid_grouping";
			this.mdataGrid_grouping.Size = new System.Drawing.Size(272, 184);
			this.mdataGrid_grouping.TabIndex = 0;
			// 
			// ctlGroupMembership
			// 
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.mpanel_groups);
			this.Name = "ctlGroupMembership";
			this.Size = new System.Drawing.Size(280, 232);
			this.mpanel_groups.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mdataGrid_grouping)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void dgbc_TrueValueChanged(object sender, EventArgs e)
		{

		}
	}
}
