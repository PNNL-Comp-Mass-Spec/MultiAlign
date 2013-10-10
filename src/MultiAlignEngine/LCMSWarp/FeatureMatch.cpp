
#include ".\FeatureMatch.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace Alignment
	{
		FeatureMatch::FeatureMatch(void)
		{
			mint_feature_index		= -1; 
			mint_feature_index_2	= -1; 
			mdouble_net				= -1; 
			mdouble_net_2			= -1; 
		}

		FeatureMatch::~FeatureMatch(void)
		{
		}
		bool SortFeatureMatchesByNet(FeatureMatch &a, FeatureMatch &b)
		{
			return (a.mdouble_net < b.mdouble_net);
		}
	}
}