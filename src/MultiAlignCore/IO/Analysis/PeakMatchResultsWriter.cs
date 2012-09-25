using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features.Hibernate;
using PNNLOmics.Algorithms.FeatureMatcher.Data;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.IO
{
    /// <summary>
    /// Writes peak match results to the local database.
    /// </summary>
    public class PeakMatchResultsWriter
    {
        /// <summary>
        /// Writes the peak matching results to the local peak matching database.
        /// </summary>
        /// <param name="analysis"></param>
        public void WritePeakMatchResults(  PeakMatchingResults<UMCClusterLight, MassTagLight> results,
                                            MassTagDatabase database,
                                            out int         matchedMassTags, 
                                            out int         matchedProteins)
        {
            List<MassTagLight> massTagArray = database.MassTags;

            ClusterToMassTagMapDAOHibernate clusterToMassTagMapDAOHibernate = new ClusterToMassTagMapDAOHibernate();
            GenericDAOHibernate<MassTagToProteinMap> massTagToProteinMapDAOHibernate =
                                new GenericDAOHibernate<MassTagToProteinMap>();

            STACDAOHibernate stacFDRDAOHibernate                = new STACDAOHibernate();
            List<MassTagLight> massTagList                      = new List<MassTagLight>();
            Dictionary<int, Protein> proteinList                = new Dictionary<int,Protein>();
            List<ClusterToMassTagMap> clusterToMassTagMapList   = new List<ClusterToMassTagMap>();
            List<STACFDR> stacFDRResultsList                    = new List<STACFDR>();

            clusterToMassTagMapDAOHibernate.ClearAll();
            stacFDRDAOHibernate.ClearAll();
            
            foreach (FeatureMatchLight<UMCClusterLight, MassTagLight> match in results.Matches)
            {
                MassTagLight         tag                = match.Target;
                UMCClusterLight feature                 = match.Observed;
                ClusterToMassTagMap clusterToMassTagMap = new ClusterToMassTagMap(feature.ID, tag.ID);
                clusterToMassTagMap.ConformerId         = tag.ConformationID;

                if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
                {
                    clusterToMassTagMapList.Add(clusterToMassTagMap);                        
                    clusterToMassTagMap.StacScore   = match.Confidence;
                    clusterToMassTagMap.StacUP      = match.Uniqueness;                        
                }

                if (!massTagList.Contains(tag))
                {
                    massTagList.Add(tag);
                }

                bool databaseContainsProtein = database.Proteins.ContainsKey(tag.ID);
                if (databaseContainsProtein)
                {
                    List<Protein> proteins = database.Proteins[tag.ID];
                    foreach (Protein protein in proteins)
                    {
                        MassTagToProteinMap massTagToProteinMap = new MassTagToProteinMap(tag.ID, protein.ProteinID, protein.RefID);
                        
                        if (!proteinList.ContainsKey(protein.ProteinID))
                        {
                            proteinList.Add(protein.ProteinID, protein);
                        }
                    }
                }
            }            

            List<Protein> uniqueProteins = new List<Protein>();
            foreach (Protein protein in proteinList.Values)
            {
                uniqueProteins.Add(protein);
            }

            matchedMassTags = massTagList.Count;
            matchedProteins = uniqueProteins.Count;                        
            clusterToMassTagMapDAOHibernate.AddAll(clusterToMassTagMapList);
            stacFDRDAOHibernate.AddAll(results.FdrTable);            
        }
    }
}
