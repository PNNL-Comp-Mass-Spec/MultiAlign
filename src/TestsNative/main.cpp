#include <iostream>
#include <vector>
#include "SingleLinkageClustering.h"

using namespace MultiAlignEngine::Clustering;



void TestSingleLinkageClustering()
{
		/*

		This is the "test" configuration the numbers here are elements
		and the dashed lines show the configuration for which we have distances

			   0 - 1
			  /				
			 /
			/			   3
		   4			  /
						 /
						2	

		This is the "test" configuration showing the above with their 
		dataset ID numbers. |x| shows the distance values as well.

				|1|
			   0 - 1
			  /				
		 |3| /
			/			   0
		   1			  /
						 /  |2|
						1	

		*/
		SingleLinkageClustering slCluster; 

		SingleLinkageClustering::Distance dist1;
		dist1.mint_elem_1 = 0;
		dist1.mint_elem_2 = 1;
		dist1.mdouble_distance		= 1;
		dist1.mint_dataset_elem_1	= 0;
		dist1.mint_dataset_elem_2   = 1;

		
		SingleLinkageClustering::Distance dist2;
		dist2.mint_elem_1 = 2;
		dist2.mint_elem_2 = 3;
		dist2.mdouble_distance		= 2;
		dist2.mint_dataset_elem_1	= 1;
		dist2.mint_dataset_elem_2	= 0;

		
		SingleLinkageClustering::Distance dist3;
		dist3.mint_elem_1 = 0;
		dist3.mint_elem_2 = 4;
		dist3.mdouble_distance		= 3;
		dist3.mint_dataset_elem_1	= 0;
		dist3.mint_dataset_elem_2	= 1;


		std::vector<SingleLinkageClustering::Distance> distances;
		distances.push_back(dist1);
		distances.push_back(dist2);
		distances.push_back(dist3);

		slCluster.SetNumberDataPoints(5);
		slCluster.PerformClustering(distances, 5);
}


void TestSingleLinkageClusteringComplex()
{
		/*

		This is the "test" configuration the numbers here are elements
		and the dashed lines show the configuration for which we have distances

			 ---0 - 1
			/  /				
		   /  /
	 	  /  /			   3
		5 - 4			  /
						 /
						2	

		This is the "test" configuration showing the above with their 
		dataset ID numbers. |x| shows the distance values as well.

				   1*
			 ---0 - 1
			/  /				
	 4*	   /  / 3*
	 	  /  /			   0
		0 - 1			  /   2*
		  1*			 /
						1	

		*/
		SingleLinkageClustering slCluster; 

		SingleLinkageClustering::Distance dist1;
		dist1.mint_elem_1			= 0;
		dist1.mint_elem_2			= 1;
		dist1.mdouble_distance		= 1;
		dist1.mint_dataset_elem_1	= 0;
		dist1.mint_dataset_elem_2   = 1;
		
		SingleLinkageClustering::Distance dist2;
		dist2.mint_elem_1			= 2;
		dist2.mint_elem_2			= 3;
		dist2.mdouble_distance		= 2;
		dist2.mint_dataset_elem_1	= 1;
		dist2.mint_dataset_elem_2	= 0;
		
		SingleLinkageClustering::Distance dist3;
		dist3.mint_elem_1			= 0;
		dist3.mint_elem_2			= 4;
		dist3.mdouble_distance		= 3;
		dist3.mint_dataset_elem_1	= 0;
		dist3.mint_dataset_elem_2	= 1;
		
		SingleLinkageClustering::Distance dist4;
		dist4.mint_elem_1			= 5;
		dist4.mint_elem_2			= 4;
		dist4.mdouble_distance		= 1;
		dist4.mint_dataset_elem_1	= 0;
		dist4.mint_dataset_elem_2	= 1;
		
		SingleLinkageClustering::Distance dist5;
		dist5.mint_elem_1			= 5;
		dist5.mint_elem_2			= 1;
		dist5.mdouble_distance		= 4;
		dist5.mint_dataset_elem_1	= 0;
		dist5.mint_dataset_elem_2	= 1;
		
		SingleLinkageClustering::Distance dist6;
		dist6.mint_elem_1			= 5;
		dist6.mint_elem_2			= 1;
		dist6.mdouble_distance		= 1.5;
		dist6.mint_dataset_elem_1	= 0;
		dist6.mint_dataset_elem_2	= 1;

		std::vector<SingleLinkageClustering::Distance> distances;
		distances.push_back(dist1);
		distances.push_back(dist2);
		distances.push_back(dist3);
		distances.push_back(dist4);
		distances.push_back(dist5);
		distances.push_back(dist6);

		slCluster.SetNumberDataPoints(6);
		slCluster.PerformClustering(distances, 6);
}



void TestSingleLinkageClusteringComplexMultipleGroups()
{
		/*

		This is the "test" configuration the numbers here are elements
		and the dashed lines show the configuration for which we have distances

			    0 - 1
			   /				
		      /
	 	     /			   3
		5 - 4			  /
						 /
						2	

		This is the "test" configuration showing the above with their 
		dataset ID numbers. |x| shows the distance values as well.

				   1*
			    0 - 3
		 	   /				
	  	      / 3*
	 	     /			   0
		3 - 1			  /   2*
		  1*			 /
						1	

		*/
		SingleLinkageClustering slCluster; 

		SingleLinkageClustering::Distance dist1;
		dist1.mint_elem_1			= 0;
		dist1.mint_elem_2			= 1;
		dist1.mdouble_distance		= 1;
		dist1.mint_dataset_elem_1	= 0;
		dist1.mint_dataset_elem_2   = 3;
		
		SingleLinkageClustering::Distance dist2;
		dist2.mint_elem_1			= 2;
		dist2.mint_elem_2			= 3;
		dist2.mdouble_distance		= 2;
		dist2.mint_dataset_elem_1	= 1;
		dist2.mint_dataset_elem_2	= 0;
		
		SingleLinkageClustering::Distance dist3;
		dist3.mint_elem_1			= 0;
		dist3.mint_elem_2			= 4;
		dist3.mdouble_distance		= 3;
		dist3.mint_dataset_elem_1	= 0;
		dist3.mint_dataset_elem_2	= 1;
		
		SingleLinkageClustering::Distance dist4;
		dist4.mint_elem_1			= 5;
		dist4.mint_elem_2			= 4;
		dist4.mdouble_distance		= 1;
		dist4.mint_dataset_elem_1	= 3;
		dist4.mint_dataset_elem_2	= 1;
		
		std::vector<SingleLinkageClustering::Distance> distances;
		distances.push_back(dist1);
		distances.push_back(dist2);
		distances.push_back(dist3);
		distances.push_back(dist4);

		slCluster.SetNumberDataPoints(6);
		slCluster.PerformClustering(distances, 6);
}


int main()
{
	TestSingleLinkageClustering();
	TestSingleLinkageClusteringComplex();
	TestSingleLinkageClusteringComplexMultipleGroups();

	return 0;
}

