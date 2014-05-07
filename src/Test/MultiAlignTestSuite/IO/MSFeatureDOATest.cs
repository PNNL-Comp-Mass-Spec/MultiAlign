using System.Collections.Generic;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;
using System.Data.SQLite;
using MultiAlignCore.IO.Features.Hibernate;
using System;

using NUnit.Framework;
namespace MultiAlignTestSuite
{

    [TestFixture]
    public class MSFeatureDOATest
    {

        [Test]
        public void SaveMSFeatures()
        {
            List<MSFeatureLight> lights = new List<MSFeatureLight>();
            for (int i = 0; i < 100; i++)
            {
                lights.Add(new MSFeatureLight());
                lights[i].ID = i;
            }

            IMSFeatureDAO cache = new MSFeatureDAOHibernate();
            cache.AddAll(lights);            
        }

        [Test]
        [TestCase(@"m:\data\proteomics\matest-gui\guitest.db3")]
        public List<MSFeatureLight> LoadMSFeaturesFromCache(string path)
        {
            List<MSFeatureLight> features = new List<MSFeatureLight>();
            System.DateTime start = System.DateTime.Now;
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + path))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "SELECT * FROM T_MSFeatures where DATASET_ID = 0";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        int x = 0;
                        object [] values = new object[100];
                        while (reader.Read())
                        {
                            MSFeatureLight feature                  = new MSFeatureLight();
                            feature.DriftTime                       = Convert.ToDouble(reader["DriftTime"]);
                            feature.Score                           = Convert.ToDouble(reader["FIT"]);
                            feature.Scan                            = Convert.ToInt32(reader["SCAN_NUM"]);
                            feature.ChargeState                     = Convert.ToInt32(reader["CHARGE"]);
                            feature.Abundance                       = Convert.ToInt64(reader["ABUNDANCE"]);
                            feature.Mz                              = Convert.ToDouble(reader["MZ"]);
                            feature.MassMonoisotopicAverage         = Convert.ToDouble(reader["AVERAGE_MW"]);
                            feature.MassMonoisotopic                = Convert.ToDouble(reader["MONOISOTOPIC_MW"]);
                            feature.MassMonoisotopicMostAbundant    = Convert.ToDouble(reader["MONOISOTOPIC_MW_ABUNDANT"]);
                            feature.UMCID                           = Convert.ToInt32(reader["LCMS_FEATURE_ID"]);
                            x++;
                            features.Add(feature);
                        }
                        System.Console.WriteLine("{0} features loaded", x);                        
                    }
                }
            }

            System.DateTime end = System.DateTime.Now;
            System.TimeSpan span = end.Subtract(start);
            System.Console.WriteLine("{0} features loaded", span.TotalMilliseconds);

            return features;
        }
    }
}
