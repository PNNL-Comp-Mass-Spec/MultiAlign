using System;
using System.Data ;

namespace PNNLProteomics.Data.Analysis
{
	public enum enmNormalizationType { LINEAR_REGRESSION = 0, EM } ; 
	/// <summary>
	/// Summary description for clsNormalize.
	/// </summary>
	public class clsNormalize
	{
		private double [][][] marr_original_intensity ; 
		private double [][][] marr_normalized_intensity ; 
		private enmNormalizationType menm_normalization_type ; 
		private double [][] marr_group_intensities ; 
		private double [][] marr_group_intensities_normalized ; 
		private clsLinearNormalizer mobj_linear ; 
		private clsEMNormalizer mobj_em ; 
		// 		private int mint
		

		public clsNormalize()
		{
			//
			// TODO: Add constructor logic here
			//
			mobj_linear = new clsLinearNormalizer() ; 
			mobj_em = new clsEMNormalizer() ;
			menm_normalization_type = enmNormalizationType.EM ; 
		}

		/// <summary>
		/// Normalize Intensities between groups based on the trends for average intensities
		/// of points in each group. The Y values normalized to are the values in the baseline group.
		/// </summary>
		/// <param name="group_num"></param>
		/// <param name="baseline_group"></param>
		private void NormalizeGroup(int group_num, int baseline_group)
		{
			double [] X = marr_group_intensities[group_num] ; 
			double [] Y = marr_group_intensities[baseline_group] ; 
			double [] NormalizedX = Normalize(X, Y) ; 
			marr_group_intensities_normalized[group_num] = NormalizedX ; 

			int num_members = marr_original_intensity[group_num].Length ; 
			for (int member_num = 0 ; member_num < num_members ; member_num++)
			{
				double []normalizedX = GetNormalizedValues(marr_normalized_intensity[group_num][member_num]) ;
				marr_normalized_intensity[group_num][member_num] = normalizedX ; 
			}			
		}

		private void NormalizeGroupMembers(int group_num, int baseline_member)
		{
			int num_members = marr_original_intensity[group_num].Length ; 
			// All group members are treated as X which are normalized to Y.
			double [] Y = marr_original_intensity[group_num][baseline_member] ; 

			for (int member_num = 0 ; member_num < num_members ; member_num++)
			{
				if (member_num == baseline_member)
				{
					marr_normalized_intensity[group_num][member_num] = marr_original_intensity[group_num][member_num] ;
					continue ; 
				}
				double [] X = marr_original_intensity[group_num][member_num] ; 
				marr_normalized_intensity[group_num][member_num] = Normalize(X, Y) ; 				
			}
			CalculateAverageGroupIntensities(group_num) ; 
		}

		private void CalculateAverageGroupIntensities(int group_num)
		{
			int num_members = marr_original_intensity[group_num].Length ; 
			if (num_members == 0)
				return ; 
			int num_pts = marr_original_intensity[group_num][0].Length ;
			marr_group_intensities[group_num] = new double [num_pts] ; 

			double [][]intensities = marr_normalized_intensity[group_num] ; 

			for (int pt_num = 0 ; pt_num < num_pts ; pt_num++)
			{
				double sum = 0 ;
				int num_vals = 0 ;
				marr_group_intensities[group_num][pt_num] = double.NaN ; 
				for (int member_num = 0 ; member_num < num_members ; member_num++)
				{
					double val = intensities[member_num][pt_num] ;
					if (val != double.NaN)
					{
						num_vals++ ; 
						sum += val ; 
					}
				}
				if (num_vals != 0)
				{
					marr_group_intensities[group_num][pt_num] = sum / num_vals ; 
				}
			}
		}

