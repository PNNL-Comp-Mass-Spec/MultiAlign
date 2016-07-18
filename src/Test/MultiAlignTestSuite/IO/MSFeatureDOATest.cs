using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using NUnit.Framework;

namespace MultiAlignTestSuite
{

    [TestFixture]
    public class MSFeatureDOATest
    {

        [Test]
        public void SaveMSFeatures()
        {
            var lights = new List<MSFeatureLight>();
            for (var i = 0; i < 100; i++)
            {
                lights.Add(new MSFeatureLight());
                lights[i].Id = i;
            }

            IMSFeatureDAO cache = new MSFeatureDAOHibernate();
            cache.AddAll(lights);
        }

        [Test]
        [TestCase(@"m:\data\proteomics\matest-gui\guitest.db3")]
        public List<MSFeatureLight> LoadMSFeaturesFromCache(string path)
        {
            var features = new List<MSFeatureLight>();
            var start = System.DateTime.Now;
            using (var connection = new SQLiteConnection("Data Source=" + path))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "SELECT * FROM T_MSFeatures where DATASET_ID = 0";
                    using (var reader = command.ExecuteReader())
                    {
                        var x = 0;
                        var values = new object[100];
                        while (reader.Read())
                        {
                            var feature                  = new MSFeatureLight();
                            feature.DriftTime                       = Convert.ToDouble(reader["DriftTime"]);
                            feature.Score                           = Convert.ToDouble(reader["FIT"]);
                            feature.Scan                            = Convert.ToInt32(reader["SCAN_NUM"]);
                            feature.ChargeState                     = Convert.ToInt32(reader["CHARGE"]);
                            feature.Abundance                       = Convert.ToInt64(reader["ABUNDANCE"]);
                            feature.Mz                              = Convert.ToDouble(reader["MZ"]);
                            feature.MassMonoisotopicAverage         = Convert.ToDouble(reader["AVERAGE_MW"]);
                            feature.MassMonoisotopic                = Convert.ToDouble(reader["MONOISOTOPIC_MW"]);
                            feature.MassMonoisotopicMostAbundant    = Convert.ToDouble(reader["MONOISOTOPIC_MW_ABUNDANT"]);
                            feature.UmcId                           = Convert.ToInt32(reader["LCMS_FEATURE_ID"]);
                            x++;
                            features.Add(feature);
                        }
                        System.Console.WriteLine("{0} features loaded", x);
                    }
                }
            }

            var end = System.DateTime.Now;
            var span = end.Subtract(start);
            System.Console.WriteLine("{0} features loaded", span.TotalMilliseconds);

            return features;
        }
    }
}
