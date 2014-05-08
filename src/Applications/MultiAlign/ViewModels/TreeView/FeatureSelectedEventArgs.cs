using System;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels.TreeView
{
    public class ClusterSelectedEventArgs : EventArgs
    {
        public ClusterSelectedEventArgs(UMCClusterTreeViewModel cluster)
        {
            Cluster = cluster;
        }

        public UMCClusterTreeViewModel Cluster { get; private set; }
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
            Feature = feature;
            Spectrum = spectrum;
            Peptide = id;
        }

        public UMCLight Feature { get; private set; }
        public MSSpectra Spectrum { get; private set; }
        public Peptide Peptide { get; private set; }
    }
}