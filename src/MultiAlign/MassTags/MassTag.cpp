#include "StdAfx.h"
#include ".\MassTag.h"

namespace MultiAlignEngine
{
	namespace MassTags
	{
		MassTag::MassTag(void): mintMassTagId(-1), 
			mdblMonoMass(0),  
			mdblAvgGANET(0),  
			mdblPNET(0),  
			mdblHighXCorr(0),  
			mdblStdGANET(0),  
			mdblMaxDiscriminant(0), 
			mintNumObsPassingFilter(0), 
			mshortModCount(0), 
			mfltAvgFCS1(-100),
			mfltAvgFCS2(-100), 
			mfltAvgFCS3(-100),
			mshortCleavageState(2)
		{
		}
		MassTag::~MassTag()
		{
		}

	}
}