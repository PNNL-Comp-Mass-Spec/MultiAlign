using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Annotations;
using PNNLOmics.Utilities;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    public class FeatureMatcher<TObserved, TTarget> :
        IFeatureMatcher<TObserved, TTarget>
        where TObserved : FeatureLight, new()
        where TTarget   : FeatureLight, new()
    {
        #region Members

        private FeatureMatcherParameters m_matchParameters;

        private readonly List<TObserved> m_observedFeatureList;
        private readonly List<TTarget>   m_targetFeatureList;

        const int MIN_MATCHES_FOR_NORMAL_ASSUMPTION = 1;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for use in matching features.  Uses passed parameters to calculate desired results.
        /// </summary>
        /// <param name="observedFeatureList">List of features observed in an analysis.  Generally UMC or UMCCluster.</param>
        /// <param name="targetFeatureList">List of features to be matched to.  Generally AMTTags.</param>
        /// <param name="matchParameters">FeatureMatcherParameters object containing initial parameters and algorithm settings.</param>
        public FeatureMatcher(List<TObserved> observedFeatureList, List<TTarget> targetFeatureList, FeatureMatcherParameters matchParameters)
        {
            Clear();

            m_observedFeatureList = observedFeatureList;
            m_targetFeatureList = targetFeatureList;
            m_matchParameters = matchParameters;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the initial parameters used for matching.
        /// </summary>
        [UsedImplicitly]
        public FeatureMatcherParameters MatchParameters
        {
            get { return m_matchParameters; }
            set { m_matchParameters = value; }
        }

        /// <summary>
        /// Gets the list of feature matches.
        /// </summary>
        [UsedImplicitly]
        public List<FeatureMatch<TObserved, TTarget>> MatchList { get; private set; }

        /// <summary>
        /// Gets the list of features matched with a shift.
        /// </summary>
        [UsedImplicitly]
        public List<FeatureMatch<TObserved, TTarget>> ShiftedMatchList { get; private set; }

        /// <summary>
        /// Gets the FDR calculated by using a fixed shift.  Calculated as (# shifted matches)/(# shifted matches + # non-shifted matches).
        /// </summary>
        [UsedImplicitly]
        public double ShiftFdr { get; private set; }

        /// <summary>
        /// Gets the FDR calculated by using a fixed shift.  Calculated as 2*(# shifted matches)/(# shifted matches + # non-shifted matches).
        /// </summary>
        [UsedImplicitly]
        public double ShiftConservativeFdr { get; private set; }

        /// <summary>
        /// Gets the FDR calculated by using a mass error histogram.
        /// </summary>
        [UsedImplicitly]
        public double ErrorHistogramFdr { get; private set; }

        /// <summary>
        /// Gets the list of parameters trained by STAC.  Each entry is a different charge state.
        /// </summary>
        public List<STACInformation> StacParameterList { get; private set; }

        /// <summary>
        /// Get the STAC FDR table.
        /// </summary>
        public List<STACFDR> StacFdrTable { get; private set; }

        /// <summary>
        /// Gets the list of refined tolerances used for SLiC and shift.  Each entry is a different charge state.
        /// </summary>
        [UsedImplicitly]
        public List<FeatureMatcherTolerances> RefinedToleranceList { get; private set; }

        /// <summary>
        /// Gets the parameters used in calculating SLiC.
        /// </summary>
        [UsedImplicitly]
        public SLiCInformation SliCParameters { get; private set; }

        #endregion

        #region Private functions
        /// <summary>
        /// Reset all algorithm results to default values.
        /// </summary>
        private void Clear()
        {
            StacFdrTable = new List<STACFDR>();
            m_matchParameters = new FeatureMatcherParameters();
            ShiftFdr = 0;
            ShiftConservativeFdr = 0;
            ErrorHistogramFdr = 0;
            StacParameterList = new List<STACInformation>();
            RefinedToleranceList = new List<FeatureMatcherTolerances>();
            SliCParameters = new SLiCInformation();
            MatchList = new List<FeatureMatch<TObserved, TTarget>>();
            ShiftedMatchList = new List<FeatureMatch<TObserved, TTarget>>();
        }

        /// <summary>
        /// Find a list of matches between two lists.
        /// </summary>
        /// <param name="shortObservedList">List of observed features.  Possibly a subset of the entire list corresponding to a particular charge state.</param>
        /// <param name="shortTargetList">List of target features.  Possibly a subset of the entire list corresponding to a particular charge state.</param>
        /// <param name="tolerances">Tolerances to be used for matching.</param>
        /// <param name="shiftAmount">A fixed shift amount to use for populating the shifted match list.</param>
        /// <returns>A list of type FeatureMatch containing matches within the defined region.</returns>
        public List<FeatureMatch<TObserved, TTarget>> FindMatches(List<TObserved> shortObservedList, List<TTarget> shortTargetList, FeatureMatcherTolerances tolerances, double shiftAmount)
        {
            // Create a list to hold the matches until they are returned.
            var matchList = new List<FeatureMatch<TObserved, TTarget>>();

            // Set indices to use when iterating over the lists.
            var observedIndex = 0;
            var lowerBound = 0;


            // Sort both lists by mass.
            if (!double.IsNaN(shortObservedList[0].MassMonoisotopicAligned) && shortObservedList[0].MassMonoisotopicAligned > 0.0)
            {
                shortObservedList.Sort(FeatureLight.MassAlignedComparison);
            }
            else
            {
                shortObservedList.Sort(FeatureLight.MassComparison);
            }
            if (!double.IsNaN(shortTargetList[0].MassMonoisotopicAligned) && shortTargetList[0].MassMonoisotopicAligned > 0.0)
            {
                shortTargetList.Sort(FeatureLight.MassAlignedComparison);
            }
            else
            {
                shortTargetList.Sort(FeatureLight.MassComparison);
            }

            // Locally store the tolerances.
            var massTolerancePpm = tolerances.MassTolerancePPM;
            var netTolerance = tolerances.NETTolerance;
            var driftTimeTolerance = tolerances.DriftTimeTolerance;

            // Iterate through the list of observed features.
            while( observedIndex < shortObservedList.Count )
            {
                // Store the current observed feature locally.
                var observedFeature = shortObservedList[observedIndex];
                // Flag variable that gets set to false when the observed mass is greater than the current mass tag by more than the tolerance.
                var continueLoop = true;
                // Set the target feature iterator to the current lower bound.
                var targetIndex = lowerBound;
                // Iterate through the list of target featrues or until the observed feature is too great.
                while( targetIndex < shortTargetList.Count && continueLoop )
                {
                    // Add any shift to the mass tag.
                    var targetFeature = shortTargetList[targetIndex];

                    // Check to see that the features are within the mass tolearance of one another.
                    double massDifference;
                    if (WithinMassTolerance(observedFeature, targetFeature, massTolerancePpm, shiftAmount, out massDifference))
                    {
                        var withinTolerances = WithinNETTolerance(observedFeature, targetFeature, netTolerance);
                        if (m_matchParameters.UseDriftTime)
                        {
                            withinTolerances = withinTolerances & WithinDriftTimeTolerance(observedFeature, targetFeature, driftTimeTolerance);
                            withinTolerances = withinTolerances & (observedFeature.ChargeState == targetFeature.ChargeState);
                        }
                        // Create a temporary match between the two and check it against all tolerances before adding to the match list.
                        if (withinTolerances)
                        {
                            var match = new FeatureMatch<TObserved, TTarget>();
                            match.AddFeatures(observedFeature, targetFeature, m_matchParameters.UseDriftTime, (shiftAmount > 0));
                            matchList.Add(match);
                        }
                    }
                    else
                    {
                        // Increase the lower bound if the the MassTag masses are too low or set the continueLoop flag to false if they are too high.
                        if (massDifference < massTolerancePpm)
                        {
                            lowerBound++;
                        }
                        else
                        {
                            continueLoop = false;
                        }
                    }
                    // Increment the target index.
                    targetIndex++;
                }
                // Increment the observed index.
                observedIndex++;
            }
            // Return the list of matches.
            return matchList;
        }
        /// <summary>
        /// Populate the FDR table with default cutoffs.
        /// </summary>
        private List<STACFDR> SetSTACCutoffs()
        {
            var stacFdrList = new List<STACFDR>();

            for (var cutoff = 0.99; cutoff > 0.90; cutoff -= 0.01)
            {
                var tableLine = new STACFDR(Math.Round(cutoff, 2));
                stacFdrList.Add(tableLine);
            }
            for (var cutoff = 0.90; cutoff >= 0; cutoff -= 0.10)
            {
                var tableLine = new STACFDR(Math.Round(cutoff, 2));
                stacFdrList.Add(tableLine);
            }

            return stacFdrList;
        }
        /// <summary>
        /// Fills in the values for the STAC FDR table.
        /// </summary>
        public List<STACFDR> PopulateStacfdrTable(List<FeatureMatch<TObserved,TTarget>> matchList)
        {
            var stacFdrList = SetSTACCutoffs();
            matchList.Sort(FeatureMatch<TObserved, TTarget>.STACComparisonDescending);

            // Iterate over all defined cutoff ranges
            for (var cutoffIndex = 0; cutoffIndex < stacFdrList.Count; cutoffIndex++)
            {
                var falseDiscoveries = 0.0;
                var conformationMatches = 0;
                var amtMatches = 0;
                var uniqueIndexList = new HashSet<int>();
                var uniqueIdList = new HashSet<int>();

                // Find all matches for this particular cutoff
                foreach (var match in matchList)
                {
                    var stac = match.STACScore;
                    if (stac >= stacFdrList[cutoffIndex].Cutoff)
                    {
                        if (uniqueIndexList.Contains(match.TargetFeature.Index))
                            continue;

                        // Find out if this is a new, unique Mass Tag. If not, then it is just a new conformation.
                        if (!uniqueIdList.Contains(match.TargetFeature.Id))
                        {
                            uniqueIdList.Add(match.TargetFeature.Id);
                            amtMatches++;
                        }

                        uniqueIndexList.Add(match.TargetFeature.Index);
                        falseDiscoveries += 1 - stac;
                        conformationMatches++;
                    }
                    else
                    {
                        // Since we have sorted by descending STAC Score, if we get outside the cutoff range, we can stop
                        break;
                    }
                }

                // After all matches have been found, report the FDR
                if (conformationMatches > 0)
                {
                    var fdr = falseDiscoveries / conformationMatches;
                    stacFdrList[cutoffIndex].FillLine(fdr, conformationMatches, amtMatches, falseDiscoveries);
                }
                else
                {
                    stacFdrList[cutoffIndex].FillLine(0, 0, 0, 0);
                }
            }

            return stacFdrList;
        }

        /// <summary>
        /// Sets the false discovery rate by creating a histogram of the mass errors and comparing the proportion above a threshhold to the area below.
        /// </summary>
        private void SetMassErrorHistogramFdr()
        {
            // Populate the mass error list and create the histogram.
            var massErrorList = MatchList.Select(match => match.DifferenceVector[1, 1]).ToList();
            var histogramList = MathUtilities.GetHistogramValues(massErrorList, m_matchParameters.HistogramBinWidth);

            var peakIndex = 0;
            var peakValue = 0.0;
            var upperBound = 0.0;
            var lowerBound = 0.0;
            var meanValue = 0.0;
            // Find the maximum and average values.
            for( var i=0; i<histogramList.Count; i++ )
            {
                var bin = histogramList[i];
                if (bin.Y > peakValue)
                {
                    peakValue = bin.Y;
                    peakIndex = i;
                }
                meanValue += bin.Y;
            }
            meanValue /= histogramList.Count;
            // Set the threshold to a value between the peak and the average value.
            var threshold = meanValue + m_matchParameters.HistogramMultiplier * (peakValue - meanValue);
            // Find the upper bound.
            for (var i = peakIndex; i < histogramList.Count; i++)
            {
                var bin = histogramList[i];
                var lowerBin = histogramList[i - 1];
                if (bin.Y < threshold && lowerBin.Y >= threshold)
                {
                    upperBound = lowerBin.X + (lowerBin.Y - threshold) / (lowerBin.Y - bin.Y) * (bin.X - lowerBin.X);
                }
            }
            // Find the lower bound.
            for (var i = peakIndex; i >= 0; i--)
            {
                var bin = histogramList[i];
                var upperBin = histogramList[i + 1];
                if (bin.Y < threshold && upperBin.Y >= threshold)
                {
                    lowerBound = bin.X + (threshold-bin.Y) / (upperBin.Y-bin.Y) * (upperBin.X - bin.X);
                }
            }
            // Count the number of matches within the bounds and calculate the FDR.
            var countInBounds = massErrorList.Count(massDifference => massDifference >= lowerBound && massDifference <= upperBound);
            ErrorHistogramFdr = countInBounds/((upperBound-lowerBound)*threshold);
        }

        private bool WithinMassTolerance(TObserved observedFeature, TTarget targetFeature, double massTolerancePpm, double shiftAmount, out double difference)
        {
            if (massTolerancePpm > 0)
            {
                double observedMass;
                double targetMass;
                if (!double.IsNaN(observedFeature.MassMonoisotopicAligned) && observedFeature.MassMonoisotopicAligned > 0.0)
                {
                    observedMass = observedFeature.MassMonoisotopicAligned;
                }
                else
                {
                    observedMass = observedFeature.MassMonoisotopic;
                }
                if (!double.IsNaN(targetFeature.MassMonoisotopicAligned) && targetFeature.MassMonoisotopicAligned > 0.0)
                {
                    targetMass = targetFeature.MassMonoisotopicAligned;
                }
                else
                {
                    targetMass = targetFeature.MassMonoisotopic;
                }

                difference = (targetMass + shiftAmount - observedMass) / targetMass * 1E6;
                return (Math.Abs(difference)< massTolerancePpm);
            }
            difference = double.MaxValue;
            return false;
        }
        private bool WithinNETTolerance(TObserved observedFeature, TTarget targetFeature, double netTolerance)
        {
            if (!(netTolerance > 0)) return false;
            double targetNet;
            if (!double.IsNaN(targetFeature.NetAligned) && targetFeature.NetAligned > 0.0)
            {
                targetNet = targetFeature.NetAligned;
            }
            else
            {
                targetNet = targetFeature.Net;
            }
            double observedNet;
            if (!double.IsNaN(observedFeature.NetAligned) && observedFeature.NetAligned > 0.0)
            {
                observedNet = observedFeature.NetAligned;
            }
            else
            {
                observedNet = observedFeature.Net;
            }
            var difference = Math.Abs(targetNet - observedNet);
            return (difference < netTolerance);
        }
        private bool WithinDriftTimeTolerance(TObserved observedFeature, TTarget targetFeature, float driftTimeTolerance)
        {
            if (!(driftTimeTolerance > 0)) return false;
            double targetDriftTime;
            double observedDriftTime;
            if (!double.IsNaN(targetFeature.DriftTimeAligned) && targetFeature.DriftTimeAligned > 0.0)
            {
                targetDriftTime = targetFeature.DriftTimeAligned;
            }
            else
            {
                targetDriftTime = targetFeature.DriftTime;
            }
            if (!double.IsNaN(observedFeature.DriftTimeAligned) && observedFeature.DriftTimeAligned > 0.0)
            {
                observedDriftTime = observedFeature.DriftTimeAligned;
            }
            else
            {
                observedDriftTime = observedFeature.DriftTime;
            }

            var difference = Math.Abs(targetDriftTime - observedDriftTime);
            return (difference < driftTimeTolerance);
        }

        #endregion

        public event EventHandler<ProgressNotifierArgs> MessageEvent;
        public event EventHandler<ProgressNotifierArgs> ProcessingCompleteEvent;

        #region Public functions
        /// <summary>
        /// Function to call to re-calculate algorithm results.
        /// </summary>
        public void MatchFeatures()
        {
            MatchList.Clear();
            MatchList = FindMatches(m_observedFeatureList, m_targetFeatureList, m_matchParameters.UserTolerances, 0);

            var lengthCheck = (MatchList.Count >= MIN_MATCHES_FOR_NORMAL_ASSUMPTION);
            if (m_matchParameters.ShouldCalculateSTAC && lengthCheck)
            {
                var stacInformation = new STACInformation(m_matchParameters.UseDriftTime);

                // Attach the event handlers
                stacInformation.MessageEvent += StacInformationMessageHandler;

                ReportMessage("Performing STAC");
                PerformStac(stacInformation);

                // Add the Refined Tolerances that STAC calculated
                RefinedToleranceList.Add(stacInformation.RefinedTolerances);

                StacParameterList.Add(stacInformation);
                ReportMessage("Populating FDR table");
                StacFdrTable = PopulateStacfdrTable(MatchList);
            }
            if (m_matchParameters.ShouldCalculateHistogramFDR)
            {
                ReportMessage("Setting Mass Error Histogram FDR");
                SetMassErrorHistogramFdr();
            }
            if (m_matchParameters.ShouldCalculateShiftFDR)
            {
                ReportMessage("Calculating Shift FDR");
                foreach (var t in MatchList)
                {
                    t.InRegion(RefinedToleranceList[0], m_matchParameters.UseEllipsoid);
                }
                var count = MatchList.Count(t => t.WithinRefinedRegion);
                ShiftedMatchList.AddRange(FindMatches(m_observedFeatureList, m_targetFeatureList, RefinedToleranceList[0], m_matchParameters.ShiftAmount));
                ShiftFdr = (1.0 * count) / ShiftedMatchList.Count;
                ShiftConservativeFdr = (2.0 * count) / ShiftedMatchList.Count;
            }

            OnProcessingComplete(new ProgressNotifierArgs("Processing Complete"));
        }

        protected virtual void PerformStac(STACInformation stacInformation)
        {
            //stacInformation.PerformStac(m_matchList, m_matchParameters.UserTolerances, m_matchParameters.UseDriftTime, m_matchParameters.UsePriors);
            stacInformation.PerformStac(MatchList,
                                        m_matchParameters.UserTolerances,
                                        m_matchParameters.UseDriftTime,
                                        m_matchParameters.UsePriors);
        }

        #endregion

        #region "Events and related functions"
        private void StacInformationMessageHandler(object sender, ProgressNotifierArgs e)
        {
            ReportMessage(e);
        }

        /// <summary>
        /// Report a progress message using OnMessage
        /// </summary>
        /// <param name="message"></param>
        private void ReportMessage(string message)
        {
            OnMessage(new ProgressNotifierArgs(message));
        }

        /// <summary>
        /// Report a progress message using OnMessage
        /// </summary>
        private void ReportMessage(ProgressNotifierArgs e)
        {
            OnMessage(e);
        }

        private void OnMessage(ProgressNotifierArgs e)
        {
            MessageEvent?.Invoke(this, e);
        }

        private void OnProcessingComplete(ProgressNotifierArgs e)
        {
            ProcessingCompleteEvent?.Invoke(this, e);
        }

        #endregion
    }

}
