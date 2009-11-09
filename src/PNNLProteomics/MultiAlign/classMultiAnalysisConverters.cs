using System;
using System.Data;
using System.Collections;

using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;

using PNNLProteomics.Data.Analysis;

namespace PNNLProteomics.MultiAlign
{
    
    /// <summary>
    /// Static class that converts a given analysis object to other types.
    /// </summary>
    public static class classMultiAnalysisConverters
    {
        private static  string mstr_umc_rep_mass_col = "Mass";
        private static  string mstr_umc_rep_net_col = "NET";
        private static  string mstr_umc_rep_mass_calib_col = "Calibrated Mass";
        private static  string mstr_umc_rep_net_aligned_col = "Aligned NET";
        private static  string mstr_umc_index_col = "Row ID";
        private static  string mstr_umc_rep_size_col = "Cluster Size";
        private static  string mstr_peptide_col = "Peptide";
        private static  string mstr_mass_tag_id_col = "Mass Tags";
        private static  string mstr_mass_tag_net_col = "Mass Tag NET";
        private static  string mstr_mass_tag_mass_col = "Mass Tag Mass";
        private static  string mstr_mass_tag_F_CS1 = "Charge 1 F Score";
        private static  string mstr_mass_tag_F_CS2 = "Charge 2 F Score";
        private static  string mstr_mass_tag_F_CS3 = "Charge 3 F Score";
        private static  string mstr_mass_tag_xcorr_col = "Mass Tag Xcorr";
        private static  string mstr_mass_tag_modification_col = "Modifications";
        private static  string mstr_mass_tag_modification_count_col = "Mod count";
        private static  string mstr_protein_col = "Protein";
        private static  string mstr_proteinid_col = "RefID";
        private static  string mstr_mass_colum = "Mass";
        private static  string mstr_calibrated_mass_colum = "Calibrated_mass";
        private static  string mstr_scan_colum = "Scan";
        private static  string mstr_aligned_scan_colum = "Aligned Scan";
        private static  string mstr_umc_index_column = "UMC index";
        
        //private static  string mstr_empty = "";

