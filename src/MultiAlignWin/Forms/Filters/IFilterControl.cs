using System;
using System.Collections.Generic;
using System.Text;


using MultiAlignEngine.Features;
using PNNLProteomics.Filters;

namespace MultiAlignWin.Forms.Filters
{
    public interface IFilterControl<T>
    {
        IFilter<T> Filter { get; set;}        
    }
}
