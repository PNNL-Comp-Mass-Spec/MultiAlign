using System;

namespace PNNLProteomics.Data.Analysis
{
	/// <summary>
	/// Summary description for clsTTestVals.
	/// </summary>
	public class clsTTestVals
	{
		public double mdouble_t_statistic ; 
		public double mdouble_pvalue ; 
		public int mint_df ; 

		public clsTTestVals()
		{
			//
			// TODO: Add constructor logic here
			//
			mdouble_t_statistic = Double.NaN ; 
			mdouble_pvalue = Double.NaN ; 
			mint_df = 0 ; 
		}
		public clsTTestVals(double tvalue, double pvalue, int df)
		{
			//
			// TODO: Add constructor logic here
			//
			mdouble_t_statistic = tvalue ; 
			mdouble_pvalue = pvalue ; 
			mint_df = df ; 
		}
	}
}
