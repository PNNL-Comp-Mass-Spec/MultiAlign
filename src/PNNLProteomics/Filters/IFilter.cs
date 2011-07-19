using System;
using System.Collections.Generic;
using System.Text;

namespace MultiAlignCore.Filters
{
	public interface IFilter<T>
    {
    	bool DoesPassFilter(T t);
        bool Active { get;set;}
	}
}
