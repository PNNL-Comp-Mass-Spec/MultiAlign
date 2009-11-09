#pragma once
#include <string> 

namespace MultiAlignEngine
{
	namespace MassTags
	{
		class Protein
		{
		public:
			int mintRefID; 
			std::string mstrName; 
			Protein(); 
			~Protein(); 
		};
	}
}
