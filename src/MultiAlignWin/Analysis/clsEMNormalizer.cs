using System;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for clsEMNormalizer.
	/// </summary>
	public class clsEMNormalizer
	{
		// Believes that the input data supplied, (x, y) 
		// is such that log(y) is a mixture of log(x) and a uniform distribution. 
		double mdbl_percent_normal ; 
		double mdbl_mean_normal ; 

		public clsEMNormalizer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public double [] Normalize(double []X, double []Y, int [] common_indices)
		{
			double [] logY_by_X = new double [common_indices.Length] ;
			int num_pts = common_indices.Length ; 
			for (int index = 0 ; index < num_pts ; index++)
			{
				logY_by_X[index] = Math.Log(Y[common_indices[index]]) - Math.Log(X[common_indices[index]]) ;
			}			
			Cephes.clsMathUtilities.EMUniformRandom(logY_by_X, ref mdbl_percent_normal, ref mdbl_mean_normal) ; 
			return GetNormalizedValues(X) ; 
		}

		public double [] GetNormalizedValues(double []X)
		{
			double factor = Math.Exp(mdbl_mean_normal) ; 
			int num_pts = X.Length ; 
			double []normalizedX = new double [num_pts] ;
			for (int pt_num = 0 ; pt_num < num_pts  ; pt_num++)
			{
				normalizedX[pt_num] = double.NaN ; 
				if (!double.IsNaN(X[pt_num]))
				{
					normalizedX[pt_num] = X[pt_num] * factor ; 
				}
			}
			return normalizedX ; 
		}

	}
}
