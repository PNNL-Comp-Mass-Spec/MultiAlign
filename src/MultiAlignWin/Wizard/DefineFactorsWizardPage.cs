using System;
using System.IO ; 
using System.Data ; 
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Collections.Generic;

using Wizard.UI;
using MultiAlignEngine;
using PNNLControls;
using ExternalControls;

using PNNLProteomics.Data;
using PNNLProteomics.Data.Factors;

using MultiAlignWin.Forms.Factors;

namespace MultiAlignWin
{
	public class DefineFactorsWizardPage : Wizard.UI.InternalWizardPage
	{
		//public enum enmToolID {ICR2LS=2, QTOFPek=7, MLynxPek=10, AgilentTOFPek=11, LTQ_FTPek=12, Decon2LS=16, Decon2LS_Agilent=18} ; 
	    private const int CONST_NUM_COLUMNS = 8;

        #region Members
        private IContainer          components = null;
        private Panel               panelFileNames;        
        private ListView            mlistview_datasets;
        private ListViewItemComparer mobj_listViewItemComparer;        
        private ColumnHeader        datasetID;
		private ColumnHeader        jobnum;
		private ColumnHeader        fileName;
		private ColumnHeader        alias;
		private ColumnHeader        block ;
		private ColumnHeader        runOrder ;
        private ComboBox            cmbBox;
        private ListViewItem        mobj_selectedItem = null;
        private TextBox             editBox;
        private MenuItem            menuItemFillBelow;
        private MenuItem            menuItemFillNBelow;
        private MenuItem            menuItemFillRand;
        private ContextMenu         contextMenuFactors;
        private MenuItem            menuItemFillNCycl;
        private ColumnHeader        lcColumnID;
        private ColumnHeader        batchID;
        private Button              mbutton_factors;

        private int X = 0;
        private int Y = 0;
        private int mint_subItemSelected = 0; 

        /// <summary>
        /// Reference to the list of dataset information.
        /// </summary>
        private List<clsDatasetInfo> marray_datasetInfo;
        /// <summary>
        /// Factors and Values: array of factor information.
        /// </summary>
        private List<clsFactorInfo> mlist_factorData;         
        /// <summary>
        /// Reference to the (factor-factor value) definitions for the entire dataset.
        /// </summary>
        private classFactorDefinition mobj_factors;        
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DefineFactorsWizardPage()
		{
            InitializeComponent();		
			mobj_listViewItemComparer             = new ListViewItemComparer();
			mlistview_datasets.ListViewItemSorter = mobj_listViewItemComparer;
            SetActive                             += new System.ComponentModel.CancelEventHandler(this.ctlSelectFilesFromDBFilterWizardPage_SetActive) ;

            /// 
            /// Create the factor definition, dataset information, and factor data arrays.
            /// 
            mobj_factors        = null;
            marray_datasetInfo  = new List<clsDatasetInfo>();
            mlist_factorData    = new List<clsFactorInfo>();
        } 

