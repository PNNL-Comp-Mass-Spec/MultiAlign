#include "StdAfx.h"
#include ".\clssinglelinkageclustering.h"
#using <mscorlib.dll>
#include <algorithm> 

namespace MultiAlign
{
	bool SortDistances(clsDistance &a, clsDistance &b)
	{
		if (a.mdbl_distance <= b.mdbl_distance)
			return true ; 
		if (a.mdbl_distance > b.mdbl_distance)
			return false ; 
		return a.mint_elem_1 <= b.mint_elem_1 ;  
	}

	clsSingleLinkageClustering::clsSingleLinkageClustering(void)
	{
		mint_num_data_pts = 0 ;
	}

	clsSingleLinkageClustering::~clsSingleLinkageClustering(void)
	{
	}

	void clsSingleLinkageClustering::Clear()
	{
		mvect_clusters.clear() ; 
		mvect_top_level_cluster.clear() ; 
		mvect_data_membership.clear() ; 
		mmap_cluster_members.clear() ; 
	}

	void clsSingleLinkageClustering::CreateLeafClusters(int num_data_pts)
	{
		clsCluster cluster ; 
		for (int cluster_num = 0 ; cluster_num < num_data_pts ; cluster_num++)
		{
			// for leaf clusters, set first cluster index to data index and 
			// second cluster to -1.
			cluster.Set(cluster_num, -1) ; 
			mvect_clusters.push_back(cluster) ; 
			mvect_data_membership.push_back(cluster_num) ; 
			mvect_top_level_cluster.push_back(true) ; 
			mmap_cluster_members.insert(pair<int,int>(cluster_num, cluster_num)) ; 
		}
	}
	void clsSingleLinkageClustering::Cluster(vector<clsDistance> vect_distances, int num_data_pts)
	{
		Clear() ; 
		mint_num_data_pts = num_data_pts ; 
		// since we are performing hierarchical clustering. The most number of clusters we will have is
		// 2*num_data_pts-1 (includes the leaf clusters). So lets reserve that to begin with.
		mvect_clusters.reserve(2*mint_num_data_pts) ; 
		mvect_top_level_cluster.reserve(2*mint_num_data_pts) ; 
		mvect_data_membership.reserve(mint_num_data_pts) ; 

		// first create the leaf clusters.
		CreateLeafClusters(mint_num_data_pts) ; 
		// now sort the distances. 
		sort(vect_distances.begin(), vect_distances.end(), &SortDistances) ; 

		// now go through the list of distances, combining clusters one by one.
		int num_distances = vect_distances.size() ; 
		clsCluster new_cluster ; 
		multimap<int,int>::iterator cluster_member_iter ; 
		multimap<int,int>::iterator cluster_member_begin_iter ; 
		multimap<int,int>::iterator cluster_member_last_iter ;

		for (int elem_index = 0 ; elem_index < num_distances ; elem_index++)
		{
			clsDistance distance = vect_distances[elem_index] ; 
			int elem1 = distance.mint_elem_1 ; 
			int elem2 = distance.mint_elem_2 ; 
			int clust1 = mvect_data_membership[elem1] ; 
			int clust2 = mvect_data_membership[elem2] ; 
			if (clust1 == clust2)
				continue ; 
			// now create new cluster that is an aggregation of clust1, clust2.
			new_cluster.Set(clust1, clust2) ; 
			int new_cluster_index = mvect_clusters.size() ; 

			mvect_top_level_cluster[clust1] = false ; 
			mvect_top_level_cluster[clust2] = false ; 

			mvect_top_level_cluster.push_back(true) ; 
			mvect_clusters.push_back(new_cluster) ; 

			// now each member that belongs to clust1 should belong to new_cluster
			// and map from cluster to members should remove mapping of clust1 because
			// we don't need it for the processing any more.
			cluster_member_begin_iter = mmap_cluster_members.find(clust1) ; 
			for (cluster_member_iter = cluster_member_begin_iter ; 
				cluster_member_iter != mmap_cluster_members.end() && (*cluster_member_iter).first == clust1
				; )
			{
				cluster_member_last_iter = cluster_member_iter ; 
				int elem_index = (*cluster_member_iter).second ; 

				cluster_member_iter++ ; 
				mmap_cluster_members.erase(cluster_member_last_iter) ; 

				mmap_cluster_members.insert(pair<int,int>(new_cluster_index, elem_index)) ; 
				mvect_data_membership[elem_index] = new_cluster_index ; 
			}
//			mmap_cluster_members.erase(cluster_member_begin_iter, cluster_member_last_iter) ; 

			// do the same as above for members of clust2.
			cluster_member_begin_iter = mmap_cluster_members.find(clust2) ; 
			for (cluster_member_iter = cluster_member_begin_iter ; 
				cluster_member_iter != mmap_cluster_members.end() && (*cluster_member_iter).first == clust2
				; )
			{
				cluster_member_last_iter = cluster_member_iter ; 
				int elem_index = (*cluster_member_iter).second ;

				cluster_member_iter++ ; 
				mmap_cluster_members.erase(cluster_member_last_iter) ; 

				mmap_cluster_members.insert(pair<int,int>(new_cluster_index, elem_index)) ; 
				mvect_data_membership[elem_index] = new_cluster_index ; 
				cluster_member_last_iter = cluster_member_iter ; 
			}
//			mmap_cluster_members.erase(cluster_member_begin_iter, cluster_member_last_iter) ; 
		}
		// now all the final level cluster can be found by looking at mvect_top_level_cluster vector
		// for values of true or by looking at mmap_cluster_members for mappings from cluster number to 
		// element indices. 
	}

}