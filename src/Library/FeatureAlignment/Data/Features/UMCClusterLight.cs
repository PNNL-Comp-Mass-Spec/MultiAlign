using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data.MassTags;

namespace FeatureAlignment.Data.Features
{
    /// <summary>
    /// Basic representation of a group of UMC's observed across datasets.
    /// </summary>
    public class UMCClusterLight :  FeatureLight,
                                    IFeatureCluster<UMCLight>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public UMCClusterLight()
        {
            UmcList = new List<UMCLight>();
            this.MassTags = new List<MassTagLight>();
            MsMsCount = 0;
            IdentifiedSpectraCount = 0;
        }

        /// <summary>
        /// Creates a UMC Cluster from the umc, while also connecting them together.
        /// </summary>
        // ReSharper disable once UnusedMember.Global (Public API)
        public UMCClusterLight(UMCLight umc)
        {
            var umcs  = new List<UMCLight>();
            this.MassTags = new List<MassTagLight>();

            UmcList             = umcs;
            umc.UmcCluster      = this;
            Abundance           = umc.Abundance;
            ChargeState         = umc.ChargeState;
            DriftTime           = umc.DriftTime;
            Id                  = umc.Id;
            Net       = umc.NetAligned;
            NetAligned = umc.NetAligned;
            MassMonoisotopic    = umc.MassMonoisotopicAligned;
            MassMonoisotopicAligned = umc.MassMonoisotopic;
            MsMsCount           = umc.MsMsCount;
            AddChildFeature(umc);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="cluster"></param>
        // ReSharper disable once UnusedMember.Global (Public API)
        public UMCClusterLight(UMCClusterLight cluster)
        {
            var umcs     = new List<UMCLight>();
            this.MassTags = new List<MassTagLight>();
            umcs.AddRange(cluster.UmcList);
            UmcList                 = umcs;
            Abundance               = cluster.Abundance;
            ChargeState             = cluster.ChargeState;
            DriftTime               = cluster.DriftTime;
            Id                      = cluster.Id;
            MassMonoisotopic        = cluster.MassMonoisotopic;
            MassMonoisotopicAligned = cluster.MassMonoisotopicAligned;
            MsMsCount               = cluster.MsMsCount;
            IdentifiedSpectraCount  = cluster.IdentifiedSpectraCount;
            MeanSpectralSimilarity  = cluster.MeanSpectralSimilarity;
        }

        public List<MassTagLight> MassTags { get; private set; }

        public double MeanSpectralSimilarity { get; set; }

        public double Tightness
        {
            get;
            set;
        }

        public int MemberCount
        {
            get;
            set;
        }

        public int DatasetMemberCount
        {
            get;
            set;
        }

        public override double MassMonoisotopicAligned
        {
            get
            {
                return MassMonoisotopic;
            }
            set
            {
                MassMonoisotopic = value;
            }
        }

        public double MassStandardDeviation
        {
            get;
            private set;
        }

        public double NetStandardDeviation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the list of UMC's that comprise this cluster.
        /// </summary>
        public List<UMCLight> UmcList { get; set; }

        /// <summary>
        /// Calculates the centroid and other statistics about the cluster.
        /// </summary>
        /// <param name="centroid"></param>
        public void CalculateStatistics(ClusterCentroidRepresentation centroid)
        {
            if (UmcList == null)
                throw new NullReferenceException("The UMC list was not set to an object reference.");

            if (UmcList.Count < 1)
                throw new Exception("No data to compute statistics over.");

            // Lists for holding onto masses etc.
            var net = new List<double>();
            var mass = new List<double>();
            var driftTime = new List<double>();

            // Histogram of representative charge states
            var chargeStates = new Dictionary<int, int>();

            double sumNet = 0;
            double sumMass = 0;
            double sumDrifttime = 0;

            var datasetMembers = new Dictionary<int,int>();
            MemberCount = UmcList.Count;

            foreach (var umc in UmcList)
            {

                if (umc == null)
                    throw new NullReferenceException("A UMC was null when trying to calculate cluster statistics.");

                if (!datasetMembers.ContainsKey(umc.GroupId))
                {
                    datasetMembers.Add(umc.GroupId, 0);
                }
                datasetMembers[umc.GroupId]++;

                net.Add(umc.Net);
                mass.Add(umc.MassMonoisotopicAligned);
                driftTime.Add(umc.DriftTime);

                sumNet       += umc.Net;
                sumMass      += umc.MassMonoisotopicAligned;
                sumDrifttime += umc.DriftTime;

                // Calculate charge states.
                if (!chargeStates.ContainsKey(umc.ChargeState))
                {
                    chargeStates.Add(umc.ChargeState, 1);
                }
                else
                {
                    chargeStates[umc.ChargeState]++;
                }
            }

            DatasetMemberCount = datasetMembers.Keys.Count;

            var numUmCs = UmcList.Count;

            // Calculate the centroid of the cluster.
            switch (centroid)
            {
                case ClusterCentroidRepresentation.Mean:
                    MassMonoisotopic   = (sumMass / numUmCs);
                    Net      = (sumNet / numUmCs);
                    DriftTime          = Convert.ToSingle(sumDrifttime / numUmCs);
                    break;
                case ClusterCentroidRepresentation.Median:
                    net.Sort();
                    mass.Sort();
                    driftTime.Sort();

                    // If the median index is odd.  Then take the average.
                    int median;
                    if ((numUmCs % 2) == 0)
                    {
                        median                  = Convert.ToInt32(numUmCs / 2);
                        MassMonoisotopic   = (mass[median] + mass[median - 1]) / 2;
                        Net      = (net[median] + net[median - 1]) / 2;
                        DriftTime          = Convert.ToSingle((driftTime[median] + driftTime[median - 1]) / 2);
                    }
                    else
                    {
                        median                  = Convert.ToInt32((numUmCs) / 2);
                        MassMonoisotopic   = mass[median];
                        Net      = net[median];
                        DriftTime          = Convert.ToSingle(driftTime[median]);
                    }
                    break;
            }


            var distances = new List<double>();
            double distanceSum     = 0;

            double massDeviationSum = 0;
            double netDeviationSum  = 0;

            foreach (var umc in UmcList)
            {
                var netValue   = Net       - umc.Net;
                var massValue  = MassMonoisotopic    - umc.MassMonoisotopicAligned;
                var driftValue = DriftTime           - Convert.ToSingle(umc.DriftTime);

                massDeviationSum += (massValue*massValue);
                netDeviationSum += (netValue * netValue);

                var distance = Math.Sqrt((netValue * netValue) + (massValue * massValue) + (driftValue * driftValue));
                distances.Add(distance);
                distanceSum += distance;
            }

            NetStandardDeviation  = Math.Sqrt(netDeviationSum  / Convert.ToDouble(UmcList.Count));
            MassStandardDeviation = Math.Sqrt(massDeviationSum / Convert.ToDouble(UmcList.Count));

            if (centroid == ClusterCentroidRepresentation.Mean)
            {
                Tightness = Convert.ToSingle(distanceSum / UmcList.Count);
            }
            else
            {
                var mid = distances.Count / 2;

                distances.Sort();
                Tightness = Convert.ToSingle(distances[mid]);
            }
            // Calculate representative charge state as the mode.
            var maxCharge = int.MinValue;
            foreach (var charge in chargeStates.Keys)
            {
                if (maxCharge == int.MinValue || chargeStates[charge] > chargeStates[maxCharge])
                {
                    maxCharge = charge;
                }
            }
            ChargeState = maxCharge;
        }

        #region Overriden Base Methods

        public override string ToString()
        {
            var size = 0;
            if (UmcList != null)
            {
                size = UmcList.Count;
            }
            return "UMC Cluster (size = " + size + ") " + base.ToString();
        }
        /// <summary>
        /// Compares two objects' values to each other.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if similar, False if not.</returns>
        public override bool Equals(object obj)
        {
            var cluster = obj as UMCClusterLight;
            if (cluster == null)
                return false;

            var isBaseEqual = base.Equals(cluster);
            if (!isBaseEqual)
                return false;

            if (UmcList == null && cluster.UmcList != null)
                return false;

            if (UmcList != null && cluster.UmcList == null)
                return false;

            if (UmcList != null && (cluster.UmcList != null && UmcList.Count != cluster.UmcList.Count))
                return false;

            return UmcList != null && UmcList.Select(umc => cluster.UmcList.FindIndex(x => x.Equals(umc))).All(index => index >= 0);
        }
        /// <summary>
        /// Computes a hash code for the cluster.
        /// </summary>
        /// <returns>Hashcode as an integer.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region IFeatureCluster<UMCLight> Members

        public void AddChildFeature(UMCLight feature)
        {
            UmcList.Add(feature);
        }

        public List<UMCLight> Features
        {
            get { return UmcList; }
        }

        #endregion

    }
}
