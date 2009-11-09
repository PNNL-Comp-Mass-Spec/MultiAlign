using System;

namespace PNNLProteomics.Data
{
	/// <summary>
	/// Summary description for clsMassTags.
	/// </summary>
	public class clsMassTags
	{

		public int mint_mass_tag_id ; 
		public string mstr_peptide ; 
		public string mstr_modification ; 
		public double mdouble_mono_mass ; 
		public double mdouble_avg_ganet ; 
		public double mdouble_PNET ; 
		public double mdouble_high_xcorr ;
		public double mdouble_std_ganet ; 
		public double mdouble_max_discriminant ; 
		public int mint_num_obs_passing_filter ;
		public int mint_mod_count ; 

		public clsMassTags()
		{
			//
			// TODO: Add constructor logic here
			//
			mint_mass_tag_id = -1 ; 
			mstr_peptide = "" ; 
			mstr_modification = "" ; 
			mint_mod_count = 0 ; 
			mdouble_mono_mass = 0 ; 
			mdouble_avg_ganet = -1 ; 
			mdouble_PNET = -1 ; 
			mdouble_high_xcorr = 0 ; 
			mdouble_std_ganet = 0 ; 
			mdouble_max_discriminant = 0 ; 
			mint_num_obs_passing_filter = 0 ; 
		}
	}
}
