using System;
using System.Windows.Forms;
using System.Collections.Generic;

using PNNLControls.Drawing.Charting;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

using MultiAlignEngine.Features;

using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;

using FOX.Data;

namespace FOX
{

    /// <summary>
    /// Main data view for fox.
    /// </summary>
    public partial class MainView : Form
    {
        /// <summary>
        /// Default histogram size.
        /// </summary>
        private const int HISTOGRAM_SIZE = 100;

        #region Members
        /// <summary>
        /// List of all clusters in the database.
        /// </summary>
        private List<UMCClusterLight> m_clusters;
        /// <summary>
        /// Euclidean distance map.
        /// </summary>
        private DistanceMap<UMCClusterLight> m_euclideanMap;
        /// <summary>
        /// Normalized Elution Time Map
        /// </summary>
        private DistanceMap<UMCClusterLight> m_netMap;
        /// <summary>
        /// Monoisotopic mass map
        /// </summary>
        private DistanceMap<UMCClusterLight> m_massMap;
        /// <summary>
        /// Drift time map
        /// </summary>
        private DistanceMap<UMCClusterLight> m_driftTimeMap;
        /// <summary>
        /// Number of histogram bins.
        /// </summary>
        private int m_histogramSize;
        /// <summary>
        /// Feature tolerances.
        /// </summary>
        private FeatureTolerances m_tolerances;
        #endregion

        /// <summary>
        /// Main display window.
        /// </summary>
        public MainView()
        {
            InitializeComponent();

            m_clusters           = new List<UMCClusterLight>();
            m_histogramSize      = HISTOGRAM_SIZE;

            m_tolerances         = new FeatureTolerances();
            m_tolerances.Mass      = 30;
            m_tolerances.RetentionTime = .1;
            m_tolerances.DriftTime = 15;
        }