        #region Windows Forms
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
            this.panelFileNames = new System.Windows.Forms.Panel();
            this.editBox = new System.Windows.Forms.TextBox();
            this.mbutton_factors = new System.Windows.Forms.Button();
            this.cmbBox = new System.Windows.Forms.ComboBox();
            this.mlistview_datasets = new System.Windows.Forms.ListView();
            this.datasetID = new System.Windows.Forms.ColumnHeader();
            this.jobnum = new System.Windows.Forms.ColumnHeader();
            this.fileName = new System.Windows.Forms.ColumnHeader();
            this.alias = new System.Windows.Forms.ColumnHeader();
            this.block = new System.Windows.Forms.ColumnHeader();
            this.runOrder = new System.Windows.Forms.ColumnHeader();
            this.lcColumnID = new System.Windows.Forms.ColumnHeader();
            this.batchID = new System.Windows.Forms.ColumnHeader();
            this.contextMenuFactors = new System.Windows.Forms.ContextMenu();
            this.menuItemFillBelow = new System.Windows.Forms.MenuItem();
            this.menuItemFillNBelow = new System.Windows.Forms.MenuItem();
            this.menuItemFillNCycl = new System.Windows.Forms.MenuItem();
            this.menuItemFillRand = new System.Windows.Forms.MenuItem();
            this.panelFileNames.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.BackColor = System.Drawing.Color.White;
            this.Banner.Size = new System.Drawing.Size(740, 64);
            this.Banner.Subtitle = "Define factors and edit aliases";
            this.Banner.Title = "Step 2. Edit Factors and Aliases";
            // 
            // panelFileNames
            // 
            this.panelFileNames.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelFileNames.Controls.Add(this.editBox);
            this.panelFileNames.Controls.Add(this.mbutton_factors);
            this.panelFileNames.Controls.Add(this.cmbBox);
            this.panelFileNames.Controls.Add(this.mlistview_datasets);
            this.panelFileNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFileNames.Location = new System.Drawing.Point(134, 64);
            this.panelFileNames.Name = "panelFileNames";
            this.panelFileNames.Padding = new System.Windows.Forms.Padding(5);
            this.panelFileNames.Size = new System.Drawing.Size(740, 751);
            this.panelFileNames.TabIndex = 3;
            // 
            // editBox
            // 
            this.editBox.Location = new System.Drawing.Point(559, 1);
            this.editBox.Name = "editBox";
            this.editBox.Size = new System.Drawing.Size(100, 20);
            this.editBox.TabIndex = 2;
            this.editBox.Visible = false;
            this.editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            this.editBox.LostFocus += new System.EventHandler(this.FocusOver);
            // 
            // mbutton_factors
            // 
            this.mbutton_factors.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_factors.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_factors.Location = new System.Drawing.Point(8, 6);
            this.mbutton_factors.Name = "mbutton_factors";
            this.mbutton_factors.Size = new System.Drawing.Size(94, 22);
            this.mbutton_factors.TabIndex = 1;
            this.mbutton_factors.Text = "Factors";
            this.mbutton_factors.UseVisualStyleBackColor = true;
            this.mbutton_factors.Click += new System.EventHandler(this.mbutton_factors_Click);
            // 
            // cmbBox
            // 
            this.cmbBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbBox.Location = new System.Drawing.Point(400, 0);
            this.cmbBox.Name = "cmbBox";
            this.cmbBox.Size = new System.Drawing.Size(121, 21);
            this.cmbBox.TabIndex = 1;
            this.cmbBox.Text = "comboBox1";
            this.cmbBox.Visible = false;
            this.cmbBox.LostFocus += new System.EventHandler(this.cmbBoxFocusOver);
            this.cmbBox.SelectedIndexChanged += new System.EventHandler(this.cmbBoxSelectedIndexChanged);
            this.cmbBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbBoxKeyPress);
            // 
            // mlistview_datasets
            //             
            this.mlistview_datasets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistview_datasets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.datasetID,
            this.jobnum,
            this.fileName,
            this.alias,
            this.block,
            this.runOrder,
            this.lcColumnID,
            this.batchID});
            this.mlistview_datasets.FullRowSelect = true;
            this.mlistview_datasets.GridLines = true;
            this.mlistview_datasets.Location = new System.Drawing.Point(8, 36);
            this.mlistview_datasets.Name = "mlistview_datasets";
            this.mlistview_datasets.Size = new System.Drawing.Size(724, 707);
            this.mlistview_datasets.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.mlistview_datasets.TabIndex = 0;
            this.mlistview_datasets.UseCompatibleStateImageBehavior = false;
            this.mlistview_datasets.View = System.Windows.Forms.View.Details;
            this.mlistview_datasets.DoubleClick += new System.EventHandler(this.doubleClick_event);
            this.mlistview_datasets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.joblistView_ColumnClick);
            this.mlistview_datasets.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseDown_event);
            // 
            // datasetID
            // 
            this.datasetID.Text = "Dataset ID";
            this.datasetID.Width = 77;
            // 
            // jobnum
            // 
            this.jobnum.Text = "Job #";
            // 
            // fileName
            // 
            this.fileName.Text = "File name";
            this.fileName.Width = 222;
            // 
            // alias
            // 
            this.alias.Text = "Alias";
            this.alias.Width = 69;
            // 
            // block
            // 
            this.block.Text = "Block";
            // 
            // runOrder
            // 
            this.runOrder.Text = "Run Order";
            this.runOrder.Width = 70;
            // 
            // lcColumnID
            // 
            this.lcColumnID.Text = "Column ID";
            this.lcColumnID.Width = 77;
            // 
            // batchID
            // 
            this.batchID.Text = "Batch ID";
            // 
            // contextMenuFactors
            // 
            this.contextMenuFactors.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFillBelow,
            this.menuItemFillNBelow,
            this.menuItemFillNCycl,
            this.menuItemFillRand});
            // 
            // menuItemFillBelow
            // 
            this.menuItemFillBelow.Index = 0;
            this.menuItemFillBelow.Text = "Fill rows below with this value";
            this.menuItemFillBelow.Click += new System.EventHandler(this.menuItemFillBelow_Click);
            // 
            // menuItemFillNBelow
            // 
            this.menuItemFillNBelow.Index = 1;
            this.menuItemFillNBelow.Text = "Fill n blocks of rows";
            this.menuItemFillNBelow.Click += new System.EventHandler(this.menuItemFillNBelow_Click);
            // 
            // menuItemFillNCycl
            // 
            this.menuItemFillNCycl.Index = 2;
            this.menuItemFillNCycl.Text = "Fill n blocks cyclically";
            this.menuItemFillNCycl.Click += new System.EventHandler(this.menuItemFillNCycl_Click);
            // 
            // menuItemFillRand
            // 
            this.menuItemFillRand.Index = 3;
            this.menuItemFillRand.Text = "Fill randomly";
            this.menuItemFillRand.Click += new System.EventHandler(this.menuItemFillRand_Click);
            // 
            // ctlDefineFactorsWizardPage
            // 
            this.Controls.Add(this.panelFileNames);
            this.Name = "ctlDefineFactorsWizardPage";
            this.Size = new System.Drawing.Size(874, 815);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.panelFileNames, 0);
            this.panelFileNames.ResumeLayout(false);
            this.panelFileNames.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
        #endregion
       
        #region Properties
        public List<clsDatasetInfo> DatasetInfo
        {
            get
            {
                return marray_datasetInfo;
            }
            set
            {
                marray_datasetInfo = value;
            }
        }
        #endregion

        #region Wizard Events
        private void ctlSelectFilesFromDBFilterWizardPage_SetActive(object sender, CancelEventArgs e)
		{
            /// 
            /// Make this page the active one, clear the dataset information 
            /// 
			SetWizardButtons(Wizard.UI.WizardButtons.Back | Wizard.UI.WizardButtons.Next);
			mlistview_datasets.Items.Clear();
            UpdateListViewFactorColumns();
            /// 
            /// Fill the list view with our datasets, and display any pertinent help messages.
            /// 
			FillListView() ;

        }
        #endregion
        

        #region Event Handlers
        /// <summary>
        /// Sorts the data in the columns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joblistView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == mobj_listViewItemComparer.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (mobj_listViewItemComparer.Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    mobj_listViewItemComparer.Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    mobj_listViewItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                mobj_listViewItemComparer.SortColumn = e.Column;
                mobj_listViewItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.mlistview_datasets.Sort();
        }
        private void cmbBoxKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ( e.KeyChar == 13 || e.KeyChar == 27 )
			{
				cmbBox.Hide();
			}
		}
		private void cmbBoxSelectedIndexChanged(object sender, System.EventArgs e)
		{
			int sel = cmbBox.SelectedIndex;
			if ( sel >= 0 )
			{
				string itemSel = cmbBox.Items[sel].ToString();
				mobj_selectedItem.SubItems[mint_subItemSelected].Text = itemSel;
				UpdateDatasetFactorInfo() ;

                /// 
                /// Update the column length
                /// 
                mlistview_datasets.AutoResizeColumn(mint_subItemSelected, ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}
		private void doubleClick_event(object sender, System.EventArgs e)
		{
			// Check the subitem clicked .
			
            int nStart = X ;
			int spos = 0 ;
			mint_subItemSelected = 0 ;

			int sposX = mlistview_datasets.Location.X ; 
			int sposY = mlistview_datasets.Location.Y ;
			int epos = mlistview_datasets.Columns[0].Width ;
			for ( int i=0; i < mlistview_datasets.Columns.Count ; i++)
			{
				if ( nStart > spos && nStart < epos ) 
				{
					mint_subItemSelected = i ;
					break; 
				}
				
				spos = epos ;
				if (i+1 < mlistview_datasets.Columns.Count)
					epos += mlistview_datasets.Columns[i+1].Width;
			}
			
			string subItemText = mobj_selectedItem.SubItems[mint_subItemSelected].Text ;

			string colName = mlistview_datasets.Columns[mint_subItemSelected].Text ;
			
			if (mint_subItemSelected > 5)
			{ 
                // Factor columns
				cmbBox.Items.Clear() ;
				string factorName =  mlistview_datasets.Columns[mint_subItemSelected].Text ;                       // factor name

                /// 
                /// Make sure we have this factor 
                /// 
                if (mobj_factors.Factors.ContainsKey(factorName) == true)
                {
                    List<string> values = mobj_factors.Factors[factorName];
                    if (values != null)
                    {
                        string[] s = new string[values.Count];  
                        values.CopyTo(s);
                        cmbBox.Items.AddRange(s);

                        Rectangle r = new Rectangle(spos, mobj_selectedItem.Bounds.Y, epos, mobj_selectedItem.Bounds.Bottom);
                        cmbBox.Size = new System.Drawing.Size(epos - spos, mobj_selectedItem.Bounds.Bottom - mobj_selectedItem.Bounds.Top);
                        cmbBox.Location = new System.Drawing.Point(sposX + spos + 2, sposY + mobj_selectedItem.Bounds.Y);
                        cmbBox.Show();
                        cmbBox.Text = subItemText;
                        cmbBox.SelectAll();
                        cmbBox.Focus();
                    }
                }
			}

			if (mint_subItemSelected == 3)
			{ // Alias column
				Rectangle r = new Rectangle(spos , mobj_selectedItem.Bounds.Y , epos , mobj_selectedItem.Bounds.Bottom);
				editBox.Size  = new System.Drawing.Size(epos - spos , mobj_selectedItem.Bounds.Bottom-mobj_selectedItem.Bounds.Top);
				editBox.Location = new System.Drawing.Point(spos + 2 , mobj_selectedItem.Bounds.Y);
				editBox.Show() ;
				editBox.Text = subItemText;
				editBox.SelectAll() ;
				editBox.Focus();
			}
             
		}
		private void mouseDown_event(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mobj_selectedItem = mlistview_datasets.GetItemAt(e.X , e.Y);
			X = e.X ;
			Y = e.Y ;
			if (mobj_selectedItem != null)
			{
				// Check the subitem clicked .
				int nStart = X ;
				int spos = 0 ;
				mint_subItemSelected = 0 ;

				int sposX = mlistview_datasets.Location.X ; 
				int sposY = mlistview_datasets.Location.Y ;
				int epos = mlistview_datasets.Columns[0].Width ;
				for ( int i=0; i < mlistview_datasets.Columns.Count ; i++)
				{
					if ( nStart > spos && nStart < epos ) 
					{
						mint_subItemSelected = i ;
						break; 
					}
				
					spos = epos ;
					if (i+1 < mlistview_datasets.Columns.Count)
						epos += mlistview_datasets.Columns[i+1].Width;
				}

				Point p = new Point(e.X, e.Y) ;
				if ( ((e.Button & MouseButtons.Right) == MouseButtons.Right) && (mint_subItemSelected > (CONST_NUM_COLUMNS-1)) )
				{
					contextMenuFactors.Show(mlistview_datasets,p) ;
				}
			}
		}
		private void cmbBoxFocusOver(object sender , System.EventArgs e)
		{
			cmbBox.Hide() ;
		}
		private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ( e.KeyChar == 13 ) 
			{
				mobj_selectedItem.SubItems[mint_subItemSelected].Text = editBox.Text;
				editBox.Hide();
				UpdateAliases() ;
			}

			if ( e.KeyChar == 27 ) 
				editBox.Hide();
		}
		private void FocusOver(object sender, System.EventArgs e)
		{
			mobj_selectedItem.SubItems[mint_subItemSelected].Text = editBox.Text;
			editBox.Hide();
			UpdateAliases() ;
		}
		private void menuItemFillBelow_Click(object sender, System.EventArgs e)
		{
			int idx = 0 ;
			foreach (ListViewItem item in mlistview_datasets.Items)
			{
				if (item.Tag.Equals(mobj_selectedItem.Tag))
					break ;
				else
					idx++ ;
			}
			string factorVal = mobj_selectedItem.SubItems[mint_subItemSelected].Text ;
			for (int num = idx ; num < mlistview_datasets.Items.Count ; num++)
			{
				mlistview_datasets.Items[num].SubItems[mint_subItemSelected].Text = factorVal ;
				((MultiAlignEngine.clsDatasetInfo)marray_datasetInfo[num]).AssignedFactorValues[mint_subItemSelected-CONST_NUM_COLUMNS] = factorVal ;
			}
		
		}

        /// <summary>
        /// Fills the block of nBlock list view items with the factor value provided starting at idx.
        /// </summary>
        /// <param name="idx">Index to start filling from</param>
        /// <param name="nBlock">Number of blocks to fill.</param>
        /// <param name="factorValue">Value to fill with.</param>
        private void FillBlock(int idx, int nBlock, string factorValue)
        {
            /// 
            /// Now for the block of samples fill in the data.
            /// 
            for (int num = idx; num < idx + nBlock; num++)
            {
                if (num < mlistview_datasets.Items.Count)
                {
                    mlistview_datasets.Items[num].SubItems[mint_subItemSelected].Text = factorValue;
                    ((MultiAlignEngine.clsDatasetInfo)marray_datasetInfo[num]).AssignedFactorValues[mint_subItemSelected - CONST_NUM_COLUMNS] = factorValue;
                }
            }
        }
        /// <summary>
        /// Finds the selected starting index of the list view item given it could be sorted.
        /// </summary>
        /// <returns>Zero based index of list view items location.</returns>
        private int FindItemStartingIndex(ListViewItem selectedItem)
        {
            int idx = 0;
            /// 
            /// Find hte list view item, do this instead of index of since
            /// the objects may be sorted.
            /// 
            foreach (ListViewItem item in mlistview_datasets.Items)
            {
                if (item.Tag.Equals(selectedItem.Tag))
                    break;
                else
                    idx++;
            }

            return idx;
        }
        /// <summary>
        /// Fills the immediate block of N listview items with the factor value selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void menuItemFillNBelow_Click(object sender, System.EventArgs e)
		{
			int idx     = 0;
			int nBlock  = 1;

			frmInputBlockSize mfrmInputBlockSize = new frmInputBlockSize() ;
            if (mfrmInputBlockSize.ShowDialog() == DialogResult.Cancel)
				return ; 			

			nBlock  = mfrmInputBlockSize.blockSize;
            idx     = FindItemStartingIndex(mobj_selectedItem);
            
            /// 
            /// Grab the selected factor value to set
            /// 
			string factorVal = mobj_selectedItem.SubItems[mint_subItemSelected].Text ;
            FillBlock(idx, nBlock, factorVal);            
        }
        /// <summary>
        /// Handles when the user clicks to randomly fill factor values for each dataset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemFillRand_Click(object sender, System.EventArgs e)
        {
            if (mint_subItemSelected > (CONST_NUM_COLUMNS - 1))
            {
                FillValuesRandomly(mint_subItemSelected);
            }
        }
        /// <summary>
        /// Handles when the user clicks to cyclically fill factor values for each dataset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemFillNCycl_Click(object sender, System.EventArgs e)
        {
            FillNCycles();
        }        
        /// <summary>
        /// Displays the factor definition window.
        /// </summary>
        private void mbutton_factors_Click(object sender, EventArgs e)
        {
            DisplayFactorForm();
        }
        #endregion

        #region Dataset Initialization and Updates
        /// <summary>
        /// Initializes the datasets with factor value information.
        /// </summary>
        private void InitializeDatasets()
        {
            /// 
            /// For every dataset item, assign it's factor value.
            /// 
            foreach (clsDatasetInfo info in marray_datasetInfo)
            {
                /// 
                /// Clear the assignment for this dataset information.
                /// 
                info.AssignedFactorValues.Clear();
                info.factorsDefined = true;
                info.Factors.Clear();

                /// 
                /// After we cleared the factors, then we want to add
                /// in factor information for each dataset, and 
                /// assign one of the values.                
                ///                                 
                foreach (string key in mobj_factors.Factors.Keys)
                {
                    clsFactorInfo factorInfo = new clsFactorInfo();
                    factorInfo.mstrFactor    = key;
                    foreach (string value in mobj_factors.Factors[key])
                    {
                        factorInfo.marrValues.Add(value);
                    }

                    /// 
                    /// Here we assign value 
                    ///                     
                    string factorValue = factorInfo.marrValues[0] as string;
                    info.AssignedFactorValues.Add(factorValue);
                    /// 
                    /// Here we add to our factor list for this dataset.
                    /// 
                    info.Factors.Add(factorInfo);
                }
            }
        }
        private void UpdateDatasetFactorInfo()
        {
            int numEntries = mlistview_datasets.Items.Count;
            MultiAlignEngine.clsFactorInfo currentFactor = new MultiAlignEngine.clsFactorInfo();

            for (int row = 0; row < numEntries; row++)
            {
                if (mlist_factorData.Count > 0) // update factors
                {
                    /*for (int i = 0 ; i < marray_factors.Length ; i++)
                    {
                        ((MultiAlignEngine.clsDatasetInfo)marray_datasetInfo[row]).AssignedFactorValues[i] = 
                            mlistview_datasets.Items[row].SubItems[i+CONST_NUM_COLUMNS].Text ; 
                    }*/
                }
            }
        }
        private void UpdateAliases()
        {
            int numEntries = mlistview_datasets.Items.Count;
            for (int row = 0; row < numEntries; row++)
            {
                ((MultiAlignEngine.clsDatasetInfo)marray_datasetInfo[row]).mstrAlias =
                    mlistview_datasets.Items[row].SubItems[3].Text;
            }
        }
        #endregion

        #region Factor Value Filling (Cycle, Random, Block)   
        private void FillNCycles()
        {
            int start = 0;
            int cycle = 0;
            int nBlock = 1;

            /// 
            /// Get the user input on how many blocks to use
            /// 
            frmInputBlockSize mfrmInputBlockSize = new frmInputBlockSize();
            if (mfrmInputBlockSize.ShowDialog() == DialogResult.Cancel)
                return;

            /// 
            /// Find the listview index
            /// 
            nBlock = mfrmInputBlockSize.blockSize;
            start = FindItemStartingIndex(mobj_selectedItem);

            /// 
            /// Get the used factor value here
            /// 
            string factorVal = mobj_selectedItem.SubItems[mint_subItemSelected].Text; // get it's factor value

            /// 
            /// Get the factor data strings
            /// 
            string key = mlistview_datasets.Columns[mint_subItemSelected].Text;
            int numVals = mobj_factors.Factors[key].Count;    //number of values in this factor            
            List<string> fvals = mobj_factors.Factors[key];
            int startIdxVal = fvals.IndexOf(factorVal);

            /// 
            /// Look from the start index of the dataset, down to all the rest.                        
            /// 
            while (start < mlistview_datasets.Items.Count)
            {
                /// 
                /// For each block, start at start, then go until we have start + N of items to iterate                
                /// 
                for (int num = start; num < (start + nBlock); num++)
                {
                    if (num < mlistview_datasets.Items.Count)
                    {
                        /// Use the factor value defined from start Index (the index of the selected factor value in our array + the cycle)
                        /// 
                        factorVal = fvals[(startIdxVal + cycle) % numVals];
                        /// 
                        /// Set the factor value in the list view
                        /// 
                        mlistview_datasets.Items[num].SubItems[mint_subItemSelected].Text = factorVal;
                        /// 
                        /// Update the raw data array too!
                        /// 
                        ((MultiAlignEngine.clsDatasetInfo)marray_datasetInfo[num]).AssignedFactorValues[mint_subItemSelected - CONST_NUM_COLUMNS] = factorVal;
                    }
                    else
                        break;
                }
                /// 
                /// Skip over a block
                /// 
                start = start + nBlock;
                cycle++;                //i.e. the next cycle
            }
        }
        /// <summary>
        /// Updates the list view with factor information using one column for each factor.
        /// </summary>	
        private void UpdateListViewFactorColumns()
        {            
            /// 
            /// We have a list of columns that are static, then we have a list of columns that are 
            /// factor definitions.
            /// 
            while (mlistview_datasets.Columns.Count > CONST_NUM_COLUMNS)//First remove all existing ones
            {
                int lastColumn = mlistview_datasets.Columns.Count - 1;
                /// 
                /// Remove Subitems because removing a column will not remove the subitems.                 
                /// 					
                for (int rows = 0; rows < mlistview_datasets.Items.Count; rows++)
                    mlistview_datasets.Items[rows].SubItems.RemoveAt(lastColumn);

                /// 
                /// Remove the column header.
                /// 
                ColumnHeader header = mlistview_datasets.Columns[lastColumn];
                mlistview_datasets.Columns.Remove(header);
            }

            /// 
            /// Now all of the headers and subitems are deleted, Add new column headers based 
            /// on our factor information.            
            /// 
            ListViewItem currentItem;
            ListViewItem.ListViewSubItem currentSubItem;

            /// 
            /// If factors were in fact defined, add the dataset info to the new columns.
            /// 
            if (mobj_factors != null)
            {
                mlistview_datasets.BeginUpdate();
                int col = 0;  
                int lastColumn = mlistview_datasets.Columns.Count - 1;  // For resizing columns.
                /// 
                /// Then add the factor information
                /// 
                foreach (string key in mobj_factors.Factors.Keys)
                {

                    /// 
                    /// Create a new column
                    /// 
                    ColumnHeader column = new ColumnHeader();
                    column.Text = key;
                    mlistview_datasets.Columns.Add(column);

                    /// 
                    /// Now add back all the subitems.
                    /// 
                    for (int row = 0; row < mlistview_datasets.Items.Count; row++)
                    {
                        clsDatasetInfo info = marray_datasetInfo[row] as clsDatasetInfo;
                        if (info != null)
                        {
                            /// 
                            /// Add the item as a sub item to the parent at the row specified.
                            /// 
                            string text = info.AssignedFactorValues[col];
                            currentItem = mlistview_datasets.Items[row];
                            currentSubItem = new ListViewItem.ListViewSubItem(currentItem, text);
                            mlistview_datasets.Items[row].SubItems.Add(currentSubItem);
                        }
                    }
                    

                    col++;
                    /// Resize this column to fit to the header.
                    mlistview_datasets.AutoResizeColumn(column.DisplayIndex, ColumnHeaderAutoResizeStyle.ColumnContent);            
                }

                /// 
                /// Resize the last data column, do this after the others have been added so that it allows everything
                /// to be visible, fitting to header size will make last column stretch to edge of window.                
                /// Also resize the last guy to fit the windows.
                /// 
                mlistview_datasets.AutoResizeColumn(lastColumn, ColumnHeaderAutoResizeStyle.HeaderSize);
                mlistview_datasets.AutoResizeColumn(mlistview_datasets.Columns.Count - 1, ColumnHeaderAutoResizeStyle.HeaderSize);   
                mlistview_datasets.EndUpdate();              
            }            
        }
        /// <summary>
        /// Updates the listview with the data set information.
        /// </summary>
        private void FillListView()
        {
            mlistview_datasets.BeginUpdate();
            string[] datasetID = new string[marray_datasetInfo.Count];
            int i = 0;
            foreach (clsDatasetInfo datasetInfo in marray_datasetInfo)
            {
                /// 
                /// Create the first listview item
                /// 
                ListViewItem dataItem = new ListViewItem(datasetInfo.mstrDatasetId);

                /// 
                /// Then the following subitems 
                ///     Analysis Job ID
                ///     Dataset Name
                ///     Alias                
                /// 
                dataItem.SubItems.Add(datasetInfo.mstrAnalysisJobId);
                dataItem.SubItems.Add(datasetInfo.mstrDatasetName);
                dataItem.SubItems.Add(datasetInfo.mstrAlias);

                /// 
                /// This is to query the factor information
                /// 
                datasetID[i++] = datasetInfo.mstrDatasetId;

                /// 
                /// Tag the listview item with the info reference
                /// 
                dataItem.Tag = datasetInfo;

                /// 
                /// Handle telling the user if the item is not sufficient.
                /// 
                if (datasetInfo.mintBlockID == 0)
                    dataItem.SubItems.Add("NA");
                else
                    dataItem.SubItems.Add(datasetInfo.mintBlockID.ToString());

                if (datasetInfo.mintRunOrder == 0)
                    dataItem.SubItems.Add("NA");
                else
                    dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintRunOrder));

                if (datasetInfo.mintColumnID == 0)
                    dataItem.SubItems.Add("NA");
                else
                    dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintColumnID));

                if (datasetInfo.mintBatchID == 0)
                    dataItem.SubItems.Add("NA");
                else
                    dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintBatchID));


                foreach (string factor in datasetInfo.AssignedFactorValues)
                {
                    dataItem.SubItems.Add(factor);
                }
                /// 
                /// Then add it to the listview
                /// 
                mlistview_datasets.Items.Add(dataItem);
            }

            mlistview_datasets.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            mlistview_datasets.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_datasets.EndUpdate();


            /// 
            /// DMS Server connection information
            /// 
            //clsDMSServerInformation info = new clsDMSServerInformation();            
            //info.ServerName         = Properties.Settings.Default.DMSServerName;
            //info.DatabaseName       = Properties.Settings.Default.DMSDatabaseName;
            //info.Username           = "dmswebuser";
            //info.Password           = "icr4fun";
            //info.ConnectionTimeout  = Properties.Settings.Default.DMSConnectionTimeout;

            /// 
            /// Factor Assignment Dictionary
            /// 
            //Dictionary<string, List<KeyValuePair<string, string>>> factorAssignments = new Dictionary<string, List<KeyValuePair<string, string>>>();
            //Dictionary<string, List<string>> factorTable = new Dictionary<string, List<string>>();

            //PNNLProteomics.Data.Loaders.FactorLoader factorloader   = new PNNLProteomics.Data.Loaders.FactorLoader(info);
            //factorloader.LoadFactors(datasetID, out factorAssignments, out factorTable);            
        }
        /// <summary>
        /// Fills the list view factor values based off of a random number generator.
        /// </summary>
        /// <param name="column">Column to randomly fill with values.</param>
        private void FillValuesRandomly(int itemIndex)
        {
            Random randomNumberGenerator    = new Random();
            int column                      = itemIndex - CONST_NUM_COLUMNS;
                       
            /// 
            /// Get the factor data strings
            /// 
            string key          = mlistview_datasets.Columns[mint_subItemSelected].Text;
            int numVals         = mobj_factors.Factors[key].Count;    //number of values in this factor            
            List<string> fvals  = mobj_factors.Factors[key];
            
            for (int i = 0; i < marray_datasetInfo.Count; i++)
            {
                /// 
                /// Generate the new value index
                /// 
                int randomIndex                      = randomNumberGenerator.Next(numVals);
                clsDatasetInfo info                  = marray_datasetInfo[i];

                ListViewItem.ListViewSubItem subItem = mlistview_datasets.Items[i].SubItems[itemIndex];
                string value                         = fvals[randomIndex];
                subItem.Text                         = value;
                info.AssignedFactorValues[column]    = value;
            }
        }
        #endregion

        #region Factor Definition
        /// <summary>
        /// Displays the factor definition window allowing the user to change the factor information.
        /// </summary>
        private void DisplayFactorForm()
        { 
            /// 
            /// Check to see if the factors have already been defined.
            /// 

            classFactorDefinition clonedFactors = null;
            DialogResult result = DialogResult.Cancel;

            if (mobj_factors != null)
            {
                    result = MessageBox.Show("Current factor value assignments will be lost. Continue?",
                                             "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return; 
                }

                /// 
                /// Clone because we are going to erase the factor information
                /// 
                clonedFactors = mobj_factors.Clone() as classFactorDefinition; 
            }

            /// 
            /// Create and show the factor definition window
            /// 
            frmFactorDefinition factors = new frmFactorDefinition(clonedFactors);                  
            if (ParentForm != null)
                factors.Icon = ParentForm.Icon;

            /// 
            /// Then if the OK button was accepted, push the data into the dataset information
            /// 
            result = factors.ShowDialog();
            if (result == DialogResult.OK)
            {                
                /// 
                /// Accept the factor input.
                /// 
                mobj_factors = factors.Factors;  
               
                /// 
                /// Update the user interface now
                /// 
                InitializeDatasets();
                UpdateListViewFactorColumns();
            }
        }
        #endregion
	}
}
