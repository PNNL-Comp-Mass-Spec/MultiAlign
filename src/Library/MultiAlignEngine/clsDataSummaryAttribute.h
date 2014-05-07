#pragma once
#using <mscorlib.dll>
using namespace System; 

namespace MultiAlignEngine
{
	//[System::AttributeUsage(System::AttributeTargets::Property | System::AttributeTargets::Field, AllowMultiple = true)]
	[ AttributeUsageAttribute(System::AttributeTargets::All, AllowMultiple = true)]
	public __gc class clsDataSummaryAttribute : 
		public System::Attribute
	{
	public:
		
		System::String *Description; 
		clsDataSummaryAttribute(System::String *str);
		~clsDataSummaryAttribute(void);
	};
}