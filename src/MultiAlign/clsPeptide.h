#pragma once

namespace MultiAlignEngine
{
	namespace MassTags
	{
		[System::Serializable]
		public __gc class clsPeptide
		{
		public:
			int mintId; 
			System::String* mstrPeptide; 
			clsPeptide(void);
			~clsPeptide(void);

			__property int get_Id()
			{
				return mintId;
			}

			__property void set_Id(int value){
				mintId = value;
			}

			__property System::String* get_PeptideString()
			{
				return mstrPeptide;
			}

			__property void set_PeptideString(System::String* value){
				mstrPeptide = value;
			}
		};
	}
}
