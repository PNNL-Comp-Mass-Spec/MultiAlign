using System;
using PNNLOmics.Data.MassTags;
using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class MassTagExtesnsions
    {
        /// <summary>
        /// Builds an ID for mapping and caching features
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static object BuildId(this MassTagLight tag)
        {
            return string.Format("{0}-{1}", tag.ID, tag.ConformationID);
        }
        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, int> CreateMassTagClusterSizeHistogram(this List<MassTagToCluster> tags)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();

            foreach (MassTagToCluster tag in tags)
            {
                int size = 0;

                foreach (UMCClusterLightMatched cluster in tag.Matches)
                {
                    size += cluster.Cluster.MemberCount;
                }

                if (size < 1)
                    continue;

                if (!map.ContainsKey(size))
                {
                    map.Add(size, 0);
                }
                map[size]++;
            }

            return map;
        }

        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, int> CreateMassTagMatchedClusterSizeHistogram(this List<MassTagToCluster> tags)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();

            foreach (MassTagToCluster tag in tags)
            {
                int size = 0;

                foreach (UMCClusterLightMatched cluster in tag.Matches)
                {
                    size += cluster.Cluster.MemberCount;
                }

                if (!map.ContainsKey(size))
                {
                    map.Add(size, 0);
                }
                map[size]++;
            }

            return map;
        }    
    }
}
