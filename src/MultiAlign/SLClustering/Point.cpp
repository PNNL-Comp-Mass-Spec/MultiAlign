#include "StdAfx.h"
#include ".\point.h"

namespace MultiAlignEngine
{
	namespace Clustering
	{
		bool ComparePointMasses(Point &a, Point &b)
		{
			/*if (a.mdouble_mass <= b.mdouble_mass)
				return true; 
			if (a.mdouble_mass > b.mdouble_mass)
				return false; 
			return a.mdouble_net <= b.mdouble_net;  */
			return (a.mdouble_mass < b.mdouble_mass);
		}
		bool ComparePointIndices(Point &a, Point &b)
		{
			return a.mint_index <= b.mint_index;  
		}
	}
}