		private int [] FindCommonIndices(double []X, double []Y)
		{
			int num_pts = X.Length ; 
			int num_common_pts = 0 ; 
			for (int point_num = 0 ; point_num < num_pts ; point_num++)
			{
				bool x_nan = double.IsNaN(X[point_num]) ; 
				bool y_nan = double.IsNaN(Y[point_num]) ;
 
				if (!x_nan && !y_nan)
				{
					num_common_pts++ ; 
				}
			}
			int []common_indices = new int[num_common_pts] ; 
			num_common_pts = 0 ; 
			for (int point_num = 0 ; point_num < num_pts ; point_num++)
			{
				bool x_nan = double.IsNaN(X[point_num]) ; 
				bool y_nan = double.IsNaN(Y[point_num]) ;
 
				if (!x_nan && !y_nan)
				{
					common_indices[num_common_pts++] = point_num ; 
				}
			}
			return common_indices ; 
		}

		private double [] Normalize(double []X, double []Y)
		{
			int [] common_indices = FindCommonIndices(X, Y) ; 

			switch(menm_normalization_type)
			{
				case enmNormalizationType.LINEAR_REGRESSION:
					return mobj_linear.Normalize(X, Y, common_indices) ; 					
				case enmNormalizationType.EM:
					return mobj_em.Normalize(X, Y, common_indices) ; 					
				default:
					break ; 
			}
			return null ; 
		}
		private double [] GetNormalizedValues(double []X)
		{
			switch(menm_normalization_type)
			{
				case enmNormalizationType.LINEAR_REGRESSION:
					return mobj_linear.GetNormalizedValues(X) ; 					
				case enmNormalizationType.EM:
					return mobj_em.GetNormalizedValues(X) ;					
				default:
					return null ; 					
			}
		}

		private void SetData(int num_groups, int []group_indices, clsMultiAlignAnalysis analysis)
		{
			int num_pts = analysis.UMCData.mobjClusterData.NumClusters ;
			int numDatasets = analysis.UMCData.NumDatasets ; 
			marr_original_intensity = new double [num_groups][][]; 
			marr_normalized_intensity = new double [num_groups][][]; 
			marr_group_intensities = new double[num_groups][] ; 
			marr_group_intensities_normalized = new double[num_groups][] ; 

			int [] num_elements_in_groups = new int[num_groups] ; 
			for (int group_num = 0 ; group_num < num_groups ; group_num++)
			{
				num_elements_in_groups[group_num] = 0 ; 
				marr_group_intensities[group_num] = new double [num_pts] ; 
				marr_group_intensities_normalized[group_num] = new double [num_pts] ; 
			}
			for (int col_num = 0 ; col_num < group_indices.Length ; col_num++)
			{
				int group_num = group_indices[col_num] ; 
				if (group_num != -1)
				{
					num_elements_in_groups[group_num]++ ; 
				}
			}

			for (int group_num = 0 ; group_num < num_groups ; group_num++)
			{
				int num_members = num_elements_in_groups[group_num] ;
				marr_original_intensity [group_num] = new double [num_members] [] ; 
				marr_normalized_intensity [group_num] = new double [num_members] [] ; 
				for (int member_num = 0 ; member_num < num_members ; member_num++)
				{
					marr_original_intensity [group_num][member_num] = new double [num_pts] ; 
					for (int pt_num = 0 ; pt_num < num_pts ; pt_num++)
					{
						marr_original_intensity [group_num][member_num][pt_num] = double.NaN ; 
					}
				}
			}

			int num_columns = group_indices.Length ; 
			int [] grp_member_num = new int[num_groups] ; 
			double [] arrIntensity = analysis.UMCData.mobjClusterData.marrClusterIntensity ; 
			for (int row_num = 0 ; row_num < num_pts ; row_num++)
			{
				for (int grp_num = 0 ; grp_num < num_groups ; grp_num++)
				{
					grp_member_num[grp_num] = 0 ; 
				}
				for (int col_num = 0 ; col_num < num_columns ; col_num++)
				{
					int group_num = group_indices[col_num] ; 
					if (group_num == -1)
						continue ; 
					marr_original_intensity [group_num][grp_member_num[group_num]][row_num] =  arrIntensity[row_num*numDatasets + col_num] ; 
					grp_member_num[group_num]++ ; 
				}
			}

		}

