
#include ".\clsClusterOptions.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace Clustering
	{
		clsClusterOptions::clsClusterOptions(void)
		{
			mdblMassTolerance		 = 6.0; 
			mdblNETTolerance		 = 0.03; 
			mdblDriftTimeTolerance   = 50.0;
			menmClusterIntensityType		= enmClusterIntensityType::SUM_PER_DATASET; 
			menmClusterRepresentativeType	= MEDIAN; 
			mbool_ignoreCharge				= true;
			mbool_alignToDatabase			= true;
		}

		clsClusterOptions::~clsClusterOptions(void)
		{
		}
	}
}