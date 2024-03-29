﻿using System;
using System.Collections.Generic;

namespace FeatureAlignment.Data.Features
{
    /// <summary>
    /// Representation of a UMC with only basic information
    /// </summary>
    public class UMCLight : FeatureLight,
                            IFeatureCluster<MSFeatureLight>,        // This allows for ms features
                            IFeatureCluster<UMCLight>,              // This allows for labeled development
                            IChildFeature<UMCClusterLight>,
                            IChildFeature<UMCLight>
    {
        /// <summary>
        /// Default group ID.
        /// </summary>
        private const int DEFAULT_GROUP_ID = -1;
        private readonly List<UMCLight> m_umcList;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UMCLight()
        {
            ChargeStateChromatograms = new Dictionary<int, Chromatogram>();
            IsotopeChromatograms     = new Dictionary<int, List<Chromatogram>>();

            GroupId     = DEFAULT_GROUP_ID;
            UmcCluster  = null;
            Scan        = -1;
            ScanEnd     = Scan;
            ScanStart   = Scan;
            MsMsCount   = 0;
            this.MinCharge = 1;
            this.MaxCharge = 1;

            if (MsFeatures == null)
                MsFeatures = new List<MSFeatureLight>();
            MsFeatures.Clear();

            if (m_umcList == null)
                m_umcList = new List<UMCLight>();
            m_umcList.Clear();
        }

        // ReSharper disable once UnusedMember.Global (Public API)
        public UMCLight(UMCLight feature)
        {
            Abundance                          = feature.Abundance;
            AbundanceSum                       = feature.AbundanceSum;
            AmbiguityScore                     = feature.AmbiguityScore;
            AverageDeconFitScore               = feature.AverageDeconFitScore;
            AverageInterferenceScore           = feature.AverageInterferenceScore;
            ChargeState                        = feature.ChargeState;
            ClusterId                          = feature.ClusterId;
            ConformationFitScore               = feature.ConformationFitScore;
            ConformationId                     = feature.ConformationId;
            DriftTime                          = feature.DriftTime;
            GroupId                            = feature.GroupId;
            Id                                 = feature.Id;
            IdentifiedSpectraCount             = feature.IdentifiedSpectraCount;
            Index                              = feature.Index;
            MassMonoisotopic                   = feature.MassMonoisotopic;
            MassMonoisotopicAligned            = feature.MassMonoisotopicAligned;
            MassMonoisotopicOriginal           = feature.MassMonoisotopicOriginal;
            MeanChargeStateRsquared            = feature.MeanChargeStateRsquared;
            MeanIsotopicRsquared               = feature.MeanIsotopicRsquared;
            MinimumCentroidDistance            = feature.MinimumCentroidDistance;
            MsMsCount                          = feature.MsMsCount;
            Mz                                 = feature.Mz;
            Net                                = feature.Net;
            NetAligned                         = feature.NetAligned;
            SaturatedMemberCount               = feature.SaturatedMemberCount ;
            Scan                               = feature.Scan;
            ScanAligned                        = feature.ScanAligned;
            ScanEnd                            = feature.ScanEnd;
            ScanStart                          = feature.ScanStart;
            Score                              = feature.Score;
            SpectralCount                      = feature.SpectralCount;
            MinCharge                          = feature.MinCharge;
            MaxCharge                          = feature.MaxCharge;
            // UmcCluster                      = feature.UmcCluster;

            // Charge state and Isotopic Chromatograms
            ChargeStateChromatograms           = new Dictionary<int, Chromatogram>();
            IsotopeChromatograms               = new Dictionary<int, List<Chromatogram>>();
        }

        /// <summary>
        /// Gets or sets the UMC Cluster this feature is part of.
        /// </summary>
        public UMCClusterLight UmcCluster { get; set; }

        /// <summary>
        /// Gets or sets the list of MS features for the given UMC.
        /// </summary>
        public List<MSFeatureLight> MsFeatures { get; private set; }

        /// <summary>
        /// Gets or sets the first scan number the feature was seen in.
        /// </summary>
        public int ScanStart { get; set; }

        /// <summary>
        /// Gets or sets the last scan number the feature was seen in.
        /// </summary>
        public int ScanEnd { get; set; }

        /// <summary>
        /// Gets or sets the earliest normalized elution time for the feature.
        /// </summary>
        public double NetStart { get; set; }

        /// <summary>
        /// Gets or sets the latest normalized elution time for the feature.
        /// </summary>
        public double NetEnd { get; set; }

        /// <summary>
        /// Gets or sets the minimum charge state.
        /// </summary>
        public int MinCharge { get; set; }

        /// <summary>
        /// Gets or sets the maximum charge state.
        /// </summary>
        public int MaxCharge { get; set; }

        /// <summary>
        /// Gets or sets the predicted scan number after alignment.
        /// </summary>
        public int ScanAligned { get; set; }

        /// <summary>
        /// Gets or sets the sum of abundances from all MS features
        /// </summary>
        public double AbundanceSum { get; set; }

        /// <summary>
        /// For UMCs, tracks the number of scans the UMC spans
        /// For AMT Tags from database, tracks the ObservationCount (used by LCMSWarp's scorer)
        /// </summary>
        public int SpectralCount { get; set; }

        /// <summary>
        /// Gets or sets the chromatograms based on charge state.
        /// </summary>
        public Dictionary<int, Chromatogram> ChargeStateChromatograms { get; set; }

        /// <summary>
        /// Gets or sets the chromatograms for each isotope for a given charge state.
        /// </summary>
        public Dictionary<int, List<Chromatogram>> IsotopeChromatograms { get; set; }

        public double MeanIsotopicRsquared { get; set; }

        public double MeanChargeStateRsquared { get; set; }

        public UMCLight ParentUMC { get; private set; }

        #region IMS Data Members
        public double AverageInterferenceScore { get; set; }

        public double ConformationFitScore { get; set; }

        public double AverageDeconFitScore { get; set; }

        public int SaturatedMemberCount { get; set; }

        public int ConformationId { get; set; }
        #endregion

        public int ClusterId { get; set; }

        #region Overriden Base Methods

        /// <summary>
        /// Returns a basic string representation of the cluster.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "UMCLight Group ID " + GroupId + " " + base.ToString();
        }
        /// <summary>
        /// Compares two objects' values to each other.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if similar, False if not.</returns>
        public override bool Equals(object obj)
        {
            var umc = obj as UMCLight;
            if (umc == null)
                return false;

            if (Id != umc.Id)
                return false;

            var isBaseEqual = base.Equals(umc);
            if (!isBaseEqual)
                return false;

            return true;
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

        /// <summary>
        /// Calculates the centroid and other statistics about the cluster.
        /// </summary>
        /// <param name="centroid"></param>
        public void CalculateStatistics(ClusterCentroidRepresentation centroid = ClusterCentroidRepresentation.Apex)
        {
            if (MsFeatures == null)
                throw new NullReferenceException("The UMC list was not set to an object reference.");

            if (MsFeatures.Count < 1)
                throw new Exception("No data in feature to compute statistics over.");

            // Lists for holding onto masses etc.
            var net        = new List<double>();
            var mass       = new List<double>();
            var driftTime  = new List<double>();

            double  sumNet          = 0;
            double  sumMass         = 0;
            double  sumDrifttime    = 0;
            double  sumAbundance    = 0;
            var     minScan         = int.MaxValue;
            var     maxScan         = int.MinValue;
            var minNet              = double.PositiveInfinity;
            var maxNet              = 0.0;
            double  maxAbundance = int.MinValue;
            double representativeMz = 0;
            foreach (var feature in MsFeatures)
            {
                if (feature == null)
                    throw new NullReferenceException("A MS feature was null when trying to calculate cluster statistics.");

                if (feature.Abundance > maxAbundance)
                {
                    maxAbundance     = feature.Abundance;
                    Scan             = feature.Scan;
                    ChargeState      = feature.ChargeState;
                    representativeMz = feature.Mz;
                }

                this.MinCharge = Math.Min(this.MinCharge, feature.ChargeState);
                this.MaxCharge = Math.Max(this.MaxCharge, feature.ChargeState);

                net.Add(feature.Net);
                mass.Add(feature.MassMonoisotopic);
                driftTime.Add(feature.DriftTime);

                sumAbundance    += feature.Abundance;
                sumNet          += feature.Net;
                sumMass         += feature.MassMonoisotopicAligned;
                sumDrifttime    += feature.DriftTime;
                minScan          = Math.Min(feature.Scan, minScan);
                maxScan          = Math.Max(feature.Scan, maxScan);
                minNet           = Math.Min(feature.Net, minNet);
                maxNet           = Math.Max(feature.Net, maxNet);
            }
            Abundance       = maxAbundance;
            AbundanceSum    = sumAbundance;
            ScanEnd         = maxScan;
            ScanStart       = minScan;
            NetStart        = minNet;
            NetEnd          = maxNet;
            var numUmCs     = MsFeatures.Count;

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
                        Net      = (net[median] + net[median - 1]) / 2;
                        DriftTime          = Convert.ToSingle((driftTime[median] + driftTime[median - 1]) / 2);
                    }
                    else
                    {
                        median                  = Convert.ToInt32((numUmCs) / 2);
                        Net      = net[median];
                        DriftTime          = Convert.ToSingle(driftTime[median]);
                    }
                    break;
                case ClusterCentroidRepresentation.Apex:
                    double apexNet = 0;
                    double apexAbundance = 0;
                    foreach (var msFeature in this.MsFeatures)
                    {
                        if (msFeature.Abundance >= apexAbundance)
                        {
                            apexNet = msFeature.Net;
                            apexAbundance = msFeature.Abundance;
                        }

                        this.Net = apexNet;
                    }
                    break;
            }
            if ((numUmCs % 2) == 1)
            {
                MassMonoisotopic = mass[numUmCs / 2];
            }
            else
            {
                MassMonoisotopic = .5 * (mass[numUmCs / 2 - 1] + mass[numUmCs / 2]);
            }

