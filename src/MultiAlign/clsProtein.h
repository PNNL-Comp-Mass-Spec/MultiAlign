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
		};
	}
}
