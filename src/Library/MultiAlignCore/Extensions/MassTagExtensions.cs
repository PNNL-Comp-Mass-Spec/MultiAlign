#region

using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Extensions
{
    public static class MassTagExtensions
    {
        /// <summary>
        /// Builds an ID for mapping and caching features
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static object BuildId(this MassTagLight tag)
        {
            return string.Format("{0}-{1}", tag.Id, tag.ConformationId);
        }

        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, int> CreateMassTagClusterSizeHistogram(this List<MassTagToCluster> tags)
        {
            var map = new Dictionary<int, int>();

            foreach (var tag in tags)
            {
                var size = 0;

                foreach (var cluster in tag.Matches)
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
            var map = new Dictionary<int, int>();

            foreach (var tag in tags)
            {
                var size = 0;

                foreach (var cluster in tag.Matches)
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