#region

using System;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.SequenceData;

#endregion

namespace MultiAlignCore.IO.Features
{
    public class DataAccessFactory
    {
        public static FeatureDataAccessProviders CreateDataAccessProviders(MultiAlignAnalysis analysis,
            bool deleteIfExists)
        {
            var path = AnalysisPathUtils.BuildAnalysisName(analysis.MetaData.AnalysisPath,
                analysis.MetaData.AnalysisName);
            return CreateDataAccessProviders(path, deleteIfExists);
        }

        public static FeatureDataAccessProviders CreateDataAccessProviders(string path, bool deleteIfExists)
        {
            if (deleteIfExists)
            {
                var exists = File.Exists(path);
                if (exists)
                {
                    File.Delete(path);
                }
            }
            try
            {
                NHibernateUtil.ConnectToDatabase(path, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            IUmcDAO featureCache = new UmcDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IMSFeatureDAO msFeatureCache = new MSFeatureDAOHibernate();
            IMSnFeatureDAO msnFeatureCache = new MSnFeatureDAOHibernate();
            IMsnFeatureToMSFeatureDAO msnMSCache = new MSnFeatureToMSFeatureDAOHibernate();
            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IMassTagDAO massTagCache = new MassTagDAOHibernate();
            IMassTagMatchDAO massTagMatchCache = new MassTagMatchDAO();
            ISequenceToMsnFeatureDAO sequenceMap = new SequenceToMsnDAOHibernate();

            var providers =
                new FeatureDataAccessProviders(featureCache,
                    clusterCache,
                    msFeatureCache,
                    msnFeatureCache,
                    msnMSCache,
                    datasetCache,
                    massTagMatchCache,
                    massTagCache,
                    new FactorDAOHibernate(),
                    new DatasetToFactorDAOHibernate(),
                    new MSMSClusterMapClusterDAOHibernate(),
                    new DatabaseSearchSequenceDAOHiberate(),
                    sequenceMap);

            return providers;
        }

        public static void CleanupDataProviders()
        {
            NHibernateUtil.Dispose();
        }
    }
}