using System;
using System.Collections.Generic;

using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features
{
    public interface IFeatureClusterWriter
    {
        void WriteClusters(List<UMCClusterLight> clusters);        
    }
}
