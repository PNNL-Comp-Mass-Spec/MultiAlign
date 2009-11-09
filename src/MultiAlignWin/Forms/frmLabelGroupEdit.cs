using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PNNLProteomics.Data.Factors;

using PNNLControls;

namespace MultiAlignWin.Forms.Factors
{
	public class frmLabelGroupEdit : PNNLControls.frmDialogBase
    {
        #region Members
        private const int SIZE_COLOR_SUBITEM = 500;

		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.ListView listTopology;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelInsideTop;
		private System.Windows.Forms.Label lblInsideBottom;
		private System.Windows.Forms.Panel panelTopRight;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.ListView listEnumerations;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label lblEnums;
		private System.Windows.Forms.Label lblGroups;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Splitter splitter1;
		private classTreeNode		m_rootTreeNode;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnMoveLeft;
		private System.Windows.Forms.Button btnMoveRight;
		
		private classFactorTree	m_factorTree;
		private System.Windows.Forms.Button btnReverseEnumerations;
		private System.Windows.Forms.Panel panelPreview;
		private PNNLControls.ctlHierarchalLabel m_previewLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chkOrientation;
		private int m_currColumn = 0;
		private System.Windows.Forms.Panel panelPreviewLabelContainer;
		private System.Windows.Forms.CheckBox chkShowPreview;
		private int m_currEnumRow = 0;

        #endregion
        
        public frmLabelGroupEdit()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			InitControl();

			m_rootTreeNode = null;
			m_factorTree   = new classFactorTree();		
	
			m_previewLabel = new ctlHierarchalLabel();
			m_previewLabel.Vertical = false;
			m_previewLabel.Dock = DockStyle.Fill;
			m_previewLabel.BringToFront();
			panelPreviewLabelContainer.Controls.Add(m_previewLabel);	


			this.listEnumerations.SelectedIndexChanged += new EventHandler(listEnumerations_SelectedIndexChanged);
			Load += new EventHandler(frmLabelGroupEdit_Load);
			
		}
		public classTreeNode BuildTree()
		{
			return BuildTree(ref m_factorTree);
		}

		/// <summary>
		/// Builds a clsTreeNode hierarchy based on the data supplied here.
		/// </summary>
		/// <returns>Tree hierarchy for visualization.</returns>
		public classTreeNode BuildTree(ref classFactorTree tree)
		{	
			tree.ClearFactors();
			for(int j = 1; j < listTopology.Columns.Count; j++)
			{
				StringCollection col = new StringCollection();
				for(int i = 0; i < listTopology.Items.Count; i++)
				{
					if (listTopology.Items[i].SubItems.Count > j)
						if (listTopology.Items[i].SubItems[j].Text != string.Empty)
							col.Add(listTopology.Items[i].SubItems[j].Text);
				}				
				tree.AddFactor(listTopology.Columns[j].Text, col);
			}
			m_rootTreeNode = tree.BuildTree();
			return m_rootTreeNode;
		}

		
		public classFactorTree FactorTree
		{	
			get
			{
				return m_factorTree;
			}
			set
			{
				m_factorTree = value;
				if (value == null) 
					return;

				/// 
				/// Create the full table
				/// 
				this.listTopology.Columns.Clear();
				this.listTopology.Columns.Add(string.Empty,-1,System.Windows.Forms.HorizontalAlignment.Left);

				foreach(clsFactor factor in m_factorTree.Factors)
				{
					listTopology.Columns.Add(factor.Name,-2, System.Windows.Forms.HorizontalAlignment.Left);			
					m_maxRows = Math.Max(m_maxRows, factor.Values.Count);
				}
				CreateFullTable();				

				/// 
				/// Now we can index all of the subitems properly
				/// 
				int i = 0;
				foreach(clsFactor factor in m_factorTree.Factors)
				{
					/// 
					/// Add the factor values in, we have made enough space already for them
					/// Now we hash out the right order from the factor tree of this factor value
					/// and go for it.
					/// 					
					foreach (string factorValue in factor.Values.Keys)
					{
						try
						{
							long index = ((long)factor.Values[factorValue]);						
							ListViewItem item = listTopology.Items[Convert.ToInt32(index)];
							item.SubItems[i + 1].Text = factorValue;
						}catch(Exception ex)
						{
							System.Console.WriteLine(ex.Message);
						}
					}
					i++;
				}	
			
				/// For now, add the data without the sorting yet.
				for(int col = 1; col < listTopology.Columns.Count; col++)				
				{
					listTopology.Columns[col].Width = listTopology.Width  / (listTopology.Columns.Count - 1);
				}
				listTopology.Columns[0].Width = 0;

				ShowEnumerations(1);
				HighlightColumn(1, Color.Navy, Color.White);				
				m_currColumn = 1;
				
				if (listEnumerations.Items.Count > 0)
				{
					
				}

				UpdatePreview();
			}
		}

		private int m_maxRows = -1;
		/// <summary>
		/// Clears the listviews on the form.
		/// </summary>
		public void ClearListViews()
		{
			listTopology.Clear();
			listTopology.Columns.Add("test",-1, System.Windows.Forms.HorizontalAlignment.Left);
			m_maxRows = -1;
			listEnumerations.Items.Clear();
		}

		/// <summary>
		/// Fills the subitems of the listTopology ListView.
		/// </summary>
		private void FillSubItems()
		{
			for(int row = listTopology.Items.Count; row < m_maxRows; row++)
			{
				ListViewItem item = listTopology.Items.Add(string.Empty);
				for(int col = 0; col < listTopology.Columns.Count - 2; col++)				
				{
					item.SubItems.Add(string.Empty);					
				}
			}
		}

		private void CreateFullTable()
		{
			for(int row = 0; row < m_maxRows; row++)
			{
				ListViewItem item = listTopology.Items.Add(string.Empty);
				for(int col = 0; col < listTopology.Columns.Count - 1; col++)				
				{
					item.SubItems.Add(string.Empty);					
				}
			}
		}


