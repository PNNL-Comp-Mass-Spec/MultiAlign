#region

using System.Data.SQLite;

#endregion

namespace MultiAlignCore.IO
{
    /// <summary>
    ///     Indexes a database for faster retrieval times.
    /// </summary>
    public class DatabaseIndexer
    {
        public static void IndexClusters(string path)
        {
            using (var connection = new SQLiteConnection(string.Format("Data Source = {0};", path), true))
            {
                connection.Open();


                using (var command = connection.CreateCommand())
                {
                    // UMC Clusters Index
                    command.CommandText = "CREATE INDEX idx_cluster_id on T_Clusters(Cluster_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_umc_cluster_id on T_LCMS_Features(Cluster_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_cluster_mt_id on T_Cluster_To_Mass_Tag_Map(Cluster_ID ASC)";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static void IndexClustersDrop(string path)
        {
            using (var connection = new SQLiteConnection(string.Format("Data Source = {0};", path), true))
            {
                connection.Open();


                using (var command = connection.CreateCommand())
                {
                    // UMC Clusters Index
                    command.CommandText = "DROP INDEX IF EXISTS idx_cluster_id";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_umc_cluster_id";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_cluster_mt_id";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static void IndexFeatures(string path)
        {
            using (var connection = new SQLiteConnection(string.Format("Data Source = {0};", path), true))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Feature Index for UMC
                    command.CommandText = "CREATE INDEX idx_lcmsFeature_id on T_LCMS_Features(Feature_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_msLcmsfeature_id on T_MSFeatures(LCMS_FEATURE_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_lcmsFeature_dataset_id on T_LCMS_Features(Dataset_ID ASC)";
                    command.ExecuteNonQuery();

                    // Feature Indexes for MS and MSn
                    command.CommandText = "CREATE INDEX idx_msfeature_id on T_MSFeatures(FEATURE_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_umc ON T_MSFeatures (Dataset_ID, LCMS_Feature_ID)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_spectra_id on T_MSn_Features(SPECTRA_ID ASC)";
                    command.ExecuteNonQuery();

                    // MSMS Mapping Table
                    command.CommandText =
                        "CREATE INDEX idx_msMsnFeature_id on T_MSnFeature_To_MSFeature_Map(MS_Feature_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        "CREATE INDEX idx_msn_umc ON T_MSnFeature_To_MSFeature_Map(MS_Dataset_ID, LCMS_Feature_ID)";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        "CREATE INDEX idx_msnFeature_id on T_MSnFeature_To_MSFeature_Map(MSn_Feature_ID ASC)";
                    command.ExecuteNonQuery();


                    // Create the peptide sequences index
                    command.CommandText =
                        "CREATE INDEX idx_databaseIndex_id on T_DatabaseSearch(Dataset_ID ASC, LCMS_Feature_ID)";
                    command.ExecuteNonQuery();


                    command.CommandText =
                        "CREATE INDEX idx_msnDatbaseIndex_id on T_DatabaseSearch_To_MsnFeature(Dataset_ID ASC, LCMS_Feature_ID )";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static void IndexFeaturesDrop(string path)
        {
            using (var connection = new SQLiteConnection(string.Format("Data Source = {0};", path), true))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Feature Index for UMC
                    command.CommandText = "DROP INDEX IF EXISTS idx_lcmsFeature_id";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_lcmsFeature_dataset_id";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_msLcmsfeature_id";
                    command.ExecuteNonQuery();

                    // Feature Indexes for MS and MSn
                    command.CommandText = "DROP INDEX IF EXISTS idx_msfeature_id";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_umc";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_spectra_id";
                    command.ExecuteNonQuery();

                    // MSMS Mapping Table
                    command.CommandText = "DROP INDEX IF EXISTS idx_msMsnFeature_id";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_msn_umc";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP INDEX IF EXISTS idx_msnFeature_id";
                    command.ExecuteNonQuery();


                    // Create the peptide sequences index
                    command.CommandText = "DROP INDEX IF EXISTS idx_databaseIndex_id";
                    command.ExecuteNonQuery();


                    command.CommandText = "DROP INDEX IF EXISTS idx_msnDatbaseIndex_id";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}