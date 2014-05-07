#pragma once

namespace MultiAlignEngine
{
	namespace Clustering
	{
		class Point
		{
		public:
			int		mint_index; 
			double	mdouble_mass; 
			double	mdouble_net; 
			int 	mint_charge;
			double  mdouble_driftTime;
			int		mint_datasetID;
		}; 
		bool ComparePointMasses(Point &a, Point &b); 
		bool ComparePointIndices(Point &a, Point &b); 
	}
}