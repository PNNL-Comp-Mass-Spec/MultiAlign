#pragma once

namespace MultiAlignEngine
{
	namespace MassTags
	{
		[System::Serializable]
		public __gc class clsProtein
		{
		public:
			int mintRefID; 
			System::String* mstrProteinName; 
			clsProtein(void);
			~clsProtein(void);

			__property int get_Id()
			{
				return mintRefID;
			}

			__property void set_Id(int value){
				mintRefID = value;
			}

			__property System::String* get_ProteinString()
			{
				return mstrProteinName;
			}

			__property void set_ProteinString(System::String* value){
				mstrProteinName = value;
			}
		};
	}
}
