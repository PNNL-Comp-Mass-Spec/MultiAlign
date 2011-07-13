using System.Collections.Generic;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data;
using PNNLProteomics.MultiAlign.Hibernate.Domain;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;
using PNNLProteomics.SMART;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignEngine.PeakMatching;

namespace PNNLProteomics.IO
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
        public void WritePeakMatchResults(MultiAlignAnalysis analysis, out int matchedMassTags, out int matchedProteins)
        {
            List<MassTag> massTagArray = analysis.MassTagDatabase.MassTags;           

            //TODO: Fix this with the providers.
            IMassTagDAO massTagDAOHibernate = new MassTagDAO();
            IProteinDAO proteinDAOHibernate = new ProteinDAO();

            GenericDAOHibernate<ClusterToMassTagMap> clusterToMassTagMapDAOHibernate =
                                new GenericDAOHibernate<ClusterToMassTagMap>();
            GenericDAOHibernate<MassTagToProteinMap> massTagToProteinMapDAOHibernate =
                                new GenericDAOHibernate<MassTagToProteinMap>();

            GenericDAOHibernate<StacFDR> stacFDRDAOHibernate    = new GenericDAOHibernate<StacFDR>();
            List<MassTag> massTagList                           = new List<MassTag>();
            List<Protein> proteinList                           = new List<Protein>();
            List<ClusterToMassTagMap> clusterToMassTagMapList   = new List<ClusterToMassTagMap>();
            List<MassTagToProteinMap> massTagToProteinMapList   = new List<MassTagToProteinMap>();
            List<StacFDR> stacFDRResultsList                    = new List<StacFDR>();

            foreach (MassTagFeatureMatch<UMCClusterLight> match in analysis.PeakMatchingResults)
            {
                MassTag         tag                     = match.Tag;
                UMCClusterLight feature                 = match.Feature;
                ClusterToMassTagMap clusterToMassTagMap = new ClusterToMassTagMap(feature.ID, tag.ID);
                List<Protein> proteins                  = analysis.MassTagDatabase.Proteins[tag.ID];

                foreach (Protein protein in proteins)
                {
                    MassTagToProteinMap massTagToProteinMap = new MassTagToProteinMap(tag.ID, protein.RefID);
                    if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
                    {
                        clusterToMassTagMapList.Add(clusterToMassTagMap);

                        if (analysis.PeakMatchingOptions.UseSTAC)
                        {                            
                            // See if a STAC score exists                            
                            List<PNNLProteomics.SMART.classSMARTProbabilityResult> smartScores = null;
                            smartScores = analysis.STACResults.GetResultFromUMCIndex(feature.ID);

                            if (smartScores != null)
                            {
                                // Then pull out the STAC score that matches for this triplet Mass Tag
                                PNNLProteomics.SMART.classSMARTProbabilityResult finalResult = null;
                                foreach (PNNLProteomics.SMART.classSMARTProbabilityResult score in smartScores)
                                {
                                    if (score.MassTagID == tag.ID)
                                    {
                                        finalResult = score;
                                        break;
                                    }
                                }

                                if (finalResult != null)
                                {
                                    clusterToMassTagMap.StacScore = finalResult.Score;
                                    clusterToMassTagMap.StacUP = finalResult.Specificity;
                                }
                            }
                        }

                    }

                    if (!massTagToProteinMapList.Contains(massTagToProteinMap))
                    {
                        massTagToProteinMapList.Add(massTagToProteinMap);
                    }

                    if (!massTagList.Contains(tag))
                    {
                        massTagList.Add(tag);
                    }

                    if (!proteinList.Contains(protein))
                    {
                        proteinList.Add(protein);
                    }
                }
            }

            foreach (classSMARTFdrResult fdrResult in analysis.STACResults.GetSummaries())
            {
                stacFDRResultsList.Add(new StacFDR(fdrResult));
            }

            matchedMassTags = massTagList.Count;
            matchedProteins = proteinList.Count;


            massTagDAOHibernate.AddAll(massTagList);
            proteinDAOHibernate.AddAll(proteinList);
            clusterToMassTagMapDAOHibernate.AddAll(clusterToMassTagMapList);
            massTagToProteinMapDAOHibernate.AddAll(massTagToProteinMapList);
            stacFDRDAOHibernate.AddAll(stacFDRResultsList);            
        }
    }
}
