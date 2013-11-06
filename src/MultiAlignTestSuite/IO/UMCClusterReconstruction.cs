using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Extensions;
using NUnit.Framework;
using PNNLOmics.Data.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.Features;
using System.IO;
using PNNLOmics.Data;
using MultiAlignCore.Data;

namespace MultiAlignTestSuite.IO
{
    [TestFixture]
    public class UMCClusterReconstruction
    {
        protected string m_basePath;

        [SetUp]
        public void TestSetup()
        {
            m_basePath = @"M:\data\proteomics\Applications\Kyle_IFL001_NEG\test";
        }
        
        [Test]
        [TestCase(@"Kyle-IFL001_NEG_MultiAlign.db3")]   
        public void TestClusterReconstruction(string name)
        {
            CreateUMCClusterLight(Path.Combine(m_basePath, name), false);
        }


        public void CreateUMCClusterLight(string databasePath, bool indexDatabase)
        {
            // If the database is not index then do so...but before the session to the db is opened.
            if (indexDatabase)
            {
                DatabaseIndexer.IndexClusters(databasePath);
                DatabaseIndexer.IndexFeatures(databasePath);
            }

            // This is a factory based method that creates a set of data access providers used throughout MultiAlign
            FeatureDataAccessProviders providers = DataAccessFactory.CreateDataAccessProviders(databasePath, false);

            // If you just wanted the clusters you could do this:
            // 1. Connect to the database
            //NHibernateUtil.ConnectToDatabase(databasePath, false);
            // 2. Then extract all of the clusters 
            //IUmcClusterDAO clusterCache     = new UmcClusterDAOHibernate(); 
            //List<UMCClusterLight> clusters  = clusterCache.FindAll();

            List<UMCClusterLight> clusters = providers.ClusterCache.FindAll();
            bool shouldGetMsFeatures   = true;
            bool shouldGetMsMsFeatures = true;
            bool shouldGetRawData      = false;
           
            // This gets all of the dataset information and maps to a dictionary...if you want the raw data
            // otherwise comment this out.
            List<DatasetInformation> datasets = providers.DatasetCache.FindAll();
            Dictionary<int, DatasetInformation> datasetMap = new Dictionary<int,DatasetInformation>();
            datasets.ForEach(x => datasetMap.Add(x.DatasetId, x));

            foreach(var cluster in clusters)
            {
                cluster.ReconstructUMCCluster(providers,
                                              shouldGetMsFeatures, 
                                              shouldGetMsMsFeatures);

                foreach (UMCLight feature in cluster.Features)
                {
                    foreach (MSFeatureLight msFeature in feature.Features)
                    {
                        foreach(MSSpectra spectrumMetaData in msFeature.MSnSpectra)
                        {
                            // then you can do stuff with the ms/ms spectra
                            // If you had the path to the raw file, you could create a reader for you to extract the MS/MS spectra
                            // This supports mzXML and .RAW Thermo files based on the file extension.
                            if (shouldGetRawData)
                            {                                        
                                DatasetInformation info = null;
                                bool hasKey = datasetMap.TryGetValue(spectrumMetaData.GroupID, out info);
                                if (hasKey)
                                {
                                    if (info.Raw != null)
                                    {
                                        // This might seem kind of klunky, but it's called a bridge, this way I can access 
                                        // MS/MS spectra from PNNLOmics without having to reference any of the Thermo DLL's
                                        // Nor support file reading capability.  This is also nice because I don't have to load
                                        // several MS/MS spectra when analyzing large datasets for my spectral clustering work.
                                        ISpectraProvider rawReader = RawLoaderFactory.CreateFileReader(info.Raw.Path);
                                        rawReader.AddDataFile(info.Raw.Path, spectrumMetaData.GroupID);

                                        // Then grab the actual spectrum...
                                        List<XYData> spectrum = rawReader.GetRawSpectra(spectrumMetaData.Scan, spectrumMetaData.GroupID, 2);

                                        // Then do what you want...
                                        // Profit???
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
