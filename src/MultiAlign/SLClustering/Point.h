#pragma once

namespace MultiAlignEngine
{
	namespace Clustering
	{
		class Point
		{
		public:
			int mint_index; 
			double mdouble_mass; 
			double mdouble_net; 
		}; 
		bool ComparePointMasses(Point &a, Point &b); 
		bool ComparePointIndices(Point &a, Point &b); 
	}
}