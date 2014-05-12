#region

using System.Collections.Generic;
using System.Data;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    ///     Class that writes the Peak Matching results back to the Mass Tag System (MTS).
    /// </summary>
    public abstract class MTSPeakMatchResultsWriter
    {
        /// <summary>
        ///     Creates a database connection.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected abstract IDbConnection CreateConnection(string path);

        /// <summary>
        ///     Creates a data parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract IDbDataParameter CreateParameter(string name, object value);

        /// <summary>
        ///     Creates an output data parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract IDbDataParameter CreateOutputParameter(string name,
            DbType type,
            int size,
            byte precision,
            byte scale);

        /// <summary>
        ///     Writes a list of mamoth clusters back to the mass tag system at the server and database provided.
        /// </summary>
        /// <param name="server">Server the database exists on.</param>
        /// <param name="database">Database to write results to.</param>
        /// <param name="clusters">Clusters that are peak matched to.</param>
        public void WriteClusters(MultiAlignAnalysis analysis,
            List<UMCClusterLight> clusters,
            string databasePath)
        {
            /*
            //clsMassTag[] massTagArray                                       = analysis.PeakMatchingResults.marrMasstags;
            //clsProtein[] proteinArray                                       = analysis.PeakMatchingResults.marrProteins;            
            //List<clsMassTag> massTagList                                    = new List<clsMassTag>();
            //List<clsProtein> proteinList                                    = new List<clsProtein>();
            //List<ClusterToMassTagMap> clusterToMassTagMapList               = new List<ClusterToMassTagMap>();
            //List<MassTagToProteinMap> massTagToProteinMapList               = new List<MassTagToProteinMap>();
            //List<StacFDR> stacFDRResultsList                                = new List<StacFDR>();
            //Dictionary<int, int> clusterMatchDict                           = new Dictionary<int,int>();            // Unique matches to mass tags.
            //Dictionary<int, List<ClusterToMassTagMap>> clusterMassTagDict   = new Dictionary<int, List<ClusterToMassTagMap>>();
            //Dictionary<double, int> stacFdrDict                             = new Dictionary<double,int>();            // counts number of matches with FDR's > key
            //double [] fdrScores                                             = new double[] {.01, .05, .10, .25, .50};

            //foreach(int fdrCutoffScore in fdrScores)
            //{                
            //    stacFdrDict.Add(fdrCutoffScore, 0);                    
            //}

            //foreach (clsPeakMatchingResults.clsPeakMatchingTriplet triplet in analysis.PeakMatchingResults.marrPeakMatchingTriplet)
            //{
            //    clsMassTag massTag                      = massTagArray[triplet.mintMassTagIndex];
            //    clsProtein protein                      = proteinArray[triplet.mintProteinIndex];
            //    ClusterToMassTagMap clusterToMassTagMap = new ClusterToMassTagMap(triplet.mintFeatureIndex, massTag.Id);
            //    MassTagToProteinMap massTagToProteinMap = new MassTagToProteinMap(massTag.Id, protein.Id);

            //    if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
            //    {
            //        clusterToMassTagMapList.Add(clusterToMassTagMap);                    
            //        if (!clusterMatchDict.ContainsKey(triplet.mintFeatureIndex))
            //        {
            //            clusterMassTagDict.Add(triplet.mintFeatureIndex, new List<ClusterToMassTagMap>());
            //            clusterMatchDict.Add(triplet.mintFeatureIndex, 1);
            //        }
            //        else
            //        {
            //            clusterMatchDict[triplet.mintFeatureIndex]++;
            //        }                    
            //        clusterMassTagDict[triplet.mintFeatureIndex].Add(clusterToMassTagMap);

            //        if (analysis.STACResults != null)
            //        {
            //            // See if a SMART score exists
            //            List<classSMARTProbabilityResult> smartScores = null;
            //            smartScores = analysis.STACResults.GetResultFromUMCIndex(triplet.mintFeatureIndex);

            //            if (smartScores != null)
            //            {
            //                // Then pull out the SMART score that matches for this triplet Mass Tag
            //                classSMARTProbabilityResult finalResult = null;
            //                foreach (classSMARTProbabilityResult score in smartScores)
            //                {
            //                    if (score.MassTagID == massTag.Id)
            //                    {
            //                        finalResult = score;
            //                        break;
            //                    }
            //                }

            //                if (finalResult != null)
            //                {
            //                    double score                    = finalResult.Score;
            //                    clusterToMassTagMap.StacScore   = score;
            //                    clusterToMassTagMap.StacUP      = finalResult.Specificity;

            //                    foreach(int fdrCutoffScore in fdrScores)
            //                    {
            //                        if (score <= fdrCutoffScore) stacFdrDict[score]++;                                      
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if (!massTagToProteinMapList.Contains(massTagToProteinMap)) massTagToProteinMapList.Add(massTagToProteinMap);                
            //    if (!massTagList.Contains(massTag)) massTagList.Add(massTag);                
            //    if (!proteinList.Contains(protein)) proteinList.Add(protein);                
            //}

            //if (analysis.STACResults != null)
            //{
            //    foreach (classSMARTFdrResult fdrResult in analysis.STACResults.GetSummaries())
            //    {
            //        stacFDRResultsList.Add(new StacFDR(fdrResult));
            //    }
            //}     

            //// find the minimum NET value for any cluster.
            //double minClusterNET = double.MaxValue;
            //double maxClusterNET = double.MinValue;
            //foreach(clsCluster cluster in clusters)
            //{
            //    if (analysis.ClusterOptions.AlignClusters)
            //    {
            //        minClusterNET = Math.Min(minClusterNET, cluster.NetAligned);
            //        maxClusterNET = Math.Max(maxClusterNET, cluster.NetAligned);
            //    }
            //    else
            //    {
            //        minClusterNET = Math.Min(minClusterNET, cluster.Net);
            //        maxClusterNET = Math.Max(maxClusterNET, cluster.Net);
            //    }
            //}
       
            // now write to the MTS System.
            string connectionString = "";
            string versionID        = "";
            string parameters       = "";
            string peakMatchingType = "1";
            int    redundantCount   = analysis.MatchResults.Matches.Count;
            int    matchMakingID    = -1;

            string parameterFile    = "";

            if (analysis.MetaData.ParameterFile != null)
            {
                parameterFile = analysis.MetaData.ParameterFile;
            }

            using (IDbConnection connection = CreateConnection(connectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(CreateParameter("Reference_Job",                analysis.MetaData.JobID));
                    command.Parameters.Add(CreateParameter("File",                         peakMatchingType));
                    command.Parameters.Add(CreateParameter("Type",                         1));
                    command.Parameters.Add(CreateParameter("Parameters",                   parameters)); 
                    command.Parameters.Add(CreateParameter("PeaksCount",                   redundantCount));  
                    
                    IDbDataParameter matchMakingParameter = CreateOutputParameter(  "MatchMakingID",
                                                                                    DbType.Int32, 
                                                                                    sizeof(int),                                                                                                                                 
                                                                                    10,
                                                                                    0);
                    command.Parameters.Add(matchMakingParameter);
                    command.Parameters.Add(CreateParameter("ToolVersion",                  versionID));			
                    command.Parameters.Add(CreateParameter("ComparisonMassTagCount",       analysis.MassTagDatabase.MassTags.Count)); 
                    command.Parameters.Add(CreateParameter("UMCTolerancePPM",              analysis.Options.FeatureFindingOptions.ConstraintMonoMass));	//TODO: do not necessarily have	if using LCMS Feature Finder Data
                    command.Parameters.Add(CreateParameter("UMCCount",                     clusters.Count));			
                    command.Parameters.Add(CreateParameter("NetAdjTolerancePPM",           analysis.Options.AlignmentOptions.MassTolerance));		
                    command.Parameters.Add(CreateParameter("NetAdjUMCsHitCount",           analysis.MatchResults.Matches.Count));		 //TODO:  is this right?
                    command.Parameters.Add(CreateParameter("NetAdjTopAbuPct",              0));	// Per direction from Matt
                    command.Parameters.Add(CreateParameter("NetAdjIterationCount",         1));	// Per direction from Matt
                    command.Parameters.Add(CreateParameter("MMATolerancePPM",              analysis.Options.STACOptions.MassTolerancePPM));		
                    command.Parameters.Add(CreateParameter("NETTolerance",                 analysis.Options.STACOptions.NETTolerance));			
                    command.Parameters.Add(CreateParameter("State",                        2));				
                    command.Parameters.Add(CreateParameter("GANETFit",                     0));	//TODO: do not have		
                    command.Parameters.Add(CreateParameter("GANETSlope",                   0));	//TODO: do not have		
                    command.Parameters.Add(CreateParameter("GANETIntercept",               0));	//TODO: do not have	
                    command.Parameters.Add(CreateParameter("NetAdjNetMin",                 minClusterNET));			
                    command.Parameters.Add(CreateParameter("NetAdjNetMax",                 maxClusterNET));			
                    command.Parameters.Add(CreateParameter("RefineMassCalPPMShift",        0));	//TODO: do not have
                    command.Parameters.Add(CreateParameter("RefineMassCalPeakHeightCounts",0));	//TODO: do not have
                    command.Parameters.Add(CreateParameter("RefineMassTolUsed",            0));	//TODO: do not have		
                    command.Parameters.Add(CreateParameter("RefineNETTolUsed",             0));	//TODO: do not have		
                    command.Parameters.Add(CreateParameter("MinimumHighNormalizedScore",   analysis.MassTagDBOptions.mfltMinXCorr)); 
                    command.Parameters.Add(CreateParameter("MinimumPMTQualityScore",       Convert.ToDouble(analysis.MassTagDBOptions.mdecimalMinPMTScore)));		
                    command.Parameters.Add(CreateParameter("IniFileName",                  parameterFile));			    
                    command.Parameters.Add(CreateParameter("MinimumHighDiscriminantScore", analysis.MassTagDBOptions.mdblMinDiscriminant));
                    command.Parameters.Add(CreateParameter("ExperimentFilter",             analysis.MassTagDBOptions.mstrExperimentFilter));           
                    command.Parameters.Add(CreateParameter("ExperimentExclusionFilter",    analysis.MassTagDBOptions.mstrExperimentExclusionFilter));  
                    command.Parameters.Add(CreateParameter("RefineMassCalPeakWidthPPM",    0));    //TODO: do not have
                    command.Parameters.Add(CreateParameter("RefineMassCalPeakCenterPPM",   0));    //TODO: do not have 
                    command.Parameters.Add(CreateParameter("RefineNETTolPeakHeightCounts", 0));    //TODO: do not have
                    command.Parameters.Add(CreateParameter("RefineNETTolPeakWidthNET",     0));    //TODO: do not have  	
                    command.Parameters.Add(CreateParameter("RefineNETTolPeakCenterNET",    0));    //TODO: do not have 	
                    command.Parameters.Add(CreateParameter("LimitToPMTsFromDataset",       0));      	
                    command.Parameters.Add(CreateParameter("MinimumPeptideProphetProbability", analysis.MassTagDBOptions.mdblPeptideProphetVal)); 
                    command.Parameters.Add(CreateParameter("MatchScoreMode",               Convert.ToInt32(analysis.PeakMatchingOptions.UseSTAC)));           	
                    command.Parameters.Add(CreateParameter("STACUsedPriorProbability",     Convert.ToInt32(analysis.STACOptions.UsePriorProbabilities))); 	
                    command.Parameters.Add(CreateParameter("AMTCount1pctFDR",              stacFdrDict[.01]));  	
                    command.Parameters.Add(CreateParameter("AMTCount5pctFDR",              stacFdrDict[.05]));  	
                    command.Parameters.Add(CreateParameter("AMTCount10pctFDR",             stacFdrDict[.10])); 	
                    command.Parameters.Add(CreateParameter("AMTCount25pctFDR",             stacFdrDict[.25])); 	
                    command.Parameters.Add(CreateParameter("AMTCount50pctFDR",             stacFdrDict[.50]));
                    command.ExecuteNonQuery();

                    matchMakingID = Convert.ToInt32(matchMakingParameter.Value);
                }
              
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    Dictionary<int, clsCluster> resultIDClusterMap = new Dictionary<int,clsCluster>();

                    foreach (clsCluster cluster in clusters)
                    {
                        int matchCount  = 0;
                        int resultID   = 0;
                        if (clusterMatchDict.ContainsKey(cluster.Id))
                        {
                            matchCount = clusterMatchDict[cluster.Id];
                        }
                        using (IDbCommand command = connection.CreateCommand())
                        {                            
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("MDID",                     matchMakingID));				
                            command.Parameters.Add(new SqlParameter("UMCInd",                   cluster.Id));
                            command.Parameters.Add(new SqlParameter("MemberCount",              cluster.MemberCount));      
                            command.Parameters.Add(new SqlParameter("UMCScore",                 cluster.MeanScore));      
                            command.Parameters.Add(new SqlParameter("ScanFirst",                -1));      
                            command.Parameters.Add(new SqlParameter("ScanLast",                 null));      
                            command.Parameters.Add(new SqlParameter("ScanMaxAbundance",         null));      
                            command.Parameters.Add(new SqlParameter("ClassMass",                cluster.Mass));     
                            command.Parameters.Add(new SqlParameter("MonoisotopicMassMin",      null));
                            command.Parameters.Add(new SqlParameter("MonoisotopicMassMax",      null));
                            command.Parameters.Add(new SqlParameter("MonoisotopicMassStDev",    null));
                            command.Parameters.Add(new SqlParameter("MonoisotopicMassMaxAbu", 	null));
                            command.Parameters.Add(new SqlParameter("ClassAbundance",           null));
                            command.Parameters.Add(new SqlParameter("AbundanceMin",             null));
                            command.Parameters.Add(new SqlParameter("AbundanceMax",             null));
                            command.Parameters.Add(new SqlParameter("ChargeStateMin",           null));
                            command.Parameters.Add(new SqlParameter("ChargeStateMax",           null));
                            command.Parameters.Add(new SqlParameter("ChargeStateMaxAbu",        null));
                            command.Parameters.Add(new SqlParameter("FitAverage",               null));
                            command.Parameters.Add(new SqlParameter("FitMin",                   null));
                            command.Parameters.Add(new SqlParameter("FitMax",                   null));
                            command.Parameters.Add(new SqlParameter("FitStDev",                 null));
                            command.Parameters.Add(new SqlParameter("ElutionTime",              cluster.Net));                
                            command.Parameters.Add(new SqlParameter("ExpressionRatio",          0));                
                            command.Parameters.Add(new SqlParameter("PeakFPRType",              0));                
                            command.Parameters.Add(new SqlParameter("MassTagHitCount",          matchCount));                
                            command.Parameters.Add(new SqlParameter("PairUMCInd",               -1));             

                            SqlParameter idParameter = new SqlParameter("UMCResultsID",             
                                                                    SqlDbType.Int, 
                                                                    sizeof(int), 
                                                                    System.Data.ParameterDirection.Output,
                                                                    false,
                                                                    10,
                                                                    0,
                                                                    "UMCResultsID",
                                                                    DataRowVersion.Current,
                                                                    resultID); 
                            command.Parameters.Add(idParameter);
                            command.Parameters.Add(new SqlParameter("ClassStatsChargeBasis",                null));               
                            command.Parameters.Add(new SqlParameter("GANETLockerCount",                     null));              
                            command.Parameters.Add(new SqlParameter("ExpressionRatioStDev",                 null));              
                            command.Parameters.Add(new SqlParameter("ExpressionRatioChargeStateBasisCount", null));
                            command.Parameters.Add(new SqlParameter("ExpressionRatioMemberBasisCount",      null));
                            command.Parameters.Add(new SqlParameter("MemberCountUsedForAbu",                cluster.MemberCount));
                            command.Parameters.Add(new SqlParameter("DriftTime", 		                    cluster.DriftTime));
                            command.ExecuteNonQuery();

                            resultID = Convert.ToInt32(idParameter.Value);
                        }

                        if (matchCount > 0)
                        {                        
                            using (IDbCommand command = connection.CreateCommand())
                            {                                
                                double minScore         = double.MaxValue;
                                double maxScore         = double.MinValue;
                                double deltaMatchScore  = 0;

                                foreach(ClusterToMassTagMap massTagMap in clusterMassTagDict[cluster.Id])
                                {
                                    minScore = Math.Min(minScore, massTagMap.StacScore);
                                    maxScore = Math.Max(maxScore, massTagMap.StacScore);
                                }
                                deltaMatchScore = Math.Abs(maxScore - minScore);
                                foreach(ClusterToMassTagMap massTagMap in clusterMassTagDict[cluster.Id])
                                {
                                    command.CommandType = CommandType.StoredProcedure;                            
                                    command.Parameters.Add(new SqlParameter("UMCResultsID",             resultID)); 
                                    command.Parameters.Add(new SqlParameter("MassTagID",                massTagMap.MassTagId)); 		
                                    command.Parameters.Add(new SqlParameter("MatchingMemberCount",      cluster.MemberCount)); 		
                                    command.Parameters.Add(new SqlParameter("MatchScore",               massTagMap.StacScore));             
                                    command.Parameters.Add(new SqlParameter("MatchState",               6));             
                                    command.Parameters.Add(new SqlParameter("SetIsConfirmedForMT",      1));    
                                    command.Parameters.Add(new SqlParameter("MassTagMods",              "N14"));            
                                    command.Parameters.Add(new SqlParameter("MassTagModMass",           0));         
                                    command.Parameters.Add(new SqlParameter("DelMatchScore",            deltaMatchScore));          
                                    command.Parameters.Add(new SqlParameter("UniquenessProbability",    massTagMap.StacUP));  
                                    command.Parameters.Add(new SqlParameter("FDRThreshold",             1));
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    List<classSMARTFdrResult> fdrList = analysis.STACResults.GetSummaries();
                    if (fdrList != null)
                    {
                        foreach (classSMARTFdrResult fdr in fdrList)
                        {
                            using (IDbCommand command = connection.CreateCommand())
                            {                                
                                command.Parameters.Add(new SqlParameter("MDID",                 matchMakingID));               		
                                command.Parameters.Add(new SqlParameter("STAC_Cutoff",          fdr.Cutoff));       	
                                command.Parameters.Add(new SqlParameter("UniqueAMTs",           0));              	
                                command.Parameters.Add(new SqlParameter("FDR",                  fdr.FDR));  		
                                command.Parameters.Add(new SqlParameter("Matches",              fdr.NumMatches));  	
                                command.Parameters.Add(new SqlParameter("Errors",               fdr.Error));  	
                                command.Parameters.Add(new SqlParameter("UPFilteredUniqueAMTs", 0));  
                                command.Parameters.Add(new SqlParameter("UPFilteredFDR",        0));   
                                command.Parameters.Add(new SqlParameter("UPFilteredMatches",    0));  
                                command.Parameters.Add(new SqlParameter("UPFilteredErrors",     0));
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();    
            }
             * */
        }
    }
}