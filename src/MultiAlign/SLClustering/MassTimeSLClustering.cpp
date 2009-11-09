// This is the main DLL file.
#include "stdafx.h"
#include "MassTimeSLClustering.h"
#include <algorithm> 
#include <fstream>

namespace MultiAlignEngine
{
	namespace Clustering
	{	
		MassTimeSLClustering::MassTimeSLClustering()
		{
			mdouble_mass_tolerance = 8; 
			mdouble_net_tolerance = 0.03; 
			mint_percent_done = 0; 
		}
		MassTimeSLClustering::~MassTimeSLClustering() 
		{
		}

		void MassTimeSLClustering::SetOptions(double massTolerance, double netTolerance)
		{
			mdouble_mass_tolerance = massTolerance; 
			mdouble_net_tolerance = netTolerance; 
		}

		void MassTimeSLClustering::CalculatePairwiseDistances(std::vector<Point> &vect_points, int start_point_num, 
			int stop_point_num, std::vector<SingleLinkageClustering::Distance> &vect_distances) 
		{
			vect_distances.clear(); 
			SingleLinkageClustering::Distance distance; 
			// now remember that the vect_features are sorted in a mass order and no other order. 
			// so we'll just work with index 0 beginning at start_umc_num and index ending at umc_num.
			for (int point_index1 = start_point_num; point_index1 <= stop_point_num; point_index1++)
			{
				// we want to keep the feature numbers 0 based. 
				int feature_num_1 = point_index1 - start_point_num; 
				for (int point_index2 = point_index1+1; point_index2 <= stop_point_num; point_index2++)
				{
					// we want to keep the feature numbers 0 based. 
					int feature_num_2 = point_index2 - start_point_num; 
					double mass_diff = ((vect_points[point_index1].mdouble_mass - vect_points[point_index2].mdouble_mass) * 1000000)/vect_points[point_index1].mdouble_mass; 
					double net_diff = vect_points[point_index1].mdouble_net - vect_points[point_index2].mdouble_net; 
					if (mass_diff < mdouble_mass_tolerance && -1 * mass_diff < mdouble_mass_tolerance
						&& net_diff < mdouble_net_tolerance && -1*net_diff < mdouble_net_tolerance)
					{
						double dist = (mass_diff * mass_diff)/ (2* mdouble_std_mass * mdouble_std_mass) + (net_diff*net_diff) / (2*mdouble_std_net * mdouble_std_net); 
						distance.Set(feature_num_1, feature_num_2, dist); 
						vect_distances.push_back(distance); 
					}
				}

			}
		}

		void MassTimeSLClustering::Cluster(std::vector<Point> &points, std::vector<int> &resultClusterIndices)
		{
			mint_percent_done = 0;
			std::vector<SingleLinkageClustering::Distance> vect_distances; 

			int num_points = points.size(); 
			resultClusterIndices.clear(); 
			resultClusterIndices.resize(num_points); 

			// now sort the points array by mass.
			sort(points.begin(), points.end(), ComparePointMasses); 
			// now go through the features array and using the supplied mass and net tolerance,
			// look for the situation where the mass difference between consecutive values is 
			// greater than mass_tolerance.

			int start_point_num = 0; 
			int point_num = start_point_num; 

			Point current_point, next_point; 
			std::multimap<int,int>::iterator cluster_iterator; 

			mint_num_clusters = 0; 
			while(point_num < num_points-1)
			{
				mint_percent_done = (100 * point_num) / num_points; 
				current_point = points[point_num]; 
				next_point = points[point_num+1]; 
				double mass_diff = ((next_point.mdouble_mass - current_point.mdouble_mass ) * 1000000)/ current_point.mdouble_mass; 
				if (mass_diff > mdouble_mass_tolerance)
				{
					if (start_point_num == point_num)
					{
						// only one element in this cluster. 
						int index = current_point.mint_index; 
						int current_cluster_num = mint_num_clusters; 
						resultClusterIndices[index] = current_cluster_num; 
						mint_num_clusters++; 
					}
					else
					{
						// discovered a break point. Time to make clustering happen between 
						// the features starting at start_point_num and the current one.
						// For that, we need to find all the pairs of distances 
						// that are within the supplied tolerances. 
						CalculatePairwiseDistances(points, start_point_num, point_num, vect_distances); 
						// next call for Single Linkage clustering to happen with the pairwise distances. 
						mobj_sl_clustering.PerformClustering(vect_distances, point_num - start_point_num+1); 
						// so now the clustering is done for this mass bin. copy all the clusterings
						// to cluster object. For multiple data points per cluster copy only the most intense value.				
						int num_clusters = 0; 
						int last_cluster_num = -1; 

						for(cluster_iterator = mobj_sl_clustering.mmap_cluster_members.begin(); cluster_iterator != mobj_sl_clustering.mmap_cluster_members.end(); cluster_iterator++)
						{
							int cluster_num = (*cluster_iterator).first; 
							// get the feature_index. Remember that 0 feature number from clustering 
							// is the 0th + start_point_num here.
							int feature_index = (*cluster_iterator).second +start_point_num; 
							int index = points[feature_index].mint_index; 
							if (cluster_num != last_cluster_num )
							{
								last_cluster_num = cluster_num; 
								num_clusters++; 
							}
							int current_cluster_num = mint_num_clusters+num_clusters-1; 
							resultClusterIndices[index] = current_cluster_num; 
						}
						mint_num_clusters += num_clusters; 
					}
					start_point_num = point_num + 1; 
				}
				point_num++; 
			}

			// the last point is unattended to. 
			current_point = points[point_num]; 
			if (start_point_num == point_num)
			{
				// only one element in this cluster. 
				int index = current_point.mint_index; 
				int current_cluster_num = mint_num_clusters; 

				resultClusterIndices[index] = current_cluster_num; 
				mint_num_clusters++; 
			}
			else
			{
				// discovered a break point. Time to make clustering happen between 
				// the features starting at start_point_num and the current one.
				// For that, we need to find all the pairs of distances 
				// that are within the supplied tolerances. 
				CalculatePairwiseDistances(points, start_point_num, point_num, vect_distances); 
				// next call for Single Linkage clustering to happen with the pairwise distances. 
				mobj_sl_clustering.PerformClustering(vect_distances, point_num - start_point_num+1); 
				// so now the clustering is done for this mass bin. copy all the clusterings
				// to cluster object. For multiple data points per cluster copy only the most intense value.				
				int num_clusters = 0; 
				int last_cluster_num = -1; 

				for(cluster_iterator = mobj_sl_clustering.mmap_cluster_members.begin(); cluster_iterator != mobj_sl_clustering.mmap_cluster_members.end(); cluster_iterator++)
				{
					int cluster_num = (*cluster_iterator).first; 
					// get the feature_index. Remember that 0 feature number from clustering 
					// is the 0th + start_point_num here.
					int feature_index = (*cluster_iterator).second +start_point_num; 
					int index = points[feature_index].mint_index; 

					if (cluster_num != last_cluster_num )
					{
						last_cluster_num = cluster_num; 
						num_clusters++; 
					}
					int current_cluster_num = mint_num_clusters+num_clusters-1; 
					resultClusterIndices[index] = current_cluster_num; 
				}
				mint_num_clusters += num_clusters; 
			}
		}

		int MassTimeSLClustering::GetPercentDone()
		{
			return mint_percent_done; 
		}

	}
}
