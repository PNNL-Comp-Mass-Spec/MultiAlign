using System;
using System.Data ; 


namespace MultiAlignWin
{
	public enum enmStatus {IDLE = 0, COMPLETE, PROCESSING, ERROR} ; 

	/// <summary>
	/// Summary description for clsTTest.
	/// </summary>
	public class clsTTest
	{
		private DataTable mobj_table ; 
		private int [] marr_group_indices ; 
		private enmStatus menm_status ; 
		private int mint_percent_complete ; 
		private int mint_num_tested ; 
		private int mint_num_to_test ; 
		private clsTTestVals [] marr_ttests ; 

		private double [] marr_A ; 
		private double [] marr_B ; 
		private int mint_num_A ; 
		private int mint_num_B ; 


		public clsTTest()
		{
			//
			// TODO: Add constructor logic here
			//
			menm_status = enmStatus.IDLE ; 
			mint_percent_complete = 0 ; 
			mint_num_A = 0 ; 
			mint_num_B = 0 ; 
		}



		public void PerformTTest()
		{
			menm_status = enmStatus.PROCESSING ; 
			mint_percent_complete = 0 ;
			mint_num_tested = 0 ; 
			mint_num_to_test = mobj_table.Rows.Count ; 

			int num_columns = mobj_table.Columns.Count ; 

			marr_A = new double [num_columns] ; 
			marr_B = new double [num_columns] ; 

			marr_ttests = new clsTTestVals [mint_num_to_test] ; 

			for(mint_num_tested = 0 ; mint_num_tested < mint_num_to_test ; mint_num_tested++)
			{
				mint_percent_complete = (100 * mint_num_tested)/mint_num_to_test ; 
				DataRow row = mobj_table.Rows[mint_num_tested] ; 
				mint_num_A = 0 ;
				mint_num_B = 0 ;
				marr_ttests[mint_num_tested] = null ; 
				for (int col_num = 0 ; col_num < num_columns ; col_num++)
				{
					int group_num = marr_group_indices[col_num] ; 
					if (group_num == 0)
					{
						if (row[col_num] != DBNull.Value && Convert.ToString(row[col_num]) != "")
						{
							double val = Convert.ToDouble(row[col_num]) ; 
							marr_A[mint_num_A++] = val ; 
						}
					}
					if (group_num == 1)
					{
						if (row[col_num] != DBNull.Value && Convert.ToString(row[col_num]) != "")
						{
							double val = Convert.ToDouble(row[col_num]) ; 
							marr_B[mint_num_B++] = val ; 
						}
					}
				}
				if ((mint_num_A > 1 && mint_num_B > 1) || (mint_num_A > 1 && mint_num_B == 1)
					|| (mint_num_A == 1 && mint_num_B > 1))
				{
					double tvalue = tstatistic() ; 
					double pvalue = Cephes.clsMathUtilities.stdtr(mint_num_A + mint_num_B - 2, tvalue) ; 
					marr_ttests[mint_num_tested] = new clsTTestVals(tvalue, pvalue, mint_num_A+mint_num_B-2) ; 
				}
			}
			mint_percent_complete = 100 ; 
			menm_status = enmStatus.COMPLETE ; 
		}


		// calculates the t-statistic between the two groups of observations 
		// in marr_A, marr_B
		public double tstatistic()
		{
			double SumA = 0.0, SumOfSqrsA = 0.0; 
			double mean_A = 0, mean_B = 0 ; 
			for (int i=0; i<mint_num_A ; i++) 
			{ 
				SumA += marr_A[i]; 
				SumOfSqrsA += marr_A[i] * marr_A[i]; 
				mean_A += marr_A[i] ; 
			} 
			mean_A /= mint_num_A ; 
			double var_A = ((mint_num_A * SumOfSqrsA) - (Math.Pow(SumA, 2)))/mint_num_A ; 

			double SumB = 0.0, SumOfSqrsB = 0.0; 
			for (int i=0; i<mint_num_B ; i++) 
			{ 
				SumB += marr_B[i]; 
				SumOfSqrsB += marr_B[i] * marr_B[i]; 
				mean_B += marr_B[i] ; 
			} 
			mean_B /= mint_num_B ; 

			double var_B = ((mint_num_B * SumOfSqrsB) - (Math.Pow(SumB, 2)))/mint_num_B ; 
			double pooled_std = Math.Sqrt(((var_A + var_B) * (mint_num_A + mint_num_B)) /(mint_num_A * mint_num_B * (mint_num_A + mint_num_B-2))) ; 
			double tval = (mean_B - mean_A) / pooled_std ; 
			return tval ; 
		}

		public void SetStatusToProcessing()
		{
			menm_status = enmStatus.PROCESSING ; 
			mint_percent_complete = 0 ;
			mint_num_tested = 0 ; 
			mint_num_to_test = mobj_table.Rows.Count ; 

		}

		public enmStatus Status
		{
			get
			{
				return menm_status ; 
			}
		}

		public int PercentComplete
		{
			get
			{
				return mint_percent_complete ; 
			}
		}

		public int NumToTest
		{
			get
			{
				return mint_num_to_test ; 
			}
		}
		public int NumTested
		{
			get
			{
				return mint_num_tested ; 
			}
		}
		public clsTTestVals [] TTests
		{
			get
			{
				return marr_ttests ; 
			}
		}
		public DataTable Table 
		{
			get
			{
				return mobj_table ; 
			}
			set
			{
				mobj_table = value ; 
			}
		}

		public int [] GroupIndices
		{
			set
			{
				marr_group_indices = value ; 
			}
		}
	}
}
