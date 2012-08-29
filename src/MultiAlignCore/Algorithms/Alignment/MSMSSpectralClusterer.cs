using System;
using MultiAlignCore.Data.SequenceData;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using System.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Algorithms.Alignment
{
    public class MSMSSpectralClusterer: IProgressNotifer
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        public MSMSSpectralClusterer()
        {
            SpectralSimilarityTolerance = .75;
        }
        
        #region Delegate Handlers / Marshallers
        /// <summary>
        /// Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message, 0));
            }
        }
        #endregion

        
        /// <summary>
        /// Gets or sets the spectral similarity tolerance.
        /// </summary>
        public double SpectralSimilarityTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the top percentage for peaks to use with spectral clustering.
        /// </summary>
        public double TopPercent
        {
            get;
            set;
        }

        private void RegisterNotifier(IProgressNotifer notifier)
        {
        }
        private void DeRegisterNotifier(IProgressNotifer notifier)
        {
        }
        /// <summary>
        /// Clusters a set of MS/MS spectra together.
        /// </summary>
        /// <param name="analysis"></param>
        public void ClusterMSMSSpectra(MultiAlignAnalysis analysis)
        {           
            Dictionary<int, Dictionary<int, Dictionary<int, DatabaseSearchSequence>>> sequenceMap =
                new Dictionary<int, Dictionary<int, Dictionary<int, DatabaseSearchSequence>>>();
            
            // Create the object that knows how to read the RAW files we are analyzing.
            using (ThermoRawDataFileReader reader = new ThermoRawDataFileReader())
            {
                //TODO: Drop msms cluster table if msms cluster table exists....
                MSMSFeatureExtractor extractor = new MSMSFeatureExtractor();
                extractor.Progress            += new EventHandler<ProgressNotifierArgs>(extractor_Progress);
                Dictionary<int, List<UMCLight>> features = extractor.ExtractUMCWithMSMS(analysis.DataProviders,
                                                                                        analysis.MetaData.Datasets);

                UpdateStatus(string.Format("Performing Spectral Clustering using MS/MS spectra at {0} similarity level.", SpectralSimilarityTolerance));
               
                string path  = analysis.MetaData.AnalysisPath; 
                
                // Create the object o align.
                SpectralNormalizedDotProductComparer comparer = new SpectralNormalizedDotProductComparer();
                comparer.TopPercent             = .8;
                MSMSClusterer msCluster         = new MSMSClusterer();
                msCluster.SimilarityTolerance   = SpectralSimilarityTolerance;
                msCluster.ScanRange             = 2000;
                msCluster.SpectralComparer      = comparer;
                msCluster.Progress              += new EventHandler<ProgressNotifierArgs>(msCluster_Progress);
                List<MSMSCluster> clusters      = msCluster.Cluster(features, reader);
                List<MSMSClusterMap> maps       = new List<MSMSClusterMap>();
                int id = 0;
                
                foreach (MSMSCluster cluster in clusters)
                {
                    cluster.ID = id++;
                    string line = string.Format("{0},", cluster.ID);

                    // Exclude singleton clusters.
                    if (cluster.Features.Count < 2) continue;
                                        
                    foreach (MSFeatureLight feature in cluster.Features)
                    {
                        MSMSClusterMap newMap   = new MSMSClusterMap();
                        newMap.ClusterID        = cluster.ID;
                        newMap.MSMSID           = feature.MSnSpectra[0].ID;
                        newMap.GroupID          = feature.GroupID;
                        maps.Add(newMap);
                    }
                }                                           
                analysis.DataProviders.MSMSClusterCache.AddAll(maps);
            }
        }

        void extractor_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }
        void msCluster_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }
    }
}
