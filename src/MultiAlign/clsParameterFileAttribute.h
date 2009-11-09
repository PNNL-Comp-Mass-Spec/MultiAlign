#pragma once
#using <mscorlib.dll>
using namespace System; 

namespace MultiAlignEngine
{
	[AttributeUsage(System::AttributeTargets::All, AllowMultiple = true)]
	public __gc class clsParameterFileAttribute : public System::Attribute
	{
	public:
		System::String *Description; /// Description of the parameter.
		System::String *GroupName; 
		clsParameterFileAttribute(System::String *str, System::String *groupName);			
		~clsParameterFileAttribute(void);
	};
}