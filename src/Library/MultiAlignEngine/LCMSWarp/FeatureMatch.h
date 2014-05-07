#pragma once

namespace MultiAlignEngine
{
	namespace Alignment
	{
		class FeatureMatch
		{
			public:
				/// Mass and Net error values for this feature match.
				double mdouble_ppmMassError;
				double mdouble_netError;
				double mdouble_driftError;

				int mint_feature_index; 
				int mint_feature_index_2; 
				double mdouble_net; 
				double mdouble_net_2; 
				FeatureMatch(void); 
				~FeatureMatch(void); 
		};
		bool SortFeatureMatchesByNet(FeatureMatch &a, FeatureMatch &b); 
	}
}