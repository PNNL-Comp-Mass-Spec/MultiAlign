using System;

namespace MultiAlignWin.Data
{
	/// <summary>
	/// Summary description for clsMassTags.
	/// </summary>
	public class clsMassTags
	{

		public int mint_mass_tag_id ; 
		public string mstr_peptide ; 
		public string mstr_modification ; 
		public double mdbl_mono_mass ; 
		public double mdbl_avg_ganet ; 
		public double mdbl_PNET ; 
		public double mdbl_high_xcorr ;
		public double mdbl_std_ganet ; 
		public double mdbl_max_discriminant ; 
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
			mdbl_mono_mass = 0 ; 
			mdbl_avg_ganet = -1 ; 
			mdbl_PNET = -1 ; 
			mdbl_high_xcorr = 0 ; 
			mdbl_std_ganet = 0 ; 
			mdbl_max_discriminant = 0 ; 
			mint_num_obs_passing_filter = 0 ; 
		}
	}
}
