
#include "mathutils.h"
#include <math.h>

namespace Utilities
{
	inline double sign(double x)
	{
		if (x<0.0)
			return -1; 
		if (x > 0.0)
			return 1; 
		return 0; 
	}


	MathUtils::MathUtils(void)
	{
	}

	MathUtils::~MathUtils(void)
	{
	}

	double MathUtils::gamma(double x)
	{
		int i,k,m;
		double ga,gr,r,z;

		static double g[] = {
			1.0,
			0.5772156649015329,
		-0.6558780715202538,
		-0.420026350340952e-1,
			0.1665386113822915,
		-0.421977345555443e-1,
		-0.9621971527877e-2,
			0.7218943246663e-2,
		-0.11651675918591e-2,
		-0.2152416741149e-3,
			0.1280502823882e-3,
		-0.201348547807e-4,
		-0.12504934821e-5,
			0.1133027232e-5,
		-0.2056338417e-6,
			0.6116095e-8,
			0.50020075e-8,
		-0.11812746e-8,
			0.1043427e-9,
			0.77823e-11,
		-0.36968e-11,
			0.51e-12,
		-0.206e-13,
		-0.54e-14,
			0.14e-14};

		if (x > 171.0) return 1e308;    // This value is an overflow flag.
		if (x == (int)x) {
			if (x > 0.0) {
				ga = 1.0;               // use factorial
				for (i=2;i<x;i++) {
				ga *= i;
				}
			}
			else
				ga = 1e308;
		}
		else {
			if (fabs(x) > 1.0) {
				z = fabs(x);
				m = (int)z;
				r = 1.0;
				for (k=1;k<=m;k++) {
					r *= (z-k);
				}
				z -= m;
			}
			else
				z = x;
			gr = g[24];
			for (k=23;k>=0;k--) {
				gr = gr*z+g[k];
			}
			ga = 1.0/(gr*z);
			if (fabs(x) > 1.0) {
				ga *= r;
				if (x < 0.0) {
					ga = -M_PI/(x*ga*sin(M_PI*x));
				}
			}
		}
		return ga;
	}

	double MathUtils::psi(double x) 
	{
		double s=0,ps=0,xa=0,x2=0;
		int n=0,k=0;
		static double a[] = {
			-0.8333333333333e-01,
			0.83333333333333333e-02,
			-0.39682539682539683e-02,
			0.41666666666666667e-02,
			-0.75757575757575758e-02,
			0.21092796092796093e-01,
			-0.83333333333333333e-01,
			0.4432598039215686};

		xa = fabs(x);
		s = 0.0;
		if ((x == (int)x) && (x <= 0.0)) {
			ps = 1e308;
			return ps;
		}
		if (xa == (int)xa) {
			n = System::Convert::ToInt32(xa);
			for (k=1;k<n;k++) {
				s += 1.0/k;
			}
			ps =  s-el;
		}
		else if ((xa+0.5) == ((int)(xa+0.5))) {
			n = System::Convert::ToInt32(xa-0.5);
			for (k=1;k<=n;k++) {
				s += 1.0/(2.0*k-1.0);
			}
			ps = 2.0*s-el-1.386294361119891;
		}
		else {
			if (xa < 10.0) {
				n = 10-(int)xa;
				for (k=0;k<n;k++) {
					s += 1.0/(xa+k);
				}
				xa += n;
			}
			x2 = 1.0/(xa*xa);
			ps = log(xa)-0.5/xa+x2*(((((((a[7]*x2+a[6])*x2+a[5])*x2+
				a[4])*x2+a[3])*x2+a[2])*x2+a[1])*x2+a[0]);
			ps -= s;
		}
		if (x < 0.0)
			ps = ps - M_PI*cos(M_PI*x)/sin(M_PI*x)-1.0/x;
			return ps;
	}

	double MathUtils::invPsi(double y)
	{
		// Using algorithm from Paul Fackler
		double L = 1.0;
		double x = exp(y);
		while (L > 10e-8)
		{
			x = x + L*sign(y-psi(x));
			L = L / 2.0;
		}
		return x; 
	}
}