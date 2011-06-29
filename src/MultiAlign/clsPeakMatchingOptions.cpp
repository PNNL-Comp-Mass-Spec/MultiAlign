#include ".\clsPeakMatchingOptions.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		clsPeakMatchingOptions::clsPeakMatchingOptions(void)
		{
			mdblMassTolerance			= 6.0; 
			mdblNETTolerance			= 0.03; 
			mdblDriftTimeTolerance		= 50.0;	
			m_performSTAC				= false;
			m_writeResultsToSystem		= false;
		}

		clsPeakMatchingOptions::~clsPeakMatchingOptions(void)
		{
		}
	}
}