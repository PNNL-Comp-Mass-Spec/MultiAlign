using System;
using System.IO;
using System.Collections.Generic;

using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data.Analysis;
using MultiAlignEngine.PeakMatching;


using MultiAlignWin.Forms.Filters;
using PNNLProteomics.SMART;
using PNNLProteomics.Filters;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain;

namespace MultiAlignWin.IO
{
    /// <summary>
    /// Class that writes the MA analysis data to a delimited text file.
    /// </summary>
    public class AnalysisTableWriter: IAnalysisWriter
    {
        #region Members and constants
        /// <summary>
        /// Default delimeter
        /// </summary>
        public  const string CONST_DELIMITER                        = ",";         
        private const string CONST_umc_rep_mass_col                 = "Mass" ; 
		private const string CONST_umc_rep_net_col                  = "NET" ; 		
		private const string CONST_umc_rep_mass_calib_col           = "Calibrated Mass" ; 
		private const string CONST_umc_rep_net_aligned_col          = "Aligned NET" ;
        private const string CONST_umc_index_col                    = "Row ID";
        private const string CONST_umc_rep_size_col                 = "Cluster Size";
        private const string CONST_umc_spectral_count               = "Spectral Count"; 
		private const string CONST_peptide_col                      = "Peptide" ; 
		private const string CONST_mass_tag_id_col                  = "Mass Tags" ;
        private const string CONST_mass_tag_net_col                 = "Mass Tag NET";
        private const string CONST_mass_tag_mass_col                = "Mass Tag Mass"; 
		private const string CONST_mass_tag_F_CS1                   = "Charge 1 F Score" ; 
		private const string CONST_mass_tag_F_CS2                   = "Charge 2 F Score" ; 
		private const string CONST_mass_tag_F_CS3                   = "Charge 3 F Score" ;
        private const string CONST_mass_tag_xcorr_col               = "Mass Tag Xcorr"; 
		private const string CONST_mass_tag_modification_col        = "Modifications" ; 
		private const string CONST_mass_tag_modification_count_col  = "Mod count" ; 
		private const string CONST_protein_col                      = "Protein" ; 
		private const string CONST_proteinid_col                    = "RefID" ;
        private const string CONST_CHARGE_ABUNDANCE_START           = "CMC-Abundance";
		private const string CONST_mass_colum                       = "Mass" ; 
		private const string CONST_calibrated_mass_colum            = "Calibrated_mass" ; 
		private const string CONST_scan_colum                       = "Scan" ;
        private const string CONST_aligned_scan_colum               = "Aligned Scan";
        private const string CONST_umc_index_column                 = "UMC index";
        private const string CONST_driftTime_column                 = "Drift Time";
        
