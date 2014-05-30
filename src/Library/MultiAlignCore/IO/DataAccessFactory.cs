#region

using System.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.SequenceData;

#endregion

namespace MultiAlignCore.IO
{
    public static class DataAccessFactory
    {
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
            NHibernateUtil.ConnectToDatabase(path, true);
           
            IUmcDAO featureCache = new UmcDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();
            IMSFeatureDAO msFeatureCache = new MSFeatureDAOHibernate();
            IMSnFeatureDAO msnFeatureCache = new MSnFeatureDAOHibernate();
            IMsnFeatureToMSFeatureDAO msnMsCache = new MSnFeatureToMSFeatureDAOHibernate();
            IDatasetDAO datasetCache = new DatasetDAOHibernate();
            IMassTagDAO massTagCache = new MassTagDAOHibernate();
            IMassTagMatchDAO massTagMatchCache = new MassTagMatchDAO();
            ISequenceToMsnFeatureDao sequenceMap = new SequenceToMsnDaoHibernate();

            var providers =
                new FeatureDataAccessProviders(featureCache,
                    clusterCache,
                    msFeatureCache,
                    msnFeatureCache,
                    msnMsCache,
                    datasetCache,
                    massTagMatchCache,
                    massTagCache,
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