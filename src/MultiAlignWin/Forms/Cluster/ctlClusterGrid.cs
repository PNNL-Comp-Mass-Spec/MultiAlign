using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Collections.Generic;

using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data.Analysis;
using MultiAlignEngine.PeakMatching;

using MultiAlignWin.Forms.Filters;
using PNNLProteomics.Filters;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for ctlClusterGrid.
	/// </summary>
	public class ctlClusterGrid : ECsoft.Windows.Forms.DataGridEx 
    {

        #region Members

        public delegate void DelegatePeptidesMatchedToProteins(object sender, Dictionary<string, List<string>> proteins);
        public event DelegatePeptidesMatchedToProteins ProteinsMapped;

        /// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private int mint_numberOfRows = 0;
        private bool mbool_proteinsMapped               = false;
		private bool mbool_show_cluster_aligned_net     = false ; 
		private bool mbool_show_cluster_calibrated_mass = false ; 
		private bool mbool_show_all_columns             = true ; 
		private bool mbool_show_calibrated_mass_columns = false ; 
		private bool mbool_show_mass_columns            = true ;
        private bool mbool_show_driftTime_columns       = false;
		private bool mbool_show_scan_columns            = true ; 
		private bool mbool_show_aligned_scan_columns    = true ; 
		private bool mbool_show_umc_index_columns       = true ; 
		private bool mbool_ShowMassTags                 = true ;
        private bool mbool_showCMCAbundances            = true;
		private bool mbool_PeakMatched                  = false ; 
		private bool mbool_ttest_performed              = false ; 
		private bool mbool_show_ttest_column            = false ;
        private bool mbool_show_normalized = false;

        #region Header Constants
        private const string mstring_umc_rep_mass_col   = "Mass" ; 
		private const string mstring_umc_rep_net_col    = "NET" ; 
		
		private const string mstring_umc_rep_mass_calib_col  = "Calibrated Mass" ; 
		private const string mstring_umc_rep_net_aligned_col = "Aligned NET" ;

        private const string mstring_umc_index_col = "Row ID";
        private const string mstring_umc_rep_size_col = "Cluster Size";
        private const string mstring_umc_spectral_count = "Spectral Count"; 
		private const string mstring_peptide_col        = "Peptide" ; 

		private const string mstring_mass_tag_id_col = "Mass Tags" ;
        private const string mstring_mass_tag_net_col = "Mass Tag NET";
        private const string mstring_mass_tag_mass_col = "Mass Tag Mass"; 
		private const string mstring_mass_tag_F_CS1 = "Charge 1 F Score" ; 
		private const string mstring_mass_tag_F_CS2 = "Charge 2 F Score" ; 
		private const string mstring_mass_tag_F_CS3 = "Charge 3 F Score" ;

        private const string mstring_mass_tag_xcorr_col = "Mass Tag Xcorr"; 
		private const string mstring_mass_tag_modification_col = "Modifications" ; 
		private const string mstring_mass_tag_modification_count_col = "Mod count" ; 
		private const string mstring_protein_col = "Protein" ; 
		private const string mstring_proteinid_col = "RefID" ;

        private const string CONST_CHARGE_ABUNDANCE_START = "CMC-Abundance";

		private const string mstring_mass_colum = "Mass" ; 
		private const string mstring_calibrated_mass_colum = "Calibrated_mass" ; 
		private const string mstring_scan_colum = "Scan" ;
        private const string mstring_aligned_scan_colum = "Aligned Scan";
        private const string mstring_umc_index_column = "UMC index";
        private const string mstring_driftTime_column = "Drift Time";
        #endregion

        private string mstring_ttest_col_name = "T-Test" ; 
		private string mstring_empty = "" ;

        private ContextMenuStrip mobj_contextMenu;

		private clsTTestVals [] marr_ttests = null ; 
		private clsMultiAlignAnalysis mobjAnalysis ; 


		public event MethodInvoker mevntExpressionPlotOpenClicked ; 
		public event MethodInvoker mevntDataSummaryOpenClicked ; 
		public event MethodInvoker mevntScatterPlotOpenClicked ; 
		public event MethodInvoker mevntHeatMapClicked ;

        public delegate void SaveTableDelegate(object sender, EventArgs e);
        public SaveTableDelegate SaveTableDelegateMethod;
        public SaveTableDelegate SaveDanteDelegate;
        public SaveTableDelegate SaveAsSQLiteDelegate;
        public SaveTableDelegate SaveAsIsDelegate;


        private List<IFilter<clsUMC>>       mlist_umcFilters;
        private List<IFilter<clsCluster>>   mlist_clusterFilters;
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterless contstructor.  No analysis associated if called.
        /// </summary>
        public ctlClusterGrid()
		{						
			InitializeComponent();		
			AllowUserToOrderColumns = true;

            SaveTableDelegateMethod = new SaveTableDelegate(mnu_save_grid_Click);
            SaveDanteDelegate       = new SaveTableDelegate(mnu_save_dante_Click);
            SaveAsSQLiteDelegate    = new SaveTableDelegate(mnu_save_sqlite_Click);
            SaveAsIsDelegate        = new SaveTableDelegate(mnu_save_grid_Click);

            mlist_clusterFilters    = new List<IFilter<clsCluster>>();
            mlist_umcFilters        = new List<IFilter<clsUMC>>();
        }
        /// <summary>
        /// Constructor that takes an analysis object and an analysis name 
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="name"></param>
        public ctlClusterGrid(clsMultiAlignAnalysis analysis, string name)
        {
            InitializeComponent();            
            Analysis                = analysis;
            AllowUserToOrderColumns = true;                   
        }
        #endregion
		

        /// <summary>
        /// Creates the data table from a given analysis object.
        /// </summary>
		private void AddClusterToTable()
		{                        
            mint_numberOfRows = 0;

			// Create a new table.
			DataTable table = new DataTable("Analysis") ; 

			table.Columns.Add(mstring_umc_index_col, typeof(int)) ; 
		table.Columns.Add(mstring_umc_rep_size_col, typeof(int)) ; 
			table.Columns.Add(mstring_umc_rep_mass_col, typeof(double)) ;
            

            Color[] colors = new Color[] { Color.Plum, Color.LightGray, Color.Lime };
			
            /// 
            /// Calibrated Mass
            /// 
            if (mbool_show_cluster_calibrated_mass)
				table.Columns.Add(mstring_umc_rep_mass_calib_col, typeof(double)) ; 
			
            /// 
            /// UMC - Representative Net
            /// 
            table.Columns.Add(mstring_umc_rep_net_col, typeof(double)) ; 
			
            /// 
            /// Cluster Aligned net
            /// 
            if (mbool_show_cluster_aligned_net)
				table.Columns.Add(mstring_umc_rep_net_aligned_col, typeof(double)) ; 

            /// 
            /// T-Test 
            /// 
			if(mbool_show_ttest_column)
			{
				table.Columns.Add(mstring_ttest_col_name, typeof(double)) ; 
			}

            /// 
            /// Dont set the data source to anything so we can use the data table to our advantage.
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
			if (mbool_ShowMassTags && mobjAnalysis.PeakMatchedToMassTagDB)
			{
				table.Columns.Add(mstring_peptide_col) ; 
				table.Columns.Add(mstring_protein_col) ; 
				table.Columns.Add(mstring_proteinid_col, typeof(int)) ; 
				table.Columns.Add(mstring_mass_tag_id_col, typeof(int)) ; 
				table.Columns.Add(mstring_mass_tag_mass_col, typeof(double)) ; 
				table.Columns.Add(mstring_mass_tag_net_col, typeof(double)) ; 
				table.Columns.Add(mstring_mass_tag_xcorr_col, typeof(double)) ; 
				table.Columns.Add(mstring_mass_tag_modification_count_col, typeof(short)) ; 
				table.Columns.Add(mstring_mass_tag_modification_col) ; 
				table.Columns.Add(mstring_mass_tag_F_CS1) ; 
				table.Columns.Add(mstring_mass_tag_F_CS2) ; 
				table.Columns.Add(mstring_mass_tag_F_CS3) ;
                

                /// 
                /// If SMART was used, then we want to pull the data out of the analysis object
                /// 
                if (mobjAnalysis.UseSMART == true)
                {
                    table.Columns.Add("SMART Score");
                    table.Columns.Add("SMART Specificity");                    
                }				
			}
            
            Dictionary<string, List<string>> proteinToPeptideMatches = new Dictionary<string,List<string>>();
			try
			{
                /// 
                /// Each dataset starts with two columns - This is for the abundance value.
                /// Then we figure out what other columns each dataset needs.  Here
                /// we see if we haev enough for all the calibrated masses etc.
                /// 
				int num_columns_per_dataset = 3 ; 
				if (mbool_show_mass_columns)
				{
					num_columns_per_dataset++ ; 
				}
				if (mbool_show_calibrated_mass_columns)
				{
					num_columns_per_dataset++ ; 
				}
				if (mbool_show_scan_columns)
                {
                    num_columns_per_dataset++;
                    num_columns_per_dataset++;
                    num_columns_per_dataset++;
                    num_columns_per_dataset++; 
				}
				if (mbool_show_aligned_scan_columns)
				{
					num_columns_per_dataset++ ; 
				}
				if (mbool_show_umc_index_columns)
				{
					num_columns_per_dataset++ ; 
				}

                if (mbool_show_driftTime_columns)
                    num_columns_per_dataset++;

                /// 
                /// Add another column for the spectral count
                /// 
                num_columns_per_dataset++;

                /// 
                /// This is to output abundances for each charge state up to N charge states.
                /// N is at most, 10 (unless changed in the engine).  Otherwise its the 
                /// largest number of charge states found in the data.
                /// 
                if (mbool_showCMCAbundances)
                    num_columns_per_dataset += mobjAnalysis.UMCData.HighestChargeState;


				int start_data_column_num   = table.Columns.Count ; 
				int num_datasets            = mobjAnalysis.UMCData.NumDatasets ; 
				int num_clusters            = mobjAnalysis.UMCData.mobjClusterData.NumClusters  ; 

                

				ArrayList arr_datasets_to_show = new ArrayList() ; 

                /// 
                /// For each dataset we need to add columns to the table, so when we construct                
                /// the rows we have enough space for each item.
                /// 
				for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
				{

					string file_name = mobjAnalysis.UMCData.DatasetName[dataset_num] ;
					int index        = file_name.LastIndexOf("\\") ;
					string col_name  = file_name.Substring(index+1) ;
                    col_name = "_" + dataset_num.ToString();
                                        
					arr_datasets_to_show.Add(dataset_num) ;

                    table.Columns.Add("Dataset Id" + col_name, typeof(int));

                    if (mbool_show_mass_columns)
                    {
                        table.Columns.Add(mstring_mass_colum + col_name, typeof(double));
                    }
					if (mbool_show_calibrated_mass_columns)					
                    {
                        table.Columns.Add(mstring_calibrated_mass_colum + col_name, typeof(double));
					}
					if (mbool_show_scan_columns)
                    {
                        table.Columns.Add("NET" + col_name, typeof(double));
                        table.Columns.Add(mstring_scan_colum + col_name, typeof(double));
                        table.Columns.Add("ScanStart" + col_name, typeof(double));
                        table.Columns.Add("ScanEnd" + col_name, typeof(double));
					}
					
					if (mbool_show_aligned_scan_columns)					
					{
                        table.Columns.Add(mstring_aligned_scan_colum + col_name, typeof(double));
					}
					
					if (mbool_show_umc_index_columns)					
					{
                        table.Columns.Add(mstring_umc_index_column + col_name, typeof(long));
					}

                    if (mbool_show_driftTime_columns)
                    {
                        table.Columns.Add(mstring_driftTime_column + col_name, typeof(double));
                    }
					                    
                    /// 
                    /// This is for the abundance value 
                    /// 
                    table.Columns.Add("Abundance-Sum" + col_name, typeof(double));
                    table.Columns.Add("Abundance-Max" + col_name, typeof(double));

                    /// 
                    /// Spectral count - # of peaks 
                    /// 
                    table.Columns.Add(mstring_umc_spectral_count +  col_name, typeof(string));

                    /// 
                    /// Charge state abundances
                    /// 
                    if (mbool_showCMCAbundances)
                    {
                        for (int j = 0; j < mobjAnalysis.UMCData.HighestChargeState; j++)
                        {
                            int chargeState = j + 1;
                            table.Columns.Add(string.Format("{0}_CS_{1}" + col_name, CONST_CHARGE_ABUNDANCE_START,
                                                                                chargeState),
                                                                                typeof(long));
                            table.Columns.Add(string.Format("{0}_mz_{1}" + col_name, CONST_CHARGE_ABUNDANCE_START,
                                                                                chargeState),
                                                                                typeof(double));                            
                        }
                    }
				}

                

				int num_datasets_to_show                        = arr_datasets_to_show.Count ; 
				int num_rows_so_far                             = 0 ; 
				clsUMC [] arrUMCs                               = mobjAnalysis.UMCData.marr_umcs ; 
				int [] arrClusterMainMemberIndex                = mobjAnalysis.UMCData.mobjClusterData.marrClusterMainMemberIndex ; 
				double [] arrClusterMemberIntensity             = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensity ; 
				double [] arrClusterMemberNormalizedIntensity   = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensityNormalized ; 
				clsPeakMatchingResults.clsPeakMatchingTriplet [] arrPeakMatchingTriplets = null ;
				clsProtein [] arrPeakMatchingProteins = null ; 
				clsMassTag [] arrPeakMatchingMassTags = null ; 

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

                /// ////////////////////////////////////////////////////////////////////////////// 
                /// Now we add the data 
                /// ////////////////////////////////////////////////////////////////////////////// 
				while(clusterNum < num_clusters)
				{
					clsCluster cluster  = mobjAnalysis.UMCData.mobjClusterData.GetCluster(clusterNum);

                    /// 
                    /// Check to make sure the cluster passes the filter.
                    /// 
                    bool passed = true;
                    foreach (IFilter<clsCluster> filter in mlist_clusterFilters)
                    {
                        if (filter.DoesPassFilter(cluster) == false)
                        {
                            passed = false;
                            break;
                        }
                    }
                    if (passed == false)
                    {
                        clusterNum++;
                        continue;
                    }


					DataRow row         = table.NewRow() ;
					row[0]              = Convert.ToString(clusterNum + 1) ; 
					row[2]              = Convert.ToString(cluster.mdouble_mass) ; 
					int num_column      = 3;

					if(mbool_show_cluster_calibrated_mass)
						row[num_column++]   = Convert.ToString(cluster.mdouble_mass_calibrated) ; 

					row[num_column++]       = Convert.ToString(cluster.mdouble_net) ; 

					if (mbool_show_cluster_aligned_net)
						row[num_column++] = Convert.ToString(cluster.mdouble_aligned_net) ; 

					if(mbool_show_ttest_column)
					{
						if (marr_ttests[clusterNum] != null)
						{
							clsTTestVals ttest_val = marr_ttests[clusterNum] ; 
							if (ttest_val != null)
							{
								row[num_column] = Convert.ToString(ttest_val.mdouble_pvalue) ; 
							}
							else
							{
								row[num_column] = DBNull.Value ; 
							}
						}
					}

					int num_non_empty = 0 ;                     
                    /// ////////////////////////////////////////////////////////////////////////////// 
                    /// Add Dataset specific information to each respective column
                    /// //////////////////////////////////////////////////////////////////////////////                     
					for (int col_num = 0 ; col_num < num_datasets_to_show * num_columns_per_dataset ; col_num++)
					{
						int dataset_index   = col_num / num_columns_per_dataset ; 
						int dataset_num     = (int) arr_datasets_to_show[dataset_index] ; 
						int pt_index        = clusterNum*num_datasets + dataset_num ; 
						int index           = arrClusterMainMemberIndex[pt_index] ; 
						clsUMC umc          = null ;

                        /// ////////////////////////////////////////////////////////////////////////////// 
                        /// Find the UMC so we can grab data from it to show
                        /// ////////////////////////////////////////////////////////////////////////////// 
						if (index != -1)
						{
							umc = arrUMCs[index] ; 
							num_non_empty++ ;
						}

                        /// 
                        /// Check to make sure the UMC passes the filter.
                        /// 
                        passed = true;
                        foreach (IFilter<clsUMC> filter in mlist_umcFilters)
                        {
                            if (filter.DoesPassFilter(umc) == false)
                            {
                                passed = false;
                                break;
                            }
                        }
                        if (passed == false)
                        {
                            col_num += num_columns_per_dataset;
                            continue;
                        }
                        if (umc != null)
                        {
                            row[start_data_column_num + col_num] = umc.DatasetId;
                        }
                        else
                        {
                            row[start_data_column_num + col_num] = DBNull.Value; 
                        }
                        col_num++;

						if (mbool_show_mass_columns)
						{
							if (umc != null)
								row[start_data_column_num+col_num] = Convert.ToString(umc.mdouble_mono_mass) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}
						if (mbool_show_calibrated_mass_columns)
						{
							if (umc != null)
								row[start_data_column_num + col_num] = Convert.ToString(umc.mdouble_mono_mass_calibrated) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}

						if (mbool_show_scan_columns)
						{
                            if (umc != null)
                            {
                                row[start_data_column_num + col_num] = Convert.ToString(umc.mdouble_net);
                                col_num++; 
                                row[start_data_column_num + col_num] = Convert.ToString(umc.mint_scan);
                                col_num++;
                                row[start_data_column_num + col_num] = Convert.ToString(umc.mint_start_scan);
                                col_num++;
                                row[start_data_column_num + col_num] = Convert.ToString(umc.mint_end_scan);
                                col_num++;                                 
                            }
                            else
                            {
                                row[start_data_column_num + col_num] = DBNull.Value;
                                col_num++;
                                row[start_data_column_num + col_num] = DBNull.Value;
                                col_num++;
                                row[start_data_column_num + col_num] = DBNull.Value;
                                col_num++;
                                row[start_data_column_num + col_num] = DBNull.Value;
                                col_num++;                                 
                            }

						}
						if (mbool_show_aligned_scan_columns)
						{
							if (umc != null)
								row[start_data_column_num+col_num] = Convert.ToString(umc.mint_scan_aligned) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ; 
						}
						if (mbool_show_umc_index_columns)
						{
							if (umc != null) 
								row[start_data_column_num+col_num] = Convert.ToString(umc.mint_umc_index) ; 
							else
								row[start_data_column_num+col_num] = DBNull.Value ; 
							col_num++ ;
                        }


                        if (mbool_show_driftTime_columns)
                        {
                            if (umc != null)
                                row[start_data_column_num + col_num] = umc.DriftTime;
                            else
                                row[start_data_column_num + col_num] = DBNull.Value;
                            col_num++;
                        }

                        /// //////////////////////////////////////////////////////////////////////////////
                        /// Abundance Information
                        /// //////////////////////////////////////////////////////////////////////////////
                        if (umc != null)
                        {
                            double intensity = 0;

                            row[start_data_column_num + col_num] = umc.AbundanceSum;
                            col_num++; 
                            row[start_data_column_num + col_num] = umc.AbundanceMax;                            
                        }
                        else
                        {
                            // missing value set as 0
                            row[start_data_column_num + col_num] = DBNull.Value;
                            col_num++;
                            row[start_data_column_num + col_num] = DBNull.Value;
                        }



                        col_num++;
                        if (umc != null)
                        {
                            /// //////////////////////////////////////////////////////////////////////////////
                            /// Add spectral count information
                            /// //////////////////////////////////////////////////////////////////////////////                        
                            row[start_data_column_num + col_num] = umc.SpectralCount;
                        }
                        else
                        {
                            row[start_data_column_num + col_num] = DBNull.Value;
                        }
                        /// //////////////////////////////////////////////////////////////////////////////
                        /// Do we show the abundances for each charge state?
                        /// //////////////////////////////////////////////////////////////////////////////
                        if (mbool_showCMCAbundances)
                        {

                            double totalAbundance = 0;

                            if (umc != null)
                            {
                                totalAbundance = umc.mdouble_sum_abundance;
                            }

                            /// 
                            /// For each charge state, we should have a column of data available.
                            /// 
                            for(int j = 0; j < mobjAnalysis.UMCData.HighestChargeState; j++)
                            {
                                double mzValue = 0;
                                col_num++;
                                if (umc != null)
                                {                                    
                                    row[start_data_column_num + col_num] = umc.marray_chargeStatesAbundances[j];
                                }
                                else
                                {
                                    row[start_data_column_num + col_num] = DBNull.Value;
                                }

                                col_num++;
                                if (umc != null)
                                {
                                    if (umc.marray_chargeStatesAbundances[j] > 0)
                                    {
                                        int chargeState = j + 1;
                                        double monoMass = umc.Mass;
                                        double charge = Convert.ToDouble(chargeState);
                                        mzValue = (monoMass + charge) / charge;                             
                                    }
                                    row[start_data_column_num + col_num] = mzValue;
                                }
                                else
                                {
                                    row[start_data_column_num + col_num] = DBNull.Value;
                                }
                            }
                        }
					}


					row[1] = Convert.ToString(num_non_empty) ;
                    ///  ////////////////////////////////////////////////////////////////////////////// 
                    /// AddMassTags to Row
					///     if it is peakmatched, and show mass tags is enabled, everything shows. 
                    ///  ////////////////////////////////////////////////////////////////////////////// 
					if(mbool_ShowMassTags && mobjAnalysis.PeakMatchedToMassTagDB) 
					{
						if (arrPeakMatchingTriplets != null && 
							currentPeakMatchNum < arrPeakMatchingTriplets.Length 
							&& arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
						{
							///
                            /// So this peakmatchtriplet corresponds to the current cluster. 
							/// Lets display it. 
                            /// 
							int current_column      = start_mass_tag_column ; 
							clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum] ; 
							clsMassTag massTag      = arrPeakMatchingMassTags[triplet.mintMassTagIndex] ; 
							clsProtein protein      = arrPeakMatchingProteins[triplet.mintProteinIndex] ; 
							row[current_column++]   = massTag.mstrPeptide ; 
							row[current_column++]   = protein.mstrProteinName ; 
							row[current_column++]   = protein.mintRefID ; 
							row[current_column++]   = massTag.mintMassTagId ; 
							row[current_column++]   = massTag.mdblMonoMass ; 
							row[current_column++]   = massTag.mdblAvgGANET ; 
							row[current_column++]   = massTag.mdblHighXCorr ; 
							row[current_column++]   = massTag.mshortModCount ; 
							row[current_column++]   = massTag.mstrModification ; 
							row[current_column++]   = massTag.mfltAvgFCS1 ; 
							row[current_column++]   = massTag.mfltAvgFCS2 ;
                            row[current_column++]   = massTag.mfltAvgFCS3;

                            if (mobjAnalysis.UseSMART)
                            {
                                /// 
                                /// See if a SMART score exists
                                /// 
                                List<PNNLProteomics.SMART.classSMARTProbabilityResult> smartScores = null;
                                smartScores = mobjAnalysis.SMARTResults.GetResultFromUMCIndex(triplet.mintFeatureIndex);
                                if (smartScores != null)
                                {                                     
                                    /// 
                                    /// Then pull out the SMART score that matches for this triplet Mass Tag
                                    /// 
                                    PNNLProteomics.SMART.classSMARTProbabilityResult finalResult = null;
                                    foreach (PNNLProteomics.SMART.classSMARTProbabilityResult score in smartScores)
                                    {                                        
                                        if (score.MassTagID == massTag.Id)
                                        {
                                            finalResult = score;
                                            break;
                                        }
                                    }
                                    /// 
                                    /// If we have a final result, then we have a smart score for this MTID for the matched UMC.
                                    /// 
                                    if (finalResult != null)
                                        row[current_column++] = finalResult.Score;
                                    else
                                        row[current_column++] = DBNull.Value;                                    
                                    /// 
                                    /// If we have a final result, then we have a smart score for this MTID for the matched UMC.
                                    /// 
                                    if (finalResult != null)
                                        row[current_column++] = finalResult.Specificity;
                                    else
                                        row[current_column++] = DBNull.Value;                                 
                                }
                            }

                            /// Map proteins to peptides 
                            if (proteinToPeptideMatches.ContainsKey(protein.mstrProteinName) == false)                            
                                proteinToPeptideMatches.Add(protein.mstrProteinName, new List<string>());    
                            
                            proteinToPeptideMatches[protein.mstrProteinName].Add(massTag.mstrPeptide);
                            
							currentPeakMatchNum++ ;
                            lastClusterNum = clusterNum; 
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
				
                /// 
                /// Finally, make this table the data source
                /// 
                DataSource = table ;
				AddGridStyle(table) ;

                mint_numberOfRows = table.Rows.Count;
			}
			catch (Exception ex)
			{				                
				DataSource = table ;                
			}

            /// 
            /// Map the protein names if they havent been already.
            /// 
            if (ProteinsMapped != null && mbool_proteinsMapped == false)
            {
                ProteinsMapped(this, proteinToPeptideMatches);
                mbool_proteinsMapped = true;
            }                               
		}
        /// <summary>
        /// Gets the number of rows displayed in the analysis.
        /// </summary>
        public long NumberOfRows
        {
            get
            {
                return mint_numberOfRows;
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
				colStyle.NullText = mstring_empty ;

                if (table.Columns[colNum].DataType == typeof(string))
                {
                    colStyle.Format = "s";
                }else
                {
                    colStyle.Format = "d";
                }
				myGridStyle.GridColumnStyles.Add(colStyle);
			}
			TableStyles.Add(myGridStyle);
        }
        /// <summary>
        /// Creates the context menu;
        /// </summary>
        /// <returns></returns>
        public ContextMenuStrip CreateContextMenu()
        {
            /// 
            /// Create all of the context menu controls.
            /// 
            ToolStripMenuItem mnu_columns           = new ToolStripMenuItem("Show Columns");
            ToolStripMenuItem mnu_normalize         = new ToolStripMenuItem("Normalize");
            ToolStripMenuItem mnu_diff_abundance    = new ToolStripMenuItem("Perform ttest");
            ToolStripMenuItem mnu_cluster           = new ToolStripMenuItem("Re-Cluster");
            ToolStripMenuItem mnu_align             = new ToolStripMenuItem("Re-Align");
            ToolStripMenuItem mnu_dataSummary       = new ToolStripMenuItem("Analysis Summary");
            ToolStripMenuItem mnu_columns_mass_tags = new ToolStripMenuItem("Mass Tags/Proteins");
            ToolStripMenuItem mnu_save_grid         = new ToolStripMenuItem("Save Table");
            ToolStripMenuItem mnu_save_dante        = new ToolStripMenuItem("For Dante");
            ToolStripMenuItem mnu_save_sqlite       = new ToolStripMenuItem("As SQLite");
            ToolStripMenuItem mnu_save_asis         = new ToolStripMenuItem("As Displayed");
            ToolStripMenuItem mnu_filters           = new ToolStripMenuItem("Filter");

            mnu_cluster.Enabled = false;
            mnu_align.Enabled   = false;

            /// 
            /// Charts 
            /// 
            ToolStripMenuItem mnu_expression        = new ToolStripMenuItem("Scatter Plots/Histograms");
            ToolStripMenuItem mnu_show_normalized   = new ToolStripMenuItem("Show Normalized Intensities");
            ToolStripMenuItem mnu_show_scatterplots = new ToolStripMenuItem("Show Correlation Plots");

            /// 
            /// If aligned to a mass tag database then show pertinent options
            /// 
            if (mobjAnalysis.PeakMatchedToMassTagDB)
            {
                ToolStripMenuItem mnu_columns_cluster_mass_calibrated   = new ToolStripMenuItem("Calibrated Cluster Mass ");
                ToolStripMenuItem mnu_columns_cluster_net_aligned       = new ToolStripMenuItem("Aligned Cluster NET");

                mnu_columns_cluster_mass_calibrated.Checked             = mbool_show_cluster_calibrated_mass;
                mnu_columns_cluster_net_aligned.Checked                 = mbool_show_cluster_aligned_net;
                mnu_columns_mass_tags.Checked                           = mbool_ShowMassTags;
                
                mnu_columns_cluster_mass_calibrated.Click               += new EventHandler(mnu_columns_cluster_mass_calibrated_Click);
                mnu_columns_cluster_net_aligned.Click                   += new EventHandler(mnu_columns_cluster_net_aligned_Click);
                mnu_columns_mass_tags.Click                             += new EventHandler(mnu_columns_mass_tags_Click);
                mnu_columns.DropDownItems.AddRange(new ToolStripMenuItem[] {	
                                                                    mnu_columns_mass_tags,
																	mnu_columns_cluster_mass_calibrated,
																	mnu_columns_cluster_net_aligned});
                mnu_columns.DropDownItems.Add(new ToolStripSeparator());
            }
            
            if (mbool_show_normalized)
                mnu_show_normalized.Checked = true;
            if (mobjAnalysis.UMCData.mobjClusterData.IsDataNormalized == true)
                mnu_show_normalized.Click   += new EventHandler(mnu_show_normalized_Click);
            
            /// 
            /// Save as is or Dante for exporting the data.
            /// 
            mnu_save_dante.Click            += new EventHandler(mnu_save_dante_Click);
            mnu_save_sqlite.Click           += new EventHandler(mnu_save_sqlite_Click);
            mnu_save_asis.Click             += new EventHandler(mnu_save_grid_Click);
            mnu_save_grid.DropDownItems.AddRange(new ToolStripItem[] { mnu_save_asis, mnu_save_dante, mnu_save_sqlite });


            mnu_expression.Click            += new EventHandler(mnu_expression_Click);
            mnu_normalize.Click             += new EventHandler(mnu_normalize_Click);
            mnu_dataSummary.Click           += new EventHandler(mnu_dataSummary_Click);
            mnu_show_scatterplots.Click     += new EventHandler(mnu_show_scatterplots_Click);

            ToolStripMenuItem mnu_columns_all               = new ToolStripMenuItem("All");
            ToolStripMenuItem mnu_columns_showCMC           = new ToolStripMenuItem("Abundances for each charge state (CMC)");
            ToolStripMenuItem mnu_columns_mass              = new ToolStripMenuItem("Mass");
            ToolStripMenuItem mnu_columns_scans             = new ToolStripMenuItem("Scan");
            ToolStripMenuItem mnu_columns_aligned_scan      = new ToolStripMenuItem("Aligned Scan");
            ToolStripMenuItem mnu_columns_umc_index         = new ToolStripMenuItem("UMC Index");
            ToolStripMenuItem mnu_columns_ttest             = new ToolStripMenuItem("TTest");
            ToolStripMenuItem mnu_columns_calibrated_mass   = new ToolStripMenuItem("Calibrated Mass");
            ToolStripMenuItem mnu_columns_driftTime = new ToolStripMenuItem("Drift Time");

            mnu_columns_all.Checked             = mbool_show_all_columns;
            mnu_columns_mass.Checked            = mbool_show_mass_columns;
            mnu_columns_calibrated_mass.Checked = mbool_show_calibrated_mass_columns;
            mnu_columns_scans.Checked           = mbool_show_scan_columns;
            mnu_columns_aligned_scan.Checked    = mbool_show_aligned_scan_columns;
            mnu_columns_umc_index.Checked       = mbool_show_umc_index_columns;
            mnu_columns_ttest.Checked           = mbool_show_ttest_column;
            mnu_columns_showCMC.Checked         = mbool_showCMCAbundances;
            mnu_columns_driftTime.Checked       = mbool_show_driftTime_columns;
            

            mnu_columns_showCMC.Click           += new EventHandler(mnu_columns_showCMC_Click);
            mnu_columns_all.Click               += new EventHandler(mnu_columns_all_Click);
            mnu_columns_mass.Click              += new EventHandler(mnu_columns_mass_Click);
            mnu_columns_scans.Click             += new EventHandler(mnu_columns_scans_Click);
            mnu_columns_ttest.Click             += new EventHandler(mnu_columns_ttest_Click);
            mnu_diff_abundance.Click            += new EventHandler(mnu_diff_abundance_Click);
            mnu_columns_aligned_scan.Click      += new EventHandler(mnu_columns_aligned_scan_Click);
            mnu_columns_umc_index.Click         += new EventHandler(mnu_columns_umc_index_Click);
            mnu_columns_calibrated_mass.Click   += new EventHandler(mnu_columns_calibrated_mass_Click);
            mnu_columns_driftTime.Click += new EventHandler(mnu_columns_driftTime_Click);


            /// 
            /// TTest 
            /// 
            if (mbool_ttest_performed)
            {
                if (mbool_PeakMatched)
                    mnu_columns.DropDownItems.Add(mnu_columns_mass_tags);
                mnu_columns.DropDownItems.AddRange(new ToolStripMenuItem[]{mnu_columns_all,																 
																 mnu_columns_mass,
																 mnu_columns_calibrated_mass, 
																 mnu_columns_scans,
																 mnu_columns_aligned_scan, 
																 mnu_columns_umc_index,
                                                                 mnu_columns_driftTime,
																 mnu_columns_ttest,
                                                                 mnu_columns_showCMC});																
            }
            else
            {
                if (mbool_PeakMatched)
                    mnu_columns.DropDownItems.Add(mnu_columns_mass_tags);
                mnu_columns.DropDownItems.AddRange(new ToolStripMenuItem[]{mnu_columns_all, 																 
																 mnu_columns_mass,
																 mnu_columns_calibrated_mass,
																 mnu_columns_scans,
																 mnu_columns_aligned_scan, 
																 mnu_columns_umc_index,
                                                                 mnu_columns_driftTime,
                                                                 mnu_columns_showCMC});
            }

            mnu_filters.Click += new EventHandler(mnu_filters_Click);

            /// 
            /// Create the popup menu object
            /// 
            ContextMenuStrip cntxtMenu = new ContextMenuStrip();



            /// 
            /// Define the list of menu commands
            /// 
            if (!mobjAnalysis.UMCData.mobjClusterData.IsDataNormalized)
            {
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[]{mnu_columns,
															   mnu_normalize, 
															   mnu_diff_abundance,
															   mnu_cluster,
															   mnu_align,
                                                               mnu_save_grid,
                                                               mnu_filters});

                cntxtMenu.Items.Add(new ToolStripSeparator());
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[] {
                                                               mnu_expression, 
															   mnu_dataSummary,
															   mnu_show_scatterplots});
            }
            else
            {
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[]{mnu_columns,
															   mnu_normalize, 
															   mnu_diff_abundance,
															   mnu_cluster,
															   mnu_align,
                                                               mnu_save_grid,
                                                               mnu_filters});

                cntxtMenu.Items.Add(new ToolStripSeparator());
                cntxtMenu.Items.AddRange(new ToolStripMenuItem[] {mnu_expression, 
															   mnu_show_normalized,															   
															   mnu_dataSummary, 
															   mnu_show_scatterplots});
            }                        

            if (mobj_contextMenu != null)
                mobj_contextMenu.Items.Clear();
            mobj_contextMenu = cntxtMenu;
            
            return cntxtMenu;
        }

        /// <summary>
        /// Create a list of filters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mnu_filters_Click(object sender, EventArgs e)
        {
            DataFilters filters = new DataFilters(mlist_clusterFilters, mlist_umcFilters);
            if (filters.ShowDialog() == DialogResult.OK)
            {
                mlist_clusterFilters.Clear();
                mlist_clusterFilters = filters.GetClusterFilters();
                mlist_umcFilters.Clear();
                mlist_umcFilters = filters.GetUMCFilters();

                AddClusterToTable();
            }
        }

        #region Properties
        /// <summary>
        /// Sets the analysis object held by this cluster grid.
        /// </summary>
        public clsMultiAlignAnalysis Analysis
        {
            set
            {
                mobjAnalysis = value;
                AddClusterToTable();
                CreateContextMenu();
            }
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
        #endregion
     
        #region Virtual Event Method Calls
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
        #endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
        }
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
                mobjAnalysis = null;
            }
            base.Dispose(disposing);
        }
		#endregion

        #region "Context Menu Event Handlers"
        void mnu_columns_driftTime_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem cmd        = sender as ToolStripMenuItem;
            cmd.Checked                  = !cmd.Checked;
            mbool_show_driftTime_columns = cmd.Checked;
            AddClusterToTable();
        }
        /// <summary>
        /// Shows all of the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mnu_save_dante_Click(object sender, EventArgs e)
        {
            mnu_columns_all_Click(sender, e);
            mnu_save_grid_Click(sender, e);
        }
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
            
            item.Checked        = (item.Checked == false);
            mbool_ShowMassTags  = item.Checked;
            AddClusterToTable();
        }
        /// <summary>
        /// Displays all the charge state abundance values for each UMC.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnu_columns_showCMC_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem cmd   = sender as ToolStripMenuItem;
            cmd.Checked             = !cmd.Checked;
            mbool_showCMCAbundances = cmd.Checked;
            AddClusterToTable();
        }
		private void mnu_columns_all_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem cmd   = sender as ToolStripMenuItem ;
			cmd.Checked             = !cmd.Checked ; 
			if (cmd.Checked)
			{
				bool need_update = false ; 
				if (!mbool_show_all_columns || !mbool_show_mass_columns || !mbool_show_scan_columns || !mbool_show_aligned_scan_columns
                    || !mbool_show_umc_index_columns || !mbool_show_calibrated_mass_columns)
				{
					need_update                         = true; 
					mbool_show_all_columns              = true; 
					mbool_show_mass_columns             = true;
                    mbool_show_calibrated_mass_columns  = true;
					mbool_show_scan_columns             = true; 
					mbool_show_aligned_scan_columns     = true; 
					mbool_show_umc_index_columns        = true;
                    mbool_show_driftTime_columns        = true;

					if (mbool_ttest_performed)
						mbool_show_ttest_column         = true; 
					else
						mbool_show_ttest_column         = false; 
				}

				if (need_update)
				{
					AddClusterToTable() ; 
				}
			}
			else
			{
				//mbool_show_all_columns = false ;
                bool need_update = false;
                if (mbool_show_all_columns || mbool_show_mass_columns || mbool_show_scan_columns || mbool_show_aligned_scan_columns
                    || mbool_show_umc_index_columns || mbool_show_calibrated_mass_columns)
                {
                    need_update                         = true;
                    mbool_show_all_columns              = false;
                    mbool_show_mass_columns             = false;
                    mbool_show_calibrated_mass_columns  = false;
                    mbool_show_scan_columns             = false;
                    mbool_show_aligned_scan_columns     = false;
                    mbool_show_umc_index_columns        = false;
                    mbool_show_ttest_column             = false;
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
			cmd.Checked           = !cmd.Checked ;
			
            if (!cmd.Checked)
				mbool_show_all_columns  = false ; 
			mbool_show_mass_columns     = cmd.Checked ; 

			/// get the name of the table from the current table. :)
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_calibrated_mass_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ;
			if (!cmd.Checked)
				mbool_show_all_columns = false ; 
			mbool_show_calibrated_mass_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_scans_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked           = !cmd.Checked ; 

			if (!cmd.Checked)
				mbool_show_all_columns = false ; 
			mbool_show_scan_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_aligned_scan_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd   = sender as ToolStripMenuItem;
			cmd.Checked             = !cmd.Checked ; 
			
            if (!cmd.Checked)
				mbool_show_all_columns = false ; 
			mbool_show_aligned_scan_columns = cmd.Checked ; 

			/// 
            /// get the name of the table from the current table. :)
            /// 
			DataTable table = (DataTable) DataSource ; 
			AddClusterToTable() ; 
		}

		private void mnu_columns_umc_index_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			if (!cmd.Checked)
				mbool_show_all_columns = false ; 
			mbool_show_umc_index_columns = cmd.Checked ; 
			// get the name of the table from the current table. :)
			AddClusterToTable() ; 
		}
		private void mnu_columns_ttest_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			if (!cmd.Checked)
				mbool_show_all_columns = false ; 
			mbool_show_ttest_column    = cmd.Checked ; 

			// get the name of the table from the current table. :)
			AddClusterToTable() ; 
		}

		private void mnu_normalize_Click(object sender, EventArgs e)
		{
			clsNormalize objNormalizer = new clsNormalize() ; 
			objNormalizer.NormalizeData(mobjAnalysis) ; 
		}

        private void mnu_save_sqlite_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.CheckPathExists = true;
            fileDialog.DefaultExt = "*.db3";
            fileDialog.DereferenceLinks = true;
            fileDialog.ValidateNames = true;
            fileDialog.Filter = "SQLite DB3 files (*.db3)|*.db3";
            fileDialog.OverwritePrompt = true;

            if (fileDialog.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("File Dialog Result was not OK");
                return;
            }

            NHibernateUtil.SetDbLocation(fileDialog.FileName);
            UmcDAOHibernate umcDAOHibernate = new UmcDAOHibernate();
            
            clsUMCData umcData = mobjAnalysis.UMCData;

            for (int i = 0; i < 1; i++)
            //for (int i = 0; i < umcData.NumDatasets; i++)
            {
                clsUMC[] umcArray = mobjAnalysis.UMCData.GetUMCS(i);
                umcDAOHibernate.AddAll(umcArray);   
            }
        }

		private void mnu_save_grid_Click(object sender, EventArgs e)
		{
			try 
			{
				SaveFileDialog fileDialog   = new System.Windows.Forms.SaveFileDialog();
				fileDialog.AddExtension     = true;
				fileDialog.CheckPathExists  = true;
				fileDialog.DefaultExt       = "*.csv";
				fileDialog.DereferenceLinks = true;
				fileDialog.ValidateNames    = true;
				fileDialog.Filter           = "csv files (*.csv)|*.csv|Tab Delimited txt files (*.txt)|*.txt" ;
				fileDialog.OverwritePrompt  = true;
				fileDialog.FilterIndex      = 1;

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


                using (TextWriter writer = new StreamWriter(fileDialog.FileName))
                {

                    int[] arrClusterMainMemberIndex     = mobjAnalysis.UMCData.mobjClusterData.marrClusterMainMemberIndex; 
                    clsUMCData umcData                  = mobjAnalysis.UMCData;
                    clsClusterData clusters             = mobjAnalysis.UMCData.mobjClusterData;

                    int totalDatasets = mobjAnalysis.FileNames.Length;                    
                    int totalClusters = clusters.NumClusters;


                    writer.WriteLine("[DatasetMap]");
                    writer.WriteLine("Dataset ID{0}Dataset Path", delimiter);
                    for (int i = 0; i < totalDatasets; i++)
                    {
                        writer.WriteLine("{1}{0}{2}",
                                            delimiter,
                                            i,
                                            mobjAnalysis.FileNames[i].Replace("_isos.csv", "")
                                            );
                    }

                    writer.WriteLine("[Data]");
                    writer.WriteLine("Row ID{0}Cluster Size{0}Cluster Mass{0}Cluster NET{0}Dataset ID{0}Mass{0}NET{0}Scan{0}ScanStart{0}ScanEnd{0}Aligned Scan{0}UMC Index{0}Abundance-Sum{0}Abundance-Max{0}MS Feature Scan Count{0}Representative mz{0}Representative charge{0}CMC-Abundance_CS_1{0}CMC-Abundance_CS_2{0}CMC-Abundance_CS_3{0}CMC-Abundance_CS_4",
                                        delimiter);

                    for (int clusterNum = 0; clusterNum < totalClusters; clusterNum++)                
                    {
                        clsCluster cluster      = clusters.GetCluster(clusterNum);
                        string clusterString    = string.Format("{1}{0}{2}{0}{3}{0}{4}",
                                                        delimiter,
                                                        clusterNum,
                                                        cluster.mshort_num_dataset_members,
                                                        cluster.Mass,
                                                        cluster.Net);

                        for (int dataset = 0; dataset < totalDatasets; dataset++)
                        {                                                        
                            int pt_index = (clusterNum * totalDatasets) + dataset;
                            int index    = arrClusterMainMemberIndex[pt_index];
                            clsUMC umc   = null;

                            /// ////////////////////////////////////////////////////////////////////////////// 
                            /// Find the UMC so we can grab data from it to show
                            /// ////////////////////////////////////////////////////////////////////////////// 
                            if (index != -1)
                            {
                                double mz   = 0;                                  
                                umc         = umcData.GetUMC(index);
                                writer.WriteLine("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}",
                                                    delimiter,
                                                    clusterString,                          //1
                                                    umc.DatasetId,                          //2
                                                    umc.MassCalibrated,                     //3
                                                    umc.Net,                                //4
                                                    umc.Scan,                               //5
                                                    umc.ScanStart,                          //6
                                                    umc.ScanEnd,                            //7
                                                    umc.ScanAligned,                        //8
                                                    umc.mint_umc_index,                     //9
                                                    umc.AbundanceSum,                       //10
                                                    umc.AbundanceMax,                       //11
                                                    umc.mint_spectral_count,                //12
                                                    umc.mdouble_class_rep_mz,               //13
                                                    umc.mshort_class_rep_charge,            //14
                                                    umc.marray_chargeStatesAbundances[0],   //15
                                                    umc.marray_chargeStatesAbundances[1],   //16
                                                    umc.marray_chargeStatesAbundances[2],   //17
                                                    umc.marray_chargeStatesAbundances[3]);  //18       
                            }                                                     
                        }
                        if ((clusterNum % 300) == 0)
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }                                                 
                    }
                }

                return;


                //// now go through each row and save it. 
                //DataTable tb = (DataTable) DataSource ;				
                //int num_rows = tb.Rows.Count ; 

                //TextWriter writer = new StreamWriter(fileDialog.FileName);

                //// first write out the column names. 
                //for (int col_num = 0 ; col_num < tb.Columns.Count ; col_num++)
                //{
                //    writer.Write(tb.Columns[col_num].ColumnName) ; 
                //    if (col_num != tb.Columns.Count - 1)
                //        writer.Write(delimiter) ;
                //    else
                //        writer.Write("\n")  ;
                //}

                //for (int row_num = 0 ; row_num < num_rows ; row_num++)
                //{
                //    if ((row_num % 300) == 0)
                //    {
                //        GC.Collect();
                //        GC.WaitForPendingFinalizers();
                //        GC.Collect();
                //        GC.WaitForPendingFinalizers();
                //    }                    
                //    DataRow row = tb.Rows[row_num] ; 
                //    for (int col_num = 0 ; col_num < tb.Columns.Count ; col_num++)
                //    {
                //        if (row[tb.Columns[col_num].ColumnName] != null && row[tb.Columns[col_num].ColumnName] != DBNull.Value)
                //        {
                //            string data_val = Convert.ToString(row[tb.Columns[col_num].ColumnName]) ; 
                //            if (data_val.IndexOf(delimiter) >= 0)
                //            {
                //                data_val = "\"" + data_val + "\"" ; 
                //            }
                //            writer.Write(data_val) ; 
                //        }
                //        if (col_num != tb.Columns.Count - 1)
                //            writer.Write(delimiter) ;
                //        else
                //            writer.Write("\n")  ;
                //    }
                //}
                //writer.Close();
				
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
			mbool_show_normalized = cmd.Checked ; 
			AddClusterToTable() ; 
		}
		private void mnu_diff_abundance_Click(object sender, EventArgs e)
		{
		}
		private void mnu_columns_cluster_mass_calibrated_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			mbool_show_cluster_calibrated_mass = cmd.Checked ; 
			AddClusterToTable() ; 
		}
		private void mnu_columns_cluster_net_aligned_Click(object sender, EventArgs e)
		{
            ToolStripMenuItem cmd = sender as ToolStripMenuItem;
			cmd.Checked = !cmd.Checked ; 
			mbool_show_cluster_aligned_net = cmd.Checked ; 
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
        private void mnu_reorderColumns_Click(object sender, EventArgs e)
        {
            frmReorderDataTable table = new frmReorderDataTable();
            table.DataGrid = this;
            table.ShowDialog();
        }
		#endregion

		
	}
}
