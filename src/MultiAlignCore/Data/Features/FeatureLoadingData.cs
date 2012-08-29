using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Data.Features
{
    public class LoadedFeatureData
    {
        public IEnumerable<UMCLight> Features
        {
            get;
            set;
        }
        public IEnumerable<MSFeatureLight> MSFeatures
        {
            get;
            set;
        }
    }
}
