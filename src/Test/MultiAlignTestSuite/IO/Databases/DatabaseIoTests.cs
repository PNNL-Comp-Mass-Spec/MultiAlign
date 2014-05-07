using System.Data.SQLite;
using NUnit.Framework;

namespace MultiAlignTestSuite.IO.Databases
{
    [TestFixture]
    public class DatabaseIoTests
    {

        [Test]
        [TestCase(@"M:\data\proteomics\Applications\Kyle_IFL001_NEG\Test\Kyle-IFL001_NEG_MultiAlign.db3")]
        public void CreateIndex(string path)
        {

            using (var connection = new SQLiteConnection(string.Format("Data Source = {0};", path)))
            {
                connection.Open();


                using (var command = connection.CreateCommand())
                {

                    // UMC Clusters Index
                    command.CommandText = "CREATE INDEX idx_cluster_id on T_Clusters(Cluster_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_umc_cluster_id on T_LCMS_Features(Cluster_ID ASC)";
                    command.ExecuteNonQuery();


                    // Feature Index for UMC
                    command.CommandText = "CREATE INDEX idx_lcmsFeature_id on T_LCMS_Features(Feature_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_msLcmsfeature_id on T_MSFeatures(LCMS_FEATURE_ID ASC)";
                    command.ExecuteNonQuery();

                    // Feature Indexes for MS and MSn
                    command.CommandText = "CREATE INDEX idx_msfeature_id on T_MSFeatures(FEATURE_ID ASC)";
                    command.ExecuteNonQuery();
                    
                    command.CommandText = "CREATE INDEX idx_spectra_id on T_MSn_Features(SPECTRA_ID ASC)";
                    command.ExecuteNonQuery();

                    // MSMS Mapping Table
                    command.CommandText = "CREATE INDEX idx_msFeature_id on T_MSnFeature_To_MSFeature_Map(MS_Feature_ID ASC)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX idx_msmsFeature_id on T_MSnFeature_To_MSFeature_Map(MSn_Feature_ID ASC)";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
