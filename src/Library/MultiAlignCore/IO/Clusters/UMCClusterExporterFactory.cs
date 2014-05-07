using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.Features;
using PNNLOmicsIO.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.IO.Features
{
    public static class UMCClusterExporterFactory
    {
        /// <summary>
        /// Creates a UMC Cluster  exporter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IFeatureClusterWriter Create(ClusterFeatureExporters type)
        {
            IFeatureClusterWriter exporter = null;           
            switch (type)
            {
                case ClusterFeatureExporters.CrossTab:
                    exporter = new UMCClusterCrossTabWriter();
                    break;
                case ClusterFeatureExporters.CrossTabAbundanceSum:
                    exporter = new UMCClusterAbundanceSumCrossTabWriter();
                    break;
                case ClusterFeatureExporters.CrossTabAbundanceSumMax:
                    exporter = new UMCClusterAbundanceCrossTabWriter();
                    break;
                case ClusterFeatureExporters.MsMsMetaData:
                    exporter = new UMCClusterMsmsWriter();
                    break;
                case ClusterFeatureExporters.ClusterScans:
                    exporter = new UMCClusterScanWriter();
                    break;
                case ClusterFeatureExporters.MsMsDta:
                    exporter = new UMCClusterMsmsSpectraWriter(
                                                                "DTA Spectra",
                                                                MsMsFileWriterFactory.CreateSpectraWriter(MsMsWriterType.DTA),
                                                                ".dta");
                    break;
                case ClusterFeatureExporters.MsMsMgf:
                    exporter = new UMCClusterMsmsSpectraWriter(
                                                                "MGF Spectra", 
                                                                MsMsFileWriterFactory.CreateSpectraWriter(MsMsWriterType.MGF),
                                                                ".mgf");
                    break;
                default:
                    break;
            }

            return exporter;
        }
        /// <summary>
        /// Creates all UMC Cluster exporters
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IFeatureClusterWriter> Create()
        {
            List<IFeatureClusterWriter> exporters = new List<IFeatureClusterWriter>();
            exporters.Add(Create(ClusterFeatureExporters.ClusterScans));
            exporters.Add(Create(ClusterFeatureExporters.CrossTabAbundanceSum));
            exporters.Add(Create(ClusterFeatureExporters.CrossTabAbundanceSumMax));
            exporters.Add(Create(ClusterFeatureExporters.CrossTab));
            exporters.Add(Create(ClusterFeatureExporters.MsMsMetaData));
            exporters.Add(Create(ClusterFeatureExporters.MsMsMgf));
            exporters.Add(Create(ClusterFeatureExporters.MsMsDta));
            return exporters;
        }
    }

    public enum ClusterFeatureExporters
    {
        CrossTab,
        CrossTabAbundanceSum,
        CrossTabAbundanceSumMax,
        MsMsMetaData,
        MsMsMgf,
        MsMsDta,
        ClusterScans
    }
}
