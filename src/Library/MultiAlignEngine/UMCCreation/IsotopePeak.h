#pragma once
namespace MultiAlignEngine
{
	namespace UMCCreation
	{
		/*
		 *
		 * Class that encapsulates data from deisotoping programs
		 * 
		 */ 
		class IsotopePeak
		{
		public:
			/// Indices and Scan (time reference)
			int mint_original_index; 
			int mint_umc_index; 
			int mint_scan; 
			
			// Charge 
			short mshort_charge; 			
			
			// Mass
			double mdouble_average_mass; 
			double mdouble_mono_mass;
			
			// Mass to charge ratio.
			double mdouble_mz; 

			/// FWHM - Isotopic fit
			float mflt_fit; 

			/// Abundance
			double mdouble_abundance; 
			double mdouble_max_abundance_mass;
			double mdouble_i2_abundance; 
			double mdouble_mono_abundance; 
			
			/* IMS Data */
			bool	mbool_imsPeak;				// Flag indicating if peak is from IMS data.
			double	mdouble_driftTime;				// Drift Time value dimension for this peak
			double  mdouble_cumulativeDriftTime;	// Co-Adding?			
			int		mint_imsFrame;				// IMS Frame

			

			IsotopePeak(void);
			~IsotopePeak(void);
		};
	}
}