		private void GetData(int num_groups, int []group_indices, clsMultiAlignAnalysis analysis)
		{
			MultiAlignEngine.Features.clsClusterData clusterData = analysis.UMCData.mobjClusterData ; 
			int num_pts = clusterData.NumClusters ; 
			clusterData.marrClusterIntensityNormalized = new double [clusterData.marrClusterIntensity.Length] ; 

			int numDatasets = group_indices.Length ; 
			int [] groupMemberNum = new int[num_groups] ; 
			for (int clusterNum = 0 ; clusterNum < num_pts ; clusterNum++)
			{
				for (int groupNum = 0 ; groupNum < num_groups ; groupNum++)
				{
					groupMemberNum[groupNum] = 0 ; 
				}
				for (int datasetNum = 0 ; datasetNum < numDatasets ; datasetNum++)
				{
					int group_num = group_indices[datasetNum] ; 
					if (group_num == -1)
						continue ; 
					double intensity =	marr_normalized_intensity [group_num][groupMemberNum[group_num]][clusterNum]  ; 
					groupMemberNum[group_num]++ ; 
					clusterData.marrClusterIntensityNormalized[clusterNum*numDatasets +datasetNum] = intensity ; 
				}
			}

		}

		int GetGroups(ref int []groupIndices, clsMultiAlignAnalysis analysis)
		{
			groupIndices = new int [analysis.FileNames.Length] ; // should be using analysis.Files.Length

			// check if factors have been defined. 
			MultiAlignEngine.clsDatasetInfo [] arrDatasetInfo = (MultiAlignEngine.clsDatasetInfo []) 
				analysis.Files.ToArray(typeof(MultiAlignEngine.clsDatasetInfo));
			if (arrDatasetInfo == null || MultiAlignEngine.clsDatasetInfo.mintNumFactorsSpecified == 0)
			{
				for (int fileNum = 0 ; fileNum < analysis.FileNames.Length ; fileNum++)
					groupIndices[fileNum] = 0 ; 
				return 1 ; 
			}
			else
			{
				// factors have been specified. lets use first factor for normalization. 
				System.Collections.Hashtable levelNameInfoHash = new System.Collections.Hashtable() ; 
				int numGroups = -1 ; 
				bool nullFactorValueSeen = false ; 
				int nullGroupNum = -1 ; 
				for (int fileNum = 0 ; fileNum < arrDatasetInfo.Length ; fileNum++)
				{
					MultiAlignEngine.clsDatasetInfo datasetInfo = arrDatasetInfo[fileNum] ; 
					if (datasetInfo.AssignedFactorValues == null || datasetInfo.AssignedFactorValues[0] == null || (string)datasetInfo.AssignedFactorValues[0] == "")
					{
						if (!nullFactorValueSeen)
						{
							groupIndices[fileNum] = ++numGroups ; 
							nullGroupNum = numGroups ; 
							nullFactorValueSeen = true ; 
						}
						else
						{
							groupIndices[fileNum] = nullGroupNum ; 
						}
					}
					else if(levelNameInfoHash.ContainsKey(datasetInfo.AssignedFactorValues[0]))
					{
						groupIndices [fileNum] = (int) levelNameInfoHash[datasetInfo.AssignedFactorValues[0]] ; 
					}
					else
					{
						levelNameInfoHash[datasetInfo.AssignedFactorValues[0]] = ++numGroups ; 
						groupIndices [fileNum] = numGroups ; 
					}
				}
				return numGroups ; 
			}
		}

		public void NormalizeData(clsMultiAlignAnalysis analysis)
		{
			int [] groupIndices = new int[1] ; 
			int numGroups = GetGroups(ref groupIndices, analysis) ; 
			SetData(numGroups, groupIndices, analysis) ; 

			for (int group_num = 0 ; group_num < numGroups ; group_num++)
			{
				NormalizeGroupMembers(group_num ,0) ;
			}

			for (int group_num = 1 ; group_num < numGroups ; group_num++)
			{
				NormalizeGroup(group_num ,0) ;
			}
			// now lets take the new cluster values and put it in..
			GetData(numGroups, groupIndices, analysis) ; 
		}

	}
}
