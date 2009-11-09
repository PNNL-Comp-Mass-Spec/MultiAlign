#pragma once

#using <mscorlib.dll>
using namespace System;

namespace MultiAlignEngine
{
	namespace Alignment
	{
		[Serializable]
		public __gc class classAlignmentResidualData
		{
			public:
                float scans			       __gc[];
                float mz		           __gc[];
                float linearNet            __gc[];
                float customNet            __gc[];
                float linearCustomNet      __gc[];
                float massError            __gc[];
                float massErrorCorrected   __gc[];
                float mzMassError          __gc[];
                float mzMassErrorCorrected __gc[];
		};
	}
}
	