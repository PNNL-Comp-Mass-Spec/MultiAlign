#pragma once
#include "clsPeakMatchingProcessor.h" 

namespace MultiAlignEngine
{
	namespace PeakMatching
	{

		[System::Serializable]
		public __gc class clsPeakProphetProcessor
		{
		public:
			__value enum enmState
			{
				IDLE=0, COMPUTING_PEAK_PROBS, COMPUTING_PROTEIN_PROBS, DONE, ERROR
			};
		private:
			clsPeakProphetProcessor::enmState menmState; 
		public:
			clsPeakProphetProcessor(void);
			~clsPeakProphetProcessor(void);

			void ComputeProbabilities(MultiAlignEngine::PeakMatching::clsPeakMatchingResults &results); 

			__property enmState get_State()
			{
				return menmState; 
			}
			__property int get_PercentDone()
			{
				return 0; 
			}

			__property System::String* get_StatusMessage()
			{
				switch(menmState)
				{
					case clsPeakProphetProcessor::COMPUTING_PEAK_PROBS:
						return new System::String(S"Computing Peak Probabilities"); 
						break; 
					case clsPeakProphetProcessor::COMPUTING_PROTEIN_PROBS:
						return new System::String(S"Computing Peak Probabilities"); 
						break; 
					case clsPeakProphetProcessor::IDLE:
						return new System::String(S"Idle"); 
						break; 
					default:
						return new System::String(S""); 
						break; 
				}
				return 0; 
			}
		};
	}
}