        /// <summary>
        /// Delimeter used between columns.
        /// </summary>
        private string mstring_delimeter;
        /// <summary>
        /// List of UMC filters for screening data.
        /// </summary>
        private List<IFilter<clsUMC>> mlist_umcFilters;
        /// <summary>
        /// List of filters related to UMC Clusters
        /// </summary>
        private List<IFilter<clsCluster>> mlist_clusterFilters;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AnalysisTableWriter()
        {
            Clear();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the delimeter to use between data columns.
        /// </summary>
        public string Delimeter
        {
            get 
            {
                return mstring_delimeter; 
            }
            set
            {
                mstring_delimeter = value;
            }
        }
        /// <summary>
        /// Gets or sets the list of UMC Filters
        /// </summary>
        public List<IFilter<clsCluster>> FiltersClusters
        {
            get
            {
                return mlist_clusterFilters;
            }
            set
            {
                mlist_clusterFilters = value;
            }
        }
        /// <summary>
        /// Gets or sets the list of UMC Filters
        /// </summary>
        public List<IFilter<clsUMC>> FiltersUMC
        {
            get
            {
                return mlist_umcFilters;
            }
            set
            {
                mlist_umcFilters = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resets the options to their default values. 
        /// </summary>
        public void Clear()
        {
            /// 
            /// Delimeter for text output
            /// 
            Delimeter = CONST_DELIMITER;

            /// 
            /// Create the filter lists
            /// 
            if (mlist_umcFilters == null)
                mlist_umcFilters = new List<IFilter<clsUMC>>();
            mlist_umcFilters.Clear();
            
            if (mlist_clusterFilters == null)
                mlist_clusterFilters = new List<IFilter<clsCluster>>();
            mlist_clusterFilters.Clear();

        }
        #endregion

        #region Writing Header Methods
        /// <summary>
        /// Writes the header information to the file.
        /// </summary>
        /// <param name="writer"></param>
        private void WriteHeader(TextWriter writer,
                                 clsMultiAlignAnalysis analysis)
        {
            string data = CONST_umc_index_col;
            data += Delimeter + CONST_umc_rep_size_col;
            data += Delimeter + CONST_umc_rep_mass_col;
            data += Delimeter + CONST_umc_rep_mass_calib_col;
            data += Delimeter + CONST_umc_rep_net_col;
            data += Delimeter + CONST_umc_rep_net_aligned_col;

            if (analysis.PeakMatchedToMassTagDB == true)
            {
                data += Delimeter + CONST_peptide_col;
                data += Delimeter + CONST_protein_col;
                data += Delimeter + CONST_proteinid_col;
                data += Delimeter + CONST_mass_tag_id_col;
                data += Delimeter + CONST_mass_tag_mass_col;
                data += Delimeter + CONST_mass_tag_net_col;
                data += Delimeter + CONST_mass_tag_xcorr_col;
                data += Delimeter + CONST_mass_tag_modification_count_col;
                data += Delimeter + CONST_mass_tag_modification_col;
                data += Delimeter + CONST_mass_tag_F_CS1;
                data += Delimeter + CONST_mass_tag_F_CS2;
                data += Delimeter + CONST_mass_tag_F_CS3;

                /// 
                /// If SMART was used, then we want to pull the data out of the analysis object
                /// 
                if (analysis.UseSMART == true)
                {
                    data += Delimeter + "SMART Score";
                    data += Delimeter + "SMART Specificity";
                }

                
            }
            writer.Write(data);
                
            /// 
            /// Write the headers for each dataset                
            /// 
            int N = analysis.FileNames.Length;
            for (int dataset_num = 0; dataset_num < N; dataset_num++)
            {
                WriteDatasetHeader(writer, analysis, dataset_num);
            }            
            writer.Write("\n");
        }
        /// <summary>
        /// Writes the header columns for every dataset.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="datasetID"></param>
        private void WriteDatasetHeader(TextWriter writer, clsMultiAlignAnalysis analysis, int datasetID)
        {
            
            string file_name = analysis.UMCData.DatasetName[datasetID];
            int index        = file_name.LastIndexOf("\\");
            string col_name  = file_name.Substring(index + 1);
            string data      = "";


            data += Delimeter + CONST_mass_colum + "_" + col_name;
            data += Delimeter + CONST_calibrated_mass_colum + "_" + col_name;
            data += Delimeter + CONST_scan_colum + "_" + col_name;
            data += Delimeter + CONST_aligned_scan_colum + "_" + col_name;
            data += Delimeter + CONST_umc_index_column + "_" + col_name;
            data += Delimeter + CONST_driftTime_column + "_" + col_name;
            data += Delimeter + "Abundance-Sum-" + col_name;
            data += Delimeter + "Abundance-Max-" + col_name;
            data += Delimeter + CONST_umc_spectral_count + "_" + col_name;

            for (int j = 0; j < analysis.UMCData.HighestChargeState; j++)
            {
                    int chargeState = j + 1;
                    data += Delimeter + string.Format("{0}_CS_{1}_{2}", CONST_CHARGE_ABUNDANCE_START,
                                                                        chargeState,
                                                                        col_name);
            }
            writer.Write(data);
        }
        #endregion

        
        #region Writing Analysis Methods
        /// <summary>
        /// Writes the analysis object to the file provided.
        /// </summary>
        /// <param name="analysis">Analysis object to write.</param>
        /// <param name="pathname">File path to write to.</param>
        public void WriteAnalysis(string pathname, clsMultiAlignAnalysis analysis)
        {            
            /// 
            /// Write the data to file.
            /// 
            using (TextWriter writer = File.CreateText(pathname))
            {
                WriteHeader(writer, analysis);                
                int num_clusters            = analysis.UMCData.mobjClusterData.NumClusters;                                                                               
                int clusterNum              = 0;
                int currentPeakMatchNum     = 0;

                /// 
                /// Cluster mapping arrays 
                ///  
                int[]    arrClusterMainMemberIndex = analysis.UMCData.mobjClusterData.marrClusterMainMemberIndex;
                double[] arrClusterMemberIntensity = analysis.UMCData.mobjClusterData.marrClusterIntensity;
                double[] arrClusterMemberNormalizedIntensity = analysis.UMCData.mobjClusterData.marrClusterIntensityNormalized;


                /// 
                /// Peak matching mapping arrays 
                /// 
                clsPeakMatchingResults.clsPeakMatchingTriplet[] arrPeakMatchingTriplets = null;
                clsProtein[] arrPeakMatchingProteins = null;
                clsMassTag[] arrPeakMatchingMassTags = null;

                if (analysis.PeakMatchingResults != null)
                {
                    arrPeakMatchingTriplets = analysis.PeakMatchingResults.marrPeakMatchingTriplet;
                    arrPeakMatchingProteins = analysis.PeakMatchingResults.marrProteins;
                    arrPeakMatchingMassTags = analysis.PeakMatchingResults.marrMasstags;
                }


                int lastClusterNumber = -1;

                /// ////////////////////////////////////////////////////////////////////////////// 
                /// Now we add the data 
                ///     We dont make this clustering writing into a separate method because 
                ///     we have to account for clusters matching to more than one tag.
                /// ////////////////////////////////////////////////////////////////////////////// 
                while (clusterNum < num_clusters)
                {

                    bool clusterDidNotPeakMatch = false;

                    clsCluster cluster = analysis.UMCData.mobjClusterData.GetCluster(clusterNum);

                    /// 
                    /// Check to make sure the cluster passes the filter.
                    /// 
                    if (FilterUtil<clsCluster>.PassesFilters(cluster, mlist_clusterFilters) == false)
                    {
                        clusterNum++;
                    }
                    else
                    {                        
                        int id              = clusterNum + 1;
                        string data         = id.ToString();

                        /// 
                        /// The second column is the number of members stored in the cluster.
                        /// 
                        data += Delimeter + cluster.mshort_num_dataset_members.ToString();

                        clsUMC[] arrUMCs = analysis.UMCData.marr_umcs;
                        
                        string massTagData = "";

                        ///  ////////////////////////////////////////////////////////////////////////////// 
                        /// AddMassTags to Row
                        ///     if it is peakmatched, and show mass tags is enabled, everything shows. 
                        ///  ////////////////////////////////////////////////////////////////////////////// 
                        if (analysis.PeakMatchedToMassTagDB)
                        {
                            if (arrPeakMatchingTriplets != null &&
                                currentPeakMatchNum < arrPeakMatchingTriplets.Length &&
                                arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
                            {
                                ///
                                /// So this peakmatchtriplet corresponds to the current cluster.                     
                                ///                     
                                clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum];
                                clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex];
                                clsProtein protein = arrPeakMatchingProteins[triplet.mintProteinIndex];

                                massTagData += string.Format("{0}{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}",
                                                        Delimeter,
                                                        massTag.mstrPeptide,
                                                        protein.mstrProteinName,

                                                        protein.mintRefID,
                                                        massTag.mintMassTagId,
                                                        massTag.mdblMonoMass,

                                                        massTag.mdblAvgGANET,
                                                        massTag.mdblHighXCorr,
                                                        massTag.mshortModCount,

                                                        massTag.mstrModification,
                                                        massTag.mfltAvgFCS1,
                                                        massTag.mfltAvgFCS2,

                                                        massTag.mfltAvgFCS3);

                                if (analysis.UseSMART)
                                {
                                    /// 
                                    /// See if a SMART score exists
                                    /// 
                                    List<classSMARTProbabilityResult> smartScores = null;
                                    smartScores = analysis.SMARTResults.GetResultFromUMCIndex(triplet.mintFeatureIndex);
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
                                        /// 
                                        /// If we have a final result, then we have a smart score for this MTID for the matched UMC.
                                        /// 
                                        if (finalResult != null)
                                        {
                                            massTagData += Delimeter + finalResult.Score.ToString();
                                            massTagData += Delimeter + finalResult.Specificity.ToString();
                                        }
                                        else
                                        {
                                            /// Otherwise just write delimeters or blank
                                            massTagData += Delimeter;
                                            massTagData += Delimeter;
                                        }
                                    }
                                }
                                currentPeakMatchNum++;
                                clusterDidNotPeakMatch = false;
                                lastClusterNumber = clusterNum;
                            }
                            else
                            {
                                massTagData += string.Format("{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}",
                                                       Delimeter
                                                       );

                                if (analysis.UseSMART)
                                {
                                    massTagData += Delimeter;
                                    massTagData += Delimeter;
                                }

                                clusterDidNotPeakMatch = true;

                                if (clusterNum == lastClusterNumber)
                                {
                                    clusterNum++;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            clusterDidNotPeakMatch = true;
                        }

                        /// /////////////////////////////////////////////////////////////////////////////////////////////
                        /// Write cluster data
                        /// /////////////////////////////////////////////////////////////////////////////////////////////
                        data += Delimeter + Convert.ToString(cluster.mdouble_mass);            
                        data += Delimeter + Convert.ToString(cluster.mdouble_mass_calibrated);
                        data += Delimeter + Convert.ToString(cluster.mdouble_net);
                        data += Delimeter + Convert.ToString(cluster.mdouble_aligned_net);

                        data += massTagData;
                        
                        /// 
                        /// Find the total number of datasets to save 
                        /// 
                        int N = analysis.FileNames.Length;

                        /// ////////////////////////////////////////////////////////////////////////////// 
                        /// Add the UMC data for each dataset.
                        /// //////////////////////////////////////////////////////////////////////////////                                 
                        for (int i = 0; i < N; i++)
                        {
                            int pt_index = clusterNum * N + i;
                            int index = arrClusterMainMemberIndex[pt_index];
                            clsUMC umc = null;

                            /// ////////////////////////////////////////////////////////////////////////////// 
                            /// Find the UMC so we can grab data from it to show
                            /// ////////////////////////////////////////////////////////////////////////////// 
                            if (index != -1)
                            {
                                umc = arrUMCs[index];
                            }

                            /// 
                            /// Check to make sure the UMC passes the filter.
                            /// 
                            if (umc != null && FilterUtil<clsUMC>.PassesFilters(umc, mlist_umcFilters))
                            {
                                data += string.Format("{0}{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}",
                                                        Delimeter,                          // 0
                                                        umc.mdouble_mono_mass,              // 1
                                                        umc.mdouble_mono_mass_calibrated,   // 2
                                                        umc.mint_scan,                      // 3
                                                        umc.mint_scan_aligned,              // 4
                                                        umc.mint_umc_index,                 // 5
                                                        umc.DriftTime,                      // 6
                                                        umc.AbundanceSum,                   // 7
                                                        umc.AbundanceMax,                   // 8
                                                        umc.SpectralCount);                 // 9

                                for (int j = 0; j < analysis.UMCData.HighestChargeState; j++)
                                {
                                    if (umc != null)
                                    {
                                        data += Delimeter + umc.marray_chargeStatesAbundances[j].ToString();
                                    }
                                }
                            }
                            else
                            {
                                data += string.Format("{0}{0}{0}{0}{0}{0}{0}{0}{0}", Delimeter);
                                /*
                                 * 1  2  3  4  5  6  7  8  9
                                 */

                                string chargeData = "";
                                for (int j = 0; j < analysis.UMCData.HighestChargeState; j++)
                                {
                                    chargeData = Delimeter + chargeData;
                                }
                                data += chargeData;
                            }
                        }

                        /// 
                        /// Only increment if we dont have more peak matching results...
                        /// 
                        if (clusterDidNotPeakMatch == true)
                        {
                            clusterNum++;
                        }

                        writer.WriteLine(data);  
                    }                    
                }
            }                     
        }
        #endregion
    }

    public interface IAnalysisWriter
    {
        void WriteAnalysis(string path, clsMultiAlignAnalysis analysis);
    }
}
