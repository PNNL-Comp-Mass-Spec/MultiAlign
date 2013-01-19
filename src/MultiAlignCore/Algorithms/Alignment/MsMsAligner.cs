using System;
using PNNLOmics.Algorithms.Solvers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;
using MultiAlignCore.Data.Alignment;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Takes a collection of matching spectra and constructs a spectral database.
    /// </summary>
    public class MsMsAligner: IFeatureAligner
    {
        private List<UMCLight> FilterFeatures(List<UMCLight> features)
        {
            return features;
        }
        private List<MSMSCluster> FilterClusters(List<MSMSCluster> clusters)
        {
            return clusters;
        }
        private void OnStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new PNNLOmics.Algorithms.ProgressNotifierArgs(message));
            }
        }
        #region Alignment
        public void ChebyShev(double[] c, double[] x, ref double func, object obj)
        {
            double sum = 0;
            double t0 = c[0];
            double t1 = c[1] * x[0];
            double prev = t0;
            for (int i = 0; i < c.Length - 1; i++)
            {
                double value = 2 * x[0] * c[i] - prev;
                prev = value;
                sum += value;
            }
            func = sum;
        }


        public void CorrectFeatures(List<UMCLight> features, List<MSMSCluster> clusters)
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            List<KeyValuePair<double, double>> values = new List<KeyValuePair<double, double>>();
            foreach(MSMSCluster cluster in clusters)
            {
                if (cluster.Features.Count < 0)
                    continue;

                double key = Convert.ToDouble(cluster.Features[0].MSnSpectra[0].Scan);
                double val = Convert.ToDouble(cluster.Features[1].MSnSpectra[0].Scan);

                KeyValuePair<double, double> value = new KeyValuePair<double, double>(key, val);
                values.Add(value);
            }

            values                  = values.OrderBy(i => i.Key).ToList();
            xValues                 = values.Select(i => i.Key).ToList();
            yValues                 = values.Select(i => i.Value).ToList();
            List<double> tX         = xValues.ToList();
            List<double> tY         = yValues.ToList();
            LevenburgMarquadt warp  = new LevenburgMarquadt();
            warp.BasisFunction      = ChebyShev;

            double[] coeffs = new double[20];
            coeffs[1]       = 1;
            bool passed     = false;
            passed          = warp.Solve(tX, tY, ref coeffs);            
            double func     = 0;

            foreach (UMCLight feature in features)
            {
               ChebyShev(coeffs, new double[] { feature.Scan }, ref func, null);
               feature.ScanAligned = Convert.ToInt32(func);
            }
        }
        #endregion

        #region IFeatureAligner Members


        public Data.Alignment.classAlignmentData AlignFeatures(Data.MassTags.MassTagDatabase database, List<PNNLOmics.Data.Features.UMCLight> features, AlignmentOptions options, bool alignDriftTimes)
        {
            throw new NotImplementedException("This alignment algorithm does not align to mass tag databases.");
        }
        public Data.Alignment.classAlignmentData AlignFeatures( List<UMCLight> baseline,
                                                                List<UMCLight> features, 
                                                                AlignmentOptions options)
        {
            MSMSSpectralClusterer clusterer = new MSMSSpectralClusterer();

            List<UMCLight> allFeatures = new List<UMCLight>();
            allFeatures.AddRange(baseline);
            allFeatures.AddRange(features);
            
            OnStatus(string.Format("Examining {0} features for alignment.", allFeatures.Count));

            OnStatus("Filtering Features");
            List<UMCLight> filteredFeatures = FilterFeatures(allFeatures);
            OnStatus(string.Format("Using {0} features after filtering.", filteredFeatures.Count));

            OnStatus(string.Format("Clustering MS/MS Spectra" ));
            List<MSMSCluster> clusters = clusterer.ClusterMSMSSpectra(filteredFeatures);            
            OnStatus(string.Format("Found {0} MS/MS Spectra Clusters", clusters.Count ));

            OnStatus("Filtering Poor Matches");
            List<MSMSCluster> filteredClusters = FilterClusters(clusters);           
            OnStatus(string.Format("Found {0} usable MS/MS Spectra Clusters", clusters.Count ));

            OnStatus("Adjusting Features scans and nets");
            CorrectFeatures(features, filteredClusters);
            
            classAlignmentData data = new classAlignmentData();
            return data;
        }   
        public event EventHandler<PNNLOmics.Algorithms.ProgressNotifierArgs> Progress;

        #endregion

        public List<MSMSCluster> FilteredClusters
        {
            get;
            private set;
        }
        public List<MSMSCluster> Clusters
        {
            get;
            private set;
        }
    }
}
