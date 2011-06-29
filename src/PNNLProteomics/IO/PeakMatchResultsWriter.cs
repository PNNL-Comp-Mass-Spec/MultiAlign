using System.Collections.Generic;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data;
using PNNLProteomics.MultiAlign.Hibernate.Domain;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;
using PNNLProteomics.SMART;
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
            clsMassTag[] massTagArray = null; // analysis.PeakMatchingResults.marrMasstags;
            clsProtein[] proteinArray = null; // analysis.PeakMatchingResults.marrProteins;            

            massTagArray = analysis.PeakMatchingResults.marrMasstags;
            proteinArray = analysis.PeakMatchingResults.marrProteins;


            //TODO: Fix this with the providers.
            IMassTagDAO massTagDAOHibernate = new MassTagDAOHibernate();
            IProteinDAO proteinDAOHibernate = new ProteinDAOHibernate();
            GenericDAOHibernate<ClusterToMassTagMap> clusterToMassTagMapDAOHibernate =
                                new GenericDAOHibernate<ClusterToMassTagMap>();

            GenericDAOHibernate<MassTagToProteinMap> massTagToProteinMapDAOHibernate =
                                new GenericDAOHibernate<MassTagToProteinMap>();

            GenericDAOHibernate<StacFDR> stacFDRDAOHibernate =
                                new GenericDAOHibernate<StacFDR>();

            List<clsMassTag> massTagList = new List<clsMassTag>();
            List<clsProtein> proteinList = new List<clsProtein>();
            List<ClusterToMassTagMap> clusterToMassTagMapList = new List<ClusterToMassTagMap>();
            List<MassTagToProteinMap> massTagToProteinMapList = new List<MassTagToProteinMap>();
            List<StacFDR> stacFDRResultsList = new List<StacFDR>();

            foreach (clsPeakMatchingResults.clsPeakMatchingTriplet triplet in analysis.PeakMatchingResults.marrPeakMatchingTriplet)
            {
                clsMassTag massTag = massTagArray[triplet.mintMassTagIndex];
                clsProtein protein = proteinArray[triplet.mintProteinIndex];

                ClusterToMassTagMap clusterToMassTagMap = new ClusterToMassTagMap(triplet.mintFeatureIndex, massTag.Id);
                MassTagToProteinMap massTagToProteinMap = new MassTagToProteinMap(massTag.Id, protein.Id);

                if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
                {
                    clusterToMassTagMapList.Add(clusterToMassTagMap);



                    if (analysis.PeakMatchingOptions.UseSTAC)
                    {
                        /// 
                        /// See if a SMART score exists
                        /// 
                        List<PNNLProteomics.SMART.classSMARTProbabilityResult> smartScores = null;
                        smartScores = analysis.STACResults.GetResultFromUMCIndex(triplet.mintFeatureIndex);

                        if (smartScores != null)
                        {
                            /// 
                            /// Then pull out the SMART score that matches for this triplet Mass Tag
                            /// 
                            PNNLProteomics.SMART.classSMARTProbabilityResult finalResult = null;
                            foreach (PNNLProteomics.SMART.classSMARTProbabilityResult score in smartScores)
                            {
                                if (score.MassTagID == massTag.Id)
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

                if (!massTagList.Contains(massTag))
                {
                    massTagList.Add(massTag);
                }

                if (!proteinList.Contains(protein))
                {
                    proteinList.Add(protein);
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
