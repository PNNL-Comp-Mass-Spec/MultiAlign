using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassFeature;
using InformedProteomics.Backend.MassSpecData;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;

namespace MultiAlignCore.Algorithms.Clustering
{
    public class PromexClusterer : IClusterer<UMCLight, UMCClusterLight>
    {
        private Dictionary<Tuple<int, int>, UMCLight> featureMap; 

        public InformedProteomicsReader Reader { get; set; }

        public event EventHandler<ProgressNotifierArgs> Progress;
        public FeatureClusterParameters<UMCLight> Parameters { get; set; }
        public List<UMCClusterLight> Cluster(List<UMCLight> data, List<UMCClusterLight> clusters, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public List<UMCClusterLight> Cluster(List<UMCLight> data, IProgress<ProgressData> progress = null)
        {
            this.featureMap = new Dictionary<Tuple<int, int>, UMCLight>();
            foreach (var feature in data)
            {
                var key = new Tuple<int, int>(feature.GroupId, feature.Id);
                this.featureMap.Add(key, feature);
            }

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
                var lcmsFeatures = new List<LcMsFeature>(ds.Value.Select(this.GetLcMsFeature));
                lcmsFeatureAligner.AddDataSet(ds.Key, lcmsFeatures, this.Reader.GetReaderForGroup(ds.Key));
            }

            // Perform clustering
            lcmsFeatureAligner.AlignFeatures();
            var clusteredFeatures = lcmsFeatureAligner.GetAlignedFeatures();
            
            // Convert InformedProteomics clusters to UMCClusterLight
            int clustId = 0;
            var clusters = new List<UMCClusterLight>();
            foreach (var cluster in clusteredFeatures)
            {
                var firstFeature = cluster.FirstOrDefault(f => f != null);
                if (firstFeature == null)
                {
                    continue;
                }

                var umcCluster = new UMCClusterLight
                {
                    Id = clustId++,
                };
                
                foreach (var feature in cluster)
                {
                    if (feature == null)
                    {
                        continue;
                    }

                    var umc = this.GetUMC(feature);
                    umcCluster.AddChildFeature(umc);
                    umc.SetParentFeature(umcCluster);
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

        private LcMsFeature GetLcMsFeature(UMCLight umcLight)
        {
            var lcmsRun = this.Reader.GetReaderForGroup(umcLight.GroupId);
            return new LcMsFeature(
                                    umcLight.MassMonoisotopic,
                                    umcLight.ChargeState,
                                    umcLight.Mz,
                                    umcLight.Scan,
                                    umcLight.Abundance,
                                    umcLight.ChargeState,
                                    umcLight.ChargeState, 
                                    umcLight.ScanStart,
                                    umcLight.ScanEnd,
                                    lcmsRun.GetElutionTime(umcLight.ScanStart),
                                    lcmsRun.GetElutionTime(umcLight.ScanEnd),
                                    umcLight.NetStart,
                                    umcLight.NetEnd)
            {
                DataSetId = umcLight.GroupId,
                FeatureId = umcLight.Id,
            };
        }

        private UMCLight GetUMC(LcMsFeature lcmsFeature)
        {
            var key = new Tuple<int, int>(lcmsFeature.DataSetId, lcmsFeature.FeatureId);
            var umc = this.featureMap[key];
            return umc;
        }
    }
}
