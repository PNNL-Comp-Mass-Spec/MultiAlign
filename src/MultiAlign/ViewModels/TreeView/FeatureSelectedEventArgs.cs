using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using MultiAlign.Windows.Plots;
using PNNLOmics.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlign.ViewModels.TreeView
{

    public class ClusterSelectedEventArgs : EventArgs
    {
        public ClusterSelectedEventArgs(UMCClusterLightMatched cluster)
        {
            Cluster = cluster;
        }
        public UMCClusterLightMatched Cluster { get; private set; }
    }


    public class FeatureSelectedEventArgs : EventArgs
    {
        public FeatureSelectedEventArgs(UMCLight feature)
        {
            Feature = feature;
        }
        public UMCLight Feature { get; private set; }
    }

    public class IdentificationFeatureSelectedEventArgs : EventArgs
    {
        public IdentificationFeatureSelectedEventArgs(MSSpectra spectrum, Peptide id, UMCLight feature)
        {
            Feature     = feature;
            Spectrum    = spectrum;
            Peptide     = id;
        }
        public UMCLight Feature { get; private set; }
        public MSSpectra Spectrum { get; private set; }
        public Peptide Peptide { get; private set; }
    }
}
