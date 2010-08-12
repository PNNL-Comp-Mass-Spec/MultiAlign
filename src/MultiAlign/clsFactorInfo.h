#pragma once

namespace MultiAlignEngine
{
	/*	[System::Serializable]
	public __gc class clsFactorInfo
	{			
	public:
		System::String* mstrFactor;
		System::Collections::ArrayList* marrValues; 
		clsFactorInfo(void);
		~clsFactorInfo(void);
		__property System::String*  get_FactorValues() __gc[]
		{
			System::String *values __gc[] = new System::String* __gc[marrValues->get_Count()];
			if (marrValues->get_Count() == 0)
				return 0;
			else
			{
				for (int i = 0; i < marrValues->get_Count(); i++)
					values[i] = marrValues->Item[i]->ToString();
				return values;
			}
		}

		__property int get_vCount()
		{
			return marrValues->get_Count(); 
		}
	};*/
}