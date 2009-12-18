#pragma once

namespace MultiAlignEngine
{
	namespace MassTags
	{
		[System::Serializable]
		public __gc class clsModification
		{
		public:
			int mintId; 
			System::String* mstrDescription; 
			clsModification(void);
			~clsModification(void);

			__property int get_Id()
			{
				return mintId;
			}

			__property void set_Id(int value){
				mintId = value;
			}

			__property System::String* get_Description()
			{
				return mstrDescription;
			}

			__property void set_Description(System::String* value){
				mstrDescription = value;
			}
		};
	}
}
