using System.Collections.Generic;
using System.IO;
using System.Text;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

namespace MultiAlignChargeStateProcessor
{
    /// <summary>
    /// UMC Cluster writer class.
    /// </summary>
    public sealed class UmcClusterWriter : IClusterWriter<UMCClusterLight>
    {
        private TextWriter m_writer;
        private TextWriter m_idMapper;        
        private readonly List<int> m_ids;

        public UmcClusterWriter()
        {            
            m_ids       = new List<int>();                        
        }

        public void Open(string path)
        {
            var folder   = Path.GetDirectoryName(path);
            var name     = Path.GetFileNameWithoutExtension(path);
            m_writer        = File.CreateText(path);
            if (folder != null) 
                m_idMapper      = File.CreateText(Path.Combine(folder, name + "_map.txt"));
            m_idMapper.WriteLine("Cluster, Dataset, Feature");
        }
        #region IClusterWriter<UMCClusterLight> Members

        public void Close()
        {
            m_writer.Close();
            m_idMapper.Close();
        }

        public void WriteHeader(IEnumerable<DatasetInformation> datasets)
        {
            var header = "Cluster ID, Total Members, Dataset Members, Tightness, Ambiguity, Mass, NET, DriftTime,";

            var builder = new StringBuilder();
            foreach (var information in datasets)
            {
                m_ids.Add(information.DatasetId);
            }
            m_ids.Sort();
            foreach (var id in m_ids)
            {
                builder.AppendFormat("AbundanceSum-{0},", id);
            }

            header += builder.ToString();
            m_writer.WriteLine(header);

        }

        private static int m_count;
        public void WriteCluster(UMCClusterLight cluster)
        {
            m_count++;

            var idMapper = new StringBuilder();
            var builder = new StringBuilder();

            builder.AppendFormat("{0},{1},{2},{3:.000},{4:.000},{5:.0000},{6:.0000},{7:.0000},",
                cluster.Id,
                cluster.MemberCount,
                cluster.DatasetMemberCount,
                cluster.Tightness,
                cluster.AmbiguityScore,
                cluster.MassMonoisotopicAligned,
                cluster.Net,
                cluster.DriftTime);

            var clustermap = new Dictionary<int, double>();
            foreach (var feature in cluster.Features)
            {
                if (clustermap.ContainsKey(feature.GroupId))
                {
                    clustermap[feature.GroupId] += feature.AbundanceSum;
                }
                else
                {
                    clustermap.Add(feature.GroupId, feature.AbundanceSum);
                }
                idMapper.AppendFormat("{0},{1},{2}\n\r", cluster.Id, feature.GroupId, feature.Id);
            }

            foreach (var did in m_ids)
            {
                // If the cluster does not have an entry for this, then leave it
                if (clustermap.ContainsKey(did))
                {
                    builder.AppendFormat("{0},", clustermap[did]);
                }
                else
                {
                    builder.AppendFormat(",");
                }
            }
            m_writer.WriteLine(builder.ToString());
            m_idMapper.Write(idMapper.ToString());
            builder.Clear();
            idMapper.Clear();

            if (m_count <= 1000)
                return;

            m_count = 0;
            m_writer.Flush();
            m_idMapper.Flush();
        }

        #endregion
    }
}