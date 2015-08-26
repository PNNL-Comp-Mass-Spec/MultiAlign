#region

using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Hibernate;

#endregion

namespace MultiAlignCore.IO.Analysis
{
    /// <summary>
    ///     Writes peak match results to the local database.
    /// </summary>
    public sealed class PeakMatchResultsWriter
    {
        /// <summary>
        ///     Writes the peak matching results to the local peak matching database.
        /// </summary>
        public void WritePeakMatchResults(PeakMatchingResults<UMCClusterLight, MassTagLight> results,
            MassTagDatabase database,
            out int matchedMassTags,
            out int matchedProteins)
        {
            var clusterToMassTagMapDaoHibernate = new ClusterToMassTagMapDAOHibernate();

            var stacFdrdaoHibernate = new STACDAOHibernate();
            var massTagList = new List<MassTagLight>();
            var proteinList = new Dictionary<int, Protein>();
            var clusterToMassTagMapList = new List<ClusterToMassTagMap>();

            clusterToMassTagMapDaoHibernate.ClearAll();
            stacFdrdaoHibernate.ClearAll();

            foreach (var match in results.Matches)
            {
                var tag = match.Target;
                var feature = match.Observed;
                var clusterToMassTagMap = new ClusterToMassTagMap(feature.Id, tag.Id);
                clusterToMassTagMap.ConformerId = tag.ConformationId;

                if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
                {
                    clusterToMassTagMapList.Add(clusterToMassTagMap);
                    clusterToMassTagMap.StacScore = match.Confidence;
                    clusterToMassTagMap.StacUP = match.Uniqueness;
                }

                if (!massTagList.Contains(tag))
                {
                    massTagList.Add(tag);
                }

                var databaseContainsProtein = database.Proteins.ContainsKey(tag.Id);
                if (databaseContainsProtein)
                {
                    var proteins = database.Proteins[tag.Id];
                    foreach (var protein in proteins.Where(protein => !proteinList.ContainsKey(protein.ProteinId)))
                    {
                        proteinList.Add(protein.ProteinId, protein);
                    }
                }
            }

            var uniqueProteins = new List<Protein>();
            foreach (var protein in proteinList.Values)
            {
                uniqueProteins.Add(protein);
            }

            matchedMassTags = massTagList.Count;
            matchedProteins = uniqueProteins.Count;
            clusterToMassTagMapDaoHibernate.AddAll(clusterToMassTagMapList);
            stacFdrdaoHibernate.AddAll(results.FdrTable);
        }
    }
}