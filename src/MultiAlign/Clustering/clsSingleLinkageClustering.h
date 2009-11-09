#pragma once
#include <vector>
#include <map> 
#include <float.h>

using namespace std ; 
namespace MultiAlign
{
	class clsCluster
	{
	public:
		// index to child cluster 1. For the leaf cluster, this is
		// the index of the data. 
		int mint_child_1 ; 
		// index to child cluster 2. -1 for leaf cluster (i.e. 
		// cluster corresponding to the actual data).
		int mint_child_2 ; 
		clsCluster(): mint_child_1(-1), mint_child_2(-1){} ; 
		clsCluster(int child1_index, int child2_index): mint_child_1(child1_index), 
			mint_child_2(child2_index) {} ; 
		void Set(int child1_index, int child2_index)
		{
			mint_child_1 = child1_index ; 
			mint_child_2 = child2_index ; 
		}
		~clsCluster() {} ;
	};
	class clsDistance
	{
	public:
		int mint_elem_1 ; 
		int mint_elem_2 ; 
		double mdbl_distance ; 
		clsDistance(): mint_elem_1(-1), mint_elem_2(-1), mdbl_distance(DBL_MAX){} ;
		void Set(int elem1, int elem2, double dist) 
		{
			mint_elem_1 = elem1 ; 
			mint_elem_2 = elem2 ; 
			mdbl_distance = dist ; 
		}

		~clsDistance(){} ; 
	} ; 

	class clsSingleLinkageClustering
	{
		vector <bool> mvect_top_level_cluster ; 
		vector <clsCluster> mvect_clusters ;
		// map to keep track of which cluster an 
		// element belongs to. 
		vector<int> mvect_data_membership ; 
		int mint_num_data_pts ;
		void Clear() ; 
		void CreateLeafClusters(int num_data_pts) ;
	public:
		// multimap to keep track of members of a cluster.
		// We will keep only the top level cluster members stored. 
		// So this should be exactly mint_num_data_pts because each leaf
		// is represented only once.
		multimap<int,int> mmap_cluster_members ; 

		clsSingleLinkageClustering(void);
		~clsSingleLinkageClustering(void);
		void Cluster(vector<clsDistance> vect_distance, int num_data_pts) ; 
	};
}