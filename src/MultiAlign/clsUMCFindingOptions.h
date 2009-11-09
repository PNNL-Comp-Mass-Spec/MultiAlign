#pragma once
#using <mscorlib.dll>
#include "clsDataSummaryAttribute.h"
#include "clsParameterFileAttribute.h"
#include "clsUMCData.h"

namespace MultiAlignEngine
{
	namespace Features
	{
		[System::Serializable]
		public __gc class clsUMCFindingOptions
		{
			float mfltWtMonoMass; 
			float mfltWtAveMass; 
			float mfltWtLogAbundance; 
			float mfltWtScan; 
			float mfltWtFit; 
			float mfltWtNET; 

			float mfltConstraintMonoMass; // in ppm 
			float mfltConstraintAveMass; 

			double mdblMaxDistance; 
			bool mblnUseNET; 

			/// Used to filter bad fit filter values.
			bool mbool_isotopicFitFilterInverted;
			/// Flag indicating whether to use the ISO Fit Filter
			bool mbool_useIsostopicFitFilter;			
			/// Flag indicating whether to use the ISO Intensity Filter
			bool mbool_useIsostopicIntensityFilter;
			/// Intensity Filter
			double mdouble_isostopicIntensityFilter;
			/// Fit Filter 
			double mdouble_isostopicFitFilter;

			/// Intensity filter for peaks.
			int mint_intensityFilter;			
			int mintMinUMCLength;

			/// How to report abundance for UMC's
			enmAbundanceReportingType menum_abundanceReportingType;

		public:
			clsUMCFindingOptions()
			{
				mfltWtMonoMass			= 0.01F; 
				mfltWtAveMass			= 0.01F; 
				mfltWtLogAbundance		= 0.10F; 
				mfltWtScan				= 0.01F; 
				mfltWtFit				= 0.10F; 
				mfltWtNET				= 0.10F; 

				mfltConstraintMonoMass	= 6.0F; // in ppm 
				mfltConstraintAveMass	= 6.0F; // in ppm; 

				mdblMaxDistance			= 0.1; 
				mblnUseNET				= true; 
				mintMinUMCLength		= 3; 
				mint_intensityFilter	= 0;
				
				mbool_isotopicFitFilterInverted   = false;
				mbool_useIsostopicFitFilter		  = false;
				mbool_useIsostopicIntensityFilter = false;
				mdouble_isostopicFitFilter		  = .15;
				mdouble_isostopicIntensityFilter  = 0;
				menum_abundanceReportingType = enmAbundanceReportingType::PeakMax;
			} 

			
			/// Intensity Filter
			[clsDataSummaryAttribute("Use Isotopic Peak Intensity Filter")]	
			[clsParameterFileAttribute("UseIsotopicPeakIntensityFilter", "UMCFindingOptions")]
			__property bool get_UseIsotopicIntensityFilter()
			{
				return mbool_useIsostopicIntensityFilter;
			}
			__property void set_UseIsotopicIntensityFilter(bool value)
			{
				mbool_useIsostopicIntensityFilter = value;
			}	

			/// Intensity Filter
			[clsDataSummaryAttribute("UMC Abundance Reporting Type")]	
			[clsParameterFileAttribute("UMCAbundanceReportingType", "UMCFindingOptions")]
			__property enmAbundanceReportingType get_UMCAbundanceReportingType()
			{
				return menum_abundanceReportingType;
			}
			__property void set_UMCAbundanceReportingType(enmAbundanceReportingType value)
			{
				menum_abundanceReportingType = value;
			}		

			[clsDataSummaryAttribute("Isotopic Intensity Filter")]	
			[clsParameterFileAttribute("IsotopicIntensityFilter", "UMCFindingOptions")]
			__property int get_IsotopicIntensityFilter()
			{
				return mint_intensityFilter;
			}
			__property void set_IsotopicIntensityFilter(int value)
			{
				mint_intensityFilter = value;
			}

			/// Fit Filter
			[clsDataSummaryAttribute("Is Isotopic Peak Fit Filter Inverted")]	
			[clsParameterFileAttribute("IsIsotopicPeakFitFilterInverted", "UMCFindingOptions")]
			__property bool get_IsIsotopicFitFilterInverted()
			{
				return mbool_isotopicFitFilterInverted;
			}
			__property void set_IsIsotopicFitFilterInverted(bool value)
			{
				mbool_isotopicFitFilterInverted = value;
			}

			[clsDataSummaryAttribute("Use Isotopic Peak Fit Filter")]	
			[clsParameterFileAttribute("UseIsotopicPeakFitFilter", "UMCFindingOptions")]
			__property bool get_UseIsotopicFitFilter()
			{
				return mbool_useIsostopicFitFilter;
			}
			__property void set_UseIsotopicFitFilter(bool value)
			{
				mbool_useIsostopicFitFilter = value;
			}

			
			[clsDataSummaryAttribute("Isotopic Peak Intensity Filter")]	
			[clsParameterFileAttribute("IsotopicIntensityFilter", "UMCFindingOptions")]
			__property double get_IsotopicFitFilter()
			{
				return mdouble_isostopicFitFilter;
			}
			__property void set_IsotopicFitFilter(double value)
			{
				mdouble_isostopicFitFilter = value;
			}
			
			
			[clsDataSummaryAttribute("Mono Mass Weight")]	
			[clsParameterFileAttribute("MonoMassWeight", "UMCFindingOptions")]
			__property float get_MonoMassWeight()
			{
				return mfltWtMonoMass; 
			}
			__property void set_MonoMassWeight(float value)
			{
				mfltWtMonoMass = value; 
			}

			[clsDataSummaryAttribute("Average Mass Weight")]
			[clsParameterFileAttribute("AveMassWeight", "UMCFindingOptions")]
			__property float get_AveMassWeight()
			{
				return mfltWtAveMass; 
			}
			__property void set_AveMassWeight(float value)
			{
				mfltWtAveMass = value; 
			}

			[clsDataSummaryAttribute("Log Abundance Weight")]
			[clsParameterFileAttribute("LogAbundanceWeight", "UMCFindingOptions")]
			__property float get_LogAbundanceWeight()
			{
				return mfltWtLogAbundance; 
			}
			__property void set_LogAbundanceWeight(float value)
			{
				mfltWtLogAbundance = value; 
			}

			[clsDataSummaryAttribute("Scan Weight")]
			[clsParameterFileAttribute("ScanWeight", "UMCFindingOptions")]			
			__property float get_ScanWeight()
			{
				return mfltWtScan; 
			}
			__property void set_ScanWeight(float value)
			{
				mfltWtScan = value; 
			}

			[clsDataSummaryAttribute("Fit Weight")]			
			[clsParameterFileAttribute("FitWeight", "UMCFindingOptions")]
			__property float get_FitWeight()
			{
				return mfltWtFit; 
			}
			__property void set_FitWeight(float value)
			{
				mfltWtFit = value; 
			}

			[clsDataSummaryAttribute("NET Weight")]		
			[clsParameterFileAttribute("NetWeight", "UMCFindingOptions")]
			__property float get_NETWeight()
			{
				return mfltWtNET; 
			}
			__property void set_NETWeight(float value)
			{
				mfltWtNET = value; 
			}

			[clsDataSummaryAttribute("Constraint Mono Mass (PPM)")]	
			[clsParameterFileAttribute("ConstraintMonoMass", "UMCFindingOptions")]
			__property float get_ConstraintMonoMass()
			{
				return mfltConstraintMonoMass; 
			}
			__property void set_ConstraintMonoMass(float value)
			{
				mfltConstraintMonoMass = value; 
			}

			[clsDataSummaryAttribute("Constraint Average Mass (PPM)")]	
			[clsParameterFileAttribute("ConstraintAveMass", "UMCFindingOptions")]
			__property float get_ConstraintAveMass()
			{
				return mfltConstraintAveMass; 
			}
			__property void set_ConstraintAveMass(float value)
			{
				mfltConstraintAveMass = value; 
			}


			[clsDataSummaryAttribute("Max Distance")]	
			[clsParameterFileAttribute("MaxDistance", "UMCFindingOptions")]
			__property double get_MaxDistance()
			{
				return mdblMaxDistance; 
			}
			__property void set_MaxDistance(double value)
			{
				mdblMaxDistance = value; 
			}

			[clsDataSummaryAttribute("Use NET")]			
			[clsParameterFileAttribute("UseNET", "UMCFindingOptions")]
			__property bool get_UseNET()
			{
				return mblnUseNET; 
			}
			__property void set_UseNET(bool value)
			{
				mblnUseNET = value; 
			}
			[clsDataSummaryAttribute("Min UMC Length")]
			[clsParameterFileAttribute("MinUMCLength", "UMCFindingOptions")]
			__property int get_MinUMCLength()
			{
				return mintMinUMCLength; 
			}

			__property void set_MinUMCLength(int len)
			{
				mintMinUMCLength = len; 
			}
		};
	}
}