        /// <summary>
        /// Converts the given 
        /// </summary>
        /// <returns></returns>
        public static DataTable ConvertAnalysisToDataTable(clsMultiAlignAnalysis analysis)
        {            
			DataTable table = new DataTable() ; 

			table.Columns.Add(mstr_umc_index_col, typeof(int)) ; 
			table.Columns.Add(mstr_umc_rep_size_col, typeof(int)) ; 
			table.Columns.Add(mstr_umc_rep_mass_col, typeof(double)) ; 
			
            /// 
            /// Calibrated Mass
            ///             
			table.Columns.Add(mstr_umc_rep_mass_calib_col, typeof(double)) ; 
			
            /// 
            /// UMC - Representative Net
            /// 
            table.Columns.Add(mstr_umc_rep_net_col, typeof(double)) ; 
			
            /// 
            /// Cluster Aligned net
            ///             
			table.Columns.Add(mstr_umc_rep_net_aligned_col, typeof(double)) ; 
			
            /// 
            /// Start of Mass Tag Column
            /// 
			int start_mass_tag_column = table.Columns.Count ; 

            /// 
            /// Mass Tags
            /// 
			if (analysis.PeakMatchedToMassTagDB)
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
				num_columns_per_dataset++ ; 
				num_columns_per_dataset++ ; 
				num_columns_per_dataset++ ; 
				num_columns_per_dataset++ ; 
				num_columns_per_dataset++ ; 
				
				int start_data_column_num = table.Columns.Count ; 

				int num_datasets = analysis.UMCData.NumDatasets ; 
				int num_clusters = analysis.UMCData.mobjClusterData.NumClusters  ; 

				ArrayList arr_datasets_to_show = new ArrayList() ; 

				for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
				{
					string file_name = analysis.UMCData.DatasetName[dataset_num] ;
					int index = file_name.LastIndexOf("\\") ;
					string col_name = file_name.Substring(index+1) ; 
				    arr_datasets_to_show.Add(dataset_num) ; 
					
                    table.Columns.Add(col_name + "_" + mstr_mass_colum, typeof(double)) ;
					table.Columns.Add(col_name + "_" + mstr_calibrated_mass_colum, typeof(double)) ;
					table.Columns.Add(col_name + "_" + mstr_scan_colum, typeof(double)) ;
					table.Columns.Add(col_name + "_" + mstr_aligned_scan_colum, typeof(double)) ;
					table.Columns.Add(col_name + "_" + mstr_umc_index_column, typeof(int)) ;
					table.Columns.Add(col_name, typeof(double)) ; 
				}

				int num_datasets_to_show = arr_datasets_to_show.Count ; 
				int num_rows_so_far = 0 ; 

				//clsUMC [] arrUMCs = analysis.UMCData.marr_umcs ; 
				int [] arrClusterMainMemberIndex = analysis.UMCData.mobjClusterData.marrClusterMainMemberIndex ; 
				double [] arrClusterMemberIntensity = analysis.UMCData.mobjClusterData.marrClusterIntensity ; 
				double [] arrClusterMemberNormalizedIntensity = analysis.UMCData.mobjClusterData.marrClusterIntensityNormalized ; 

				clsPeakMatchingResults.clsPeakMatchingTriplet [] arrPeakMatchingTriplets = null ;
				clsProtein [] arrPeakMatchingProteins = null ; 
				clsMassTag [] arrPeakMatchingMassTags = null ; 

				int clusterNum = 0 ; 
				int currentPeakMatchNum = 0 ; 
				int numPeakMatches = 0 ; 
				if (analysis.PeakMatchingResults != null)
				{
					arrPeakMatchingTriplets = analysis.PeakMatchingResults.marrPeakMatchingTriplet ;
					arrPeakMatchingProteins = analysis.PeakMatchingResults.marrProteins ; 
					arrPeakMatchingMassTags = analysis.PeakMatchingResults.marrMasstags ; 
					numPeakMatches = arrPeakMatchingTriplets.Length ; 
				}

				int lastClusterNum = -1 ; 

				while(clusterNum < num_clusters)
				{
					MultiAlignEngine.Features.clsCluster cluster = analysis.UMCData.mobjClusterData.GetCluster(clusterNum);
					DataRow row = table.NewRow() ;
					row[0] = Convert.ToString(clusterNum+1) ; 
					row[2] = Convert.ToString(cluster.mdouble_mass) ; 
					int num_column = 3 ;					
					row[num_column++] = Convert.ToString(cluster.mdouble_mass_calibrated) ; 

					row[num_column++] = Convert.ToString(cluster.mdouble_net) ; 
                    row[num_column++] = Convert.ToString(cluster.mdouble_aligned_net) ; 

						

					int num_non_empty = 0 ; 
					for (int col_num = 0 ; col_num < num_datasets_to_show * num_columns_per_dataset ; col_num++)
					{
						int dataset_index = col_num / num_columns_per_dataset ; 
						int dataset_num = (int) arr_datasets_to_show[dataset_index] ; 
						int pt_index = clusterNum*num_datasets + dataset_num ; 
						int index = arrClusterMainMemberIndex[pt_index] ; 
						clsUMC umc = null ; 
						if (index != -1)
						{
                            umc = analysis.UMCData.marr_umcs[index]; 
							num_non_empty++ ;
						}

						if (umc != null)
							row[start_data_column_num+col_num] = Convert.ToString(umc.mdouble_mono_mass) ; 
						else
                            row[start_data_column_num + col_num] = DBNull.Value; 
						col_num++ ; 
						
						if (umc != null)
							row[start_data_column_num+col_num] = Convert.ToString(umc.mdouble_mono_mass_calibrated) ; 
						else
                            row[start_data_column_num + col_num] = DBNull.Value ; 
						col_num++ ; 
					
						if (umc != null)
							row[start_data_column_num+col_num] = Convert.ToString(umc.mint_scan) ; 
						else
                            row[start_data_column_num + col_num] =  DBNull.Value ; 
						col_num++ ; 
				
                        if (umc != null)
							row[start_data_column_num+col_num] = Convert.ToString(umc.mint_scan_aligned) ; 
						else
                            row[start_data_column_num + col_num] =  DBNull.Value ; 
						col_num++ ; 
					
                        if (umc != null)
							row[start_data_column_num+col_num] = Convert.ToString(umc.mint_umc_index) ; 
						else
                            row[start_data_column_num + col_num] = DBNull.Value ; 
						col_num++ ; 
					
                        if (umc != null)
						{
							double intensity = 0 ;
							if (arrClusterMemberNormalizedIntensity != null)
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
                            row[start_data_column_num + col_num] = DBNull.Value ; 
						}
					}

					row[1] = Convert.ToString(num_non_empty) ; 
					
                    // AddMassTags to Row
					// if it is peakmatched, and show mass tags is enabled, everything shows. 
					if(analysis.PeakMatchedToMassTagDB) 
					{
						if (arrPeakMatchingTriplets != null && 
							currentPeakMatchNum < arrPeakMatchingTriplets.Length 
							&& arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
						{
							// Display the peakmatchtriplet corresponding to the current cluster. 							
							int current_column = start_mass_tag_column ; 
							clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum] ; 
							clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex] ; 
							clsProtein protein = arrPeakMatchingProteins[triplet.mintProteinIndex] ; 
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

				//AddGridStyle(table) ; 
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
			}

            return table;
		}
    }
}
