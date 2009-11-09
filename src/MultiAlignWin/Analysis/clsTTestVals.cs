using System;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for clsTTestVals.
	/// </summary>
	public class clsTTestVals
	{
		public double mdbl_t_statistic ; 
		public double mdbl_pvalue ; 
		public int mint_df ; 

		public clsTTestVals()
		{
			//
			// TODO: Add constructor logic here
			//
			mdbl_t_statistic = Double.NaN ; 
			mdbl_pvalue = Double.NaN ; 
			mint_df = 0 ; 
		}
		public clsTTestVals(double tvalue, double pvalue, int df)
		{
			//
			// TODO: Add constructor logic here
			//
			mdbl_t_statistic = tvalue ; 
			mdbl_pvalue = pvalue ; 
			mint_df = df ; 
		}
	}
}
