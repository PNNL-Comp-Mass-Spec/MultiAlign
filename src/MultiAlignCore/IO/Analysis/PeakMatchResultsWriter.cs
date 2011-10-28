using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using PNNLProteomics.SMART;

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
        public void WritePeakMatchResults(  MultiAlignAnalysis analysis, 
                                            List<PNNLOmics.Algorithms.FeatureMatcher.Data.STACFDR> fdrTable,
                                            out int matchedMassTags, 
                                            out int matchedProteins)
        {
            List<MassTagLight> massTagArray = analysis.MassTagDatabase.MassTags;           

            //TODO: Fix this with the providers.
            IMassTagDAO massTagDAOHibernate = new MassTagDAO();
            IProteinDAO proteinDAOHibernate = new ProteinDAO();

            GenericDAOHibernate<ClusterToMassTagMap> clusterToMassTagMapDAOHibernate =
                                new GenericDAOHibernate<ClusterToMassTagMap>();
            GenericDAOHibernate<MassTagToProteinMap> massTagToProteinMapDAOHibernate =
                                new GenericDAOHibernate<MassTagToProteinMap>();

            GenericDAOHibernate<SmartFDR> stacFDRDAOHibernate    = new GenericDAOHibernate<SmartFDR>();
            List<MassTagLight> massTagList                      = new List<MassTagLight>();
            Dictionary<int, Protein> proteinList                = new Dictionary<int,Protein>();
            List<ClusterToMassTagMap> clusterToMassTagMapList   = new List<ClusterToMassTagMap>();
            List<MassTagToProteinMap> massTagToProteinMapList   = new List<MassTagToProteinMap>();
            List<SmartFDR> stacFDRResultsList                    = new List<SmartFDR>();

            foreach (FeatureMatchLight<UMCClusterLight, MassTagLight> match in analysis.PeakMatchingResults)
            {
                MassTagLight         tag                = match.Target;
                UMCClusterLight feature                 = match.Observed;
                ClusterToMassTagMap clusterToMassTagMap = new ClusterToMassTagMap(feature.ID, tag.ID);
                clusterToMassTagMap.ConformerId         = tag.ConformationID;
                List<Protein> proteins                  = analysis.MassTagDatabase.Proteins[tag.ID];

                foreach (Protein protein in proteins)
                {
                    MassTagToProteinMap massTagToProteinMap = new MassTagToProteinMap(tag.ID, protein.RefID);
                    if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
                    {
                        clusterToMassTagMapList.Add(clusterToMassTagMap);                        
                        clusterToMassTagMap.StacScore   = match.Confidence;
                        clusterToMassTagMap.StacUP      = match.Uniqueness;                        
                    }

                    if (!massTagToProteinMapList.Contains(massTagToProteinMap))
                    {
                        massTagToProteinMapList.Add(massTagToProteinMap);
                    }

                    if (!massTagList.Contains(tag))
                    {
                        massTagList.Add(tag);
                    }

                    if (!proteinList.ContainsKey(protein.RefID))
                    {
                        proteinList.Add(protein.RefID, protein);
                    }
                }
            }

            /*foreach (classSMARTFdrResult fdrResult in analysis.SMARTResults.GetSummaries())
            {
                stacFDRResultsList.Add(new SmartFDR(fdrResult));
            }*/
            foreach (PNNLOmics.Algorithms.FeatureMatcher.Data.STACFDR fdr in fdrTable)
            {
                classSMARTFdrResult result  = new classSMARTFdrResult();
                result.Cutoff               = fdr.Cutoff;
                result.Error                = 0;
                result.FDR                  = fdr.FDR;
                result.NumMatches           = fdr.AMTMatches;
                
            }

            List<Protein> uniqueProteins = new List<Protein>();
            foreach (Protein protein in proteinList.Values)
            {
                uniqueProteins.Add(protein);
            }

            matchedMassTags = massTagList.Count;
            matchedProteins = uniqueProteins.Count;

            massTagDAOHibernate.AddAll(massTagList);
            proteinDAOHibernate.AddAll(uniqueProteins);
            clusterToMassTagMapDAOHibernate.AddAll(clusterToMassTagMapList);
            massTagToProteinMapDAOHibernate.AddAll(massTagToProteinMapList);
            stacFDRDAOHibernate.AddAll(stacFDRResultsList);            
        }
    }
}
