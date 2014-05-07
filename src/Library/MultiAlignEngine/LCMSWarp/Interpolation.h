#pragma once
#include <vector> 
using namespace std; 

namespace Utilities
{
	class Interpolation
	{
#pragma warning(disable : 4251)
		//! Temporary variable used in computation of spline coefficients.
		vector<float> mvect_temp_spline; 
		//! vector to store the second derivatives at the knot points of the spline.
		vector<float> mvect_Y2; 

#pragma warning(default : 4251)
	public:
		//! Default constructor
		Interpolation(void);
		//! destructor
		~Interpolation(void);
		//! Cubic Spline interpolation. This function generates the second derivatives at the knot points. 
		/*!
			\param x vector of x values.
			\param y vector of y values.
			\param yp1 second derivative at first point.
			\param ypn second derivative at the nth point.
			\remarks These alogrothims are from: Numerical Recipes in C by William H. Press, Brian P. Flannery, Saul A. Teukolsky, William T. Vetterling.
			\brief Given the arrays x[0..n-1] and y[0..n-1] containing the tabulated 
			function, i.e., yi = f(xi), with x0<x1<...<xn-1, and given values
			yp1 and ypn for the first derivative of the interpolating function
			at points 0 and n-1, respectively, this routine returns an array
			y2[1..n] that contains the second derivatives of the interpolating
			function at the tabulated points xi. If yp1 and/or ypn are equal to
			1x10^30 or larger, the routine is signaled to set the corresponding
			boundary condition for a natural spline, with zero second 
			derivative on that boundary.
		*/
		void Spline(vector<float> &x, vector<float> &y, float yp1, float ypn);
		//! Cubic Spline interpolation. This function does the actual interpolation at specified point, using provided second derivatives at the knot points. 
		/*!
			\param xa vector of x values.
			\param ya vector of y values.
			\param x is the value we want to find the interpolating y value at. 
			\return returns interpolated y at point x. 
		*/
		float  Splint(vector<float> &xa, vector<float> &ya, float x); 
	};
}