		/// <summary>
		/// Initialize the control with event handlers to avoid MS from hashing our control from the stupid RESX file.
		/// </summary>
		private void InitControl()
		{			
			listTopology.GridLines		 = true;			
			listTopology.KeyUp			+= new KeyEventHandler(listTopology_KeyUp);
			listTopology.ColumnClick	+= new ColumnClickEventHandler(listTopology_ColumnClick);
			listTopology.Items.Clear();		
			ClearListViews();	
			listEnumerations.Columns.Add("Enumeration", -2, HorizontalAlignment.Left);			
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmLabelGroupEdit));
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnMoveRight = new System.Windows.Forms.Button();
			this.btnMoveLeft = new System.Windows.Forms.Button();
			this.listTopology = new System.Windows.Forms.ListView();
			this.panelInsideTop = new System.Windows.Forms.Panel();
			this.lblGroups = new System.Windows.Forms.Label();
			this.lblInsideBottom = new System.Windows.Forms.Label();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.listEnumerations = new System.Windows.Forms.ListView();
			this.panelTopRight = new System.Windows.Forms.Panel();
			this.btnReverseEnumerations = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lblEnums = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panelPreview = new System.Windows.Forms.Panel();
			this.chkShowPreview = new System.Windows.Forms.CheckBox();
			this.panelPreviewLabelContainer = new System.Windows.Forms.Panel();
			this.chkOrientation = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panelTop.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panelInsideTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.panelTopRight.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panelPreview.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.AccessibleDescription = resources.GetString("panelTop.AccessibleDescription");
			this.panelTop.AccessibleName = resources.GetString("panelTop.AccessibleName");
			this.panelTop.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelTop.Anchor")));
			this.panelTop.AutoScroll = ((bool)(resources.GetObject("panelTop.AutoScroll")));
			this.panelTop.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelTop.AutoScrollMargin")));
			this.panelTop.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelTop.AutoScrollMinSize")));
			this.panelTop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelTop.BackgroundImage")));
			this.panelTop.Controls.Add(this.panel1);
			this.panelTop.Controls.Add(this.listTopology);
			this.panelTop.Controls.Add(this.panelInsideTop);
			this.panelTop.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelTop.Dock")));
			this.panelTop.Enabled = ((bool)(resources.GetObject("panelTop.Enabled")));
			this.panelTop.Font = ((System.Drawing.Font)(resources.GetObject("panelTop.Font")));
			this.panelTop.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelTop.ImeMode")));
			this.panelTop.Location = ((System.Drawing.Point)(resources.GetObject("panelTop.Location")));
			this.panelTop.Name = "panelTop";
			this.panelTop.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelTop.RightToLeft")));
			this.panelTop.Size = ((System.Drawing.Size)(resources.GetObject("panelTop.Size")));
			this.panelTop.TabIndex = ((int)(resources.GetObject("panelTop.TabIndex")));
			this.panelTop.Text = resources.GetString("panelTop.Text");
			this.panelTop.Visible = ((bool)(resources.GetObject("panelTop.Visible")));
			// 
			// panel1
			// 
			this.panel1.AccessibleDescription = resources.GetString("panel1.AccessibleDescription");
			this.panel1.AccessibleName = resources.GetString("panel1.AccessibleName");
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel1.Anchor")));
			this.panel1.AutoScroll = ((bool)(resources.GetObject("panel1.AutoScroll")));
			this.panel1.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel1.AutoScrollMargin")));
			this.panel1.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel1.AutoScrollMinSize")));
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.Controls.Add(this.btnMoveRight);
			this.panel1.Controls.Add(this.btnMoveLeft);
			this.panel1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel1.Dock")));
			this.panel1.Enabled = ((bool)(resources.GetObject("panel1.Enabled")));
			this.panel1.Font = ((System.Drawing.Font)(resources.GetObject("panel1.Font")));
			this.panel1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel1.ImeMode")));
			this.panel1.Location = ((System.Drawing.Point)(resources.GetObject("panel1.Location")));
			this.panel1.Name = "panel1";
			this.panel1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel1.RightToLeft")));
			this.panel1.Size = ((System.Drawing.Size)(resources.GetObject("panel1.Size")));
			this.panel1.TabIndex = ((int)(resources.GetObject("panel1.TabIndex")));
			this.panel1.Text = resources.GetString("panel1.Text");
			this.panel1.Visible = ((bool)(resources.GetObject("panel1.Visible")));
			// 
			// btnMoveRight
			// 
			this.btnMoveRight.AccessibleDescription = resources.GetString("btnMoveRight.AccessibleDescription");
			this.btnMoveRight.AccessibleName = resources.GetString("btnMoveRight.AccessibleName");
			this.btnMoveRight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnMoveRight.Anchor")));
			this.btnMoveRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMoveRight.BackgroundImage")));
			this.btnMoveRight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnMoveRight.Dock")));
			this.btnMoveRight.Enabled = ((bool)(resources.GetObject("btnMoveRight.Enabled")));
			this.btnMoveRight.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnMoveRight.FlatStyle")));
			this.btnMoveRight.Font = ((System.Drawing.Font)(resources.GetObject("btnMoveRight.Font")));
			this.btnMoveRight.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveRight.Image")));
			this.btnMoveRight.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveRight.ImageAlign")));
			this.btnMoveRight.ImageIndex = ((int)(resources.GetObject("btnMoveRight.ImageIndex")));
			this.btnMoveRight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnMoveRight.ImeMode")));
			this.btnMoveRight.Location = ((System.Drawing.Point)(resources.GetObject("btnMoveRight.Location")));
			this.btnMoveRight.Name = "btnMoveRight";
			this.btnMoveRight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnMoveRight.RightToLeft")));
			this.btnMoveRight.Size = ((System.Drawing.Size)(resources.GetObject("btnMoveRight.Size")));
			this.btnMoveRight.TabIndex = ((int)(resources.GetObject("btnMoveRight.TabIndex")));
			this.btnMoveRight.Text = resources.GetString("btnMoveRight.Text");
			this.btnMoveRight.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveRight.TextAlign")));
			this.btnMoveRight.Visible = ((bool)(resources.GetObject("btnMoveRight.Visible")));
			this.btnMoveRight.Click += new System.EventHandler(this.btnMoveRight_Click);
			// 
			// btnMoveLeft
			// 
			this.btnMoveLeft.AccessibleDescription = resources.GetString("btnMoveLeft.AccessibleDescription");
			this.btnMoveLeft.AccessibleName = resources.GetString("btnMoveLeft.AccessibleName");
			this.btnMoveLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnMoveLeft.Anchor")));
			this.btnMoveLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMoveLeft.BackgroundImage")));
			this.btnMoveLeft.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnMoveLeft.Dock")));
			this.btnMoveLeft.Enabled = ((bool)(resources.GetObject("btnMoveLeft.Enabled")));
			this.btnMoveLeft.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnMoveLeft.FlatStyle")));
			this.btnMoveLeft.Font = ((System.Drawing.Font)(resources.GetObject("btnMoveLeft.Font")));
			this.btnMoveLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveLeft.Image")));
			this.btnMoveLeft.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveLeft.ImageAlign")));
			this.btnMoveLeft.ImageIndex = ((int)(resources.GetObject("btnMoveLeft.ImageIndex")));
			this.btnMoveLeft.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnMoveLeft.ImeMode")));
			this.btnMoveLeft.Location = ((System.Drawing.Point)(resources.GetObject("btnMoveLeft.Location")));
			this.btnMoveLeft.Name = "btnMoveLeft";
			this.btnMoveLeft.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnMoveLeft.RightToLeft")));
			this.btnMoveLeft.Size = ((System.Drawing.Size)(resources.GetObject("btnMoveLeft.Size")));
			this.btnMoveLeft.TabIndex = ((int)(resources.GetObject("btnMoveLeft.TabIndex")));
			this.btnMoveLeft.Text = resources.GetString("btnMoveLeft.Text");
			this.btnMoveLeft.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveLeft.TextAlign")));
			this.btnMoveLeft.Visible = ((bool)(resources.GetObject("btnMoveLeft.Visible")));
			this.btnMoveLeft.Click += new System.EventHandler(this.btnMoveLeft_Click);
			// 
			// listTopology
			// 
			this.listTopology.AccessibleDescription = resources.GetString("listTopology.AccessibleDescription");
			this.listTopology.AccessibleName = resources.GetString("listTopology.AccessibleName");
			this.listTopology.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("listTopology.Alignment")));
			this.listTopology.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("listTopology.Anchor")));
			this.listTopology.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listTopology.BackgroundImage")));
			this.listTopology.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.listTopology.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("listTopology.Dock")));
			this.listTopology.Enabled = ((bool)(resources.GetObject("listTopology.Enabled")));
			this.listTopology.Font = ((System.Drawing.Font)(resources.GetObject("listTopology.Font")));
			this.listTopology.GridLines = true;
			this.listTopology.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("listTopology.ImeMode")));
			this.listTopology.LabelWrap = ((bool)(resources.GetObject("listTopology.LabelWrap")));
			this.listTopology.Location = ((System.Drawing.Point)(resources.GetObject("listTopology.Location")));
			this.listTopology.MultiSelect = false;
			this.listTopology.Name = "listTopology";
			this.listTopology.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("listTopology.RightToLeft")));
			this.listTopology.Size = ((System.Drawing.Size)(resources.GetObject("listTopology.Size")));
			this.listTopology.TabIndex = ((int)(resources.GetObject("listTopology.TabIndex")));
			this.listTopology.Text = resources.GetString("listTopology.Text");
			this.listTopology.View = System.Windows.Forms.View.Details;
			this.listTopology.Visible = ((bool)(resources.GetObject("listTopology.Visible")));
			// 
			// panelInsideTop
			// 
			this.panelInsideTop.AccessibleDescription = resources.GetString("panelInsideTop.AccessibleDescription");
			this.panelInsideTop.AccessibleName = resources.GetString("panelInsideTop.AccessibleName");
			this.panelInsideTop.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelInsideTop.Anchor")));
			this.panelInsideTop.AutoScroll = ((bool)(resources.GetObject("panelInsideTop.AutoScroll")));
			this.panelInsideTop.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelInsideTop.AutoScrollMargin")));
			this.panelInsideTop.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelInsideTop.AutoScrollMinSize")));
			this.panelInsideTop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelInsideTop.BackgroundImage")));
			this.panelInsideTop.Controls.Add(this.lblGroups);
			this.panelInsideTop.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelInsideTop.Dock")));
			this.panelInsideTop.Enabled = ((bool)(resources.GetObject("panelInsideTop.Enabled")));
			this.panelInsideTop.Font = ((System.Drawing.Font)(resources.GetObject("panelInsideTop.Font")));
			this.panelInsideTop.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelInsideTop.ImeMode")));
			this.panelInsideTop.Location = ((System.Drawing.Point)(resources.GetObject("panelInsideTop.Location")));
			this.panelInsideTop.Name = "panelInsideTop";
			this.panelInsideTop.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelInsideTop.RightToLeft")));
			this.panelInsideTop.Size = ((System.Drawing.Size)(resources.GetObject("panelInsideTop.Size")));
			this.panelInsideTop.TabIndex = ((int)(resources.GetObject("panelInsideTop.TabIndex")));
			this.panelInsideTop.Text = resources.GetString("panelInsideTop.Text");
			this.panelInsideTop.Visible = ((bool)(resources.GetObject("panelInsideTop.Visible")));
			// 
			// lblGroups
			// 
			this.lblGroups.AccessibleDescription = resources.GetString("lblGroups.AccessibleDescription");
			this.lblGroups.AccessibleName = resources.GetString("lblGroups.AccessibleName");
			this.lblGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblGroups.Anchor")));
			this.lblGroups.AutoSize = ((bool)(resources.GetObject("lblGroups.AutoSize")));
			this.lblGroups.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblGroups.Dock")));
			this.lblGroups.Enabled = ((bool)(resources.GetObject("lblGroups.Enabled")));
			this.lblGroups.Font = ((System.Drawing.Font)(resources.GetObject("lblGroups.Font")));
			this.lblGroups.Image = ((System.Drawing.Image)(resources.GetObject("lblGroups.Image")));
			this.lblGroups.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblGroups.ImageAlign")));
			this.lblGroups.ImageIndex = ((int)(resources.GetObject("lblGroups.ImageIndex")));
			this.lblGroups.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblGroups.ImeMode")));
			this.lblGroups.Location = ((System.Drawing.Point)(resources.GetObject("lblGroups.Location")));
			this.lblGroups.Name = "lblGroups";
			this.lblGroups.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblGroups.RightToLeft")));
			this.lblGroups.Size = ((System.Drawing.Size)(resources.GetObject("lblGroups.Size")));
			this.lblGroups.TabIndex = ((int)(resources.GetObject("lblGroups.TabIndex")));
			this.lblGroups.Text = resources.GetString("lblGroups.Text");
			this.lblGroups.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblGroups.TextAlign")));
			this.lblGroups.Visible = ((bool)(resources.GetObject("lblGroups.Visible")));
			// 
			// lblInsideBottom
			// 
			this.lblInsideBottom.AccessibleDescription = resources.GetString("lblInsideBottom.AccessibleDescription");
			this.lblInsideBottom.AccessibleName = resources.GetString("lblInsideBottom.AccessibleName");
			this.lblInsideBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblInsideBottom.Anchor")));
			this.lblInsideBottom.AutoSize = ((bool)(resources.GetObject("lblInsideBottom.AutoSize")));
			this.lblInsideBottom.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblInsideBottom.Dock")));
			this.lblInsideBottom.Enabled = ((bool)(resources.GetObject("lblInsideBottom.Enabled")));
			this.lblInsideBottom.Font = ((System.Drawing.Font)(resources.GetObject("lblInsideBottom.Font")));
			this.lblInsideBottom.Image = ((System.Drawing.Image)(resources.GetObject("lblInsideBottom.Image")));
			this.lblInsideBottom.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblInsideBottom.ImageAlign")));
			this.lblInsideBottom.ImageIndex = ((int)(resources.GetObject("lblInsideBottom.ImageIndex")));
			this.lblInsideBottom.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblInsideBottom.ImeMode")));
			this.lblInsideBottom.Location = ((System.Drawing.Point)(resources.GetObject("lblInsideBottom.Location")));
			this.lblInsideBottom.Name = "lblInsideBottom";
			this.lblInsideBottom.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblInsideBottom.RightToLeft")));
			this.lblInsideBottom.Size = ((System.Drawing.Size)(resources.GetObject("lblInsideBottom.Size")));
			this.lblInsideBottom.TabIndex = ((int)(resources.GetObject("lblInsideBottom.TabIndex")));
			this.lblInsideBottom.Text = resources.GetString("lblInsideBottom.Text");
			this.lblInsideBottom.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblInsideBottom.TextAlign")));
			this.lblInsideBottom.Visible = ((bool)(resources.GetObject("lblInsideBottom.Visible")));
			// 
			// panelBottom
			// 
			this.panelBottom.AccessibleDescription = resources.GetString("panelBottom.AccessibleDescription");
			this.panelBottom.AccessibleName = resources.GetString("panelBottom.AccessibleName");
			this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelBottom.Anchor")));
			this.panelBottom.AutoScroll = ((bool)(resources.GetObject("panelBottom.AutoScroll")));
			this.panelBottom.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelBottom.AutoScrollMargin")));
			this.panelBottom.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelBottom.AutoScrollMinSize")));
			this.panelBottom.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelBottom.BackgroundImage")));
			this.panelBottom.Controls.Add(this.listEnumerations);
			this.panelBottom.Controls.Add(this.panelTopRight);
			this.panelBottom.Controls.Add(this.panel2);
			this.panelBottom.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelBottom.Dock")));
			this.panelBottom.Enabled = ((bool)(resources.GetObject("panelBottom.Enabled")));
			this.panelBottom.Font = ((System.Drawing.Font)(resources.GetObject("panelBottom.Font")));
			this.panelBottom.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelBottom.ImeMode")));
			this.panelBottom.Location = ((System.Drawing.Point)(resources.GetObject("panelBottom.Location")));
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelBottom.RightToLeft")));
			this.panelBottom.Size = ((System.Drawing.Size)(resources.GetObject("panelBottom.Size")));
			this.panelBottom.TabIndex = ((int)(resources.GetObject("panelBottom.TabIndex")));
			this.panelBottom.Text = resources.GetString("panelBottom.Text");
			this.panelBottom.Visible = ((bool)(resources.GetObject("panelBottom.Visible")));
			// 
			// listEnumerations
			// 
			this.listEnumerations.AccessibleDescription = resources.GetString("listEnumerations.AccessibleDescription");
			this.listEnumerations.AccessibleName = resources.GetString("listEnumerations.AccessibleName");
			this.listEnumerations.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("listEnumerations.Alignment")));
			this.listEnumerations.AllowColumnReorder = true;			
			this.listEnumerations.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("listEnumerations.Anchor")));
			this.listEnumerations.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listEnumerations.BackgroundImage")));
			this.listEnumerations.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("listEnumerations.Dock")));
			this.listEnumerations.Enabled = ((bool)(resources.GetObject("listEnumerations.Enabled")));
			this.listEnumerations.Font = ((System.Drawing.Font)(resources.GetObject("listEnumerations.Font")));
			this.listEnumerations.FullRowSelect = true;
			this.listEnumerations.GridLines = true;
			this.listEnumerations.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("listEnumerations.ImeMode")));
			this.listEnumerations.LabelWrap = ((bool)(resources.GetObject("listEnumerations.LabelWrap")));
			this.listEnumerations.Location = ((System.Drawing.Point)(resources.GetObject("listEnumerations.Location")));
			this.listEnumerations.MultiSelect = false;
			this.listEnumerations.Name = "listEnumerations";
			this.listEnumerations.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("listEnumerations.RightToLeft")));
			this.listEnumerations.Size = ((System.Drawing.Size)(resources.GetObject("listEnumerations.Size")));
			this.listEnumerations.TabIndex = ((int)(resources.GetObject("listEnumerations.TabIndex")));
			this.listEnumerations.Text = resources.GetString("listEnumerations.Text");
			this.listEnumerations.View = System.Windows.Forms.View.Details;
			this.listEnumerations.Visible = ((bool)(resources.GetObject("listEnumerations.Visible")));
			// 
			// panelTopRight
			// 
			this.panelTopRight.AccessibleDescription = resources.GetString("panelTopRight.AccessibleDescription");
			this.panelTopRight.AccessibleName = resources.GetString("panelTopRight.AccessibleName");
			this.panelTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelTopRight.Anchor")));
			this.panelTopRight.AutoScroll = ((bool)(resources.GetObject("panelTopRight.AutoScroll")));
			this.panelTopRight.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelTopRight.AutoScrollMargin")));
			this.panelTopRight.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelTopRight.AutoScrollMinSize")));
			this.panelTopRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelTopRight.BackgroundImage")));
			this.panelTopRight.Controls.Add(this.btnReverseEnumerations);
			this.panelTopRight.Controls.Add(this.btnMoveDown);
			this.panelTopRight.Controls.Add(this.btnMoveUp);
			this.panelTopRight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelTopRight.Dock")));
			this.panelTopRight.Enabled = ((bool)(resources.GetObject("panelTopRight.Enabled")));
			this.panelTopRight.Font = ((System.Drawing.Font)(resources.GetObject("panelTopRight.Font")));
			this.panelTopRight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelTopRight.ImeMode")));
			this.panelTopRight.Location = ((System.Drawing.Point)(resources.GetObject("panelTopRight.Location")));
			this.panelTopRight.Name = "panelTopRight";
			this.panelTopRight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelTopRight.RightToLeft")));
			this.panelTopRight.Size = ((System.Drawing.Size)(resources.GetObject("panelTopRight.Size")));
			this.panelTopRight.TabIndex = ((int)(resources.GetObject("panelTopRight.TabIndex")));
			this.panelTopRight.Text = resources.GetString("panelTopRight.Text");
			this.panelTopRight.Visible = ((bool)(resources.GetObject("panelTopRight.Visible")));
			// 
			// btnReverseEnumerations
			// 
			this.btnReverseEnumerations.AccessibleDescription = resources.GetString("btnReverseEnumerations.AccessibleDescription");
			this.btnReverseEnumerations.AccessibleName = resources.GetString("btnReverseEnumerations.AccessibleName");
			this.btnReverseEnumerations.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnReverseEnumerations.Anchor")));
			this.btnReverseEnumerations.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnReverseEnumerations.BackgroundImage")));
			this.btnReverseEnumerations.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnReverseEnumerations.Dock")));
			this.btnReverseEnumerations.Enabled = ((bool)(resources.GetObject("btnReverseEnumerations.Enabled")));
			this.btnReverseEnumerations.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnReverseEnumerations.FlatStyle")));
			this.btnReverseEnumerations.Font = ((System.Drawing.Font)(resources.GetObject("btnReverseEnumerations.Font")));
			this.btnReverseEnumerations.Image = ((System.Drawing.Image)(resources.GetObject("btnReverseEnumerations.Image")));
			this.btnReverseEnumerations.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnReverseEnumerations.ImageAlign")));
			this.btnReverseEnumerations.ImageIndex = ((int)(resources.GetObject("btnReverseEnumerations.ImageIndex")));
			this.btnReverseEnumerations.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnReverseEnumerations.ImeMode")));
			this.btnReverseEnumerations.Location = ((System.Drawing.Point)(resources.GetObject("btnReverseEnumerations.Location")));
			this.btnReverseEnumerations.Name = "btnReverseEnumerations";
			this.btnReverseEnumerations.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnReverseEnumerations.RightToLeft")));
			this.btnReverseEnumerations.Size = ((System.Drawing.Size)(resources.GetObject("btnReverseEnumerations.Size")));
			this.btnReverseEnumerations.TabIndex = ((int)(resources.GetObject("btnReverseEnumerations.TabIndex")));
			this.btnReverseEnumerations.Text = resources.GetString("btnReverseEnumerations.Text");
			this.btnReverseEnumerations.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnReverseEnumerations.TextAlign")));
			this.btnReverseEnumerations.Visible = ((bool)(resources.GetObject("btnReverseEnumerations.Visible")));
			this.btnReverseEnumerations.Click += new System.EventHandler(this.btnReverseEnumerations_Click);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.AccessibleDescription = resources.GetString("btnMoveDown.AccessibleDescription");
			this.btnMoveDown.AccessibleName = resources.GetString("btnMoveDown.AccessibleName");
			this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnMoveDown.Anchor")));
			this.btnMoveDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMoveDown.BackgroundImage")));
			this.btnMoveDown.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnMoveDown.Dock")));
			this.btnMoveDown.Enabled = ((bool)(resources.GetObject("btnMoveDown.Enabled")));
			this.btnMoveDown.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnMoveDown.FlatStyle")));
			this.btnMoveDown.Font = ((System.Drawing.Font)(resources.GetObject("btnMoveDown.Font")));
			this.btnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveDown.Image")));
			this.btnMoveDown.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveDown.ImageAlign")));
			this.btnMoveDown.ImageIndex = ((int)(resources.GetObject("btnMoveDown.ImageIndex")));
			this.btnMoveDown.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnMoveDown.ImeMode")));
			this.btnMoveDown.Location = ((System.Drawing.Point)(resources.GetObject("btnMoveDown.Location")));
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnMoveDown.RightToLeft")));
			this.btnMoveDown.Size = ((System.Drawing.Size)(resources.GetObject("btnMoveDown.Size")));
			this.btnMoveDown.TabIndex = ((int)(resources.GetObject("btnMoveDown.TabIndex")));
			this.btnMoveDown.Text = resources.GetString("btnMoveDown.Text");
			this.btnMoveDown.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveDown.TextAlign")));
			this.btnMoveDown.Visible = ((bool)(resources.GetObject("btnMoveDown.Visible")));
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click_1);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.AccessibleDescription = resources.GetString("btnMoveUp.AccessibleDescription");
			this.btnMoveUp.AccessibleName = resources.GetString("btnMoveUp.AccessibleName");
			this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnMoveUp.Anchor")));
			this.btnMoveUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMoveUp.BackgroundImage")));
			this.btnMoveUp.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnMoveUp.Dock")));
			this.btnMoveUp.Enabled = ((bool)(resources.GetObject("btnMoveUp.Enabled")));
			this.btnMoveUp.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnMoveUp.FlatStyle")));
			this.btnMoveUp.Font = ((System.Drawing.Font)(resources.GetObject("btnMoveUp.Font")));
			this.btnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveUp.Image")));
			this.btnMoveUp.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveUp.ImageAlign")));
			this.btnMoveUp.ImageIndex = ((int)(resources.GetObject("btnMoveUp.ImageIndex")));
			this.btnMoveUp.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnMoveUp.ImeMode")));
			this.btnMoveUp.Location = ((System.Drawing.Point)(resources.GetObject("btnMoveUp.Location")));
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnMoveUp.RightToLeft")));
			this.btnMoveUp.Size = ((System.Drawing.Size)(resources.GetObject("btnMoveUp.Size")));
			this.btnMoveUp.TabIndex = ((int)(resources.GetObject("btnMoveUp.TabIndex")));
			this.btnMoveUp.Text = resources.GetString("btnMoveUp.Text");
			this.btnMoveUp.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMoveUp.TextAlign")));
			this.btnMoveUp.Visible = ((bool)(resources.GetObject("btnMoveUp.Visible")));
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click_1);
			// 
			// panel2
			// 
			this.panel2.AccessibleDescription = resources.GetString("panel2.AccessibleDescription");
			this.panel2.AccessibleName = resources.GetString("panel2.AccessibleName");
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel2.Anchor")));
			this.panel2.AutoScroll = ((bool)(resources.GetObject("panel2.AutoScroll")));
			this.panel2.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel2.AutoScrollMargin")));
			this.panel2.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel2.AutoScrollMinSize")));
			this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
			this.panel2.Controls.Add(this.lblEnums);
			this.panel2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel2.Dock")));
			this.panel2.Enabled = ((bool)(resources.GetObject("panel2.Enabled")));
			this.panel2.Font = ((System.Drawing.Font)(resources.GetObject("panel2.Font")));
			this.panel2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel2.ImeMode")));
			this.panel2.Location = ((System.Drawing.Point)(resources.GetObject("panel2.Location")));
			this.panel2.Name = "panel2";
			this.panel2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel2.RightToLeft")));
			this.panel2.Size = ((System.Drawing.Size)(resources.GetObject("panel2.Size")));
			this.panel2.TabIndex = ((int)(resources.GetObject("panel2.TabIndex")));
			this.panel2.Text = resources.GetString("panel2.Text");
			this.panel2.Visible = ((bool)(resources.GetObject("panel2.Visible")));
			// 
			// lblEnums
			// 
			this.lblEnums.AccessibleDescription = resources.GetString("lblEnums.AccessibleDescription");
			this.lblEnums.AccessibleName = resources.GetString("lblEnums.AccessibleName");
			this.lblEnums.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblEnums.Anchor")));
			this.lblEnums.AutoSize = ((bool)(resources.GetObject("lblEnums.AutoSize")));
			this.lblEnums.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblEnums.Dock")));
			this.lblEnums.Enabled = ((bool)(resources.GetObject("lblEnums.Enabled")));
			this.lblEnums.Font = ((System.Drawing.Font)(resources.GetObject("lblEnums.Font")));
			this.lblEnums.Image = ((System.Drawing.Image)(resources.GetObject("lblEnums.Image")));
			this.lblEnums.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblEnums.ImageAlign")));
			this.lblEnums.ImageIndex = ((int)(resources.GetObject("lblEnums.ImageIndex")));
			this.lblEnums.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblEnums.ImeMode")));
			this.lblEnums.Location = ((System.Drawing.Point)(resources.GetObject("lblEnums.Location")));
			this.lblEnums.Name = "lblEnums";
			this.lblEnums.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblEnums.RightToLeft")));
			this.lblEnums.Size = ((System.Drawing.Size)(resources.GetObject("lblEnums.Size")));
			this.lblEnums.TabIndex = ((int)(resources.GetObject("lblEnums.TabIndex")));
			this.lblEnums.Text = resources.GetString("lblEnums.Text");
			this.lblEnums.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblEnums.TextAlign")));
			this.lblEnums.Visible = ((bool)(resources.GetObject("lblEnums.Visible")));
			// 
			// splitter1
			// 
			this.splitter1.AccessibleDescription = resources.GetString("splitter1.AccessibleDescription");
			this.splitter1.AccessibleName = resources.GetString("splitter1.AccessibleName");
			this.splitter1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitter1.Anchor")));
			this.splitter1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitter1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitter1.BackgroundImage")));
			this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.splitter1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitter1.Dock")));
			this.splitter1.Enabled = ((bool)(resources.GetObject("splitter1.Enabled")));
			this.splitter1.Font = ((System.Drawing.Font)(resources.GetObject("splitter1.Font")));
			this.splitter1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitter1.ImeMode")));
			this.splitter1.Location = ((System.Drawing.Point)(resources.GetObject("splitter1.Location")));
			this.splitter1.MinExtra = ((int)(resources.GetObject("splitter1.MinExtra")));
			this.splitter1.MinSize = ((int)(resources.GetObject("splitter1.MinSize")));
			this.splitter1.Name = "splitter1";
			this.splitter1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitter1.RightToLeft")));
			this.splitter1.Size = ((System.Drawing.Size)(resources.GetObject("splitter1.Size")));
			this.splitter1.TabIndex = ((int)(resources.GetObject("splitter1.TabIndex")));
			this.splitter1.TabStop = false;
			this.splitter1.Visible = ((bool)(resources.GetObject("splitter1.Visible")));
			// 
			// panelPreview
			// 
			this.panelPreview.AccessibleDescription = resources.GetString("panelPreview.AccessibleDescription");
			this.panelPreview.AccessibleName = resources.GetString("panelPreview.AccessibleName");
			this.panelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelPreview.Anchor")));
			this.panelPreview.AutoScroll = ((bool)(resources.GetObject("panelPreview.AutoScroll")));
			this.panelPreview.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelPreview.AutoScrollMargin")));
			this.panelPreview.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelPreview.AutoScrollMinSize")));
			this.panelPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelPreview.BackgroundImage")));
			this.panelPreview.Controls.Add(this.panelPreviewLabelContainer);
			this.panelPreview.Controls.Add(this.chkOrientation);
			this.panelPreview.Controls.Add(this.chkShowPreview);
			this.panelPreview.Controls.Add(this.label1);
			this.panelPreview.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelPreview.Dock")));
			this.panelPreview.Enabled = ((bool)(resources.GetObject("panelPreview.Enabled")));
			this.panelPreview.Font = ((System.Drawing.Font)(resources.GetObject("panelPreview.Font")));
			this.panelPreview.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelPreview.ImeMode")));
			this.panelPreview.Location = ((System.Drawing.Point)(resources.GetObject("panelPreview.Location")));
			this.panelPreview.Name = "panelPreview";
			this.panelPreview.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelPreview.RightToLeft")));
			this.panelPreview.Size = ((System.Drawing.Size)(resources.GetObject("panelPreview.Size")));
			this.panelPreview.TabIndex = ((int)(resources.GetObject("panelPreview.TabIndex")));
			this.panelPreview.Text = resources.GetString("panelPreview.Text");
			this.panelPreview.Visible = ((bool)(resources.GetObject("panelPreview.Visible")));
			// 
			// chkShowPreview
			// 
			this.chkShowPreview.AccessibleDescription = resources.GetString("chkShowPreview.AccessibleDescription");
			this.chkShowPreview.AccessibleName = resources.GetString("chkShowPreview.AccessibleName");
			this.chkShowPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkShowPreview.Anchor")));
			this.chkShowPreview.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkShowPreview.Appearance")));
			this.chkShowPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkShowPreview.BackgroundImage")));
			this.chkShowPreview.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkShowPreview.CheckAlign")));
			this.chkShowPreview.Checked = true;
			this.chkShowPreview.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowPreview.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkShowPreview.Dock")));
			this.chkShowPreview.Enabled = ((bool)(resources.GetObject("chkShowPreview.Enabled")));
			this.chkShowPreview.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkShowPreview.FlatStyle")));
			this.chkShowPreview.Font = ((System.Drawing.Font)(resources.GetObject("chkShowPreview.Font")));
			this.chkShowPreview.Image = ((System.Drawing.Image)(resources.GetObject("chkShowPreview.Image")));
			this.chkShowPreview.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkShowPreview.ImageAlign")));
			this.chkShowPreview.ImageIndex = ((int)(resources.GetObject("chkShowPreview.ImageIndex")));
			this.chkShowPreview.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkShowPreview.ImeMode")));
			this.chkShowPreview.Location = ((System.Drawing.Point)(resources.GetObject("chkShowPreview.Location")));
			this.chkShowPreview.Name = "chkShowPreview";
			this.chkShowPreview.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkShowPreview.RightToLeft")));
			this.chkShowPreview.Size = ((System.Drawing.Size)(resources.GetObject("chkShowPreview.Size")));
			this.chkShowPreview.TabIndex = ((int)(resources.GetObject("chkShowPreview.TabIndex")));
			this.chkShowPreview.Text = resources.GetString("chkShowPreview.Text");
			this.chkShowPreview.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkShowPreview.TextAlign")));
			this.chkShowPreview.Visible = ((bool)(resources.GetObject("chkShowPreview.Visible")));
			// 
			// panelPreviewLabelContainer
			// 
			this.panelPreviewLabelContainer.AccessibleDescription = resources.GetString("panelPreviewLabelContainer.AccessibleDescription");
			this.panelPreviewLabelContainer.AccessibleName = resources.GetString("panelPreviewLabelContainer.AccessibleName");
			this.panelPreviewLabelContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelPreviewLabelContainer.Anchor")));
			this.panelPreviewLabelContainer.AutoScroll = ((bool)(resources.GetObject("panelPreviewLabelContainer.AutoScroll")));
			this.panelPreviewLabelContainer.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelPreviewLabelContainer.AutoScrollMargin")));
			this.panelPreviewLabelContainer.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelPreviewLabelContainer.AutoScrollMinSize")));
			this.panelPreviewLabelContainer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelPreviewLabelContainer.BackgroundImage")));
			this.panelPreviewLabelContainer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelPreviewLabelContainer.Dock")));
			this.panelPreviewLabelContainer.Enabled = ((bool)(resources.GetObject("panelPreviewLabelContainer.Enabled")));
			this.panelPreviewLabelContainer.Font = ((System.Drawing.Font)(resources.GetObject("panelPreviewLabelContainer.Font")));
			this.panelPreviewLabelContainer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelPreviewLabelContainer.ImeMode")));
			this.panelPreviewLabelContainer.Location = ((System.Drawing.Point)(resources.GetObject("panelPreviewLabelContainer.Location")));
			this.panelPreviewLabelContainer.Name = "panelPreviewLabelContainer";
			this.panelPreviewLabelContainer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelPreviewLabelContainer.RightToLeft")));
			this.panelPreviewLabelContainer.Size = ((System.Drawing.Size)(resources.GetObject("panelPreviewLabelContainer.Size")));
			this.panelPreviewLabelContainer.TabIndex = ((int)(resources.GetObject("panelPreviewLabelContainer.TabIndex")));
			this.panelPreviewLabelContainer.Text = resources.GetString("panelPreviewLabelContainer.Text");
			this.panelPreviewLabelContainer.Visible = ((bool)(resources.GetObject("panelPreviewLabelContainer.Visible")));
			// 
			// chkOrientation
			// 
			this.chkOrientation.AccessibleDescription = resources.GetString("chkOrientation.AccessibleDescription");
			this.chkOrientation.AccessibleName = resources.GetString("chkOrientation.AccessibleName");
			this.chkOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkOrientation.Anchor")));
			this.chkOrientation.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkOrientation.Appearance")));
			this.chkOrientation.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkOrientation.BackgroundImage")));
			this.chkOrientation.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkOrientation.CheckAlign")));
			this.chkOrientation.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkOrientation.Dock")));
			this.chkOrientation.Enabled = ((bool)(resources.GetObject("chkOrientation.Enabled")));
			this.chkOrientation.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkOrientation.FlatStyle")));
			this.chkOrientation.Font = ((System.Drawing.Font)(resources.GetObject("chkOrientation.Font")));
			this.chkOrientation.Image = ((System.Drawing.Image)(resources.GetObject("chkOrientation.Image")));
			this.chkOrientation.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkOrientation.ImageAlign")));
			this.chkOrientation.ImageIndex = ((int)(resources.GetObject("chkOrientation.ImageIndex")));
			this.chkOrientation.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkOrientation.ImeMode")));
			this.chkOrientation.Location = ((System.Drawing.Point)(resources.GetObject("chkOrientation.Location")));
			this.chkOrientation.Name = "chkOrientation";
			this.chkOrientation.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkOrientation.RightToLeft")));
			this.chkOrientation.Size = ((System.Drawing.Size)(resources.GetObject("chkOrientation.Size")));
			this.chkOrientation.TabIndex = ((int)(resources.GetObject("chkOrientation.TabIndex")));
			this.chkOrientation.Text = resources.GetString("chkOrientation.Text");
			this.chkOrientation.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkOrientation.TextAlign")));
			this.chkOrientation.Visible = ((bool)(resources.GetObject("chkOrientation.Visible")));
			this.chkOrientation.CheckedChanged += new System.EventHandler(this.chkOrientation_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// frmLabelGroupEdit
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelPreview);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panelTop);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "frmLabelGroupEdit";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Controls.SetChildIndex(this.panelTop, 0);
			this.Controls.SetChildIndex(this.splitter1, 0);
			this.Controls.SetChildIndex(this.panelPreview, 0);
			this.Controls.SetChildIndex(this.panelBottom, 0);
			this.panelTop.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panelInsideTop.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.panelTopRight.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panelPreview.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Updates the preview labels for displaying factor information.
		/// </summary>
		private void UpdatePreview()
		{
            //TODO: Update this
            
            //if (this.chkShowPreview.Checked == false)
            //    return;

            //clsFactorTree tree	= new clsFactorTree();
            //tree = m_factorTree.Clone() as clsFactorTree;
            //clsTreeNode node	= BuildTree(ref tree);			

            //PNNLControls.clsLabelAttributes root = new clsLabelAttributes();
            //clsTreeNodeConverter converter = new clsTreeNodeConverter();
            /////converter.BuildLabels(node, ref root);
            //m_previewLabel.Root = root;

            //ArrayList leaves = m_previewLabel.GetLeaves();						
            //int leaveCount = leaves.Count;			
            //m_previewLabel.Init();
            //m_previewLabel.LoadLabels(0,leaveCount - 1);	
		}

		private void UpdateMainTable()
		{
			for(int i = 0; i < listEnumerations.Items.Count; i++)
			{
				listTopology.Items[i].SubItems[m_currColumn].Text = 
					listEnumerations.Items[i].Text;				
			}									
		}
		/// <summary>
		/// Moves the selected item up in the list.
		/// </summary>
		private void MoveSelectedUp()
		{
			if (listEnumerations.SelectedIndices == null || listEnumerations.SelectedIndices.Count == 0)
				return;
						
			int index = listEnumerations.SelectedIndices[0];					
			if (index > 0)
			{				
				ListViewItem o  = listEnumerations.SelectedItems[0];				
				m_currEnumRow = index - 1;
				listEnumerations.Items.RemoveAt(index);
				listEnumerations.Items.Insert(index - 1, o.Text);
				listEnumerations.Items[index - 1].Selected = true;								
				HighlightItem(index - 1, Color.Navy, Color.White);
				UpdateMainTable();
				
				UpdatePreview();		
			}
		}

		/// <summary>
		/// Moves the selected item down in the list.
		/// </summary>
		private void MoveSelectedDown()
		{			
			if (listEnumerations.SelectedIndices == null || listEnumerations.SelectedIndices.Count == 0)
				return;
						
			int index = listEnumerations.SelectedIndices[0];					
			if (index < listEnumerations.Items.Count-1)
			{				
				ListViewItem o  = listEnumerations.SelectedItems[0];
				m_currEnumRow = index + 1;				
				listEnumerations.Items.RemoveAt(index);
				listEnumerations.Items.Insert(index + 1, o.Text);
				listEnumerations.Items[index + 1].Selected = true;	
				HighlightItem(index + 1, Color.Navy, Color.White);
				listEnumerations.Refresh();			
				UpdateMainTable();		
				UpdatePreview();							
			}
		}

		private void listTopology_KeyUp(object sender, KeyEventArgs e)
		{
						
			switch(e.KeyCode)
			{
				case Keys.Up:
					MoveSelectedUp();			
					break;
				case Keys.Down:
					MoveSelectedDown();						 			
					break;
			}										
		}

		private void btnEditColor_Click(object sender, System.EventArgs e)
		{
			colorDialog.AllowFullOpen = true;
			colorDialog.FullOpen = true;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				if (listTopology.SelectedItems.Count > 0)
				{
					listTopology.SelectedItems[0].SubItems[1].BackColor = colorDialog.Color;
				}
			}
		}		
		
		/// <summary>
		/// Highlights a column with the indicated colors.
		/// </summary>
		/// <param name="column">Column to highlight.</param>
		/// <param name="background">Color of backgrond.</param>
		/// <param name="foreground">Color of text.</param>
		private void HighlightColumn(int column, Color background, Color foreground)
		{
			for(int i = 0; i < this.listTopology.Items.Count; i++)
			{
				ListViewItem item = listTopology.Items[i];
				item.UseItemStyleForSubItems = false;
				//if (item.SubItems[column].Text != string.Empty)
				{
					item.SubItems[column].BackColor = background;
					item.SubItems[column].ForeColor = foreground;
				}
			}
			listTopology.Refresh();
		}

		/// <summary>
		/// Highlights an item with the indicated colors.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="background"></param>
		/// <param name="foreground"></param>
		private void HighlightItem(int row, Color background, Color foreground)
		{
			ListViewItem item	= listEnumerations.Items[row];						
			item.BackColor		= background;
			item.ForeColor		= foreground;									
		}
		private void ShowEnumerations(int column)
		{
			if (m_currColumn != column)
			{
				HighlightColumn(m_currColumn, Color.White, Color.Black);
			}
			m_currColumn = column;
			HighlightColumn(m_currColumn, Color.Navy, Color.White);			
			listEnumerations.Items.Clear();			
			for(int i = 0; i < this.listTopology.Items.Count; i++)
			{
				if (listTopology.Items[i].SubItems.Count >= m_currColumn && listTopology.Items[i].SubItems[m_currColumn].Text != string.Empty)
					listEnumerations.Items.Add(listTopology.Items[i].SubItems[m_currColumn].Text);				
			}
		}

		private void listTopology_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			ShowEnumerations(e.Column);					
        }

		private void btnEdit_Click(object sender, System.EventArgs e)
		{

			if (this.listEnumerations.SelectedItems != null)
			{
				ColorDialog colorDialog = new ColorDialog();
				colorDialog.SolidColorOnly = true;
				colorDialog.ShowDialog();

				foreach(ListViewItem item in this.listEnumerations.SelectedItems)
				{
					item.BackColor = colorDialog.Color;
				}
			}
		}

		private void btnMoveUp_Click_1(object sender, System.EventArgs e)
		{
			MoveSelectedUp();		
		}

		private void btnMoveDown_Click_1(object sender, System.EventArgs e)
		{
			MoveSelectedDown();
		}

		private void btnMoveLeft_Click(object sender, System.EventArgs e)
		{
			/// Make sure its not the last column, cant move anymore right.
			if (m_currColumn <= 1)
				return;

			for(int row = 0; row < listTopology.Items.Count; row++)
			{			
				string tempText = listTopology.Items[row].SubItems[m_currColumn].Text;
				listTopology.Items[row].SubItems[m_currColumn].Text = listTopology.Items[row].SubItems[m_currColumn - 1].Text;
				listTopology.Items[row].SubItems[m_currColumn - 1].Text = tempText;				
			}

			string tempColumnName = listTopology.Columns[m_currColumn].Text;
			listTopology.Columns[m_currColumn].Text = listTopology.Columns[m_currColumn - 1].Text;
			listTopology.Columns[m_currColumn - 1].Text = tempColumnName;
			

			HighlightColumn(m_currColumn, Color.White, Color.Black);			
			m_currColumn--;
			HighlightColumn(m_currColumn, Color.Navy, Color.White);	
			UpdatePreview();					
		}

		private void btnMoveRight_Click(object sender, System.EventArgs e)
		{			
			/// Make sure its not the last column, cant move anymore right.
			if (m_currColumn >= listTopology.Columns.Count - 1)
				return;

			for(int row = 0; row < listTopology.Items.Count; row++)
			{			
				string tempText = listTopology.Items[row].SubItems[m_currColumn].Text;
				listTopology.Items[row].SubItems[m_currColumn].Text = listTopology.Items[row].SubItems[m_currColumn + 1].Text;
				listTopology.Items[row].SubItems[m_currColumn + 1].Text = tempText;				
			}

			string tempColumnName = listTopology.Columns[m_currColumn].Text;
			listTopology.Columns[m_currColumn].Text = listTopology.Columns[m_currColumn + 1].Text;
			listTopology.Columns[m_currColumn + 1].Text = tempColumnName;

			HighlightColumn(m_currColumn, Color.White, Color.Black);						
			m_currColumn++;			
			HighlightColumn(m_currColumn, Color.Navy, Color.White);			
			UpdatePreview();			
		}

		private void btnReverseEnumerations_Click(object sender, System.EventArgs e)
		{			
			int k = listEnumerations.Items.Count;			
			for(int i = 0; i < k/2; i++)
			{
				string tempText = listEnumerations.Items[i].Text;				
				listEnumerations.Items[i].Text = listEnumerations.Items[k - i - 1].Text; 
				listEnumerations.Items[k - i - 1].Text = tempText;
			}			
			UpdateMainTable();
			UpdatePreview();		
		}

		private void chkOrientation_CheckedChanged(object sender, System.EventArgs e)
		{
			m_previewLabel.Vertical = chkOrientation.Checked;
			UpdatePreview();
		}

		private void frmLabelGroupEdit_Load(object sender, EventArgs e)
		{
		
		}

		private void listEnumerations_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listEnumerations.SelectedIndices == null || listEnumerations.SelectedIndices.Count == 0)
				return;						
			int index = listEnumerations.SelectedIndices[0];					
			HighlightItem(m_currEnumRow, Color.White, Color.Black);						
			m_currEnumRow = index;
			HighlightItem(m_currEnumRow, Color.Navy, Color.White);			
		}
	}
}

