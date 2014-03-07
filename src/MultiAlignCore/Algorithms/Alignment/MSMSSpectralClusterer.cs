using System;
using MultiAlignCore.Data.MetaData;
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
            SpectralNormalizedDotProductComparer comparer =
                    new SpectralNormalizedDotProductComparer();
            comparer.TopPercent         = .8;
            SpectralComparer            = comparer;
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
        public ISpectralComparer SpectralComparer
        {
            get;
            set;
        }
        /// <summary>
        /// Clusters a set of MS/MS spectra together.
        /// </summary>
        /// <param name="analysis"></param>
        public List<MSMSCluster> ClusterMSMSSpectra(List<UMCLight>           features,
                                                    List<DatasetInformation> information)
        {
            List<MSMSCluster> clusters = new List<MSMSCluster>();
                  
            UpdateStatus(string.Format("Performing Spectral Clustering using MS/MS spectra at {0} similarity level.", 
                                SpectralSimilarityTolerance));
                
            MSMSClusterer msCluster         = new MSMSClusterer();
            msCluster.SimilarityTolerance   = SpectralSimilarityTolerance;
            msCluster.ScanRange             = 2000;
            msCluster.SpectralComparer      = this.SpectralComparer;
            //msCluster.Progress              += new EventHandler<ProgressNotifierArgs>(msCluster_Progress);

            using (ThermoRawDataFileReader reader = new ThermoRawDataFileReader())
            {
                foreach(DatasetInformation info in information)
                {
                    reader.AddDataFile(info.Raw.Path, info.DatasetId);
                }
                clusters      = msCluster.Cluster(features, reader);                
            }
            return clusters;
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
