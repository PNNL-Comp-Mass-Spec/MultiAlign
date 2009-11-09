using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	public enum enmInputType { TABLE = 0, NAMES} ; 
	/// <summary>
	/// Summary description for ctlGroupDefine.
	/// </summary>
	public class ctlGroupDefine : System.Windows.Forms.UserControl
	{
		#region "Controls"
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button mbtn_add_to_node;
		private System.Windows.Forms.Button mbtn_remove;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.ListView mlst_datasets;
		private System.Windows.Forms.TreeView mtree_groups;
		#endregion

		private int mint_num_groups ; 
		private ArrayList marr_dataset_names ; 
		private ArrayList marr_group_names ; 
		private Hashtable mhash_group_num ; 
		private Hashtable mhash_group_name ; 
		private Hashtable mhash_dataset_group_num ; 

		private int mint_start_column ; 
		//private enmInputType menm_input ; 
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlGroupDefine()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			Init() ;
		}

		private void Init()
		{
			mhash_group_num = new Hashtable() ; 
			mhash_dataset_group_num = new Hashtable() ; 
			mhash_group_name = new Hashtable() ; 
			marr_dataset_names = new ArrayList(); 
			marr_group_names = new ArrayList() ; 
			mint_num_groups = 0 ;
		}

		public void SetOptions(clsGroup  root_group, DataTable table, int start_column)
		{
			//menm_input = enmInputType.TABLE ; 
			mint_start_column = start_column ; 
			this.mlst_datasets.Clear() ; 
			this.mtree_groups.Nodes.Clear() ; 
			AddDatasetNames(table) ; 
			AddRootGroup(root_group) ;
		}

		public void SetOptions(clsGroup root_group, string [] dataset_names)
		{
			//menm_input = enmInputType.NAMES ; 
			mint_start_column = 0 ; 
			this.mlst_datasets.Clear() ; 
			this.mtree_groups.Nodes.Clear() ; 
			AddDatasetNames(dataset_names) ; 
			AddRootGroup(root_group) ;
		}

		public int [] GroupIndices
		{
			get
			{
				// now go through each dataset and get order indices. 
				int [] group_indices = new int [marr_dataset_names.Count + mint_start_column] ; 

				for (int index = 0 ; index < mint_start_column ; index++)
				{
					group_indices[index] = -1 ; 
				}

				for (int dataset_num = 0 ; dataset_num < marr_dataset_names.Count ; dataset_num++)
				{
					group_indices[dataset_num+mint_start_column] = (int) mhash_dataset_group_num[marr_dataset_names[dataset_num]] ; 
				}
				return group_indices ; 
			}
			set
			{
			}
		}

		public string [] GroupNames 
		{
			get
			{
				string [] group_names = new string [mint_num_groups] ; 
				for (int group_num = 0  ; group_num < mint_num_groups ; group_num++)
				{
					group_names[group_num] = (string) marr_group_names[group_num] ; 
				}
				return group_names ; 
			}
			set
			{
			}
		}

		public int NumGroups
		{
			get
			{
				return mint_num_groups ; 
			}
		}
		/// <summary>
		/// Sets the ColumnNames of provided table as the dataset names
		/// (i.e. row entries) of the data grid. 
		/// </summary>
		/// <param name="table"></param>
		private void AddDatasetNames(DataTable table)
		{
			this.mlst_datasets.Clear() ; 
			for (int column_num = mint_start_column ; column_num < table.Columns.Count ; column_num++)
			{
				string col_name = table.Columns[column_num].ColumnName ; 
				this.mlst_datasets.Items.Add(col_name) ; 
				marr_dataset_names.Add(col_name) ; 
			}
		}

		/// <summary>
		/// Sets the entries in the array as the dataset names
		/// </summary>
		/// <param name="dataset_names"></param>
		private void AddDatasetNames(string [] dataset_names)
		{
			this.mlst_datasets.Clear() ; 
			for (int index = 0 ; index < dataset_names.Length ; index++)
			{
				string col_name = dataset_names[index] ; 
				this.mlst_datasets.Items.Add(col_name) ; 
				marr_dataset_names.Add(col_name) ; 
			}
		}

		private void AddRootGroup(clsGroup group)
		{
			TreeNode memnode = new TreeNode(group.Name) ;
			memnode.Tag = group ; 
			AddGroupsToList(memnode, group.marr_members) ; 
			mtree_groups.Nodes.Add(memnode) ; 
		}

		private void AddGroupsToList(TreeNode root_node, clsGroup [] grp)
		{
			if (grp == null || grp.Length == 0)
				return ; 

			int num_groups = grp.Length ; 
			for (int group_num = 0 ; group_num < num_groups ; group_num++)
			{
				TreeNode memnode = new TreeNode(grp[group_num].Name) ;
				memnode.Tag = grp[group_num] ; 
				mhash_group_name[grp[group_num]] = grp[group_num].Name ; 
				marr_group_names.Add(grp[group_num].Name) ; 
				mhash_group_num[grp[group_num]] = mint_num_groups ; 
				AddGroupsToList(memnode, grp[group_num].marr_members) ; 
				root_node.Nodes.Add(memnode) ; 
				mint_num_groups++ ; 
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.mlst_datasets = new System.Windows.Forms.ListView();
			this.panel2 = new System.Windows.Forms.Panel();
			this.mbtn_remove = new System.Windows.Forms.Button();
			this.mbtn_add_to_node = new System.Windows.Forms.Button();
			this.panel3 = new System.Windows.Forms.Panel();
			this.mtree_groups = new System.Windows.Forms.TreeView();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.mlst_datasets);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(256, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(208, 312);
			this.panel1.TabIndex = 0;
			// 
			// mlst_datasets
			// 
			this.mlst_datasets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mlst_datasets.Location = new System.Drawing.Point(0, 0);
			this.mlst_datasets.Name = "mlst_datasets";
			this.mlst_datasets.Size = new System.Drawing.Size(208, 312);
			this.mlst_datasets.TabIndex = 0;
			this.mlst_datasets.View = System.Windows.Forms.View.List;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.mbtn_remove);
			this.panel2.Controls.Add(this.mbtn_add_to_node);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(216, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(40, 312);
			this.panel2.TabIndex = 1;
			// 
			// mbtn_remove
			// 
			this.mbtn_remove.Location = new System.Drawing.Point(8, 144);
			this.mbtn_remove.Name = "mbtn_remove";
			this.mbtn_remove.Size = new System.Drawing.Size(24, 24);
			this.mbtn_remove.TabIndex = 1;
			this.mbtn_remove.Text = ">";
			this.mbtn_remove.Click += new System.EventHandler(this.mbtn_remove_Click);
			// 
			// mbtn_add_to_node
			// 
			this.mbtn_add_to_node.Location = new System.Drawing.Point(8, 88);
			this.mbtn_add_to_node.Name = "mbtn_add_to_node";
			this.mbtn_add_to_node.Size = new System.Drawing.Size(24, 24);
			this.mbtn_add_to_node.TabIndex = 0;
			this.mbtn_add_to_node.Text = "<";
			this.mbtn_add_to_node.Click += new System.EventHandler(this.mbtn_add_to_node_Click);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.mtree_groups);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(216, 312);
			this.panel3.TabIndex = 2;
			// 
			// mtree_groups
			// 
			this.mtree_groups.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mtree_groups.HideSelection = false;
			this.mtree_groups.ImageIndex = -1;
			this.mtree_groups.Location = new System.Drawing.Point(0, 0);
			this.mtree_groups.Name = "mtree_groups";
			this.mtree_groups.SelectedImageIndex = -1;
			this.mtree_groups.Size = new System.Drawing.Size(216, 312);
			this.mtree_groups.TabIndex = 0;
			// 
			// ctlGroupDefine
			// 
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "ctlGroupDefine";
			this.Size = new System.Drawing.Size(464, 312);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void mbtn_add_to_node_Click(object sender, System.EventArgs e)
		{
			// Add the datasets to tree.
			int num_datasets_selected = mlst_datasets.SelectedItems.Count ; 

			if (num_datasets_selected == 0)
			{
				MessageBox.Show(this, "Please select a dataset before adding to a group") ; 
				return ; 
			}

			// Selected tree node..
			TreeNode selected_node = mtree_groups.SelectedNode ; 

			if (selected_node == null)
			{
				MessageBox.Show(this, "Please select a node before proceeding") ; 
				return ; 
			}
			if (selected_node.Tag == null)
				return ; 

			if (selected_node.Tag.GetType() != typeof(clsGroup))
				return ; 

			ArrayList selected_indices = new ArrayList() ; 

			for (int dataset_num = 0 ; dataset_num < num_datasets_selected ; dataset_num++)
			{
				string dataset_name = mlst_datasets.SelectedItems[dataset_num].Text ;
				TreeNode node = new TreeNode(dataset_name) ;
				selected_indices.Add(mlst_datasets.SelectedIndices[dataset_num]) ; 
				mhash_dataset_group_num[dataset_name] = mhash_group_num[selected_node.Tag] ; 
				selected_node.Nodes.Add(node) ; 
			}
			selected_indices.Sort() ; 
			for (int item_num = selected_indices.Count - 1 ; item_num >= 0 ; item_num--)
			{
				mlst_datasets.Items.RemoveAt((int)selected_indices[item_num]) ; 
			}
		}

		private void mbtn_remove_Click(object sender, System.EventArgs e)
		{
		
		}

	}
}
