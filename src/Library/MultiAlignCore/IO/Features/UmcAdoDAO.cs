#region

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Class that extracts features from a sqlite database using ADO
    /// </summary>
    public class UmcAdoDAO : IUmcDAO
    {
        public string DatabasePath { get; set; }

        #region IUmcDAO Members

        public List<UMCLight> FindByMass(double mass)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> FindByMassRange(double mass1, double mass2)
        {
            throw new NotImplementedException();
        }

        public UMCLight FindByFeatureID(int id)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> FindByClusterID(List<int> idList)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> FindByClusterID(int id)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> FindByDatasetId(int datasetId)
        {
            throw new NotImplementedException();
        }

        public void DeleteByDataset(int datasetId)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> FindByCharge(int charge)
        {
            var features = new List<UMCLight>();


            var featurecount = 0;
            var cuont = 0;
            using (var connection = new SQLiteConnection(string.Format("Data Source = {0}", DatabasePath), true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM T_LCMS_FEATURES WHERE CHARGE = {0}", charge);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var values = new object[20];
                            reader.GetValues(values);

                            var feature = new UMCLight();
                            feature.ChargeState = Convert.ToInt32(values[11]);
                            feature.MassMonoisotopicAligned = Convert.ToDouble(values[5]);
                            feature.Net = Convert.ToDouble(values[6]);
                            feature.DriftTime = Convert.ToDouble(values[15]);
                            feature.AbundanceSum = Convert.ToInt64(values[14]);
                            feature.Abundance = Convert.ToInt64(values[13]);
                            feature.GroupId = Convert.ToInt32(values[1]);
                            feature.Id = Convert.ToInt32(values[0]);
                            features.Add(feature);

                            featurecount++;
                            if (featurecount > 100000)
                            {
                                cuont ++;
                                Logger.PrintMessage(string.Format("\tLoaded {0}00000 features", cuont));
                                featurecount = 0;
                            }
                        }
                    }
                }
                connection.Close();
            }

            return features;
        }

        public List<UMCLight> FindByChargeDataset(int charge, int dataset)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> FindAllClustered()
        {
            throw new NotImplementedException();
        }

        public int FindMaxCharge()
        {
            throw new NotImplementedException();
        }

        public void ClearAlignmentData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGenericDAO<UMCLight> Members

        public void Add(UMCLight t)
        {
            throw new NotImplementedException();
        }

        public void AddAll(ICollection<UMCLight> tList, IProgress<PRISM.ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public void AddAllStateless(ICollection<UMCLight> tList, IProgress<PRISM.ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public void Update(UMCLight t)
        {
            throw new NotImplementedException();
        }

        public void UpdateAll(ICollection<UMCLight> tList, IProgress<PRISM.ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateAllStateless(ICollection<UMCLight> tList, IProgress<PRISM.ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateAll(ICollection<UMCLight> tList)
        {
            throw new NotImplementedException();
        }

        public void Delete(UMCLight t)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(ICollection<UMCLight> tList)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllStateless(ICollection<UMCLight> tList)
        {
            throw new NotImplementedException();
        }

        public UMCLight FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> FindAll()
        {
            throw new NotImplementedException();
        }

        #endregion

        public IEnumerable<int> RetrieveChargeStates()
        {
            return new List<int>();
        }
    }
}