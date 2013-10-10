#pragma once

namespace MultiAlignEngine
{
	namespace PeakMatching
	{
		class PeakMatch
		{
		public:
			int mintMasstagID; 
			int mintFeatureIndex; 
			double mdblFScore_1; 
			double mdblFScore_2; 
			double mdblFScore_3; 
			double mdblPVal; 
			short mshortCharge; 
			double mdblMassError; 
			double mdblNETError; 
			PeakMatch(int mtID, int featureIndex, double fscore1, double fscore2, double fscore3, double pVal, short charge, 
				double massErr, double netErr): mintMasstagID(mtID), mintFeatureIndex(featureIndex), 
				mdblFScore_1(fscore1), mdblFScore_2(fscore2), mdblFScore_3(fscore3), mdblPVal(pVal), mshortCharge(charge), mdblMassError(massErr), 
				mdblNETError(netErr) {}; 
			~PeakMatch() {}; 
		}; 
	}
}