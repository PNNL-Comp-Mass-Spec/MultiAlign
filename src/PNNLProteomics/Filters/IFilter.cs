using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLProteomics.Filters
{
	public interface IFilter<T>
    {
    	bool DoesPassFilter(T t);
        bool Active { get;set;}
	}
}
