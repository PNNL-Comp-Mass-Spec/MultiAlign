#pragma once
#include <string> 

namespace MultiAlignEngine
{
	namespace MassTags
	{
		class MassTag
		{
		public:
			int mintMassTagId; 
			std::string mstrPeptide; 
			std::string mstrModification; 
			double mdblMonoMass; 
			double mdblAvgGANET; 
			double mdblPNET; 
			double mdblHighXCorr; 
			double mdblStdGANET; 
			double mdblMaxDiscriminant; 
			int mintNumObsPassingFilter; 
			short mshortModCount; 
			float mfltAvgFCS1; 
			float mfltAvgFCS2; 
			float mfltAvgFCS3; 
			short mshortCleavageState; 
			MassTag(); 
			~MassTag(); 
		};
	}
}
