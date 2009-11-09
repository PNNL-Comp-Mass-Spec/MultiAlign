using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;


using PNNLProteomics.Data;
using PNNLProteomics.Data.Factors;

namespace MultiAlignWin.Forms.Factors
{
	/// <summary>
	/// Class that inherits a Form object for editing of a set of factors.
	/// </summary>
    public class frmFactorDefinition : Form
    {
        #region Members - Windows Forms
        IContainer components = null;
        private Label label3;
        private Label label2;
        private TextBox mtextBox_factors;
        private TextBox mtextBox_values;
        private Button mbutton_deleteNode;
        private Button mbutton_addFactor;
        private Button mbutton_addValue;
        private Button mbutton_ok;
        private Button mbutton_cancel;
        private TreeView mtreeView_factorList;
        private ExternalControls.NiceLine mline;
        private StatusStrip mstatusStrip;
        private ToolStripStatusLabel mlabel_status;
        #endregion

        /// <summary>
        /// Flag whether to ignore the lost focus event or not.
        /// </summary>
        private bool mbool_ignoreAddEscape = false;

        /// <summary>
        /// Reference to the list of factors and factor operations.
        /// </summary>
        private classFactorDefinition mobj_factors;
        private Label mlabel_hints;

        /// <summary>
        /// Factor Tree node sorter.
        /// </summary>
        clsFactorTreeNodeSorter mobj_nodeSorter;

