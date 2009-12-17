#pragma once
#include "clsUMCData.h"
#include "clsMassTagDB.h" 
#include "clsPeakMatchingResults.h" 

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		[System::Serializable]
		public __gc class clsPeakMatchingProcessor
		{
		public:
			__value enum enmState
			{
				IDLE=0, PEAKMATCHING_INITIALIZED, PEAKMATCHING, DONE, ERROR
			};
		private:			
			enmState	menmState; 
			double		mdblMassTolerance; 
			double		mdblNETTolerance;
			int			mintPercentDone; 

		public:
			clsPeakMatchingProcessor(void);
			~clsPeakMatchingProcessor(void);

			clsPeakMatchingResults* PerformPeakMatching(Features::clsUMCData *umcData, 
														int datasetIndex,
														MassTags::clsMassTagDB *masstagDB, 
														double shiftDaltons); 
			
			clsPeakMatchingResults* PerformPeakMatching(Features::clsClusterData *clusterData,
														MassTags::clsMassTagDB *masstagDB);

			clsPeakMatchingResults* PerformPeakMatching(Features::clsClusterData *clusterData, 
														MassTags::clsMassTagDB *masstagDB,
														double shiftDaltons);

			__property enmState get_State()
			{
				return menmState; 
			}
			__property int get_PercentDone()
			{
				return mintPercentDone; 
			}
			__property double get_MassTolerance()
			{
				return mdblMassTolerance; 
			}

			__property void set_MassTolerance(double tolerance)
			{
				mdblMassTolerance = tolerance; 
			}

			__property double get_NETTolerance()
			{
				return mdblNETTolerance; 
			}

			__property void set_NETTolerance(double tolerance)
			{
				mdblNETTolerance = tolerance; 
			}

			__property System::String* get_StatusMessage()
			{
				switch(menmState)
				{
					case PEAKMATCHING_INITIALIZED:
						return new System::String(S"Initialized Variables for PeakMatching"); 
						break; 
					case IDLE: 
						return new System::String(S"Idle"); 
						break; 
					case PEAKMATCHING: 
						return new System::String(S"Performing Peakmatching"); 
						break; 
					default:
						return new System::String(S""); 
						break; 
				}
				return 0; 
			}

			__property int get_PercentComplete()
			{
				return mintPercentDone; 
			}
		};
	}
}