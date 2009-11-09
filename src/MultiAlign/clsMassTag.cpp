#include "StdAfx.h"
#include ".\clsMassTag.h"

namespace MultiAlignEngine
{
	namespace MassTags
	{
		clsMassTag::clsMassTag(): mintMassTagId(-1), 
			mstrPeptide(0), 
			mstrModification(0), 
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
		clsMassTag::~clsMassTag()
		{
		}
	}
}