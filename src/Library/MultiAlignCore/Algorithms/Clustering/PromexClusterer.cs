using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassFeature;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;

namespace MultiAlignCore.Algorithms.Clustering
{
    public class PromexClusterer : IClusterer<UMCLight, UMCClusterLight>
    {
        private InformedProteomicsReader reader;

        public PromexClusterer(InformedProteomicsReader reader)
        {
            this.reader = reader;
        }

        public event EventHandler<ProgressNotifierArgs> Progress;
        public FeatureClusterParameters<UMCLight> Parameters { get; set; }
        public List<UMCClusterLight> Cluster(List<UMCLight> data, List<UMCClusterLight> clusters, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public List<UMCClusterLight> Cluster(List<UMCLight> data, IProgress<ProgressData> progress = null)
        {
            var lcmsFeatureAligner =
                new LcMsFeatureAlignment(new LcMsFeatureAlignComparer(new Tolerance(10, ToleranceUnit.Ppm)));

            // Group features by dataset
            var idToFeatures = new Dictionary<int, List<UMCLight>>();
            foreach (var umcLight in data)
            {
                if (!idToFeatures.ContainsKey(umcLight.GroupId))
                {
                    idToFeatures.Add(umcLight.GroupId, new List<UMCLight>());
                }

                idToFeatures[umcLight.GroupId].Add(umcLight);
            }

            // Convert UMCLights to InformedProteomics LcMsFeatures
            foreach (var ds in idToFeatures)
            {
                var lcmsFeatures = new List<LcMsFeature>(ds.Value.Select(umcLight => new LcMsFeature(
                                                    umcLight.MassMonoisotopic,
                                                    umcLight.ChargeState,
                                                    umcLight.Mz,
                                                    umcLight.Scan,
                                                    umcLight.Abundance)));
                lcmsFeatureAligner.AddDataSet(ds.Key, lcmsFeatures, this.reader.GetReaderForGroup(ds.Key));
            }

            // Perform clustering
            lcmsFeatureAligner.AlignFeatures();
            var clusteredFeatures = lcmsFeatureAligner.GetAlignedFeatures();
            
            // Convert InformedProteomics clusters to UMCClusterLight
            int clustId = 0, umcId = 0, msId = 0;
            var clusters = new List<UMCClusterLight>();
            foreach (var cluster in clusteredFeatures)
            {
                if (cluster == null || cluster.Length == 0)
                {
                    continue;
                }

                var umcCluster = new UMCClusterLight
                {
                    GroupId = cluster[0].DataSetId,
                    Id = clustId++,
                };
                
                foreach (var feature in cluster)
                {
                    var umcs = this.GetUMCs(feature, ref umcId, ref msId);
                    foreach (var umc in umcs)
                    {
                        umcCluster.AddChildFeature(umc);
                    }
                }

                umcCluster.CalculateStatistics(ClusterCentroidRepresentation.Median);
                clusters.Add(umcCluster);
            }

            return clusters;
        }

        public void ClusterAndProcess(List<UMCLight> data, IClusterWriter<UMCClusterLight> writer, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        private List<UMCLight> GetUMCs(LcMsFeature lcmsFeature, ref int umcId, ref int msId)
        {
            var umcLights = new List<UMCLight>();
            for (int chargestate = lcmsFeature.MinCharge; chargestate <= lcmsFeature.MaxCharge; chargestate++)
            {
                // Parent feature
                var umcLight = new UMCLight
                {
                    Id = umcId++,
                    GroupId = lcmsFeature.DataSetId,
                    ScanStart = lcmsFeature.MinScanNum,
                    ScanEnd = lcmsFeature.MaxScanNum,
                    Abundance = lcmsFeature.Abundance,
                    AbundanceSum = lcmsFeature.Abundance,
                    ChargeState = chargestate,
                    Net = lcmsFeature.Net,
                    NetAligned = lcmsFeature.Net,
                };

                // Add min point
                umcLight.AddChildFeature(new MSFeatureLight
                {
                    Id = msId++,
                    GroupId = lcmsFeature.DataSetId,
                    Scan = lcmsFeature.MinScanNum,
                    Abundance = lcmsFeature.Abundance,
                    ChargeState = chargestate,
                    Net = lcmsFeature.MinNet
                });

                // Add max point
                umcLight.AddChildFeature(new MSFeatureLight
                {
                    Id = msId++,
                    GroupId = lcmsFeature.DataSetId,
                    Scan = lcmsFeature.MaxScanNum,
                    Abundance = lcmsFeature.Abundance,
                    ChargeState = chargestate,
                    Net = lcmsFeature.MinNet
                });

                umcLights.Add(umcLight);
            }

            return umcLights;
        }
    }
}
