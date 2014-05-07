#pragma once
#include <vector> 
namespace MultiAlignEngine
{
	public __gc class clsUtilities
	{
	public:
		clsUtilities(void);
		~clsUtilities(void);
		static inline void Copy(std::vector<double> &src, float (&arr) __gc [])
		{
			int numPts = (int) src.size(); 
			arr = new float __gc [numPts]; 
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				arr[ptNum] = (float) src[ptNum]; 
			}
		}
		static inline void Copy(std::vector<int> &src, int (&arr) __gc [])
		{
			int numPts = (int) src.size(); 
			arr = new int __gc [numPts]; 
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				arr[ptNum] = src[ptNum]; 
			}
		}
	};
}