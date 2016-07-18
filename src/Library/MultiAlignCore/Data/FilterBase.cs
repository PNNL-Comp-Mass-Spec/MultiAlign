using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLProteomics.Data
{
    public abstract class FilterBase<T>: IComparer<T>
    {
        public abstract int Compare(T x, T y);
    }
}
