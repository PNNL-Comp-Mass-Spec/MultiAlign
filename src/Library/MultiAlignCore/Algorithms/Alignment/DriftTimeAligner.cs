#region

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    public class DriftTimeAligner : IProgressNotifer
    {
        #region IProgressNotifer Members

        public event EventHandler<ProgressNotifierArgs> Progress;

        #endregion

        /// <summary>
        ///     Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            Progress?.Invoke(this, new ProgressNotifierArgs(message));
        }

        /// <summary>
        ///     Correct for the drift times.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="baselineFeatures"></param>
        /// <param name="options"></param>
        public
            KeyValuePair<DriftTimeAlignmentResults<UMCLight, UMCLight>, DriftTimeAlignmentResults<UMCLight, UMCLight>>
            AlignDriftTimes(List<UMCLight> features, List<UMCLight> baselineFeatures, DriftTimeAlignmentOptions options)
        {
            UpdateStatus("Correcting drift times.");
            var baselineUmCs = new List<UMCLight>();
            var aligneeUmCs = new List<UMCLight>();

            UpdateStatus("Mapping data structures.");
            var featureIdMap = new Dictionary<int, UMCLight>();

            foreach (var feature in features)
            {
                var umc = new UMCLight
                {
                    MassMonoisotopicAligned = feature.MassMonoisotopicAligned,
                    NetAligned = feature.NetAligned,
                    DriftTime = Convert.ToSingle(feature.DriftTime),
                    Id = feature.Id,
                    ChargeState = feature.ChargeState
                };
                aligneeUmCs.Add(umc);

                featureIdMap.Add(feature.Id, feature);
            }

            foreach (var feature in baselineFeatures)
            {
                var umc = new UMCLight
                {
                    MassMonoisotopicAligned = feature.MassMonoisotopicAligned,
                    NetAligned = feature.Net,
                    DriftTime = Convert.ToSingle(feature.DriftTime),
                    Id = feature.Id,
                    ChargeState = feature.ChargeState
                };
                baselineUmCs.Add(umc);
            }

            // filter based on charge state.
            var chargeMax = options.MaxChargeState;
            var chargeMin = options.MinChargeState;

            UpdateStatus(string.Format("Filtering Features Min Charge: {0} <= charge <= Max Charge {1}", chargeMin,
                chargeMax));
            var filteredQuery = from feature in aligneeUmCs
                where feature.ChargeState <= chargeMax && feature.ChargeState >= chargeMin
                select feature;
            var filteredUmCs = filteredQuery.ToList();

            UpdateStatus("Finding Aligned Matches and correcting drift times.");
            var alignedResults =
                DriftTimeAlignment<UMCLight, UMCLight>.AlignObservedEnumerable(aligneeUmCs,
                    filteredUmCs,
                    baselineUmCs,
                    options.MassPPMTolerance,
                    options.NETTolerance);


            DriftTimeAlignmentResults<UMCLight, UMCLight> offsetResults = null;
            if (options.ShouldPerformOffset)
            {
                UpdateStatus("Adjusting drift time offsets.");
                var aligneeData = aligneeUmCs;
                if (!options.ShouldUseAllObservationsForOffsetCalculation)
                {
                    UpdateStatus("Using only filtered matches for offset correction.");
                    aligneeData = filteredUmCs;
                }
                else
                {
                    UpdateStatus("Using all feature matches for offset correction.");
                }
                offsetResults = DriftTimeAlignment<UMCLight, UMCLight>.CorrectForOffset(aligneeData, baselineUmCs,
                    options.MassPPMTolerance, options.NETTolerance, options.DriftTimeTolerance);
            }


            UpdateStatus("Remapping data structures for persistence to database.");

            foreach (var umc in aligneeUmCs)
            {
                featureIdMap[umc.Id].DriftTime = umc.DriftTimeAligned;
            }

            var pair =
                new KeyValuePair
                    <DriftTimeAlignmentResults<UMCLight, UMCLight>, DriftTimeAlignmentResults<UMCLight, UMCLight>>(
                    alignedResults,
                    offsetResults);
            return pair;
        }
    }
}