        #region Constructors & Initialization
        /// <summary>
        /// Default constructor for a factor definition form.
        /// </summary>
        public frmFactorDefinition()
        {
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        /// Populates the interface with previously defined non-hierarchical factor definitions.  
        /// </summary>
        /// <param name="factors">Keys are strings and values should be stored in a strongly typed string List.</param>
        public frmFactorDefinition(classFactorDefinition factors)
        {
            InitializeComponent();
            Initialize();
            LoadFactors(factors);
        }

        /// <summary>
        /// Initializes class members that are similar to multiple constructors.
        /// </summary>
        private void Initialize()
        {
            mobj_nodeSorter                         = new clsFactorTreeNodeSorter();
            mtreeView_factorList.TreeViewNodeSorter = mobj_nodeSorter;
            mtreeView_factorList.HideSelection      = false;

            mtreeView_factorList.NodeMouseClick         += new TreeNodeMouseClickEventHandler(mtreeView_factorList_NodeMouseClick);
            mtreeView_factorList.NodeMouseDoubleClick   += new TreeNodeMouseClickEventHandler(mtreeView_factorList_NodeMouseDoubleClick);
            mtreeView_factorList.LostFocus              += new EventHandler(mtreeView_factorList_LostFocus);
            mtreeView_factorList.KeyUp                  += new KeyEventHandler(mtreeView_factorList_KeyUp);

            mobj_factors = new classFactorDefinition();
        }
        #endregion

        #region Windows Auto-Generated
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFactorDefinition));
            this.mbutton_addValue = new System.Windows.Forms.Button();
            this.mtextBox_values = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mbutton_deleteNode = new System.Windows.Forms.Button();
            this.mbutton_addFactor = new System.Windows.Forms.Button();
            this.mtextBox_factors = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.mtreeView_factorList = new System.Windows.Forms.TreeView();
            this.mline = new ExternalControls.NiceLine();
            this.mstatusStrip = new System.Windows.Forms.StatusStrip();
            this.mlabel_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.mlabel_hints = new System.Windows.Forms.Label();
            this.mstatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbutton_addValue
            // 
            this.mbutton_addValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_addValue.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_addValue.Enabled = false;
            this.mbutton_addValue.Image = ((System.Drawing.Image)(resources.GetObject("mbutton_addValue.Image")));
            this.mbutton_addValue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_addValue.Location = new System.Drawing.Point(301, 80);
            this.mbutton_addValue.Name = "mbutton_addValue";
            this.mbutton_addValue.Size = new System.Drawing.Size(77, 26);
            this.mbutton_addValue.TabIndex = 3;
            this.mbutton_addValue.Text = "Add";
            this.mbutton_addValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_addValue.UseVisualStyleBackColor = true;
            this.mbutton_addValue.Click += new System.EventHandler(this.mbutton_addValue_Click);
            // 
            // mtextBox_values
            // 
            this.mtextBox_values.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_values.Enabled = false;
            this.mtextBox_values.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtextBox_values.Location = new System.Drawing.Point(6, 84);
            this.mtextBox_values.Name = "mtextBox_values";
            this.mtextBox_values.Size = new System.Drawing.Size(289, 20);
            this.mtextBox_values.TabIndex = 2;
            this.mtextBox_values.Text = "Value1";
            this.mtextBox_values.KeyUp += new System.Windows.Forms.KeyEventHandler(this.valueText_KeyUp);
            this.mtextBox_values.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_KeyPress);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 24);
            this.label2.TabIndex = 7;
            this.label2.Text = "Values:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mbutton_deleteNode
            // 
            this.mbutton_deleteNode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_deleteNode.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_deleteNode.Enabled = false;
            this.mbutton_deleteNode.Image = ((System.Drawing.Image)(resources.GetObject("mbutton_deleteNode.Image")));
            this.mbutton_deleteNode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_deleteNode.Location = new System.Drawing.Point(301, 495);
            this.mbutton_deleteNode.Name = "mbutton_deleteNode";
            this.mbutton_deleteNode.Size = new System.Drawing.Size(77, 27);
            this.mbutton_deleteNode.TabIndex = 5;
            this.mbutton_deleteNode.Text = "Delete";
            this.mbutton_deleteNode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_deleteNode.UseVisualStyleBackColor = true;
            this.mbutton_deleteNode.Click += new System.EventHandler(this.mbutton_factorDelete_Click);
            // 
            // mbutton_addFactor
            // 
            this.mbutton_addFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_addFactor.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_addFactor.Image = ((System.Drawing.Image)(resources.GetObject("mbutton_addFactor.Image")));
            this.mbutton_addFactor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_addFactor.Location = new System.Drawing.Point(301, 30);
            this.mbutton_addFactor.Name = "mbutton_addFactor";
            this.mbutton_addFactor.Size = new System.Drawing.Size(77, 25);
            this.mbutton_addFactor.TabIndex = 1;
            this.mbutton_addFactor.Text = "Add";
            this.mbutton_addFactor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_addFactor.UseVisualStyleBackColor = true;
            this.mbutton_addFactor.Click += new System.EventHandler(this.mbutton_addFactor_Click);
            // 
            // mtextBox_factors
            // 
            this.mtextBox_factors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_factors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtextBox_factors.Location = new System.Drawing.Point(6, 33);
            this.mtextBox_factors.Name = "mtextBox_factors";
            this.mtextBox_factors.Size = new System.Drawing.Size(289, 20);
            this.mtextBox_factors.TabIndex = 0;
            this.mtextBox_factors.Text = "Factor1";
            this.mtextBox_factors.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mtextBox_factors_KeyUp);
            this.mtextBox_factors.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_KeyPress);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 24);
            this.label3.TabIndex = 12;
            this.label3.Text = "Factors:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Enabled = false;
            this.mbutton_ok.Location = new System.Drawing.Point(148, 549);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(111, 28);
            this.mbutton_ok.TabIndex = 6;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            // 
            // mbutton_cancel
            // 
            this.mbutton_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_cancel.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_cancel.Location = new System.Drawing.Point(265, 549);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(111, 28);
            this.mbutton_cancel.TabIndex = 7;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = true;
            // 
            // mtreeView_factorList
            // 
            this.mtreeView_factorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtreeView_factorList.Location = new System.Drawing.Point(6, 112);
            this.mtreeView_factorList.Name = "mtreeView_factorList";
            this.mtreeView_factorList.Size = new System.Drawing.Size(370, 371);
            this.mtreeView_factorList.TabIndex = 4;
            this.mtreeView_factorList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mtreeView_factorList_KeyPress);
            // 
            // mline
            // 
            this.mline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mline.Location = new System.Drawing.Point(6, 530);
            this.mline.Name = "mline";
            this.mline.Size = new System.Drawing.Size(370, 15);
            this.mline.TabIndex = 16;
            // 
            // mstatusStrip
            // 
            this.mstatusStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mstatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mlabel_status});
            this.mstatusStrip.Location = new System.Drawing.Point(0, 580);
            this.mstatusStrip.Name = "mstatusStrip";
            this.mstatusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mstatusStrip.Size = new System.Drawing.Size(382, 22);
            this.mstatusStrip.TabIndex = 17;
            // 
            // mlabel_status
            // 
            this.mlabel_status.Name = "mlabel_status";
            this.mlabel_status.Size = new System.Drawing.Size(126, 17);
            this.mlabel_status.Text = "Enter factor information.";
            // 
            // mlabel_hints
            // 
            this.mlabel_hints.AutoSize = true;
            this.mlabel_hints.Location = new System.Drawing.Point(3, 486);
            this.mlabel_hints.Name = "mlabel_hints";
            this.mlabel_hints.Size = new System.Drawing.Size(108, 26);
            this.mlabel_hints.TabIndex = 18;
            this.mlabel_hints.Text = "Ctrl+f to add a factor\r\nCtrl+v, to add a value";
            // 
            // frmFactorDefinition
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(382, 602);
            this.Controls.Add(this.mlabel_hints);
            this.Controls.Add(this.mstatusStrip);
            this.Controls.Add(this.mtreeView_factorList);
            this.Controls.Add(this.mbutton_addFactor);
            this.Controls.Add(this.mline);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.mtextBox_values);
            this.Controls.Add(this.mbutton_addValue);
            this.Controls.Add(this.mtextBox_factors);
            this.Controls.Add(this.mbutton_deleteNode);
            this.MinimumSize = new System.Drawing.Size(390, 636);
            this.Name = "frmFactorDefinition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Factor Definition";
            this.mstatusStrip.ResumeLayout(false);
            this.mstatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        #endregion

        #region Utility
        /// <summary>
        /// Loads the factors provided by the argument into the tree.
        /// </summary>
        /// <param name="factors">Factors to display.</param>
        private void LoadFactors(classFactorDefinition factors)
        {            
            /// 
            /// If the factor values are null,
            /// then we must make sure we have a reference to a
            /// factor definition class.
            /// 
            mobj_factors = factors;
            if (factors == null)
            {
                mobj_factors = new classFactorDefinition();
                SetOkButton(mobj_factors.FullyDefined);
                return;
            }

            /// 
            /// Update the tree with the nodes for each factor
            /// 
            mtreeView_factorList.BeginUpdate();
            mtreeView_factorList.Nodes.Clear();
            foreach (string factor in factors.Factors.Keys)
            {
                TreeNode parentNode = new TreeNode();
                parentNode.Name     = factor;
                parentNode.Text     = factor;
                List<string> values = factors.Factors[factor];

                /// 
                /// Make sure that we dont have a null list for this factor
                /// 
                if (values != null)
                {
                    foreach (string value in values)
                    {
                        TreeNode node = new TreeNode();
                        node.Text = value;
                        node.Name = value;
                        parentNode.Nodes.Add(node);
                    }
                }
                mtreeView_factorList.Nodes.Add(parentNode);
            }

            /// 
            /// Sort and expand the tree.
            /// 
            mtreeView_factorList.Sort();
            mtreeView_factorList.ExpandAll();
            if (mtreeView_factorList.Nodes.Count > 0)
                mtreeView_factorList.SelectedNode = mtreeView_factorList.Nodes[0];
            mtreeView_factorList.EndUpdate();

            /// 
            /// Enable or disable the ok button based on if we have 
            /// defined factors
            /// 
            if (mobj_factors.Factors.Keys.Count < 1)
                SetFactorValues(false);
            SetOkButton(mobj_factors.FullyDefined);
        }

        /// <summary>
        /// Gets or sets the list of factors as a HashTable of string keys, and a List of strings for factor values at each hash index.
        /// Each list of factor values is sorted alphabetically in ascending order.
        /// </summary>
        public classFactorDefinition Factors
        {
            get
            {
                return mobj_factors;
            }
            set
            {                
                LoadFactors(value);
            }
        }
        /// <summary>
        /// Sets whether
        /// </summary>
        /// <param name="value"></param>
        private void SetFactorValues(bool value)
        {
            mtextBox_values.Enabled          = value;
            mbutton_addValue.Enabled         = value;
        }
        /// <summary>
        /// Sets the delete button state for these multiple buttons that share the same functionality.
        /// </summary>
        /// <param name="value"></param>
        private void SetDeleteButton(bool value)
        {
            mbutton_deleteNode.Enabled = value;
        }
        /// <summary>
        /// Sets the ok button to the value specified.
        /// </summary>
        /// <param name="value">Value to set the ok button's enabled property to.</param>
        private void SetOkButton(bool value)
        {
            mbutton_ok.Enabled = value;
        }
        #endregion

        #region Addition and Deletion Of Factors and Values
        /// <summary>
        /// Adds the factor string to the list of factors in the factor definition class
        /// and in the tree view.
        /// </summary>
        /// <param name="factorName">Name of the factor to add.</param>
        private bool AddFactor(string factorName, TreeNode node)
        {
            bool result = false;
            /// 
            /// Add the factor
            /// 
            factorName = factorName.Replace(" ", "");

            int added = mobj_factors.AddFactor(factorName);
            
            /// Now handle the user-interface issues 
            if (added == classFactorDefinition.CONST_FACTOR_EXISTS)
            {
                mlabel_status.Text = "The factor already exists.";                
            }
            else if (added == classFactorDefinition.CONST_FACTOR_ADDED)
            {
                /// 
                /// Add the node to the list of nodes.
                /// 
                if (node == null)
                {
                    node = new TreeNode();
                    node.Text = factorName;
                    node.Name = factorName;
                    mtreeView_factorList.Nodes.Add(node);
                }

                /// 
                /// Select the node for the user
                /// 
                mtreeView_factorList.Sort();
                mtreeView_factorList.SelectedNode = node;

                mbutton_ok.Enabled = false;
                mlabel_status.Text = string.Format("Factor \"{0}\" was added.",
                                                        mtextBox_factors.Text);

                /// 
                /// Enable the text box for value editing and removal.
                /// 
                SetFactorValues(true);
                SetDeleteButton(true);
                result = true;
            }
            SetOkButton(mobj_factors.FullyDefined);
            return result;
        }
        /// <summary>
        /// Adds the factor value to the user interface and the factor definition class.
        /// </summary>
        /// <param name="factorName">Name of the factor to add the value to.</param>
        /// <param name="factorValue">Name of the factor value to add.</param>
        private bool AddFactorValue(string factorName, string factorValue, TreeNode node)
        {
            bool result = false;

            int added = mobj_factors.AddFactorValue(factorName, factorValue);
            if (added == classFactorDefinition.CONST_FACTOR_VALUE_EXISTS)
            {
                mlabel_status.Text = "The value already exists.";                
            }
            else if (added == classFactorDefinition.CONST_FACTOR_VALUE_ADDED)
            {
                /// 
                /// See if we can enable the factor button.
                /// 
                bool defined = mobj_factors.FullyDefined;
                mbutton_ok.Enabled = defined;

                /// 
                /// Add the factor value to the user interface
                /// 
                TreeNode parent = mtreeView_factorList.Nodes[factorName];
                if (node == null)
                {
                    node = new TreeNode();
                    node.Name = factorValue;
                    node.Text = factorValue;
                    
                    parent.Nodes.Add(node);
                    parent.ExpandAll();
                }
                
                mtreeView_factorList.Sort();
                mtreeView_factorList.SelectedNode = parent;
                
                /// 
                /// Indicate the factor value was added.
                /// 
                mlabel_status.Text = string.Format("The value \"{0}\" was added.", factorValue);
                result = true;
            }

            SetOkButton(mobj_factors.FullyDefined);
            return result;
        }
        /// <summary>
        /// Deletes the factor value given the specified factor treenode and value node.
        /// </summary>
        /// <param name="factor">Factor node.</param>
        private void DeleteFactor(TreeNode factor)
        {
            bool deleted = mobj_factors.DeleteFactor(factor.Name);
            if (deleted == true)
            {
                mtreeView_factorList.Nodes.Remove(factor);
            }
            if (mobj_factors.Factors.Keys.Count < 1)
                SetFactorValues(false);
            SetOkButton(mobj_factors.FullyDefined);
        }
        /// <summary>
        /// Deletes the factor value given the specified factor treenode and value node.
        /// </summary>
        /// <param name="factor">Factor node.</param>
        /// <param name="value">Factor value node.</param>
        private void DeleteFactorValue(TreeNode factor, TreeNode value)
        {
            bool deleted = mobj_factors.DeleteFactorValue(factor.Name, value.Name);
            if (deleted == true)
            {
                factor.Nodes.Remove(value);
            }
            if (mobj_factors.Factors.Keys.Count < 1)
                SetFactorValues(false);
            SetOkButton(mobj_factors.FullyDefined);
        }
        #endregion

        #region Tree Node Clicks
        /// <summary>
        /// Sets the selected node to the topmost node and enables the delete button state if a 
        /// node is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtreeView_factorList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            /// 
            /// Make sure we have:
            ///     1.  A node
            ///     2.  A right click - to only allow parent selections            
            /// 
            if (e.Node != null)
            {
                SetDeleteButton(true);
            }
            else
            {
                mtreeView_factorList.SelectedNode = null;
                SetDeleteButton(false);
            }
        }
        /// <summary>
        /// Displays the edit box for that node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtreeView_factorList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                e.Node.Expand();

                if (e.Node.Parent != null)
                {
                    ShowFactorValueEditTextBox(e.Node);
                }
                else
                {

                    ShowFactorEditTextBox(e.Node);
                }
            }            
        }
        #endregion

        #region Tree KeysPress and Focus Handlers
        /// <summary>
        /// Disables the delete node button if no node is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtreeView_factorList_KeyUp(object sender, KeyEventArgs e)
        {
            TreeNode node = mtreeView_factorList.SelectedNode;
            if (node != null)
            {
                if (e.KeyData == Keys.Enter)
                {                
                    /// 
                    /// See if it's a parent(null)
                    /// 
                    if (node.Parent == null)
                    {
                        ShowFactorEditTextBox(node);
                    }
                    else
                    {
                        ShowFactorValueEditTextBox(node);
                    }
                }
                else if (e.KeyData == Keys.Delete)
                {
                    /// 
                    /// See if it's a parent(null)
                    /// 
                    if (node.Parent == null)
                    {
                        /// 
                        /// Delete the factor
                        /// 
                        DeleteFactor(node);
                    }
                    else
                    {
                        /// 
                        /// Delete the value.
                        /// 
                        DeleteFactorValue(node.Parent, node);
                    }
                }
                /// 
                /// Create a new value
                /// 
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
                {
                    TreeNode newNode = new TreeNode();                    
                    /// 
                    /// Find if we add to the parent node of the selected node,
                    /// or add to the selected node.
                    /// 
                    if (node.Parent == null)
                    {
                        node.Nodes.Add(newNode);                        
                    }
                    else
                    {
                        node.Parent.Nodes.Add(newNode);                        
                    }
                    newNode.Text = string.Format("Value{0}",
                                            newNode.Parent.Nodes.Count);
                    mtreeView_factorList.Sort();
                    mtreeView_factorList.SelectedNode = newNode;                    
                    ShowFactorValueAddTextBox(newNode);
                }
            }            
            else
            {
                SetDeleteButton(false);             
            }
            
            /// 
            /// Create a new factor, we do not have to have any 
            /// nodes selected
            /// 
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
            {
                TreeNode newNode    = new TreeNode();
                newNode.Text        = "NewFactor";
                mtreeView_factorList.Nodes.Add(newNode);
                mtreeView_factorList.Sort();
                mtreeView_factorList.SelectedNode = newNode;
                ShowFactorAddTextBox(newNode);
            }
            e.SuppressKeyPress  = true;
            e.Handled           = true;
        }
        /// <summary>
        /// Disables the delete node button if the control loses focus and no node is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtreeView_factorList_LostFocus(object sender, EventArgs e)
        {
            if (mtreeView_factorList.SelectedNode == null)
            {
                SetDeleteButton(false);
            }
        }
        #endregion

        #region Factor Textbox and Button Event Handlers
        /// <summary>
        /// Adds a new factor definition if the user hits enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtextBox_factors_KeyUp(object sender, KeyEventArgs e)
        {
            mlabel_status.Text = "";
            if (e.KeyData == Keys.Enter)
            {
                AddFactor(mtextBox_factors.Text, null);                
            }
        }
        /// <summary>
        /// Adds a new factor to the tree view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_addFactor_Click(object sender, EventArgs e)
        {
            mlabel_status.Text = "";
            string data = mtextBox_factors.Text.Replace(" ", "");
            if (data != "")
            {
                AddFactor(data, null);
            }
        }
        #endregion

        #region Value Textbox and Button Event Handlers
        /// <summary>
        /// Given the text box, figures out if the text can be added to the 
        /// factor value lists.  It verifies that the node is not a parent
        /// and gets the name of the values factor key name.
        /// </summary>
        /// <param name="text"></param>
        private void AddFactorValue(TextBox text)
        {
            mlabel_status.Text = "";
            string data = text.Text.Replace(" ", "");
            if (data != "")
            {
                /// 
                /// Make sure a node is selected for the values.
                /// 
                if (mtreeView_factorList.SelectedNode != null)
                {
                    TreeNode node = mtreeView_factorList.SelectedNode;
                    if (node.Parent == null)
                    {
                        AddFactorValue(node.Text, data, null);
                    }
                    else
                    {
                        AddFactorValue(node.Parent.Text, data, null);
                    }
                }
                else
                {
                    mlabel_status.Text = "Please select a factor first.";
                }
            }
        }
        /// <summary>
        /// Addes a factor value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_addValue_Click(object sender, EventArgs e)
        {
            AddFactorValue(mtextBox_values);
        }
        /// <summary>
        /// Adds a new factor value if the enter button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valueText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                TextBox text = sender as TextBox;
                AddFactorValue(text);
            }
        }
        
        #endregion

        #region Delete Button Handlers
        /// <summary>
        /// Deletes a factor from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_factorDelete_Click(object sender, EventArgs e)
        {
            TreeNode node = mtreeView_factorList.SelectedNode;
            if (node != null)
            {
                /// 
                /// See if the node is a parent or a child.
                ///     Parent = Factor (Parent == null)
                ///     Node   = Value  (Parent != null)
                /// 
                if (node.Parent == null)
                {
                    DeleteFactor(node);
                }
                else
                {
                    DeleteFactorValue(node.Parent, node);
                }
            }
        }
        #endregion

        #region Dynamic Text Box Generation
        /// <summary>
        /// Creates and displays an edit factor textox..
        /// </summary>
        private void ShowFactorEditTextBox(TreeNode node)
        {            
            if (node != null)
            {
                /// 
                /// Create the textbox.  Make sure the width and height cover the text behind.
                /// 
                TextBox textBox = new TextBox();
                textBox.Name = node.Text;
                textBox.Text = node.Text;
                textBox.Bounds = node.Bounds;
                textBox.MinimumSize = new Size(node.Bounds.Width, node.Bounds.Height);

                /// 
                /// Create a textbox with the right size for the next text.
                /// 
                Graphics g = Graphics.FromHwnd(textBox.Handle);
                SizeF sizeOfOneText = g.MeasureString("a", textBox.Font);
                int pixelWidthPerCharacter = Convert.ToInt32(sizeOfOneText.Width);

                /// 
                /// Setup Width, and event handlers
                /// 
                textBox.Width       += 3 * pixelWidthPerCharacter;
                textBox.LostFocus   += new EventHandler(DynamicFactorTextBox_LostFocus);
                textBox.KeyUp       += new KeyEventHandler(DynamicFactorTextBox_KeyUp);

                /// 
                /// Suppress the sound when the user hits enter
                /// 
                textBox.KeyPress += new KeyPressEventHandler(textbox_KeyPress);

                /// 
                /// Add it to the treeview control list so it will be visible and accesible later.
                /// 
                mtreeView_factorList.Controls.Add(textBox);
                SizeTextBox(textBox);
                textBox.Focus();
            }
        }
        /// <summary>
        /// Creates and displays an add factor textox.
        /// </summary>
        private void ShowFactorAddTextBox(TreeNode node)
        {
            if (node != null)
            {
                /// 
                /// Create the textbox.  Make sure the width and height cover the text behind.
                /// 
                TextBox textBox = new TextBox();
                textBox.Name = node.Text;
                textBox.Text = node.Text;
                textBox.Bounds = node.Bounds;
                textBox.MinimumSize = new Size(node.Bounds.Width, node.Bounds.Height);

                /// 
                /// Create a textbox with the right size for the next text.
                /// 
                Graphics g = Graphics.FromHwnd(textBox.Handle);
                SizeF sizeOfOneText = g.MeasureString("a", textBox.Font);
                int pixelWidthPerCharacter = Convert.ToInt32(sizeOfOneText.Width);

                /// 
                /// Setup Width, and event handlers
                /// 
                textBox.Width       += 3 * pixelWidthPerCharacter;
                textBox.LostFocus   += 
                                    new EventHandler(DynamicFactorAddTextBox_LostFocus);
                textBox.KeyUp       +=
                                    new KeyEventHandler(DynamicFactorAddTextBox_KeyUp);

                /// 
                /// Suppress the sound when the user hits enter
                /// 
                textBox.KeyPress += new KeyPressEventHandler(textbox_KeyPress);

                /// 
                /// Add it to the treeview control list so it will be visible and accesible later.
                /// 
                mtreeView_factorList.Controls.Add(textBox);
                SizeTextBox(textBox);
                textBox.Focus();
            }
        }
        /// <summary>
        /// Creates and displays a textbox over a possible new factor value entry.
        /// </summary>
        /// <param name="node">Value node to display over.</param>
        private void ShowFactorValueEditTextBox(TreeNode node)
        {
            if (node != null)
            {
                TextBox newValueTextBox     = new TextBox();
                newValueTextBox.Name        = node.Text;
                newValueTextBox.Text        = node.Text;
                newValueTextBox.Bounds      = node.Bounds;
                newValueTextBox.MinimumSize = new Size(node.Bounds.Width,
                                                        node.Bounds.Height);

                /// 
                /// Increase the size of the textbox by 5 to provide text entry slack.
                /// 
                Graphics g                   = Graphics.FromHwnd(newValueTextBox.Handle);
                SizeF sizeOfOneText          = g.MeasureString("a", newValueTextBox.Font);
                int pixelWidthPerCharacter   = Convert.ToInt32(sizeOfOneText.Width);
                newValueTextBox.Width       += 3 * pixelWidthPerCharacter;

                /// 
                /// To handle when to hide/dispose of the text box.
                /// 
                newValueTextBox.LostFocus +=
                            new EventHandler(DynamicFactorValueTextBox_LostFocus);
                /// 
                /// To grow the text box or to accept its input.
                /// 
                newValueTextBox.KeyUp += 
                            new KeyEventHandler(DynamicFactorValueTextBox_KeyUp);
                /// 
                /// Suppress the sound when the user hits enter
                /// 
                newValueTextBox.KeyPress += 
                            new KeyPressEventHandler(textbox_KeyPress);

                mtreeView_factorList.Controls.Add(newValueTextBox);
                SizeTextBox(newValueTextBox);
                newValueTextBox.Focus();
            }
        }
        /// <summary>
        /// Creates and displays a textbox over a possible new factor value entry.
        /// </summary>
        /// <param name="node">Value node to display over.</param>
        private void ShowFactorValueAddTextBox(TreeNode node)
        {
            if (node != null)
            {
                TextBox newValueTextBox     = new TextBox();
                newValueTextBox.Name        = node.Text;
                newValueTextBox.Text        = node.Text;
                newValueTextBox.Bounds      = node.Bounds;
                newValueTextBox.MinimumSize = new Size(node.Bounds.Width, node.Bounds.Height);

                /// 
                /// Increase the size of the textbox by 5 to provide text entry slack.
                /// 
                Graphics g                  = Graphics.FromHwnd(newValueTextBox.Handle);
                SizeF sizeOfOneText         = g.MeasureString("a", newValueTextBox.Font);
                int pixelWidthPerCharacter  = Convert.ToInt32(sizeOfOneText.Width);
                newValueTextBox.Width       += 3 * pixelWidthPerCharacter;

                /// 
                /// To handle when to hide/dispose of the text box.
                /// 
                newValueTextBox.LostFocus   += 
                                    new EventHandler(DynamicFactorValueAddTextBox_LostFocus);
                /// 
                /// To grow the text box or to accept its input.
                /// 
                newValueTextBox.KeyUp       += 
                                    new KeyEventHandler(DynamicFactorValueAddTextBox_KeyUp);
                /// 
                /// Suppress the sound when the user hits enter
                /// 
                newValueTextBox.KeyPress    += 
                                    new KeyPressEventHandler(textbox_KeyPress);
                mtreeView_factorList.Controls.Add(newValueTextBox);
                
                SizeTextBox(newValueTextBox);
                newValueTextBox.Focus();
            }
        }


        #endregion

        #region Dynamic Text Box Resizing and Renaming of data
        /// <summary>
        /// Sizes the textbox based off it's internal text.
        /// </summary>
        /// <param name="box"></param>
        private void SizeTextBox(TextBox text)
        {
            /// 
            /// Determine the width of the textbox.
            /// 
            int width           = text.Width;
            Graphics g          = Graphics.FromHwnd(text.Handle);
            SizeF sizeOfAllText = g.MeasureString(text.Text, text.Font);
            SizeF sizeOfOneText = g.MeasureString("a", text.Font);
            int pixelWidthPerCharacter = Convert.ToInt32(sizeOfOneText.Width);

            /// 
            /// Now see if we need to shrink or grow the textbox
            /// 
            if (sizeOfAllText.Width + 5 * pixelWidthPerCharacter > width)
            {
                text.Width += 2 * pixelWidthPerCharacter;
            }
            else if (sizeOfAllText.Width < width - 7 * pixelWidthPerCharacter)
            {
                text.Width -= pixelWidthPerCharacter;
            }
            /// 
            /// Now size it based on what we found, but make sure there is a min and max value
            /// 
            text.Width = Math.Min(Math.Max(5*pixelWidthPerCharacter, text.Width), mtreeView_factorList.Width);
        }
        /// <summary>
        /// Updates the selected node if it's not null with the given TextBox's text data.
        /// </summary>
        /// <param name="text"></param>
        private bool RenameFactorInput(TreeNode node, string text)
        {
            bool result = false;
            int renamed = mobj_factors.RenameFactor(node.Text, text);

            if (renamed == classFactorDefinition.CONST_FACTOR_RENAMED)
            {                
                node.Text = text;
                node.Name = text;
                result = true;
            }
            else if (renamed == classFactorDefinition.CONST_FACTOR_NEW_EXISTS)
            {
                mlabel_status.Text = "The factor name already exists.";
            }
            else if (renamed == classFactorDefinition.CONST_FACTOR_DOES_NOT_EXIST)
            {
                mlabel_status.Text = "The old factor name does not exist.";
            }
            return result;
        }
        /// <summary>
        /// Handles the input from the factor value textbox.  Removes user-interface nodes if 
        /// update operation fails.
        /// </summary>
        /// <param name="factorName">Name of the factor</param>
        /// <param name="factorValue"></param>
        /// <returns></returns>
        private bool RenameFactorValueInput(TreeNode parentNode, TreeNode node, string factorValue)
        {
            bool result = false;
            int renamed = mobj_factors.RenameFactorValue(parentNode.Text, node.Text, factorValue);

            if (renamed == classFactorDefinition.CONST_FACTOR_VALUE_RENAMED)
            {
                node.Text = factorValue;
                node.Name = factorValue;
                result = true;
            }
            else if (renamed == classFactorDefinition.CONST_FACTOR_VALUE_NEW_EXISTS)
            {
                mlabel_status.Text = "The value already exists.";
            }
            else if (renamed == classFactorDefinition.CONST_FACTOR_VALUE_DOES_NOT_EXIST)
            {
                mlabel_status.Text = "The old value does not exist.";
            }
            return result;
        }
        #endregion

        #region Dynamic Factor KeyPress and Focus Handlers
        /// <summary>
        /// Handles key events for the factor dynamic text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            mlabel_status.Text = "";
            TextBox text = sender as TextBox;

            /// 
            /// Makes sure that the textbox is legit.
            /// 
            if (text != null)
            {   
                if (e.KeyData == Keys.Enter)
                {
                    text.Text = text.Text.Replace(" ", "");
                    /// 
                    /// Accept its input based on whether we were editing a factor or factor value.
                    /// 
                    RenameFactorInput(mtreeView_factorList.SelectedNode, text.Text);                    
                    mbool_ignoreAddEscape = true;

                    /// 
                    /// Dispose of the textbox
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                    mbool_ignoreAddEscape = true;
                }
                else if (e.KeyData == Keys.Escape)
                {
                    /// 
                    /// Then dispose of the text box
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                    mbool_ignoreAddEscape = true;
                }
                else
                {
                    /// 
                    /// New or removed text, resize for clean user input.
                    /// 
                    SizeTextBox(text);
                }
            }
        }
        /// <summary>
        /// Cleans up the dynamically created text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorTextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text != null)
            {
                if (mbool_ignoreAddEscape == false)
                {
                    /// 
                    /// Accept factor update if possible, 
                    ///  
                    RenameFactorInput(mtreeView_factorList.SelectedNode, text.Text);

                    /// Dipose of the input textbox.
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }                
                mbool_ignoreAddEscape = false;                
            }
        }
        /// <summary>
        /// Handles key events for the factor dynamic text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorAddTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            mlabel_status.Text = "";
            TextBox text = sender as TextBox;

            /// 
            /// Makes sure that the textbox is legit.
            /// 
            if (text != null)
            {
                TreeNode node = mtreeView_factorList.SelectedNode;
            
                if (e.KeyData == Keys.Enter)
                {
                    text.Text = text.Text.Replace(" ", "");
                    node.Text = text.Text;
                    node.Name = text.Text;

                    /// 
                    /// Accept its input based on whether we were editing a factor or factor value.
                    /// 
                    bool added = AddFactor(text.Text, node);
                    if (added == false)                    
                        mtreeView_factorList.Nodes.Remove(node);
                    mbool_ignoreAddEscape = true;

                    /// 
                    /// Dispose of the textbox
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                else if (e.KeyData == Keys.Escape)
                {
                    mbool_ignoreAddEscape = true;
                    /// 
                    /// Then dispose of the text box
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                else
                {
                    /// 
                    /// New or removed text, resize for clean user input.
                    /// 
                    SizeTextBox(text);
                }
            }
        }
        /// <summary>
        /// Cleans up the dynamically created text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorAddTextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text != null)
            {
                text.Text = text.Text.Replace(" ", "");
                if (mbool_ignoreAddEscape == false)
                {
                    TreeNode node = mtreeView_factorList.SelectedNode;                    
                    node.Text = text.Text;
                    node.Name = text.Text;

                    /// 
                    /// Accept factor update if possible, 
                    ///  
                    bool added = AddFactor(text.Text, node);
                    if (added == false)
                    {
                        mtreeView_factorList.Nodes.Remove(node);
                    }

                    /// Dipose of the input textbox.
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                
                mbool_ignoreAddEscape = false;                
            }
        }
        #endregion

        #region Dynamic Value KeyPress and Focus Handlers
        /// <summary>
        /// Handles key events for the factor value dynamic textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorValueTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            mlabel_status.Text = "";
            TextBox text = sender as TextBox;
            if (text != null)
            {
                TreeNode node       = mtreeView_factorList.SelectedNode;
                TreeNode parentNode = node.Parent;

                /// 
                /// IF: 
                ///     Enter  - accept the input
                ///     Escape - abandon input.
                ///     Other  - grow the textbox.
                /// 
                if (e.KeyData == Keys.Enter)
                {
                    /// 
                    /// Rename if possible
                    /// 
                    RenameFactorValueInput(parentNode, node, text.Text);
                    mbool_ignoreAddEscape = true;

                    /// 
                    /// Dipose of the textbox
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                else if (e.KeyData == Keys.Escape)
                {
                    mbool_ignoreAddEscape = true;
                    /// 
                    /// Dispose of the textbox.
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                else
                {
                    /// 
                    /// New or removed text, resize for clean user input.
                    /// 
                    SizeTextBox(text);
                }
            }
        }
        /// <summary>
        /// Handles accepting the input for a value textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorValueTextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text != null)
            {
                if (mbool_ignoreAddEscape == false)
                {
                    /// 
                    /// Accept the input if possible
                    /// 
                    TreeNode node       = mtreeView_factorList.SelectedNode;
                    TreeNode parentNode = node.Parent;
                    
                    RenameFactorValueInput(parentNode, node, text.Text);

                    /// 
                    /// Dipose of the text box
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                mbool_ignoreAddEscape = false;
            }
        }
        /// <summary>
        /// Handles key events for the factor value dynamic textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorValueAddTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            mlabel_status.Text = "";
            TextBox text = sender as TextBox;
            if (text != null)
            {
                TreeNode node = mtreeView_factorList.SelectedNode;
                TreeNode parentNode = node.Parent;

                if (e.KeyData == Keys.Enter)
                {
                    text.Text = text.Text.Replace(" ", "");
                    /// 
                    /// Accept the input if possible.
                    /// 
                    node.Name   = text.Text;
                    node.Text   = text.Text;
                    bool added  = AddFactorValue(parentNode.Text, text.Text, node);
                    if (added == false)
                    {
                        parentNode.Nodes.Remove(node);
                    }
                    mbool_ignoreAddEscape = true;

                    /// 
                    /// Dipose of the textbox
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                else if (e.KeyData == Keys.Escape)
                {

                    mbool_ignoreAddEscape = true;
                    /// 
                    /// Dispose of the textbox.
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                else
                {
                    /// 
                    /// New or removed text, resize for clean user input.
                    /// 
                    SizeTextBox(text);
                }
            }
        }
        /// <summary>
        /// Handles accepting the input for a value textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicFactorValueAddTextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text != null)
            {
                if (mbool_ignoreAddEscape == false)
                {
                    text.Text = text.Text.Replace(" ", "");
                    /// 
                    /// Accept the input if possible
                    /// 
                    TreeNode node = mtreeView_factorList.SelectedNode;
                    TreeNode parentNode = node.Parent;
                    node.Name = text.Text;
                    node.Text = text.Text;

                    /// 
                    /// Rename if possible
                    /// 
                    bool added = AddFactorValue(parentNode.Text, text.Text, node);
                    if (added == false)
                    {
                        parentNode.Nodes.Remove(node);
                    }

                    /// 
                    /// Dipose of the text box
                    /// 
                    mtreeView_factorList.Controls.Remove(text);
                    text.Dispose();
                }
                mbool_ignoreAddEscape = false;
            }
        }               
        #endregion

        #region Textbox Sound Suppressor
        /// <summary>
        /// Suppresses the textbox sound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textbox_KeyPress(object sender, KeyPressEventArgs e)
        {            
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
            }
            mbool_ignoreAddEscape = false;
        }
        #endregion

        private void mtreeView_factorList_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }

    class clsFactorTreeNodeSorter: IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            TreeNode xNode = x as TreeNode;
            TreeNode yNode = y as TreeNode;
            
            return xNode.Text.CompareTo(yNode.Text);
        }
        #endregion
    }
}