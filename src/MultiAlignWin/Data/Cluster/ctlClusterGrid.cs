using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO ; 
using System.Data;
using System.Windows.Forms;


namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for ctlClusterGrid.
	/// </summary>
	public class ctlClusterGrid : ECsoft.Windows.Forms.DataGridEx //System.Windows.Forms.DataGrid
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private bool mbln_show_cluster_aligned_net = false ; 
		private bool mbln_show_cluster_calibrated_mass = false ; 

		private bool mbln_show_all_columns = false ; 
		private bool mbln_show_calibrated_mass_columns = false ; 
		private bool mbln_show_mass_columns = false ; 
		private bool mbln_show_scan_columns = false ; 
		private bool mbln_show_aligned_scan_columns = false ; 
		private bool mbln_show_umc_index_columns = false ; 
		private bool mblnShowMassTags = false ; 

		private bool mblnPeakMatched = false ; 
		private bool mbln_ttest_performed = false ; 
		private bool mbln_show_ttest_column = false ; 
		private bool mbln_show_normalized = false ; 

		private const string mstr_umc_rep_mass_col = "Mass" ; 
		private const string mstr_umc_rep_net_col = "NET" ; 
		
		private const string mstr_umc_rep_mass_calib_col = "Calibrated Mass" ; 
		private const string mstr_umc_rep_net_aligned_col = "Aligned NET" ; 

		private const string mstr_umc_index_col = "Row ID" ; 
		private const string mstr_umc_rep_size_col = "Cluster Size" ; 
		private const string mstr_peptide_col = "Peptide" ; 

		private const string mstr_mass_tag_id_col = "Mass Tags" ;
        private const string mstr_mass_tag_net_col = "Mass Tag NET";
        private const string mstr_mass_tag_mass_col = "Mass Tag Mass"; 
		private const string mstr_mass_tag_F_CS1 = "Charge 1 F Score" ; 
		private const string mstr_mass_tag_F_CS2 = "Charge 2 F Score" ; 
		private const string mstr_mass_tag_F_CS3 = "Charge 3 F Score" ;

        private const string mstr_mass_tag_xcorr_col = "Mass Tag Xcorr"; 
		private const string mstr_mass_tag_modification_col = "Modifications" ; 
		private const string mstr_mass_tag_modification_count_col = "Mod count" ; 
		private const string mstr_protein_col = "Protein" ; 
		private const string mstr_proteinid_col = "RefID" ; 

		private const string mstr_mass_colum = "Mass" ; 
		private const string mstr_calibrated_mass_colum = "Calibrated_mass" ; 
		private const string mstr_scan_colum = "Scan" ; 
		private const string mstr_aligned_scan_colum = "Aligned Scan" ; 
		private const string mstr_umc_index_column = "UMC index" ; 

		private string mstr_ttest_col_name = "T-Test" ; 
		private string mstr_empty = "" ;

        private ContextMenuStrip mobj_contextMenu;

		private clsTTestVals [] marr_ttests = null ; 
		private clsMultiAlignAnalysis mobjAnalysis ; 
		public event MethodInvoker mevntExpressionPlotOpenClicked ; 
		public event MethodInvoker mevntDataSummaryOpenClicked ; 
		public event MethodInvoker mevntScatterPlotOpenClicked ; 
		public event MethodInvoker mevntHeatMapClicked ;

        public delegate void SaveTableDelegate(object sender, EventArgs e);
        public SaveTableDelegate saveTableDel;

		public ctlClusterGrid()
		{						
			InitializeComponent();
			Init() ; 

			AllowUserToOrderColumns = true;
            saveTableDel = new SaveTableDelegate(mnu_save_grid_Click);
		}

		public clsMultiAlignAnalysis Analysis
		{
			set
			{
				mobjAnalysis = value ; 
				AddClusterToTable() ; 
				CreateContextMenu();
			}
		}

		public ctlClusterGrid(clsMultiAlignAnalysis analysis, string name)
		{
			InitializeComponent();					
			mobjAnalysis = analysis ; 
			Init() ; 
		}

		/// <summary>
		/// Class level initializations for the control not related to component building and initialization.
		/// </summary>
		private void Init()
		{
			if (mobjAnalysis == null)
				return; 					
		}

        /// <summary>        
        /// Create the data table to bind to the data grid.        
        /// </summary>
		private void AddClusterToTable()
		{
			// Create a new table.
			DataTable table = new DataTable("Analysis") ; 

			table.Columns.Add(mstr_umc_index_col, typeof(int)) ; 
			table.Columns.Add(mstr_umc_rep_size_col, typeof(int)) ; 
			table.Columns.Add(mstr_umc_rep_mass_col, typeof(double)) ; 
			
            /// 
            /// Calibrated Mass
            /// 
            if (mbln_show_cluster_calibrated_mass)
				table.Columns.Add(mstr_umc_rep_mass_calib_col, typeof(double)) ; 
			
            /// 
            /// UMC - Representative Net
            /// 
            table.Columns.Add(mstr_umc_rep_net_col, typeof(double)) ; 
			
            /// 
            /// Cluster Aligned net
            /// 
            if (mbln_show_cluster_aligned_net)
				table.Columns.Add(mstr_umc_rep_net_aligned_col, typeof(double)) ; 

            /// 
            /// T-Test 
            /// 
			if(mbln_show_ttest_column)
			{
				table.Columns.Add(mstr_ttest_col_name, typeof(double)) ; 
			}

            /// 
            /// Data source = null
            /// 
			try
			{
				this.DataSource = null ; 
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + ex.StackTrace) ; 
			}

            /// 
            /// Start of Mass Tag Column
            /// 
			int start_mass_tag_column = table.Columns.Count ; 

            /// 
            /// Mass Tags
            /// 
			if (mblnShowMassTags && mobjAnalysis.PeakMatchedToMassTagDB)
			{
				table.Columns.Add(mstr_peptide_col) ; 
				table.Columns.Add(mstr_protein_col) ; 
				table.Columns.Add(mstr_proteinid_col, typeof(int)) ; 
				table.Columns.Add(mstr_mass_tag_id_col, typeof(int)) ; 
				table.Columns.Add(mstr_mass_tag_mass_col, typeof(double)) ; 
				table.Columns.Add(mstr_mass_tag_net_col, typeof(double)) ; 
				table.Columns.Add(mstr_mass_tag_xcorr_col, typeof(double)) ; 
				table.Columns.Add(mstr_mass_tag_modification_count_col, typeof(short)) ; 
				table.Columns.Add(mstr_mass_tag_modification_col) ; 
				table.Columns.Add(mstr_mass_tag_F_CS1) ; 
				table.Columns.Add(mstr_mass_tag_F_CS2) ; 
				table.Columns.Add(mstr_mass_tag_F_CS3) ; 
				
			}
            			
			try
			{
				int num_columns_per_dataset = 1 ; 
				if (mbln_show_mass_columns)
				{
					num_columns_per_dataset++ ; 
				}
				if (mbln_show_calibrated_mass_columns)
				{
					num_columns_per_dataset++ ; 
				}
				if (mbln_show_scan_columns)
				{
					num_columns_per_dataset++ ; 
				}
				if (mbln_show_aligned_scan_columns)
				{
					num_columns_per_dataset++ ; 
				}
				if (mbln_show_umc_index_columns)
				{
					num_columns_per_dataset++ ; 
				}

				int start_data_column_num = table.Columns.Count ; 

				int num_datasets = mobjAnalysis.UMCData.NumDatasets ; 
				int num_clusters = mobjAnalysis.UMCData.mobjClusterData.NumClusters  ; 

				ArrayList arr_datasets_to_show = new ArrayList() ; 

				for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
				{
					string file_name = mobjAnalysis.UMCData.DatasetName[dataset_num] ;
					int index = file_name.LastIndexOf("\\") ;
					string col_name = file_name.Substring(index+1) ; 
					arr_datasets_to_show.Add(dataset_num) ; 
					if (mbln_show_mass_columns)
					{
						table.Columns.Add(col_name + "_" + mstr_mass_colum, typeof(double)) ;
					}
					if (mbln_show_calibrated_mass_columns)
					{
						table.Columns.Add(col_name + "_" + mstr_calibrated_mass_colum, typeof(double)) ;
					}
					if (mbln_show_scan_columns)
					{
						table.Columns.Add(col_name + "_" + mstr_scan_colum, typeof(double)) ;
					}
					if (mbln_show_aligned_scan_columns)
					{
						table.Columns.Add(col_name + "_" + mstr_aligned_scan_colum, typeof(double)) ;
					}
					if (mbln_show_umc_index_columns)
					{
						table.Columns.Add(col_name + "_" + mstr_umc_index_column, typeof(int)) ;
					}
					table.Columns.Add(col_name, typeof(double)) ; 
				}

				int num_datasets_to_show = arr_datasets_to_show.Count ; 
				int num_rows_so_far = 0 ; 
				MultiAlign.Features.clsUMC [] arrUMCs = mobjAnalysis.UMCData.marr_umcs ; 
				int [] arrClusterMainMemberIndex = mobjAnalysis.UMCData.mobjClusterData.marrClusterMainMemberIndex ; 
				double [] arrClusterMemberIntensity = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensity ; 
				double [] arrClusterMemberNormalizedIntensity = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensityNormalized ; 
				MultiAlign.PeakMatching.clsPeakMatchingResults.clsPeakMatchingTriplet [] arrPeakMatchingTriplets = null ;
				MultiAlign.MassTags.clsProtein [] arrPeakMatchingProteins = null ; 
				MultiAlign.MassTags.clsMassTag [] arrPeakMatchingMassTags = null ; 

				int clusterNum = 0 ; 
				int currentPeakMatchNum = 0 ; 
				int numPeakMatches = 0 ; 
				if (mobjAnalysis.PeakMatchingResults != null)
				{
					arrPeakMatchingTriplets = mobjAnalysis.PeakMatchingResults.marrPeakMatchingTriplet ;
					arrPeakMatchingProteins = mobjAnalysis.PeakMatchingResults.marrProteins ; 
					arrPeakMatchingMassTags = mobjAnalysis.PeakMatchingResults.marrMasstags ; 
					numPeakMatches = arrPeakMatchingTriplets.Length ; 
				}

				int lastClusterNum = -1 ; 

				while(clusterNum < num_clusters)
				{
					MultiAlign.Features.clsCluster cluster = mobjAnalysis.UMCData.mobjClusterData.GetCluster(clusterNum);
					DataRow row = table.NewRow() ;
					row[0] = Convert.ToString(clusterNum+1) ; 
					row[2] = Convert.ToString(cluster.mdbl_mass) ; 
					int num_column = 3 ;
					if(mbln_show_cluster_calibrated_mass)
						row[num_column++] = Convert.ToString(cluster.mdbl_mass_calibrated) ; 
					row[num_column++] = Convert.ToString(cluster.mdbl_net) ; 
					if (mbln_show_cluster_aligned_net)
						row[num_column++] = Convert.ToString(cluster.mdbl_aligned_net) ; 

					if(mbln_show_ttest_column)
					{
						if (marr_ttests[clusterNum] != null)
						{
							clsTTestVals ttest_val = marr_ttests[clusterNum] ; 
							if (ttest_val != null)
							{
								row[num_column] = Convert.ToString(ttest_val.mdbl_pvalue) ; 
							}
							else
							{
								row[num_column] = DBNull.Value ; 
							}
						}
					}

					int num_non_empty = 0 ; 
					for (int col_num = 0 ; col_num < num_datasets_to_show * num_columns_per_dataset ; col_num++)
					{
						int dataset_index = col_num / num_columns_per_dataset ; 
						int dataset_num = (int) arr_datasets_to_show[dataset_index] ; 
						int pt_index = clusterNum*num_datasets + dataset_num ; 
						int index = arrClusterMainMemberIndex[pt_index] ; 
						MultiAlign.Features.clsUMC umc = null ; 
						if (index != -1)
						{
							umc = arrUMCs[index] ; 
							num_non_empty++ ;
						}

						if (mbln_show_mass_columns)
						{
							if (umc != null)
								row[start_data_column_num+col_num] = Convert.ToString(umc.mdbl_mono_mass) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}
						if (mbln_show_calibrated_mass_columns)
						{
							if (umc != null)
								row[start_data_column_num+col_num] = Convert.ToString(umc.mdbl_mono_mass_calibrated) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}

						if (mbln_show_scan_columns)
						{
							if (umc != null)
								row[start_data_column_num+col_num] = Convert.ToString(umc.mint_scan) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}
						if (mbln_show_aligned_scan_columns)
						{
							if (umc != null)
								row[start_data_column_num+col_num] = Convert.ToString(umc.mint_scan_aligned) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}
						if (mbln_show_umc_index_columns)
						{
							if (umc != null)
								row[start_data_column_num+col_num] = Convert.ToString(umc.mint_umc_index) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}
						if (umc != null)
						{
							double intensity = 0 ;
							if (mbln_show_normalized)
							{
								intensity = arrClusterMemberNormalizedIntensity[pt_index] ;
							}
							else
							{
								intensity = arrClusterMemberIntensity[pt_index] ;
							}
							row[start_data_column_num+col_num] = Convert.ToString(intensity) ; 
						}
						else
						{
							// missing value set as 0
							row[start_data_column_num+col_num] = DBNull.Value ; 
						}
					}
					row[1] = Convert.ToString(num_non_empty) ; 
					// AddMassTags to Row
					// if it is peakmatched, and show mass tags is enabled, everything shows. 
					if(mblnShowMassTags && mobjAnalysis.PeakMatchedToMassTagDB) 
					{
						if (arrPeakMatchingTriplets != null && 
							currentPeakMatchNum < arrPeakMatchingTriplets.Length 
							&& arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
						{
							// so this peakmatchtriplet corresponds to the current cluster. 
							// Lets display it. 
							int current_column = start_mass_tag_column ; 
							MultiAlign.PeakMatching.clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum] ; 
							MultiAlign.MassTags.clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex] ; 
							MultiAlign.MassTags.clsProtein protein = arrPeakMatchingProteins[triplet.mintProteinIndex] ; 
							row[current_column++] = massTag.mstrPeptide ; 
							row[current_column++] = protein.mstrProteinName ; 
							row[current_column++] = protein.mintRefID ; 
							row[current_column++] = massTag.mintMassTagId ; 
							row[current_column++] = massTag.mdblMonoMass ; 
							row[current_column++] = massTag.mdblAvgGANET ; 
							row[current_column++] = massTag.mdblHighXCorr ; 
							row[current_column++] = massTag.mshortModCount ; 
							row[current_column++] = massTag.mstrModification ; 
							row[current_column++] = massTag.mfltAvgFCS1 ; 
							row[current_column++] = massTag.mfltAvgFCS2 ; 
							row[current_column++] = massTag.mfltAvgFCS3 ; 
							currentPeakMatchNum++ ;	
							lastClusterNum = clusterNum ; 
							table.Rows.Add(row) ; 
						}
						else
						{
							if (lastClusterNum != clusterNum)
								table.Rows.Add(row) ; 
							clusterNum++ ; 
						}
					}
					else
					{
						table.Rows.Add(row) ; 
						clusterNum++ ; 
					}
					num_rows_so_far++ ; 
				}
				this.DataSource = table ;
				AddGridStyle(table) ; 
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + ex.StackTrace) ; 
				this.DataSource = table ;
			}
		}

        /// <summary>
        /// Adds a grid style to the underlying data table.
        /// </summary>
        /// <param name="table"></param>
		private void AddGridStyle(DataTable table)
		{
			this.TableStyles.Clear() ; 

			DataGridTableStyle myGridStyle = new DataGridTableStyle();
			myGridStyle.MappingName = table.TableName ;

			for (int colNum = 0 ; colNum < table.Columns.Count ; colNum++)
			{
				DataGridTextBoxColumn colStyle = new DataGridTextBoxColumn();				
				colStyle.MappingName = table.Columns[colNum].ColumnName ;
				colStyle.HeaderText= table.Columns[colNum].ColumnName  ;
				colStyle.NullText = mstr_empty ; 
				myGridStyle.GridColumnStyles.Add(colStyle);
			}
			TableStyles.Add(myGridStyle);
		}

		/// <summary>
		/// Gets and sets the context menu to be used on this control.
		/// </summary>
        public new ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return mobj_contextMenu;
			}
			set
			{
				mobj_contextMenu = value;
			}
		}


		/// <summary>
		/// Creates the context menu;
		/// </summary>
		/// <returns></returns>
        public ContextMenuStrip CreateContextMenu()
		{
			/// 
            /// Data 
            /// 
			ToolStripMenuItem mnu_columns			= new ToolStripMenuItem("Show Columns");
			ToolStripMenuItem mnu_normalize			= new ToolStripMenuItem("Normalize");
			ToolStripMenuItem mnu_diff_abundance	= new ToolStripMenuItem("Perform ttest");
			ToolStripMenuItem mnu_cluster			= new ToolStripMenuItem("Re-Cluster");
			ToolStripMenuItem mnu_align				= new ToolStripMenuItem("Re-Align");			
			ToolStripMenuItem mnu_dataSummary		= new ToolStripMenuItem("Analysis Summary");								
			ToolStripMenuItem mnu_columns_mass_tags	= new ToolStripMenuItem("Mass Tags/Proteins");
            ToolStripMenuItem mnu_save_grid         = new ToolStripMenuItem("Save Table");
            
            mnu_cluster.Enabled = false;
            mnu_align.Enabled   = false;
            	
			/// 
            /// Charts 
            /// 
            ToolStripMenuItem mnu_expression = new ToolStripMenuItem("Scatter Plots/Histograms");
			//ToolStripMenuItem mnu_show_alignment	= new ToolStripMenuItem("Show Alignment Charts");
			ToolStripMenuItem mnu_show_normalized	= new ToolStripMenuItem("Show Normalized Intensities");			
			ToolStripMenuItem mnu_show_scatterplots	= new ToolStripMenuItem("Show Correlation Plots") ; 

			/// 
			/// If aligned to a mass tag database then show pertinent options
			/// 
			if (mobjAnalysis.PeakMatchedToMassTagDB)
			{
				ToolStripMenuItem mnu_columns_cluster_mass_calibrated   = new ToolStripMenuItem("Calibrated Cluster Mass ");
				ToolStripMenuItem mnu_columns_cluster_net_aligned       = new ToolStripMenuItem("Aligned Cluster NET");

				mnu_columns_cluster_mass_calibrated.Checked  = mbln_show_cluster_calibrated_mass ; 
				mnu_columns_cluster_net_aligned.Checked		 = mbln_show_cluster_aligned_net ; 
				mnu_columns_cluster_mass_calibrated.Click	+= new EventHandler(mnu_columns_cluster_mass_calibrated_Click);
				mnu_columns_cluster_net_aligned.Click		+= new EventHandler(mnu_columns_cluster_net_aligned_Click);					
				mnu_columns_mass_tags.Checked	             = mblnShowMassTags ; 
				mnu_columns_mass_tags.Click		            += new EventHandler(mnu_columns_mass_tags_Click);
				mnu_columns.DropDownItems.AddRange(new ToolStripMenuItem[] {	
                                                                    mnu_columns_mass_tags,
																	mnu_columns_cluster_mass_calibrated,
																	mnu_columns_cluster_net_aligned});
                mnu_columns.DropDownItems.Add(new ToolStripSeparator());
			}
				
			mnu_save_grid.Click		 += new EventHandler(mnu_save_grid_Click);
			//mnu_show_alignment.Click += new EventHandler(mnu_show_alignment_Click);
				

			// If show normaized was checked
			if (mbln_show_normalized)
				mnu_show_normalized.Checked  = true ; 				
			if (mobjAnalysis.UMCData.mobjClusterData.IsDataNormalized == true)
				mnu_show_normalized.Click	+= new EventHandler(mnu_show_normalized_Click);				
			mnu_expression.Click		    += new EventHandler(mnu_expression_Click);
			mnu_normalize.Click			    += new EventHandler(mnu_normalize_Click);			
			mnu_dataSummary.Click		    += new EventHandler(mnu_dataSummary_Click);
			mnu_show_scatterplots.Click     += new EventHandler(mnu_show_scatterplots_Click);
				
			ToolStripMenuItem mnu_columns_all               = new ToolStripMenuItem("All");
			//ToolStripMenuItem mnu_columns_sep               = new ToolStripMenuItem("---------");
			ToolStripMenuItem mnu_columns_mass              = new ToolStripMenuItem("Mass");
			ToolStripMenuItem mnu_columns_scans			    = new ToolStripMenuItem("Scan");
			ToolStripMenuItem mnu_columns_aligned_scan	    = new ToolStripMenuItem("Aligned Scan");
			ToolStripMenuItem mnu_columns_umc_index		    = new ToolStripMenuItem("UMC Index");
			ToolStripMenuItem mnu_columns_ttest			    = new ToolStripMenuItem("TTest");
			ToolStripMenuItem mnu_columns_calibrated_mass   = new ToolStripMenuItem("Calibrated Mass");
            

			mnu_columns_all.Checked				= mbln_show_all_columns ; 
			mnu_columns_mass.Checked			= mbln_show_mass_columns ; 
			mnu_columns_calibrated_mass.Checked = mbln_show_calibrated_mass_columns ; 
			mnu_columns_scans.Checked			= mbln_show_scan_columns ; 
			mnu_columns_aligned_scan.Checked	= mbln_show_aligned_scan_columns ; 
			mnu_columns_umc_index.Checked		= mbln_show_umc_index_columns ; 
			mnu_columns_ttest.Checked			= mbln_show_ttest_column ; 

			mnu_columns_all.Click				+=	new EventHandler(mnu_columns_all_Click);
			mnu_columns_mass.Click				+=	new EventHandler(mnu_columns_mass_Click);
			mnu_columns_scans.Click				+=	new EventHandler(mnu_columns_scans_Click);
			mnu_columns_ttest.Click				+=	new EventHandler(mnu_columns_ttest_Click);
			mnu_diff_abundance.Click			+=	new EventHandler(mnu_diff_abundance_Click);
			mnu_columns_aligned_scan.Click		+=	new EventHandler(mnu_columns_aligned_scan_Click); 
			mnu_columns_umc_index.Click			+=	new EventHandler(mnu_columns_umc_index_Click);
			mnu_columns_calibrated_mass.Click	+=	new EventHandler(mnu_columns_calibrated_mass_Click);
				
			 

            /// 
            /// TTest 
            /// 
			if (mbln_ttest_performed)
			{
				if (mblnPeakMatched)
					mnu_columns.DropDownItems.Add(mnu_columns_mass_tags);
                mnu_columns.DropDownItems.AddRange(new ToolStripMenuItem[]{mnu_columns_all,
																 //mnu_columns_sep, 
																 mnu_columns_mass,
																 mnu_columns_calibrated_mass, 
																 mnu_columns_scans,
																 mnu_columns_aligned_scan, 
																 mnu_columns_umc_index,
																 mnu_columns_ttest																 
																});
			}
			else
			{
				if (mblnPeakMatched)
                    mnu_columns.DropDownItems.Add(mnu_columns_mass_tags);
                mnu_columns.DropDownItems.AddRange(new ToolStripMenuItem[]{mnu_columns_all, 
																 //mnu_columns_sep, 
																 mnu_columns_mass,
																 mnu_columns_calibrated_mass,
																 mnu_columns_scans,
																 mnu_columns_aligned_scan, 
																 mnu_columns_umc_index});
			}
						
			/// 
            /// Create the popup menu object
            /// 
            ContextMenuStrip cntxtMenu = new ContextMenuStrip();            
				
			// Define the list of menu commands
            if (!mobjAnalysis.UMCData.mobjClusterData.IsDataNormalized)
            {
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[]{mnu_columns,
															   mnu_normalize, 
															   mnu_diff_abundance,
															   mnu_cluster,
															   mnu_align,
                                                               mnu_save_grid
                });

                cntxtMenu.Items.Add(new ToolStripSeparator());
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[] {
                                                               mnu_expression, 
															   mnu_dataSummary,
															   mnu_show_scatterplots
															   //mnu_show_alignment
                });
            }
            else
            {
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[]{mnu_columns,
															   mnu_normalize, 
															   mnu_diff_abundance,
															   mnu_cluster,
															   mnu_align,
                                                               mnu_save_grid
                });
                cntxtMenu.Items.Add(new ToolStripSeparator());
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[] {mnu_expression, 
															   mnu_show_normalized,															   
															   mnu_dataSummary, 
															   mnu_show_scatterplots
															 //  mnu_show_alignment
				});
            }
			
			if (mobj_contextMenu != null)
				mobj_contextMenu.Items.Clear();
			mobj_contextMenu = cntxtMenu;

			return cntxtMenu;
		}

		/// <summary>
		/// Handles mouse up event.  Shows data and chart operation menu if right clicked.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			if (e.Button == MouseButtons.Right)
			{				
				CreateContextMenu();
                mobj_contextMenu.Show(System.Windows.Forms.Cursor.Position);
                //mobj_contextMenu.Show();
				//mobj_contextMenu.Show(this, new Point(e.X, e.Y));
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
                mobjAnalysis = null;
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
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region "Context Menu Event Handlers"

        /// <summary>
        /// Handles when the user clicks to toggle showing mass tags or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void mnu_columns_mass_tags_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
                return;

            item.Checked = (item.Checked == false);

            if (item.Checked == true)
            {
                mblnShowMassTags = true;
                AddClusterToTable();
            }
            else
            {
                mblnShowMassTags = false;
                AddClusterToTable();
            }
		}

		private void mnu_columns_all_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem cmd = sender as ToolStripMenuItem ;
			cmd.Checked = !cmd.Checked ; 
			if (cmd.Checked)
			{
				bool need_update = false ; 
				if (!mbln_show_all_columns || !mbln_show_mass_columns || !mbln_show_scan_columns || !mbln_show_aligned_scan_columns
                    || !mbln_show_umc_index_columns || !mbln_show_calibrated_mass_columns)
				{
					need_update = true ; 
					mbln_show_all_columns = true ; 
					mbln_show_mass_columns = true ;
                    mbln_show_calibrated_mass_columns = true;
					mbln_show_scan_columns = true ; 
					mbln_show_aligned_scan_columns = true ; 
					mbln_show_umc_index_columns = true ; 
					if (mbln_ttest_performed)
						mbln_show_ttest_column = true ; 
					else
						mbln_show_ttest_column = false ; 
				}

				if (need_update)
				{
					AddClusterToTable() ; 
				}
			}
			else
			{
				//mbln_show_all_columns = false ;
                bool need_update = false;
                if (mbln_show_all_columns || mbln_show_mass_columns || mbln_show_scan_columns || mbln_show_aligned_scan_columns
                    || mbln_show_umc_index_columns || mbln_show_calibrated_mass_columns)
                {
                    need_update = true;
                    mbln_show_all_columns = false;
                    mbln_show_mass_columns = false;
                    mbln_show_calibrated_mass_columns = false;
                    mbln_show_scan_columns = false;
                    mbln_show_aligned_scan_columns = false;
                    mbln_show_umc_index_columns = false;
                    mbln_show_ttest_column = false;
                }
                if (need_update)
                {
                    AddClusterToTable();
                }
			}
		}

		private void mnu_columns_mass_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ;
			if (!cmd.Checked)
				mbln_show_all_columns = false ; 
			mbln_show_mass_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_calibrated_mass_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ;
			if (!cmd.Checked)
				mbln_show_all_columns = false ; 
			mbln_show_calibrated_mass_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_scans_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			if (!cmd.Checked)
				mbln_show_all_columns = false ; 
			mbln_show_scan_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_aligned_scan_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			if (!cmd.Checked)
				mbln_show_all_columns = false ; 
			mbln_show_aligned_scan_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_umc_index_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			if (!cmd.Checked)
				mbln_show_all_columns = false ; 
			mbln_show_umc_index_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			AddClusterToTable() ; 
		}
		private void mnu_columns_ttest_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			if (!cmd.Checked)
				mbln_show_all_columns = false ; 
			mbln_show_ttest_column = cmd.Checked ; 
			// get the name of the table from the current table. :)
			AddClusterToTable() ; 
		}

		private void mnu_normalize_Click(object sender, EventArgs e)
		{
			clsNormalize objNormalizer = new clsNormalize() ; 
			objNormalizer.NormalizeData(mobjAnalysis) ; 
		}
         
		private void mnu_save_grid_Click(object sender, EventArgs e)
		{
			try 
			{
				System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
				fileDialog.AddExtension = true;
				fileDialog.CheckPathExists = true;
				fileDialog.DefaultExt = "*.csv";
				fileDialog.DereferenceLinks = true;
				fileDialog.ValidateNames = true;
				fileDialog.Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt" ;
				fileDialog.OverwritePrompt = true;
				fileDialog.FilterIndex = 1;
				if (fileDialog.ShowDialog() != DialogResult.OK) 
				{
					return;
				}

				string delimiter = "," ; 

				switch (fileDialog.FilterIndex)
				{
					case 1:
						delimiter = "," ; 
						break ; 
					case 2:
						delimiter = "\t" ; 
						break  ;
					default:
						break ; 
				}


				// now go through each row and save it. 
				DataTable tb = (DataTable) DataSource ; 
				
				int num_rows = tb.Rows.Count ; 

				TextWriter writer = new StreamWriter(fileDialog.FileName);

				// first write out the column names. 
				for (int col_num = 0 ; col_num < tb.Columns.Count ; col_num++)
				{
					writer.Write(tb.Columns[col_num].ColumnName) ; 
					if (col_num != tb.Columns.Count - 1)
						writer.Write(delimiter) ;
					else
						writer.Write("\n")  ;
				}

				for (int row_num = 0 ; row_num < num_rows ; row_num++)
				{
					DataRow row = tb.Rows[row_num] ; 
					for (int col_num = 0 ; col_num < tb.Columns.Count ; col_num++)
					{
						if (row[tb.Columns[col_num].ColumnName] != null && row[tb.Columns[col_num].ColumnName] != DBNull.Value)
						{
							string data_val = Convert.ToString(row[tb.Columns[col_num].ColumnName]) ; 
							if (data_val.IndexOf(delimiter) >= 0)
							{
								data_val = "\"" + data_val + "\"" ; 
							}
							writer.Write(data_val) ; 
						}
						if (col_num != tb.Columns.Count - 1)
							writer.Write(delimiter) ;
						else
							writer.Write("\n")  ;
					}
				}
				writer.Close();
				
			}
			catch (Exception ex) 
			{
				MessageBox.Show("Save failed: " + ex.Message);
			}

		}

		private void mnu_expression_Click(object sender, EventArgs e)
		{
			if (mevntExpressionPlotOpenClicked != null)
				mevntExpressionPlotOpenClicked() ; 
		}

		private void mnu_show_normalized_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			mbln_show_normalized = cmd.Checked ; 
			AddClusterToTable() ; 
		}
		private void mnu_diff_abundance_Click(object sender, EventArgs e)
		{
		}
		private void mnu_columns_cluster_mass_calibrated_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			mbln_show_cluster_calibrated_mass = cmd.Checked ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_cluster_net_aligned_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			mbln_show_cluster_aligned_net = cmd.Checked ; 
			AddClusterToTable() ; 
		}

		private void mnu_show_heatmap_Click(object sender, EventArgs e)
		{
			if (mevntHeatMapClicked != null)
			{
				mevntHeatMapClicked() ; 
			}
		}

		private void mnu_show_scatterplots_Click(object sender, EventArgs e)
		{
			if (mevntScatterPlotOpenClicked != null)
			{
				mevntScatterPlotOpenClicked() ; 
			}
		}

		/// <summary>
		/// Event handler to show a new alignment chart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu_show_alignment_Click(object sender, EventArgs e)
		{			
//			mobj_mediator.RaiseShowAlignmentFunctions(this, mobj_clusters);			
		}

		/// <summary>
		/// Raises event to display data summary.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu_dataSummary_Click(object sender, EventArgs e)
		{
			if (mevntDataSummaryOpenClicked != null)
			{
				mevntDataSummaryOpenClicked() ; 
			}
		}
		
		#endregion

		private void mnu_reorderColumns_Click(object sender, EventArgs e)
		{
			frmReorderDataTable table = new frmReorderDataTable();			
			table.DataGrid	= this;
			table.ShowDialog();
		}
	}
}