        #region Distance Functions
        /// <summary>
        /// Determine if two clusters are close or not.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private bool AreClose(UMCClusterLight x, UMCClusterLight y)
        {
            double netDiff = x.RetentionTime - y.RetentionTime;
            if (Math.Abs(netDiff) > m_tolerances.RetentionTime)
            {
                return false;
            }

            double massDiff = Feature.ComputeMassPPMDifference(x.MassMonoisotopic, y.MassMonoisotopic);
            if (Math.Abs(massDiff) > m_tolerances.Mass)
            {
                return false;
            }

            double driftDiff = x.DriftTime - y.DriftTime;
            if (Math.Abs(driftDiff) > m_tolerances.DriftTime)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Mass Difference (PPM)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private double MassDistance(UMCClusterLight x, UMCClusterLight y)
        {
            if (!AreClose(x, y))
                return double.PositiveInfinity;
            return Math.Abs(Feature.ComputeMassPPMDifference(x.MassMonoisotopic, y.MassMonoisotopic));          
        }
        /// <summary>
        /// Absolute drift time difference.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private double DriftDistance(UMCClusterLight x, UMCClusterLight y)
        {
            if (!AreClose(x, y))
                return double.PositiveInfinity;

            return Math.Abs(x.DriftTime - y.DriftTime);
        }
        /// <summary>
        /// Absolute NET Distance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private double NETDistance(UMCClusterLight x, UMCClusterLight y)
        {
            if (!AreClose(x, y))
                return double.PositiveInfinity;
            return Math.Abs(x.NET - y.NET);
        }
        /// <summary>
        /// Euclidean Distance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private double EuclideanDistance(UMCClusterLight x, UMCClusterLight y)
        {
            if (!AreClose(x, y))
                return double.PositiveInfinity;

            double massDiff  = Feature.ComputeMassPPMDifference(x.MassMonoisotopic, y.MassMonoisotopic);
            double netDiff   = x.NET - y.NET;
            double driftDiff = x.DriftTime - y.DriftTime;

            return Math.Sqrt(massDiff * massDiff + netDiff * netDiff + driftDiff * driftDiff);
        }
        #endregion

        #region Loading and Computation.
        /// <summary>
        /// Loads the data 
        /// </summary>
        /// <param name="path"></param>
        private void LoadData(string path)
        {
            NHibernateUtil.SetDbLocationForRead(path);
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();
            UmcDAOHibernate featureCache        = new UmcDAOHibernate();

            List<clsCluster> clusters           = clusterCache.FindAll();
            m_clusters                          = OMICSMapper.ClusterToOmicsCluster(clusters);           
        }
        /// <summary>
        /// Computes a distance map for some clusters.
        /// </summary>
        private void ComputeDistanceMaps()
        {
            UpdateStatus("Loading Database.", -1);
            
            m_driftTimeMap = new DistanceMap<UMCClusterLight>();
            m_driftTimeMap.Status += new EventHandler<StatusEventArgs>(StatusEventHandler);
            m_driftTimeMap.CalculateDistances(m_clusters, new DistanceDelegate<UMCClusterLight>(DriftDistance));
            m_driftTimeMap.Status -= StatusEventHandler;

            m_massMap = new DistanceMap<UMCClusterLight>();
            m_massMap.Status += new EventHandler<StatusEventArgs>(StatusEventHandler);
            m_massMap.CalculateDistances(m_clusters, new DistanceDelegate<UMCClusterLight>(MassDistance));
            m_massMap.Status -= StatusEventHandler;

            m_netMap = new DistanceMap<UMCClusterLight>();
            m_netMap.Status += new EventHandler<StatusEventArgs>(StatusEventHandler);
            m_netMap.CalculateDistances(m_clusters, new DistanceDelegate<UMCClusterLight>(NETDistance));
            m_netMap.Status -= StatusEventHandler;

            m_euclideanMap = new DistanceMap<UMCClusterLight>();
            m_euclideanMap.Status += new EventHandler<StatusEventArgs>(StatusEventHandler);
            m_euclideanMap.CalculateDistances(m_clusters, new DistanceDelegate<UMCClusterLight>(EuclideanDistance));
            m_euclideanMap.Status -= StatusEventHandler;

            Histogram<UMCClusterLight> massHistogram = new Histogram<UMCClusterLight>(m_histogramSize);
            massHistogram.CalculateHistogram(m_massMap);

            Histogram<UMCClusterLight> netHistogram = new Histogram<UMCClusterLight>(m_histogramSize);
            netHistogram.CalculateHistogram(m_netMap);

            Histogram<UMCClusterLight> driftTimeHistogram = new Histogram<UMCClusterLight>(m_histogramSize);
            driftTimeHistogram.CalculateHistogram(m_driftTimeMap);

            Histogram<UMCClusterLight> euclideanHistogram = new Histogram<UMCClusterLight>(m_histogramSize);
            euclideanHistogram.CalculateHistogram(m_euclideanMap);

            DisplayHistogram(this.m_massHistogram, massHistogram);
            DisplayHistogram(this.m_NETHistogram, netHistogram);
            DisplayHistogram(this.m_driftTimeHistogram, driftTimeHistogram);
            DisplayHistogram(this.m_distanceHistogram, euclideanHistogram);

            UpdateStatus("Ready.", -1);
        }
        /// <summary>
        /// Updates display with appropiate messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StatusEventHandler(object sender, StatusEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<StatusEventArgs>(StatusEventHandlerThreadSafe), new object[] { sender, e });
            }
            else
            {
                StatusEventHandlerThreadSafe(sender, e);
            }
        }
        /// <summary>
        /// Thread safe call to update the user interface with status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StatusEventHandlerThreadSafe(object sender, StatusEventArgs e)
        {
            UpdateStatus(e.Status, e.Percent);
        }
        /// <summary>
        /// Updates the status and percent bars.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="percent"></param>
        void UpdateStatus(string status, int percent)
        {
            if (percent < 0 )
            {
                m_progressBar.Visible = false;
            }
            else if (percent > 100)
            {
                m_progressBar.Visible = true;
                m_progressBar.Value   = 100;
            }
            else
            {
                m_progressBar.Visible = true;
                m_progressBar.Value   = percent;
            }

            m_statusLabel.Text = status;
        }
        /// <summary>
        /// Displays the histogram.
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="data"></param>
        private void DisplayHistogram(ctlHistogram histogram, Histogram<UMCClusterLight> data)
        {
            float[] bins   = new float[data.Bins.Length];
            float[] counts = new float[data.Bins.Length];
            int i = 0;
            foreach (int count in data.Counts)
            {
                bins[i]     = Convert.ToSingle(data.Bins[i]);
                counts[i]   = Convert.ToSingle(data.Counts[i]);
                i++;
            }

            PNNLControls.BubbleShape shape          = new PNNLControls.BubbleShape(2, false);            
            PNNLControls.clsPlotParams parameters   = new PNNLControls.clsPlotParams(shape, System.Drawing.Color.Red);
            PNNLControls.clsSeries series           = new PNNLControls.clsSeries(ref bins, ref counts, parameters);

            histogram.BinSize = Convert.ToSingle(data.BinSize);
            histogram.AddSeries(series);
            histogram.AutoViewPort();
        }
        #endregion

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Sqlite files (*.db3)|*.db3|All Files (*.*)|*.*" ;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadData(dialog.FileName);
                ComputeDistanceMaps();
            }
        }
    }

    /// <summary>
    /// Status event arguments.
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        private readonly int     m_percent;
        private readonly string  m_status;

        public StatusEventArgs(string status, int percent)
        {
            m_status  = status;
            m_percent = percent;
        }

        /// <summary>
        /// Gets the status message.
        /// </summary>
        public string Status
        {
            get
            {
                return m_status;
            }
        }
        /// <summary>
        /// Gets the percent complete.
        /// </summary>
        public int Percent
        {
            get
            {
                return m_percent;
            }
        }
    }
}