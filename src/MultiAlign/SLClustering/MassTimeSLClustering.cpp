// This is the main DLL file.

#include "MassTimeSLClustering.h"
#include <algorithm> 
#include <fstream>
#include <math.h>

namespace MultiAlignEngine
{
	namespace Clustering
	{	
		MassTimeSLClustering::MassTimeSLClustering()
		{
			mdouble_mass_tolerance   	= 8; 
			mdouble_net_tolerance	    = 0.03; 
			mdouble_driftTime_tolerance = 25;
			mint_percent_done		    = 0; 
			mbool_ignoreCharge			= true;
		}
		MassTimeSLClustering::~MassTimeSLClustering() 
		{
		}

		void MassTimeSLClustering::SetOptions(double massTolerance, double netTolerance, double driftTimeTolerance, bool ignoreCharge)
		{
			mdouble_mass_tolerance		= massTolerance; 
			mdouble_net_tolerance		= netTolerance; 
			mdouble_driftTime_tolerance = driftTimeTolerance;
			mbool_ignoreCharge			= ignoreCharge;
		}

		void MassTimeSLClustering::CalculatePairwiseDistances(	std::vector<Point> &vect_points, 
																int start_point_num, 
																int stop_point_num, 
																std::vector<SingleLinkageClustering::Distance> &vect_distances) 
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
					Point point1 = vect_points[point_index1];
					Point point2 = vect_points[point_index2];

					double mass_diff = ((vect_points[point_index1].mdouble_mass - vect_points[point_index2].mdouble_mass) * 1000000)/vect_points[point_index1].mdouble_mass; 
					double net_diff = vect_points[point_index1].mdouble_net - vect_points[point_index2].mdouble_net; 
					double dt_diff = vect_points[point_index1].mdouble_driftTime - vect_points[point_index2].mdouble_driftTime; 

					if (mass_diff < mdouble_mass_tolerance && -1*mass_diff < mdouble_mass_tolerance
						&& net_diff < mdouble_net_tolerance && -1*net_diff < mdouble_net_tolerance
						&& dt_diff < mdouble_driftTime_tolerance && -1*dt_diff < mdouble_driftTime_tolerance)
					{
						//double dist = (mass_diff * mass_diff); /// (2* mdouble_std_mass * mdouble_std_mass) + (net_diff*net_diff) / (2*mdouble_std_net * mdouble_std_net); 
						double dist = ((mass_diff * mass_diff) / (mdouble_std_mass * mdouble_std_mass)) + ((net_diff*net_diff) / (mdouble_std_net * mdouble_std_net)); 
						//dist += (net_diff * net_diff);
						dist += (dt_diff  * dt_diff) / (mdouble_std_driftTime * mdouble_std_driftTime) ;
						dist = sqrt(dist);

						// BLL - we separate here if the datasets are the same, we dont want them to be.
						if (point1.mint_datasetID != point2.mint_datasetID)
						{
							/// We separate here if the charge states are the same...
							if (mbool_ignoreCharge == false)
							{						
								if (point1.mint_charge == point2.mint_charge)
								{
									distance.Set(feature_num_1, feature_num_2, dist, point1.mint_datasetID, point2.mint_datasetID); 
									vect_distances.push_back(distance); 
								}
							}else
							{
								distance.Set(feature_num_1, feature_num_2, dist, point1.mint_datasetID, point2.mint_datasetID); 
								vect_distances.push_back(distance); 
							}
						}
					}
				}

			}
		}

		void MassTimeSLClustering::CalculateDimensionStatistics(std::vector<Point> &points)
		{
			mdouble_std_mass      = 0;
			mdouble_std_net		  = 0;
			mdouble_std_driftTime = 0;

			mint_percent_done = 0;
			std::vector<SingleLinkageClustering::Distance> vect_distances; 

			int num_points = points.size();
			if (num_points < 1)
				return;

			double meanMass = 0;
			double meanNET  = 0;
			double meanDT   = 0;

			std::vector<Point>::iterator iter = points.begin();
			while(iter != points.end())
			{
				meanMass += (*iter).mdouble_mass;
				meanNET  += (*iter).mdouble_net;
				meanDT   += (*iter).mdouble_driftTime;
				iter++;
			}
			
			meanMass /= num_points;
			meanNET  /= num_points;
			meanDT   /= num_points;
			
			iter = points.begin();
			double stdMass = 0;
			double stdNET  = 0;
			double stdDT   = 0;			

			while(iter != points.end())
			{
				stdMass += ((*iter).mdouble_mass		- meanMass);
				stdNET  += ((*iter).mdouble_net			- meanNET);
				stdDT   += ((*iter).mdouble_driftTime	- meanDT);
				iter++;
			}
			mdouble_std_mass		= sqrt(stdMass / num_points);
			mdouble_std_net			= sqrt(stdNET  / num_points);
			mdouble_std_driftTime	= sqrt(stdDT   / num_points);			
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

			// We need this to normalize the dimensions for our distance calculations.  This is a O(2N) calculation.
			// Where N = number of points 
			CalculateDimensionStatistics(points);

			// now go through the features array and using the supplied mass and net tolerance,
			// look for the situation where the mass difference between consecutive values is 
			// greater than mass_tolerance.

			int start_point_num = 0; 
			int point_num = start_point_num; 

			Point current_point, next_point; 
			std::multimap<int,int>::iterator cluster_iterator; 

			mobj_sl_clustering.SetNumberDataPoints(num_points);
			
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
