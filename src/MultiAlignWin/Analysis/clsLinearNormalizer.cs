using System;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for clsLinearNormalizer.
	/// </summary>
	public class clsLinearNormalizer
	{
		double mdbl_slope ;
		double mdbl_intercept ; 
		double mdbl_rsq ; 

		public clsLinearNormalizer()
		{
			//
			// TODO: Add constructor logic here
			//
			mdbl_slope = 0 ;
			mdbl_intercept = 0 ; 
			mdbl_rsq = 0 ; 
		}
		public double [] Normalize(double []X, double []Y, int [] common_indices)
		{
			double [] XCommon = new double [common_indices.Length] ; 
			double [] YCommon = new double [common_indices.Length] ;
			int num_pts = common_indices.Length ; 
			for (int index = 0 ; index < num_pts ; index++)
			{
				XCommon[index] = X[common_indices[index]] ;
				YCommon[index] = Y[common_indices[index]] ;
			}			
			Cephes.clsMathUtilities.LinearRegression(XCommon, YCommon, ref mdbl_slope, ref mdbl_intercept, ref mdbl_rsq) ; 
			return GetNormalizedValues(X) ; 
		}

		public double [] GetNormalizedValues(double []X)
		{
			int num_pts = X.Length ; 
			double []normalizedX = new double [num_pts] ;
			for (int pt_num = 0 ; pt_num < num_pts  ; pt_num++)
			{
				normalizedX[pt_num] = double.NaN ; 
				if (!double.IsNaN(X[pt_num]))
				{
					normalizedX[pt_num] = mdbl_slope * X[pt_num] + mdbl_intercept ; 
				}
			}
			return normalizedX ; 
		}

		double Slope
		{
			get
			{
				return mdbl_slope ; 
			}
		}
		double Intercept
		{
			get
			{
				return mdbl_intercept ; 
			}
		}
		double RSquared
		{
			get
			{
				return mdbl_rsq ; 
			}
		}
	}
}
