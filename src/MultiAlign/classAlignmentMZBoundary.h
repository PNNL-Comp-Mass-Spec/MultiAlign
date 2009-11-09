#pragma once

#using <mscorlib.dll>
using namespace System;

#include "clsDataSummaryAttribute.h"
#include "clsParameterFileAttribute.h"

namespace MultiAlignEngine
{
		
	namespace Alignment
	{
		/*
			Class that defines the boundary of an m/z range to align to.

			Holds a low boundary and a high boundary.
		*/ 
		[Serializable]
		public __gc class classAlignmentMZBoundary
		{
			private:
				double mdouble_boundaryLow;
				double mdouble_boundaryHigh;

			public:

				/// constructor that takes a low and high boundary range.
				classAlignmentMZBoundary(double low, double high)
				{
					mdouble_boundaryHigh = high;
					mdouble_boundaryLow  = low; 
				}

				[clsDataSummaryAttribute("m/z boundary low")]
				[clsParameterFileAttribute("mzBoundaryLow","AlignmentOptions")]
				__property double get_LowBoundary()
				{
					return mdouble_boundaryLow; 
				}
				__property void set_LowBoundary(double value)
				{				
					mdouble_boundaryLow = value;
				}
				
				[clsDataSummaryAttribute("m/z boundary high")]
				[clsParameterFileAttribute("mzBoundaryHigh","AlignmentOptions")]
				__property double get_HighBoundary()
				{
					return mdouble_boundaryHigh; 
				}
				__property void set_HighBoundary(double value)
				{				
					mdouble_boundaryHigh = value;
				}
		};
	}
}
			