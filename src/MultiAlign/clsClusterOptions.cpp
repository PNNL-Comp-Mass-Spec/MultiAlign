#include "StdAfx.h"
#include ".\clsClusterOptions.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace Clustering
	{
		clsClusterOptions::clsClusterOptions(void)
		{
			mdblMassTolerance = 6.0; 
			mdblNETTolerance = 0.03; 
			menmClusterIntensityType = enmClusterIntensityType::SUM_PER_DATASET; 
			menmClusterRepresentativeType = MEDIAN; 
		}

		clsClusterOptions::~clsClusterOptions(void)
		{
		}
	}
}