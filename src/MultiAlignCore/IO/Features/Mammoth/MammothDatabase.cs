using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;

using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Mammoth
{

    /// <summary>
    /// Class that marshalls access to a UMC database.
    /// </summary>
    public class MammothDatabase: IFeatureDatabase<UMCLight, UMCClusterLight>, IDisposable
    {
        #region Constants 
        /// <summary>
        /// Total size of the feature cluster query size.
        /// </summary>
        private const int CONST_TOTAL_FEATURE_NO_CLUSTER_QUERY_SIZE = 12;
        /// <summary>
        /// UMC ID index from data reader results.
        /// </summary>
        private const int CONST_UMC_ID = 0;
        /// <summary>
        /// UMC Monoisotopic mass index from data reader results.
        /// </summary>
        private const int CONST_UMC_MONOISOTOPIC_MASS = 1;
        /// <summary>
        /// UMC NET index from data reader results.
        /// </summary>
        private const int CONST_UMC_NET = 2;
        /// <summary>
        /// UMC Drift Time index from data reader results.
        /// </summary>
        private const int CONST_UMC_DRIFT_TIME = 3;
        /// <summary>
        /// UMC Group ID (dataset) index from data reader results.
        /// </summary>
        private const int CONST_UMC_GROUP_ID = 4;
        /// <summary>
        /// UMC Cluster ID index from data reader results.
        /// </summary>
        private const int CONST_UMC_CLUSTER_ID = 5;
        /// <summary>
        /// Cluster Monoisotopic Mass index from data reader results.
        /// </summary>
        private const int CONST_CLUSTER_MONOISOTOPIC_MASS = 6;
        /// <summary>
        /// Cluster NET index from data reader results.
        /// </summary>
        private const int CONST_CLUSTER_NET = 7;
        /// <summary>
        /// Cluster Drift Time index from data reader results.
        /// </summary>
        private const int CONST_DRIFT_TIME = 8;
        /// <summary>
        /// Cluster charge state.
        /// </summary>
        private const int CONST_CLUSTER_CHARGE = 10;
        /// <summary>
        /// UMC Charge state.
        /// </summary>
        private const int CONST_UMC_CHARGE = 10;
        /// <summary>
        /// UMC abundance value column
        /// </summary>
        private const int CONST_UMC_ABUNDANCE = 11;
        /// <summary>
        /// UMC Charge state.
        /// </summary>
        private const int CONST_UMC_CHARGE_FEATURE = 6;
        #endregion

        #region Members
        /// <summary>
        /// Database connection.
        /// </summary>
        protected SQLiteConnection m_connection;
        /// <summary>
        /// Cached features from a RunQuery method call.
        /// </summary>
        private List<UMCLight> m_features;
        /// <summary>
        /// Cached clusters from a RunQuery method call.
        /// </summary>
        private List<UMCClusterLight> m_clusters;
        /// <summary>
        /// Cached search options for speedy access to the umc data.
        /// </summary>
		private MammothDatabaseRange m_queryRange;
		/// <summary>
		/// Prepared command for inserting clusters into the database.
		/// </summary>
		protected SQLiteCommand m_clusterInsertStatementCommand;
		/// <summary>
		/// Prepared command for updating features in the database.
		/// </summary>
		protected SQLiteCommand m_featureUpdateStatementCommand;
		/// <summary>
		/// Prepared command for inserting features into the database.
		/// </summary>
        protected SQLiteCommand m_featureInsertStatementCommand;
        /// <summary>
        /// Prepared command for querying features from the database.
        /// </summary>
        protected SQLiteCommand m_featureQueryStatementCommand;
        /// <summary>
        /// Prepared command for querying features from the database.
        /// </summary>
        protected SQLiteCommand m_featureChargeQueryStatementCommand;
        /// <summary>
        /// Prepared command for finding features without a cluster in the database.
        /// </summary>
        protected SQLiteCommand m_featureQueryNoClusterStatementCommand;
        /// <summary>
        /// Prepared command for finding features without a cluster in the database.
        /// </summary>
        protected SQLiteCommand m_featureChargeQueryNoClusterStatementCommand;
		/// <summary>
		/// Prepared command for counting features in the database.
		/// </summary>
		protected SQLiteCommand m_preparedCountStatementCommand;
        /// <summary>
        /// Prepared command for counting features in the database.
        /// </summary>
        protected SQLiteCommand m_preparedChargeCountStatementCommand;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MammothDatabase(string databasePath)
        {
            // Make sure the database if valid.
            if (databasePath == null)
                throw new NullReferenceException("The path to the database cannot be null.");

            // Make sure the database file exists.
            bool exists = File.Exists(databasePath);            
            if (exists == false)
                throw new FileNotFoundException("The mammoth database file does not exist: " + databasePath);

            DatabasePath    = databasePath;
            m_queryRange    = null;
            m_features      = new List<UMCLight>();
            m_clusters      = new List<UMCClusterLight>();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the path to the database.
        /// </summary>
        public string DatabasePath { get; private set; }
        /// <summary>
        /// Gets the state of the database connection. 
        ///     Returns closed if no connection has existed.
        /// </summary>
        public ConnectionState DatabaseState
        {
            get
            {

                if (m_connection == null)
                    return ConnectionState.Closed;

                return m_connection.State;
            }
        }
        #endregion

        /// <summary>
        /// Adapts the database prepared statement queries for the SQLite database used by MultiAlign.
        /// </summary>
        private void PrepareOptimizedStatements()
        {

            // Cluster Inserts
            m_clusterInsertStatementCommand = m_connection.CreateCommand();
            m_clusterInsertStatementCommand.CommandText =
                                                "INSERT INTO T_CLUSTERS  (Cluster_ID, Mass, NET, Drift_Time, Charge, Score, Dataset_Member_Count, Member_Count) " +
                                                "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            m_clusterInsertStatementCommand.Prepare();

            // Feature updates
            m_featureUpdateStatementCommand = m_connection.CreateCommand();
            m_featureUpdateStatementCommand.CommandText =
                                                "UPDATE T_LCMS_FEATURES " +
                                                " SET   Cluster_ID = :cluster " +
                                                " WHERE Feature_ID = :featureID AND Dataset_ID = :datasetID";
            m_featureUpdateStatementCommand.Prepare();

            // Feature Queries
            m_featureQueryStatementCommand = m_connection.CreateCommand();
            m_featureQueryStatementCommand.CommandText = " SELECT     F.Feature_ID as Fid, F.Mass_Calibrated as FMass, F.NET as FNet, " +
                                      " F.Drift_Time as FDrift, F.Dataset_ID as FDid, " +
                                      " F.cluster_ID as FCid, F.charge as FCharge, F.Abundance_Max, F.Scan_LC_Start, F.Scan_LC_End, F.Scan_LC  " +
                                      " FROM       T_LCMS_FEATURES F" +
                                      " WHERE " +
                                      " FCid < 0    AND " +
                                      " FMass >= :MassLow    AND FMass < :MassHigh AND " +
                                      " FNet >= :NetLow      AND FNet < :NetHigh AND " +
                                      " FDrift >= :DriftLow  AND FDrift < :DriftHigh";                                      
            m_featureQueryStatementCommand.Prepare();
            
            m_featureChargeQueryStatementCommand = m_connection.CreateCommand();
            m_featureChargeQueryStatementCommand.CommandText = " SELECT     F.Feature_ID as Fid, F.Mass_Calibrated as FMass, F.NET as FNet, " +
                                      " F.Drift_Time as FDrift, F.Dataset_ID as FDid, " +
                                      " F.cluster_ID as FCid, F.charge as FCharge, F.Abundance_Max, F.Scan_LC_Start, F.Scan_LC_End, F.Scan_LC   " +
                                      " FROM       T_LCMS_FEATURES F" +
                                      " WHERE " +
                                      " FCid < 0    AND " +
                                      " FMass >= :MassLow    AND FMass < :MassHigh AND " +
                                      " FNet >= :NetLow      AND FNet < :NetHigh AND " +
                                      " (FCharge = :Charge) AND " +
                                      " FDrift >= :DriftLow  AND FDrift < :DriftHigh";                                      
            m_featureChargeQueryStatementCommand.Prepare();

            
            m_featureQueryNoClusterStatementCommand = m_connection.CreateCommand();
            m_featureQueryNoClusterStatementCommand.CommandText =
                                    " SELECT " +
                                    " F.Feature_ID, F.Mass_Calibrated as FMass, F.NET as FNet, F.Drift_Time as FDrift, " +
                                    " F.dataset_id, F.cluster_ID AS FClusterID,  " +
                                    " C.mass AS ClusterMass, C.net AS ClusterNet, C.Drift_time AS ClusterDrift, F.Charge as FeatureCharge, C.Charge as ClusterCharge, F.Abundance_Max, F.Scan_LC_Start, F.Scan_LC_End, F.Scan_LC  " +
                                    " FROM  T_LCMS_FEATURES F INNER JOIN " +
                                    "           T_CLUSTERS C ON " +
                                    "               FClusterID > -1   AND " +
                                    "               FClusterID = C.Cluster_ID AND " +
                                    " FMass  >= :MassLow   AND FMass  < :MassHigh AND " +
                                    " FNet   >= :NetLow    AND FNet   < :NetHigh AND " +
                                    " FDrift >= :DriftLow  AND FDrift < :DriftHigh";    
            m_featureQueryNoClusterStatementCommand.Prepare();


            
            m_featureChargeQueryNoClusterStatementCommand = m_connection.CreateCommand();
            m_featureChargeQueryNoClusterStatementCommand.CommandText =
                                    " SELECT " +
                                    " F.Feature_ID, F.Mass_Calibrated as FMass, F.NET as FNet, F.Drift_Time as FDrift, " +
                                    " F.dataset_id, F.cluster_ID AS FClusterID,  " +
                                    " C.mass AS ClusterMass, C.net AS ClusterNet, C.Drift_time AS ClusterDrift, F.Charge as FeatureCharge, C.Charge as ClusterCharge, F.Abundance_Max, F.Scan_LC_Start, F.Scan_LC_End, F.Scan_LC " +
                                    " FROM  T_LCMS_FEATURES F INNER JOIN " +
                                    "           T_CLUSTERS C ON " +
                                    "               FClusterID > -1   AND " +
                                    "               FClusterID = C.Cluster_ID AND " +
                                    " FMass  >= :MassLow   AND FMass  < :MassHigh AND " +
                                    " FNet   >= :NetLow    AND FNet   < :NetHigh AND " +
                                      " (F.Charge = :Charge) AND " +
                                    " FDrift >= :DriftLow  AND FDrift < :DriftHigh";    
            m_featureChargeQueryNoClusterStatementCommand.Prepare();


            m_preparedCountStatementCommand = m_connection.CreateCommand();
            m_preparedCountStatementCommand.CommandText = " SELECT   COUNT(*)" +
                                            " FROM       T_LCMS_FEATURES F" +
                                            " WHERE " +
                                            " F.mass_calibrated >= :MassLow    AND F.mass_calibrated < :MassHigh AND " +
                                            " F.net >= :NetLow     AND F.net < :NetHigh AND " +
                                            " F.drift_time >= :DriftLow AND F.drift_time < :DriftHigh";                    
            m_preparedCountStatementCommand.Prepare();


            m_preparedChargeCountStatementCommand = m_connection.CreateCommand();
            m_preparedChargeCountStatementCommand.CommandText = " SELECT   COUNT(*)" +
                                            " FROM       T_LCMS_FEATURES F" +
                                            " WHERE " +
                                            " F.mass_calibrated >= :MassLow    AND F.mass_calibrated < :MassHigh AND " +
                                            " F.net >= :NetLow     AND F.net < :NetHigh AND " +
                                            " (F.Charge = :Charge) AND " +
                                            " F.drift_time >= :DriftLow AND F.drift_time < :DriftHigh";
            m_preparedChargeCountStatementCommand.Prepare();
        }                


        #region Connection Handling
        /// <summary>
        /// Creates a connection to the database file and opens it.
        /// </summary>
        /// <exception cref="System.Exception">Thrown if the database connection already exists and is open.</exception>
        public void Connect()
        {
            if (m_connection != null)
            {
                // Make sure the database is not already open.  It could be doing 
                // something that we dont want to interrupt.
                if (m_connection.State == ConnectionState.Closed || m_connection.State == ConnectionState.Broken)
                {
                    m_connection.Dispose();
                    m_connection = null;
                }
                else if (m_connection.State != ConnectionState.Closed)
                {
                    throw new MammothDatabaseStateException("The database connection already exists.");
                }
            }

            // We don't use a "using" statement for this object because we want to maintain
            // the link to the database through processing.  We handle the explicit dispose calls in the 
            // dispose method and the close method.
            m_connection = new SQLiteConnection("Data Source=" + DatabasePath + ";");
            m_connection.Open();

			using (SQLiteCommand command = m_connection.CreateCommand())
			{
				command.CommandText = "PRAGMA synchronous=0;";
				command.ExecuteNonQuery();
			}

			PrepareOptimizedStatements();			
        }
		
        /// <summary>
        /// Closes a database.
        /// </summary>
        /// <exception cref="System.Exception">Thrown if the database is already closed.</exception>
        public void Close()
        {
            if (m_connection.State == ConnectionState.Closed)
                throw new MammothDatabaseStateException("The database is already closed.");
			
            m_connection.Close();
            m_connection.Dispose();
            m_connection = null;			
        }
        
        #endregion

        #region Query 
        /// <summary>
        /// Checks to see if a value is equal to the minimum or maximum value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if equal to min or max double value.  False otherwise.</returns>
        private bool IsValueMinimumOrMaximum(double value)
        {
            if (value.Equals(double.MinValue))
                return true;
            if (value.Equals(double.MaxValue))
                return true;

            return false;
		}/// <summary>
		/// Performs a query to the database to retrieve clusters and features in one shot.
		/// Uses a caching mechanism based on the last range query options.
		/// </summary>
		/// <param name="options">Range query options.</param>
		public int GetFeatureCount(MammothDatabaseRange options)
		{
			if (options == null)
				throw new NullReferenceException("The database search options were null.");

			// If the search options are the same, then return because it has been run before.
			//if (m_queryRange != null && options.Equals(m_queryRange))
			//	return 0;

			// SQL doesnt like the double minvalue and maxvalue 
			bool invalid = IsValueMinimumOrMaximum(options.MassMaximum);
			invalid = invalid || IsValueMinimumOrMaximum(options.MassMinimum);
			invalid = invalid || IsValueMinimumOrMaximum(options.NETMaximum);
			invalid = invalid || IsValueMinimumOrMaximum(options.NETMinimum);
			invalid = invalid || IsValueMinimumOrMaximum(options.DriftTimeMaximum);
			invalid = invalid || IsValueMinimumOrMaximum(options.DriftTimeMinimum);

			if (invalid)
				throw new InvalidDataException("One of the mammoth database search options was invalid.");

			if (m_connection == null)
				throw new NullReferenceException("The database connection has not been established.");

			if (m_connection.State != System.Data.ConnectionState.Open)
				throw new Exception("The database connection is not open: " + m_connection.State.ToString());

			// Cache the search options
			//m_queryRange = options;

			
			int count = 0;
            IDbCommand command = null;

            if (options.SingleChargeState < 1)
            {
                command = m_preparedCountStatementCommand;
                m_preparedCountStatementCommand.Parameters.Clear();
                m_preparedCountStatementCommand.Parameters.Add(new SQLiteParameter("MassLow", options.MassMinimum));
                m_preparedCountStatementCommand.Parameters.Add(new SQLiteParameter("MassHigh", options.MassMaximum));
                m_preparedCountStatementCommand.Parameters.Add(new SQLiteParameter("NetLow", options.NETMinimum));
                m_preparedCountStatementCommand.Parameters.Add(new SQLiteParameter("NetHigh", options.NETMaximum));
                m_preparedCountStatementCommand.Parameters.Add(new SQLiteParameter("DriftLow", options.DriftTimeMinimum));
                m_preparedCountStatementCommand.Parameters.Add(new SQLiteParameter("DriftHigh", options.DriftTimeMaximum));
            }
            else
            {
                command = m_preparedChargeCountStatementCommand;
                m_preparedChargeCountStatementCommand.Parameters.Clear();
                m_preparedChargeCountStatementCommand.Parameters.Add(new SQLiteParameter("MassLow", options.MassMinimum));
                m_preparedChargeCountStatementCommand.Parameters.Add(new SQLiteParameter("MassHigh", options.MassMaximum));
                m_preparedChargeCountStatementCommand.Parameters.Add(new SQLiteParameter("NetLow", options.NETMinimum));
                m_preparedChargeCountStatementCommand.Parameters.Add(new SQLiteParameter("NetHigh", options.NETMaximum));
                m_preparedChargeCountStatementCommand.Parameters.Add(new SQLiteParameter("DriftLow", options.DriftTimeMinimum));
                m_preparedChargeCountStatementCommand.Parameters.Add(new SQLiteParameter("DriftHigh", options.DriftTimeMaximum));
                m_preparedChargeCountStatementCommand.Parameters.Add(new SQLiteParameter("Charge", Convert.ToInt32(options.SingleChargeState)));
            }
			using (IDataReader reader = command.ExecuteReader())
			{					
				bool readOk = reader.Read();
				if (readOk)
				{
					count = Convert.ToInt32(reader.GetValue(0));
				}					
			}		
				
			return count;
		}
        /// <summary>
        /// Performs a query to the database to retrieve clusters and features in one shot.
        /// Uses a caching mechanism based on the last range query options.
        /// </summary>
        /// <param name="options">Range query options.</param>        
        private void RunQuery(MammothDatabaseRange options, bool includeClusteredData)
        {
            if (options == null)
                throw new NullReferenceException("The database search options were null.");
           
            // If the search options are the same, then return because it has been run before.
            if (m_queryRange != null && options.Equals(m_queryRange))
                return;

            // SQL doesnt like the double minvalue and maxvalue 
            bool invalid = IsValueMinimumOrMaximum(options.MassMaximum);            
            invalid      = invalid || IsValueMinimumOrMaximum(options.MassMinimum);
            invalid      = invalid || IsValueMinimumOrMaximum(options.NETMaximum);
            invalid      = invalid || IsValueMinimumOrMaximum(options.NETMinimum);
            invalid      = invalid || IsValueMinimumOrMaximum(options.DriftTimeMaximum);
            invalid      = invalid || IsValueMinimumOrMaximum(options.DriftTimeMinimum);

           // if (invalid)
           //     throw new InvalidDataException("One of the mammoth database search options was invalid.");
                        
            if (m_connection == null)
                throw new NullReferenceException("The database connection has not been established.");

            if (m_connection.State != System.Data.ConnectionState.Open)
                throw new Exception("The database connection is not open: " + m_connection.State.ToString());

            // Cache the search options
            m_queryRange = options;

            IDbCommand command = null;
            Flush();
            if (includeClusteredData)
            {
                if (options.SingleChargeState < 1)
                {
                    command = m_featureQueryNoClusterStatementCommand;
                    // Read features with clusters.              
                    m_featureQueryNoClusterStatementCommand.Parameters.Clear();
                    m_featureQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("MassLow", options.MassMinimum));
                    m_featureQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("MassHigh", options.MassMaximum));
                    m_featureQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("NetLow", options.NETMinimum));
                    m_featureQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("NetHigh", options.NETMaximum));
                    m_featureQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("DriftLow", options.DriftTimeMinimum));
                    m_featureQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("DriftHigh", options.DriftTimeMaximum));
                }
                else
                {
                    command = m_featureChargeQueryNoClusterStatementCommand;
                    // Read features with clusters.              
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Clear();
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("MassLow", options.MassMinimum));
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("MassHigh", options.MassMaximum));
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("NetLow", options.NETMinimum));
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("NetHigh", options.NETMaximum));
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("DriftLow", options.DriftTimeMinimum));
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("DriftHigh", options.DriftTimeMaximum));
                    m_featureChargeQueryNoClusterStatementCommand.Parameters.Add(new SQLiteParameter("Charge", Convert.ToInt32(options.SingleChargeState)));
                }

                using (IDataReader reader = command.ExecuteReader())
                {

                    // This holds a map of cluster id to cluster objects so we can easily add features
                    // to clusters in O(1) time.  
                    Dictionary<int, UMCClusterLight> clusters = new Dictionary<int, UMCClusterLight>();

                    // Read the results and keep them if they have a valid cluster ID.
                    while (reader.Read())
                    {
                        object[] values = new object[CONST_TOTAL_FEATURE_NO_CLUSTER_QUERY_SIZE];
                        reader.GetValues(values);

                        // Extract all of the UMC data. 
                        UMCLight umc            = new UMCLight();
                        umc.ID                  = Convert.ToInt32(values[CONST_UMC_ID]);
                        umc.MassMonoisotopic    = Convert.ToDouble(values[CONST_UMC_MONOISOTOPIC_MASS]);
                        umc.NET                 = Convert.ToDouble(values[CONST_UMC_NET]);
                        umc.RetentionTime       = umc.NET;
                        umc.DriftTime           = Convert.ToSingle(values[CONST_UMC_DRIFT_TIME]);
                        umc.GroupID             = Convert.ToInt32(values[CONST_UMC_GROUP_ID]);
                        umc.ChargeState         = Convert.ToInt32(values[CONST_UMC_CHARGE]);
                        int clusterID           = Convert.ToInt32(values[CONST_UMC_CLUSTER_ID]);
                        umc.Abundance           = Convert.ToInt32(values[CONST_TOTAL_FEATURE_NO_CLUSTER_QUERY_SIZE - 1]);
                        umc.ScanStart           = Convert.ToInt32(values[CONST_TOTAL_FEATURE_NO_CLUSTER_QUERY_SIZE]);
                        umc.ScanEnd             = Convert.ToInt32(values[CONST_TOTAL_FEATURE_NO_CLUSTER_QUERY_SIZE + 1]);
                        umc.Scan                = Convert.ToInt32(values[CONST_TOTAL_FEATURE_NO_CLUSTER_QUERY_SIZE + 2]);
                        if (!clusters.ContainsKey(clusterID))
                        {
                            // No cluster exists in the map, so we need to create a new one.
                            UMCClusterLight cluster     = new UMCClusterLight();
                            cluster.UMCList.Add(umc);
                            cluster.ID                  = clusterID;
                            cluster.MassMonoisotopic    = Convert.ToDouble(values[CONST_UMC_MONOISOTOPIC_MASS]);
                            cluster.NET                 = Convert.ToDouble(values[CONST_UMC_NET]);
                            cluster.RetentionTime       = cluster.NET;
                            cluster.DriftTime           = Convert.ToSingle(values[CONST_UMC_DRIFT_TIME]);                                                        
                            cluster.ChargeState         = Convert.ToInt32(values[CONST_CLUSTER_CHARGE]);

                            umc.UMCCluster = cluster;
                            clusters.Add(clusterID, cluster);
                        }
                        else
                        {
                            // The cluster exists, so just update the cluster with this UMC.
                            clusters[clusterID].UMCList.Add(umc);
                            umc.UMCCluster = clusters[clusterID];
                        }
                        m_features.Add(umc);
                    }

                    // Make sure that we get the cluster array as well.
                    m_clusters.AddRange(clusters.Values);
                }
            }

            // Read Features without clusters.    

            if (options.SingleChargeState < 1)
            {
                command = m_featureQueryStatementCommand;
                m_featureQueryStatementCommand.Parameters.Clear();
                m_featureQueryStatementCommand.Parameters.Add(new SQLiteParameter("MassLow", options.MassMinimum));
                m_featureQueryStatementCommand.Parameters.Add(new SQLiteParameter("MassHigh", options.MassMaximum));
                m_featureQueryStatementCommand.Parameters.Add(new SQLiteParameter("NetLow", options.NETMinimum));
                m_featureQueryStatementCommand.Parameters.Add(new SQLiteParameter("NetHigh", options.NETMaximum));
                m_featureQueryStatementCommand.Parameters.Add(new SQLiteParameter("DriftLow", options.DriftTimeMinimum));
                m_featureQueryStatementCommand.Parameters.Add(new SQLiteParameter("DriftHigh", options.DriftTimeMaximum));                
            }
            else
            {
                command = m_featureChargeQueryStatementCommand;
                m_featureChargeQueryStatementCommand.Parameters.Clear();
                m_featureChargeQueryStatementCommand.Parameters.Add(new SQLiteParameter("MassLow", options.MassMinimum));
                m_featureChargeQueryStatementCommand.Parameters.Add(new SQLiteParameter("MassHigh", options.MassMaximum));
                m_featureChargeQueryStatementCommand.Parameters.Add(new SQLiteParameter("NetLow", options.NETMinimum));
                m_featureChargeQueryStatementCommand.Parameters.Add(new SQLiteParameter("NetHigh", options.NETMaximum));
                m_featureChargeQueryStatementCommand.Parameters.Add(new SQLiteParameter("DriftLow", options.DriftTimeMinimum));
                m_featureChargeQueryStatementCommand.Parameters.Add(new SQLiteParameter("DriftHigh", options.DriftTimeMaximum));                
                m_featureChargeQueryStatementCommand.Parameters.Add(new SQLiteParameter("Charge", Convert.ToInt32(options.SingleChargeState)));
            }

            using (IDataReader reader = command.ExecuteReader())
            {
                // Read the results and keep them if they have a valid cluster ID.
                while (reader.Read())
                {
                    object[] values             = new object[11];
                    reader.GetValues(values);
                     
                    UMCLight umc                     = new UMCLight();
                    umc.ID                      = Convert.ToInt32(values[CONST_UMC_ID]);
                    umc.MassMonoisotopic		= Convert.ToDouble(values[CONST_UMC_MONOISOTOPIC_MASS]);
                    umc.NET			            = Convert.ToDouble(values[CONST_UMC_NET]);
                    umc.RetentionTime           = umc.NET; 
                    umc.DriftTime               = Convert.ToSingle(values[CONST_UMC_DRIFT_TIME]);
                    umc.GroupID                 = Convert.ToInt32(values[CONST_UMC_GROUP_ID]);
                    umc.ChargeState             = Convert.ToInt32(values[CONST_UMC_CHARGE_FEATURE]);
                    umc.UMCCluster              = null;
                    umc.Abundance               = Convert.ToInt32(values[7]);
                    umc.ScanStart               = Convert.ToInt32(values[8]);
                    umc.ScanEnd                 = Convert.ToInt32(values[9]);
                    umc.Scan                    = Convert.ToInt32(values[10]);
                    m_features.Add(umc);
                }
            }            
        }
        /// <summary>
        /// Retrieves only the non-clustered features.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public List<UMCLight> GetNonClusteredFeatures(MammothDatabaseRange options)
        {
            
            // We cache results from the previous query.  This way we don't 
            // run the query twice.  Results are cached in m_features. 
            // NOTE:  Running the query will also update the m_clusters cache.  
            // We hide this fact from the user.  This should come at no performance
            // penalty to the user, since we are caching data.
            Flush();
            RunQuery(options, false);
            return m_features;
        }
        /// <summary>
        /// Finds all features within the specified tolerances of options.
        /// </summary>
        /// <param name="options">Tolerances for defining the range query.</param>
        /// <returns>A list of potential features</returns>
        public List<UMCLight> GetFeatures(MammothDatabaseRange options)
        {
            // We cache results from the previous query.  This way we don't 
            // run the query twice.  Results are cached in m_features. 
            // NOTE:  Running the query will also update the m_clusters cache.  
            // We hide this fact from the user.  This should come at no performance
            // penalty to the user, since we are caching data.
            RunQuery(options, true);
            return m_features;
        }
        /// <summary>
        /// Given a set of search options, retrieve the closest set of clusters.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public List<UMCClusterLight> GetClusters(MammothDatabaseRange options)
        {
            // We cache results from the previous query.  This way we don't 
            // run the query twice.  Results are cached in m_clusters.
            // NOTE:  Running the query will also update the m_features cache.  
            // We hide this fact from the user.  This should come at no performance
            // penalty to the user, since we are caching data.
            RunQuery(options, true);
            return m_clusters;
        }
        public void Flush()
        {
            m_clusters.Clear();
            m_features.Clear();
            m_queryRange = null;
        }
        #endregion

        #region Inserting and Updating feature/cluster data.
        /// <summary>
        /// Validates the database connection and feature data.
        /// </summary>
        /// <param name="features">Features provided from the user to validate.</param>
        /// <exception cref="System.NullReferenceException"></exception>
        /// <exception cref="System.Exception"></exception>
        private void ValidateConnectionAndFeatureData<T>(List<T> features) where T : FeatureLight
        {
            if (m_connection == null)
                throw new NullReferenceException("The database connection has not been established.");

            if (m_connection.State != System.Data.ConnectionState.Open)
                throw new Exception("The database connection is not open: " + m_connection.State.ToString());

            // Make sure the UMC's and UMC Clusters are valid.
            if (features == null)
                throw new NullReferenceException("The features provided were null.");

            int nullIndex = features.FindIndex(delegate(T x) { return x == null; });
            if (nullIndex >= 0)
                throw new NullReferenceException("One of the values of the feature list was null.");
        }       
        /// <summary>
        /// Updates the database with the feature data provided.  Also updates the found clusters.
        /// </summary>
        /// <param name="features">Features to update.</param>
        public int UpdateFeaturesAndClusters(List<UMCLight> features)
        {
            int totalSkipped = 0;
            ValidateConnectionAndFeatureData<UMCLight>(features);

			if (features.Count <= 0)
			{
				return totalSkipped;
			}
            // Since we update, then we should invalidate the searches.
            m_queryRange = null;

            Dictionary<int, bool> newClusterMap = new Dictionary<int, bool>();

            using (SQLiteTransaction transaction = m_connection.BeginTransaction())
            {
                // For every feature update the feature ID to cluster links.
                //   - We dont need to modify the masses, nets, or drifts, because we don't 
                //     modify that information.                    
                foreach (UMCLight feature in features)
                {
                    // Make sure that we also update the cluster data.
                    int clusterID           = -1;
                    UMCClusterLight cluster = feature.UMCCluster as UMCClusterLight;
                    if (cluster != null)
                    {
                        clusterID = feature.UMCCluster.ID;
                        // Insert a new cluster if we have not seen it before.
                        if (!newClusterMap.ContainsKey(clusterID))
                        {
                            newClusterMap.Add(clusterID, true);

                            // Calculate the UMC Member count.
                            int membercount = cluster.UMCList.Count;
                            Dictionary<int, int> map = new Dictionary<int, int>();
                            foreach (UMCLight umc in cluster.UMCList)
                            {
                                if (!map.ContainsKey(umc.GroupID))
                                {
                                    map.Add(umc.GroupID, 0);
                                }
                                map[umc.GroupID]++;
                            }

                            m_clusterInsertStatementCommand.Parameters.Clear();
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("id", clusterID));
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("mass", cluster.MassMonoisotopic));
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("net", cluster.RetentionTime));
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("drift_time", cluster.DriftTime));
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("charge", cluster.ChargeState));
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("score", cluster.Score));
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("dataset_member_count", map.Keys.Count));
                            m_clusterInsertStatementCommand.Parameters.Add(new SQLiteParameter("member_count", membercount));
                            m_clusterInsertStatementCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        totalSkipped++;
                    }
                    m_featureUpdateStatementCommand.Parameters.Clear();
                    m_featureUpdateStatementCommand.Parameters.Add(new SQLiteParameter("cluster",   clusterID.ToString()));
                    m_featureUpdateStatementCommand.Parameters.Add(new SQLiteParameter("featureID", feature.ID.ToString()));
                    m_featureUpdateStatementCommand.Parameters.Add(new SQLiteParameter("datasetID", feature.GroupID.ToString()));
                    m_featureUpdateStatementCommand.ExecuteNonQuery();                                             					
                }
                transaction.Commit();
            }
            return totalSkipped;          
        }       
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Cleans up any connected database objects.
        /// </summary>
        public void Dispose()
        {
            if (m_connection != null)
            {
                try
                {
                    // Don't call the close method, as it already cleans up the dispose methods.
                    if (m_connection.State != ConnectionState.Closed)
                        m_connection.Close();

					m_featureQueryNoClusterStatementCommand.Dispose();
					m_featureInsertStatementCommand.Dispose();
					m_featureQueryStatementCommand.Dispose();
					m_featureUpdateStatementCommand.Dispose();
					m_clusterInsertStatementCommand.Dispose();
					m_preparedCountStatementCommand.Dispose();
                }
                catch
                {
                    //Nothing here to do, we know we may get an exception if the database is 
                }
                finally
                {
                    Flush();
                    m_connection.Dispose();
                    m_connection = null;
                }
            }
        }

        #endregion
    }   
}
