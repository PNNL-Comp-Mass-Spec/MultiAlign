#include "StdAfx.h"
#include ".\clsParameterFileAttribute.h"
#using <mscorlib.dll>

namespace MultiAlignEngine
{
	clsParameterFileAttribute::clsParameterFileAttribute(System::String *s, System::String *groupName):Description(s), GroupName(groupName)
	{
				
	}

	clsParameterFileAttribute::~clsParameterFileAttribute(void)
	{
	}
}
