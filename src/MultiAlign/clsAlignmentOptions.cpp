#include "StdAfx.h"
#include ".\clsalignmentoptions.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace Alignment
	{
		clsAlignmentOptions::clsAlignmentOptions(void)
		{
			mintNumTimeSections				= 100; 
			mshortContractionFactor			= 3; 
			mshortMaxTimeDistortion			= 10; 
			mshortMaxPromiscuity			= 3; 
			mblnUsePromiscuousPoints		= false; 
			mblnMassCalibUseLSQ				= false; 
			mdblMassCalibrationWindow		= 6.0; 
			mshortMassCalibNumXSlices		= 12; 
			mshortMassCalibNumYSlices		= 50; 
			mshortMassCalibMaxJump			= 20; 
			mdblMassCalibMaxZScore			= 3; 
			mdblMassCalibLSQMaxZScore		= 2.5; 
			mshortMassCalLSQNumKnots		= 12; 
			mdblMassTolerance				= 6.0; 
			mdblNETTolerance				= 0.03; 
			menmAlignmentType				= NET_MASS_WARP; 
			menmCalibrationType				= HYBRID_CALIB;
			mblnAlignToMassTagDatabase		= false; 
			mstrAlignmentBaselineName		= 0; 
			mdouble_massBinSize				= .2;
			mdouble_massBinSize				= .001;

			mbool_alignSplitMZs				= false;
			mlist_mzBoundaries				= new List<classAlignmentMZBoundary*>();

			/// Construct the m/z boundary object.			
			mlist_mzBoundaries->Add(new classAlignmentMZBoundary(0.0, 505.7));
			mlist_mzBoundaries->Add(new classAlignmentMZBoundary(505.7, 999999999.0));
		}

		clsAlignmentOptions::~clsAlignmentOptions(void)
		{
		}
	}
}