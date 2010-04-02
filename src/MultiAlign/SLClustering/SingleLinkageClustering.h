#pragma once
#include <vector>
#include <map> 
#include <float.h>
#include "Point.h" 

namespace MultiAlignEngine
{
	namespace Clustering
	{
		class SingleLinkageClustering
		{
			/// This datastructure is only used for speedy clustering, by using 
			/// a hierarchical tree to represent clusters in the intermediate. 
			/// Each cluster is linked to other clusters. For leaf clusters, 
			/// representing the actual data point itself this represents the index
			/// of the data point.
			class Cluster
			{
			public:
				// index to child cluster 1. For the leaf cluster, this is
				// the index of the data. 
				int mint_child_1; 
				// index to child cluster 2. -1 for leaf cluster (i.e. 
				// cluster corresponding to the actual data).
				int mint_child_2; 
				Cluster(): mint_child_1(-1), mint_child_2(-1){}; 
				Cluster(int child1_index, int child2_index): mint_child_1(child1_index), 
					mint_child_2(child2_index) {}; 
				void Set(int child1_index, int child2_index)
				{
					mint_child_1 = child1_index; 
					mint_child_2 = child2_index; 
				}
				~Cluster() {};
			};

			std::vector <bool> mvect_top_level_cluster;
			std::vector <int> mvect_datasetIDs; 
			std::vector <Cluster> mvect_clusters;
			// map to keep track of which cluster an 
			// element belongs to. 
			std::vector<int> mvect_data_membership; 
			int mint_num_data_pts;
			void Clear(); 
			void CreateLeafClusters(int num_data_pts);
		public:
			class Distance
			{
			public:
				int mint_elem_1; 
				int mint_elem_2; 
				double mdouble_distance;
				/// These track what dataset the element belonged to
				int mint_dataset_elem_1; 
				int mint_dataset_elem_2;
				Distance(): mint_elem_1(-1), mint_elem_2(-1), mdouble_distance(DBL_MAX), mint_dataset_elem_1(-1), mint_dataset_elem_2(-1){};
				void Set(int elem1, int elem2, double dist, int dataset1, int dataset2) 
				{
					mint_dataset_elem_1 = dataset1;
					mint_dataset_elem_2 = dataset2;
					mint_elem_1 = elem1; 
					mint_elem_2 = elem2; 
					mdouble_distance	= dist; 
				}
				~Distance(){}; 
			}; 


			void SetNumberDataPoints(int dataPoints);
			// multimap to keep track of members of a cluster.
			// We will keep only the top level cluster members stored. 
			// So this should be exactly mint_num_data_pts because each leaf
			// is represented only once.
			std::multimap<int,int> mmap_cluster_members; 

			SingleLinkageClustering(void);
			~SingleLinkageClustering(void);
			void PerformClustering(std::vector<Distance> vect_distance, int num_data_pts); 
		};
	}
}