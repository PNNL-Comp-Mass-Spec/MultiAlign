#pragma once

namespace MultiAlignEngine
{
	namespace Regression
	{
		class RegressionPts
		{
		public:
			double mdouble_x; 
			double mdouble_mass_error; 
			double mdouble_net_error; 
			void Set(double x, double mass_error, double net_eror)
			{
				mdouble_x = x; 
				mdouble_mass_error = mass_error; 
				mdouble_net_error = net_eror; 
			}
		}; 
	}
}