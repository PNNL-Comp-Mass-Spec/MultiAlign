
#include ".\clscluster.h"
#using <mscorlib.dll>
namespace MultiAlignEngine
{
	namespace Features
	{
		clsCluster::clsCluster(void)
		{
			mdouble_driftTime	= 0;
			mdouble_driftError	= -1;
			mdouble_netError	= -1;
			mdouble_massError	= -1;
		}

		clsCluster::~clsCluster(void)
		{
		}
	}
}