            var distances  = new List<double>();
            double distanceSum      = 0;
            foreach (var umc in MsFeatures)
            {
                var netValue   = Net - umc.Net;
                var massValue  = MassMonoisotopic - umc.MassMonoisotopicAligned;
                var driftValue = DriftTime - umc.DriftTime;
                var distance   = Math.Sqrt((netValue * netValue) + (massValue * massValue) + (driftValue * driftValue));
                distances.Add(distance);
                distanceSum += distance;
            }

            if (centroid == ClusterCentroidRepresentation.Mean)
            {
                Score = Convert.ToSingle(distanceSum / MsFeatures.Count);
            }
            else
            {
                var mid = distances.Count / 2;

                distances.Sort();
                Score       = Convert.ToSingle(distances[mid]);
            }
            Mz = representativeMz;
        }

        #region IFeatureCluster<MSFeatureLight> Members

        public void AddChildFeature(MSFeatureLight feature)
        {
            feature.SetParentFeature(this);
            MsFeatures.Add(feature);
        }

        public List<MSFeatureLight> Features
        {
            get { return MsFeatures; }
        }

        #endregion

        #region IChildFeature<UMCClusterLight> Members

        public void SetParentFeature(UMCClusterLight parentFeature)
        {
            UmcCluster = parentFeature;
        }

        public UMCClusterLight GetParentFeature()
        {
            return UmcCluster;
        }

        #endregion

        # region IChildFeature<UMCLight> Members
        public void SetParentFeature(UMCLight parentFeature)
        {
            ParentUMC = parentFeature;
        }

        UMCLight IChildFeature<UMCLight>.GetParentFeature()
        {
            return ParentUMC;
        }

        public void AddChildFeature(UMCLight feature)
        {
            m_umcList.Add(feature);
            feature.MsFeatures.ForEach(AddChildFeature);
        }
        #endregion

        List<UMCLight> IFeatureCluster<UMCLight>.Features
        {
            get { return m_umcList; }
        }

        public class UmcMassComparer : IComparer<UMCLight>
        {
            public int Compare(UMCLight left, UMCLight right)
            {
                if (left != null)
                {
                    if (right != null)
                    {
                        return left.MassMonoisotopic.CompareTo(right.MassMonoisotopic);
                    }
                    return 1;
                }
                if (right == null)
                {
                    return 0;
                }
                return -1;
            }
        }
    }
}
