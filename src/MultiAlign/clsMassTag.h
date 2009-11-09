#pragma once
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	namespace MassTags
	{
		[System::Serializable]
		public __gc class clsMassTag : public System::IComparable
		{
		public:
			int CompareTo(System::Object* obj)
			{ 
				clsMassTag *u = dynamic_cast<clsMassTag *>(obj); 
				if (mdblMonoMass == u->mdblMonoMass) 
				{
					if (mdblAvgGANET == u->mdblAvgGANET)
						return 0;
					if (mdblAvgGANET < u->mdblAvgGANET)
						return -1; 
					return 1; 
				} 
				if (mdblMonoMass < u->mdblMonoMass) 
					return -1; 
				return 1; 
			} 
			int mintMassTagId; 
			System::String *mstrPeptide; 
			System::String *mstrModification; 
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
			clsMassTag(); 
			~clsMassTag(); 
		};
	}
}