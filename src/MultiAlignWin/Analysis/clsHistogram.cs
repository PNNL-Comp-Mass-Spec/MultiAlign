using System;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for clsHistogram.
	/// </summary>
	public class clsHistogram
	{
		float mflt_min_value ; 
		float mflt_max_value ; 
		int mint_num_bins = 20 ; 
		public float [] marr_frequency ;
		public float [] marr_values ; 

		public clsHistogram(float [] values)
		{
			//
			// TODO: Add constructor logic here
			//
			CalculateHistogram(values) ; 
		}
		public clsHistogram()
		{
		}

		public int NumBins
		{
			get
			{
				return mint_num_bins ; 
			}
			set
			{
				mint_num_bins = value ; 
			}
		}

		private void CalculateHistogram(float [] values) 
		{
			marr_frequency = new float[mint_num_bins] ; 
			marr_values = new float[mint_num_bins] ; 

			mflt_min_value = float.MaxValue ; 
			mflt_max_value = float.MinValue ; 
			for (int i = 0 ; i < values.Length ; i++)
			{
				if (values[i] > mflt_max_value)
				{
					mflt_max_value = values[i] ; 
				}
				if (values[i] < mflt_min_value)
				{
					mflt_min_value = values[i] ; 
				}
			}
			float bin_range = (mflt_max_value - mflt_min_value) / mint_num_bins ; 
			if (bin_range == 0)
			{
				int mid_index = mint_num_bins/2 ; 
				marr_frequency[mid_index] = values.Length ; 
				marr_values[mid_index] = mflt_max_value ; 
				int index = mid_index-1 ; 
				while(index >= 0)
				{
					marr_values[index] = 0 ;
					index++ ; 
				}
				index = mid_index +1 ; 
				while(index < mint_num_bins)
				{
					marr_values[index] = 0 ; 
					index++ ; 
				}
				return ;
			}

			marr_values[0] = mflt_min_value ;
			marr_frequency[0] = 0 ;
			for (int bin_num = 1 ; bin_num < mint_num_bins ; bin_num++)
			{
				marr_values[bin_num] = marr_values[bin_num-1] + bin_range ; 
				marr_frequency[bin_num] = 0 ;
			}

			// otherwise fill in the values. 
			for (int i = 0 ; i < values.Length ; i++)
			{
				int bin_index = (int) ((values[i] - mflt_min_value) / bin_range) ; 
				if (bin_index >= mint_num_bins)
					bin_index = mint_num_bins - 1 ; 
				marr_frequency[bin_index]++ ; 
			}
			return ; 
		}
	}
}
