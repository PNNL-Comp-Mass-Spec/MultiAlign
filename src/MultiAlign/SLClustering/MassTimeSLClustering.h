#pragma once
#include <vector>
#include "SingleLinkageClustering.h" 

namespace MultiAlignEngine
{
	namespace Clustering
	{

		class MassTimeSLClustering
		{
			int mint_percent_done; 
			int mint_num_clusters;
			double mdouble_std_net; 
			double mdouble_std_mass; 
			double mdouble_mass_tolerance; 
			double mdouble_net_tolerance; 

			void CalculatePairwiseDistances(std::vector<Point> &vect_points, int start_point_num, 
				int stop_point_num, std::vector<SingleLinkageClustering::Distance> &vect_distances); 
			SingleLinkageClustering mobj_sl_clustering; 
		public:
			void SetOptions(double massTolerance, double netTolerance); 
			// clusters the points and returns cluster number of each point in resultClusterIndices. 
			// The vector points is sorted inside the function, so if the caller needs the same original 
			// order, the caller needs to resort it back. 
			void Cluster(std::vector<Point> &points, std::vector<int> &resultClusterIndices); 
			MassTimeSLClustering();
			~MassTimeSLClustering(); 
			int GetPercentDone(); 
		};
	}
}
