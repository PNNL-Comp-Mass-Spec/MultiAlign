#include "StdAfx.h"
#include ".\clsalignmentfunction.h"

namespace MultiAlignEngine
{
	namespace Alignment
	{
		clsAlignmentFunction::clsAlignmentFunction(System::String* alignee, System::String* reference, 
			enmCalibrationType calibType, enmAlignmentType alignmentType)
		{
			mstrDataset = alignee; 
			mstrReference = reference; 
			menmCalibrationType = calibType; 
			menmAlignmentType = alignmentType; 
			
		}

		clsAlignmentFunction::clsAlignmentFunction(enmCalibrationType calibType, enmAlignmentType alignmentType)
		{
			menmCalibrationType = calibType; 
			menmAlignmentType = alignmentType; 
		}

		clsAlignmentFunction::~clsAlignmentFunction(void)
		{
		}
	}
}