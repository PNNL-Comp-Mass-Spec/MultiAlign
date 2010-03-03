
#include ".\clsmasstagdatabaseoptions.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace MassTags
	{
		clsMassTagDatabaseOptions::clsMassTagDatabaseOptions(void)
		{
				mintMinObservationCountFilter = 0;
				mbyteConfirmedTags		= 0; 
				mfltMinXCorr			= 0.0F; 
				mdecimalMinPMTScore		= 1; 
				mbyteNETValType			= 0; //-- 0 to use GANET values, 1 to use PNET values
				mdblMinDiscriminant		= 0;
				mdblPeptideProphetVal	= 0.5; 
				mstrServer				= S"albert"; 
				mstrDatabase			= 0; 
				mstrUserID				= S"mtuser"; 
				mstrPasswd				= S"mt4fun"; 

				mstrExperimentExclusionFilter	= S""; 
				mstrExperimentFilter			= S"";
				menm_databaseType				= MassTagDatabaseType::None;
		}

		clsMassTagDatabaseOptions::~clsMassTagDatabaseOptions(void)
		{
		}
	}
}
