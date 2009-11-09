#include "StdAfx.h"
#include ".\clsfeaturedata.h"
#using <mscorlib.dll>

bool SortFeatureDataByMass(clsFeatureData &a, clsFeatureData &b)
{
	/*if (a.mdouble_mass <= b.mdouble_mass)
		return true; 
	if (a.mdouble_mass > b.mdouble_mass)
		return false; 
	return a.mdouble_net <= b.mdouble_net;  */
	return (a.mdouble_mass < b.mdouble_mass);
}
