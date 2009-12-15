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

			__property int get_Id()
			{
				return mintMassTagId;
			}

			__property void set_Id(int value){
				mintMassTagId = value;
			}

			__property double get_Mass()
			{
				return mdblMonoMass;
			}

			__property void set_Mass(double value){
				mdblMonoMass = value;
			}

			__property double get_NetAverage()
			{
				return mdblAvgGANET;
			}

			__property void set_NetAverage(double value){
				mdblAvgGANET = value;
			}

			__property double get_NetPredicted()
			{
				return mdblPNET;
			}

			__property void set_NetPredicted(double value){
				mdblPNET = value;
			}

			__property double get_NetStandardDeviation()
			{
				return mdblStdGANET;
			}

			__property void set_NetStandardDeviation(double value){
				mdblStdGANET = value;
			}

			__property double get_XCorr()
			{
				return mdblHighXCorr;
			}

			__property void set_XCorr(double value){
				mdblHighXCorr = value;
			}

			__property double get_DiscriminantMax()
			{
				return mdblMaxDiscriminant;
			}

			__property void set_DiscriminantMax(double value){
				mdblMaxDiscriminant = value;
			}

			__property float get_Charge1FScore()
			{
				return mfltAvgFCS1;
			}

			__property void set_Charge1FScore(float value){
				mfltAvgFCS1 = value;
			}

			__property float get_Charge2FScore()
			{
				return mfltAvgFCS2;
			}

			__property void set_Charge2FScore(float value){
				mfltAvgFCS2 = value;
			}

			__property float get_Charge3FScore()
			{
				return mfltAvgFCS3;
			}

			__property void set_Charge3FScore(float value){
				mfltAvgFCS3 = value;
			}

			__property short get_CleavageState()
			{
				return mshortCleavageState;
			}

			__property void set_CleavageState(short value){
				mshortCleavageState = value;
			}

			__property System::String* get_Modifications()
			{
				return mstrModification;
			}

			__property void set_Modifications(System::String* value){
				mstrModification = value;
			}

		};
	}
}