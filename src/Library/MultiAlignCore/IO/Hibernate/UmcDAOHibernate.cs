namespace MultiAlignCore.IO.Hibernate
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.Features;
    using NHibernate.Criterion;

    /// <summary>
    /// Class for implementing operations that can be performed on feature data from a NHibernate
    /// data-access object.
    /// </summary>
    public class UmcDAOHibernate : GenericDAOHibernate<UMCLight>, IUmcDAO
    {
        /// <summary>
        /// Find features by mass.
        /// </summary>
        /// <param name="mass">The mass of the feature.</param>
        /// <returns>The features found for the provided mass.</returns>
        public List<UMCLight> FindByMass(double mass)
        {
            ICriterion criterion = Restrictions.Eq("Mass", mass);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);

            return this.FindByCriteria(criterionList);
        }

        /// <summary>
        /// Find features within a mass range.
        /// </summary>
        /// <param name="mass1">The minimum mass (inclusive).</param>
        /// <param name="mass2">The maximum mass (inclusive).</param>
        /// <returns>The features within the provided mass range.</returns>
        public List<UMCLight> FindByMassRange(double mass1, double mass2)
        {
            ICriterion criterion;

            if (mass1 <= mass2)
            {
                criterion = Restrictions.Between("Mass", mass1, mass2);
            }
            else
            {
                criterion = Restrictions.Between("Mass", mass2, mass1);
            }

            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);

            return this.FindByCriteria(criterionList);
        }

        /// <summary>
        /// Finds a feature based on a unique feature id.
        /// </summary>
        /// <param name="id">The unique identification number of the feature to find.</param>
        /// <returns>The feature found with the provided identification number.</returns>
        public UMCLight FindByFeatureID(int id)
        {
            ICriterion criterion = Restrictions.Eq("Id", id);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            var umcs = FindByCriteria(criterionList);
            if (umcs.Count < 1)
                return null;

            return umcs[0];
        }

        /// <summary>
        /// Delete all features for a provided dataset.
        /// </summary>
        /// <param name="datasetId">The dataset to delete features for.</param>
        public void DeleteByDataset(int datasetId)
        {
            DeleteByCriteria("T_LCMS_Features", "DATASET_ID", datasetId);
        }

        /// <summary>
        /// Finds all features with the provided charge state.
        /// </summary>
        /// <param name="charge">The charge state.</param>
        /// <returns>The features found with the provided charge state.</returns>
        public List<UMCLight> FindByCharge(int charge)
        {
            ICriterion criterion = Restrictions.Eq("ChargeState", charge);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Finds all features with provided charge that are part of the provided dataset.
        /// </summary>
        /// <param name="charge">The charge state.</param>
        /// <param name="dataset">The unique identification number of features to find.</param>
        /// <returns>The features.</returns>
        public List<UMCLight> FindByChargeDataset(int charge, int dataset)
        {
            ICriterion criterion = Restrictions.Eq("ChargeState", charge);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Finds a feature based on a unique feature id.
        /// </summary>
        /// <param name="id">The unique identification number of the feature to find.</param>
        /// <returns>The feature found with the provided identification number.</returns>
        public List<UMCLight> FindByFeatureID(List<int> id)
        {
            ICriterion criterion = Restrictions.In("Id", id);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            var umcs = this.FindByCriteria(criterionList);
            if (umcs.Count < 1)
            {
                return null;
            }

            return umcs;
        }

        /// <summary>
        /// Save all features for the provided dataset.
        /// </summary>
        /// <param name="features">The features to save.</param>
        /// <param name="datasetId">The ID of the the dataset.</param>
        /// <param name="progress">The progress reporter to report percent complete and status message to.</param>
        public void SaveFeaturesByDataset(List<UMCLight> features, int datasetId, IProgress<ProgressData> progress = null)
        {
            this.DeleteByDataset(datasetId);
            this.AddAllStateless(features, progress);
        }

        /// <summary>
        /// Find the highest charge state in all datasets.
        /// </summary>
        /// <returns>The charge state.</returns>
        public int FindMaxCharge()
        {
            using (var session = this.GetSession())
            {
                var data = session.CreateCriteria(typeof (UMCLight))
                    .SetProjection(Projections.Max("ChargeState"))
                    .UniqueResult();
                return Convert.ToInt32(data);
            }
        }

        /// <summary>
        /// Finds features that belong to a cluster with the provided unique identification number.
        /// </summary>
        /// <param name="id">The unique identification number of the cluster.</param>
        /// <returns>The features that are part of the cluster.</returns>
        public List<UMCLight> FindByClusterID(int id)
        {
            ICriterion criterion = Restrictions.Eq("ClusterId", id);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            var umcs = FindByCriteria(criterionList);
            if (umcs.Count < 1)
                return null;

            return umcs;
        }

        /// <summary>
        /// Finds features that belong to several clusters with the provided unique identification numbers.
        /// </summary>
        /// <param name="id">The unique identification number of the cluster.</param>
        /// <returns>The features that are part of the clusters.</returns>
        public List<UMCLight> FindByClusterID(List<int> idList)
        {
            ICriterion criterion = Restrictions.In("ClusterId", idList);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            var umcs = FindByCriteria(criterionList);
            if (umcs.Count < 1)
            {
                return null;
            }

            return umcs;
        }

        /// <summary>
        /// Finds all features that are part of the given dataset.
        /// </summary>
        /// <param name="">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        public List<UMCLight> FindByDatasetId(int datasetId)
        {
            ICriterion criterion = Restrictions.Eq("GroupId", datasetId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Finds all features that are members of clusters.
        /// </summary>
        /// <returns>The features found.</returns>
        public List<UMCLight> FindAllClustered()
        {
            ICriterion criterion = Restrictions.Gt("ClusterId", -1);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Clears old alignment data from all of the features.
        /// </summary>
        public void ClearAlignmentData()
        {
            using (var connection = new SQLiteConnection("Data Source=" + NHibernateUtil.Path, true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "UPDATE T_LCMS_Features  SET Mass_Aligned = -1, NET = -1, Scan_Aligned = -1";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Retrieves the charge states for all features.
        /// </summary>
        /// <returns>Collection of charge states.</returns>
        public IEnumerable<int> RetrieveChargeStates()
        {
            var charges = new List<int>();
            using (var connection = new SQLiteConnection("Data Source=" + NHibernateUtil.Path, true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT Charge FROM T_LCMS_Features";
                    var reader = command.ExecuteReader();

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var charge = reader.GetInt32(0);
                            charges.Add(charge);
                        }
                    }
                }
                connection.Close();
            }
            return charges;
        }
    }
}