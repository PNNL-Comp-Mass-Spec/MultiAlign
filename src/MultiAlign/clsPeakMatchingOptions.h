#pragma once
#include "clsDataSummaryAttribute.h"
#include "clsParameterFileAttribute.h"

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		[System::Serializable]
		public __gc class clsPeakMatchingOptions
		{
			/// <summary>
			/// Mass Tolerance used in the clustering and peak matching.
			/// </summary>
			double mdblMassTolerance; 
			/// <summary>
			/// NET tolerance to be used in the clustering and peak matching.
			/// </summary>
			double mdblNETTolerance; 
			/// <summary>
			/// Drift-Time Tolerance for peak matching.
			/// </summary>
			double mdblDriftTimeTolerance;
			/// flag to use stack.
			bool m_performSTAC;
		public:

			
		public: 
			clsPeakMatchingOptions(void);
			~clsPeakMatchingOptions(void);
		
			[clsDataSummaryAttribute("Perform STAC")]			
			[clsParameterFileAttribute("PerformStac", "PeakMatchingOptions")]
			__property bool get_UseSTAC()
			{
				return m_performSTAC; 
			}
			__property void set_UseSTAC(bool perform)
			{
				m_performSTAC = perform; 
			}

			[clsDataSummaryAttribute("Mass Tolerance")]			
			[clsParameterFileAttribute("MassTolerance", "PeakMatchingOptions")]
			__property double get_MassTolerance()
			{
				return mdblMassTolerance; 
			}
			__property void set_MassTolerance(double value)
			{
				mdblMassTolerance = value; 
			}
			[clsDataSummaryAttribute("NET Tolerance")]			
			[clsParameterFileAttribute("NETTolerance", "PeakMatchingOptions")]
			__property double get_NETTolerance()
			{
				return mdblNETTolerance; 
			}
			__property void set_NETTolerance(double value)
			{
				mdblNETTolerance = value; 
			}			
			[clsDataSummaryAttribute("Drift Time Tolerance")]			
			[clsParameterFileAttribute("DriftTimeTolerance", "PeakMatchingOptions")]
			__property double get_DriftTimeTolerance()
			{
				return mdblDriftTimeTolerance; 
			}
			__property void set_DriftTimeTolerance(double value)
			{
				mdblDriftTimeTolerance = value; 
			}
		};
	}
}