using System;
using System.Threading;
using System.Collections.Generic;

using PNNLOmics.Data.Features;

namespace FOX.Data
{
    /// <summary>
    /// Calculates the distance between two features X or Y.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y"></param>
    /// <returns></returns>
    public delegate double DistanceDelegate<T>(T x, T y) where T: FeatureLight;

    /// <summary>
    /// Holds pairwise distances between all clusters in a MA analysis.
    /// </summary>
    public class DistanceMap<T> where T: FeatureLight        
    {

        /// <summary>
        /// Holds distance map for quick lookup in a nested dictionary.
        /// </summary>
        private Dictionary<T, Dictionary<T, double>> m_distanceMap;
        /// <summary>
        /// List of distances for thresholding purposes.
        /// </summary>
        private List<DistancePair<T>> m_distanceList;
        /// <summary>
        /// Distance function to use between clusters.
        /// </summary>
        private DistanceDelegate<T> m_distanceFunction;
        /// <summary>
        /// List of clusters to compute distance for.
        /// </summary>
        private List<T> m_clusters;
        /// <summary>
        /// Processing thread.
        /// </summary>
        private Thread m_thread;
        /// <summary>
        /// Triggered when distance calculation is complete.
        /// </summary>
        private ManualResetEvent m_event;
        /// <summary>
        /// Fired when new status is complete.
        /// </summary>
        public event EventHandler<StatusEventArgs> Status;


        /// <summary>
        /// Constructor.
        /// </summary>
        public DistanceMap()
        {
            m_distanceMap   = new Dictionary<T, Dictionary<T, double>>();
            m_distanceList  = new List<DistancePair<T>>();
            m_event         = new ManualResetEvent(false);
        }
        /// <summary>
        /// Gets the number of comparisons made available for viewing.
        /// </summary>
        public int NumberOfComparisons
        {
            get
            {
                return m_distanceList.Count;
            }
        }
        /// <summary>
        /// Calculates pair-wise distances for all clusters.
        /// </summary>
        /// <param name="clusters">Clusters to calculate distances between.</param>
        /// <param name="distanceFunction">Function to use for calculating distance between two features.</param>
        public void CalculateDistances(List<T> clusters, DistanceDelegate<T> distanceFunction)
        {
            CancelDistanceCalculation();
            DereferenceClusters();

            m_distanceFunction  = distanceFunction;
            m_clusters          = clusters;

            m_event.Reset();
            ThreadStart start   = new ThreadStart(CalculateDistances);
            m_thread            = new Thread(start);
            m_thread.Start();


            // this is barf code.
            while (!WaitHandle.WaitAll(new WaitHandle[] { m_event }, 10))
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }
        /// <summary>
        /// Aborts the distance map calculation.
        /// </summary>
        public void CancelDistanceCalculation()
        {
            try
            {
                if (m_thread == null)
                    return;

                if (!m_thread.IsAlive)
                    return;

                m_thread.Abort();
                m_thread.Join(500);
            }
            catch
            {
                m_thread = null;
            }
        }
        /// <summary>
        /// Dereferences clusters and internal objects.
        /// </summary>
        private void DereferenceClusters()
        {
            m_clusters          = null;
            m_distanceFunction  = null;
        }
        /// <summary>
        /// Threaded distance calculation.
        /// </summary>
        private void CalculateDistances()
        {
            int percent = 0;
            for (int i = 0; i < m_clusters.Count; i++)
            {
                percent = Convert.ToInt32( 100*(Convert.ToDouble(i) / Convert.ToDouble(m_clusters.Count)));
                if (Status != null)
                {
                    Status(this, new StatusEventArgs("Calculating Distance Map.", percent));
                }

                T x = m_clusters[i];
                // We calculate the distance even for i = j features.
                //m_distanceMap.Add(x, new Dictionary<T, double>());
                for (int j = i + 1; j < m_clusters.Count; j++)
                {
                    T y = m_clusters[j];
                    
                    double distance = m_distanceFunction(x, y);
                    if (!double.IsInfinity(distance))
                    {
                        m_distanceList.Add(new DistancePair<T>(x, y, distance));
                    }
                    //m_distanceMap[x].Add(y, distance);                    
                }                
            }
            if (Status != null)
            {
                Status(this, new StatusEventArgs("Sorting distance map.", -1));
            }
            m_distanceList.Sort();

            m_event.Set();
        }
        /// <summary>
        /// Gets the map of distances based on the feature.
        /// </summary>
        public Dictionary<T, Dictionary<T, double>> ClusterDistanceMap
        {
            get
            {
                return m_distanceMap;
            }
        }
        /// <summary>
        /// Gets a sorted distance pair list.
        /// </summary>
        public List<DistancePair<T>> DistancePairs
        {
            get
            {
                return m_distanceList;
            }
        }
    }

    
        /// <summary>
        /// Holds a list of distances between two features.
        /// </summary>
    public class DistancePair<T> : IComparable<DistancePair<T>> where T : FeatureLight
        {       
            private T m_featureX;
            private T m_featureY;
            private double m_distance;

            public DistancePair(T x, T y, double distance)
            {
                m_distance = distance;
                m_featureX = x;
                m_featureY = y;
            }

            public T FeatureX 
            {
                get
                {
                    return m_featureX;
                }
            }

            public T FeatureY 
            {
                get
                {
                    return m_featureY;
                }
            }
            public double Distance
            {
                get
                {
                    return m_distance;
                }
            }

            /// <summary>
            /// Compares other with this.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(DistancePair<T> other)
            {
                return m_distance.CompareTo(other.Distance);
            }        
        }
}
