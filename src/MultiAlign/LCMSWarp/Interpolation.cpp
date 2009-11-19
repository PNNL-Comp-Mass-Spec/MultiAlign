
#include ".\interpolation.h"

namespace Utilities
{
	Interpolation::Interpolation(void)
	{
	}

	Interpolation::~Interpolation(void)
	{
	}

	void Interpolation::Spline(vector<float> &x, vector<float> &y, float yp1, float ypn)
	{
		mvect_temp_spline.clear(); 
		int n = (int) x.size(); 
		int i, k;
		float p,qn,sig,un;

		mvect_Y2.resize(n); 

		if(yp1 > 0.99e30) 
		{
			mvect_Y2[0]=0.0;
			mvect_temp_spline.push_back(0); 
		}
		else
		{
			mvect_Y2[0] = -0.5f;
			mvect_temp_spline.push_back((3.0f/(x[1]-x[0]))*((y[1]-y[0])/(x[1]-x[0])-yp1));
		}
		// generate second derivatives at internal points using recursive spline equations.
		for (i=1;i<=n-2;i++)
		{
			sig = (x[i]-x[i-1])/(x[i+1]-x[i-1]);
			p = sig*mvect_Y2[i-1]+2.0f;
			mvect_Y2[i]=(sig-1.0f)/p;
			mvect_temp_spline.push_back((y[i+1]-y[i])/(x[i+1]-x[i]) - (y[i]-y[i-1])/(x[i]-x[i-1]));
			mvect_temp_spline[i]=(6.0f*mvect_temp_spline[i]/(x[i+1]-x[i-1])-sig*mvect_temp_spline[i-1])/p;
		}
		if (ypn > 0.99e30) 
			qn=un=0.0;
		else
		{
			qn = 0.5f;
			un = (3.0f/(x[n-1]-x[n-2]))*(ypn-(y[n-1]-y[n-2])/(x[n-1]-x[n-2]));
		}
		mvect_Y2[n-1]= (un-qn*mvect_temp_spline[n-2])/(qn*mvect_Y2[n-2]+ 1.0f);
		for(k=n-2;k>=0;k--) 
			mvect_Y2[k]=mvect_Y2[k]*mvect_Y2[k+1]+mvect_temp_spline[k];

	}

	float  Interpolation::Splint(vector<float> &xa, vector<float> &ya, float x)
	{

		int n = (int) xa.size(); 
		int    klo,khi,k;
		float h,b,a;

		klo=0;
		khi=n-1;

		// binary search for khi, klo where xa[klo] <= x < xa[khi]
		while(khi-klo > 1)
		{
			k=(khi+klo) >> 1;
			if(xa[k] > x) 
				khi = k;
			else 
				klo = k;
		}
		h = xa[khi]-xa[klo];
		if(h==0.0) 
			return(-1);
		a = (xa[khi]-x)/h;
		b = (x-xa[klo])/h;
		// cubic interpolation at x.
		float y = a*ya[klo]+b*ya[khi]+((a*a*a-a)*mvect_Y2[klo]+(b*b*b-b)*mvect_Y2[khi])*(h*h)/6.0f;

		return y;
	}

}