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
        public InformedProteomicsReader Reader { get; set; }

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
                var lcmsFeatures = new List<LcMsFeature>(ds.Value.Select(this.GetLcMsFeature));
                lcmsFeatureAligner.AddDataSet(ds.Key, lcmsFeatures, this.Reader.GetReaderForGroup(ds.Key));
            }

            // Perform clustering
            lcmsFeatureAligner.AlignFeatures();
            var clusteredFeatures = lcmsFeatureAligner.GetAlignedFeatures();
            
            // Convert InformedProteomics clusters to UMCClusterLight
            int clustId = 0, umcId = 0, msId = 0;
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
                    GroupId = firstFeature.DataSetId,
                    Id = clustId++,
                };
                
                foreach (var feature in cluster)
                {
                    if (feature == null)
                    {
                        continue;
                    }

                    umcCluster.AddChildFeature(this.GetUMC(feature, ref umcId, ref msId));
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
            int minCharge = Int32.MaxValue, maxCharge = 0;
            foreach (var msFeature in umcLight.MsFeatures)
            {
                minCharge = Math.Min(minCharge, msFeature.ChargeState);
                maxCharge = Math.Max(maxCharge, msFeature.ChargeState);
            }

            return new LcMsFeature(
                                    umcLight.MassMonoisotopic,
                                    umcLight.ChargeState,
                                    umcLight.Mz,
                                    umcLight.Scan,
                                    umcLight.Abundance,
                                    minCharge,
                                    maxCharge,
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

        private UMCLight GetUMC(LcMsFeature lcmsFeature, ref int umcId, ref int msId)
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
                ChargeState = (lcmsFeature.MinCharge + lcmsFeature.MaxCharge) / 2,
                Net = lcmsFeature.Net,
                NetStart = lcmsFeature.MinNet,
                NetEnd = lcmsFeature.MaxNet,
                NetAligned = lcmsFeature.Net,
                MassMonoisotopic = lcmsFeature.Mass,
                MassMonoisotopicAligned = lcmsFeature.Mass,
            };
            for (int chargestate = lcmsFeature.MinCharge; chargestate <= lcmsFeature.MaxCharge; chargestate++)
            {

                // Add min point
                umcLight.AddChildFeature(new MSFeatureLight
                {
                    Id = msId++,
                    GroupId = lcmsFeature.DataSetId,
                    Scan = lcmsFeature.MinScanNum,
                    Abundance = lcmsFeature.Abundance,
                    ChargeState = chargestate,
                    Net = lcmsFeature.MinNet,
                    MassMonoisotopic = lcmsFeature.Mass,
                    MassMonoisotopicAligned = lcmsFeature.Mass,
                });

                // Add max point
                umcLight.AddChildFeature(new MSFeatureLight
                {
                    Id = msId++,
                    GroupId = lcmsFeature.DataSetId,
                    Scan = lcmsFeature.MaxScanNum,
                    Abundance = lcmsFeature.Abundance,
                    ChargeState = chargestate,
                    Net = lcmsFeature.MaxNet,
                    MassMonoisotopic = lcmsFeature.Mass,
                    MassMonoisotopicAligned = lcmsFeature.Mass,
                });
            }

            return umcLight;
        }
    }
}
