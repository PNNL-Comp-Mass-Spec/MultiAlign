using System;
using System.Collections.Generic;
using System.Linq;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms;

namespace MultiAlignCore.Algorithms.Alignment
{
    public class DriftTimeAligner: IProgressNotifer
    {

        #region IProgressNotifer Members

        public event EventHandler<ProgressNotifierArgs> Progress;

        #endregion

        /// <summary>
        /// Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }

        /// <summary>
        /// Correct for the drift times.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="baselineFeatures"></param>
        /// <param name="options"></param>
        public KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>>
            AlignDriftTimes(List<UMCLight> features, List<UMCLight> baselineFeatures, DriftTimeAlignmentOptions options)
        {
            UpdateStatus("Correcting drift times.");
            List<UMC> baselineUMCs = new List<UMC>();
            List<UMC> aligneeUMCs = new List<UMC>();

            UpdateStatus("Mapping data structures.");
            Dictionary<int, UMCLight> featureIDMap = new Dictionary<int, UMCLight>();

            foreach (UMCLight feature in features)
            {
                UMC umc = new UMC();
                umc.MassMonoisotopicAligned = feature.MassMonoisotopicAligned;
                umc.NETAligned = feature.NET;
                umc.DriftTime = Convert.ToSingle(feature.DriftTime);
                umc.ID = feature.ID;
                umc.ChargeState = feature.ChargeState;
                aligneeUMCs.Add(umc);

                featureIDMap.Add(feature.ID, feature);
            }

            foreach (UMCLight feature in baselineFeatures)
            {
                UMC umc = new UMC();
                umc.MassMonoisotopicAligned = feature.MassMonoisotopicAligned;
                umc.NETAligned = feature.NET;
                umc.DriftTime = Convert.ToSingle(feature.DriftTime);
                umc.ID = feature.ID;
                umc.ChargeState = feature.ChargeState;
                baselineUMCs.Add(umc);
            }

            // filter based on charge state.                  
            int chargeMax = options.MaxChargeState;
            int chargeMin = options.MinChargeState;

            UpdateStatus(string.Format("Filtering Features Min Charge: {0} <= charge <= Max Charge {1}", chargeMin, chargeMax));
            var filteredQuery = from feature in aligneeUMCs
                                where feature.ChargeState <= chargeMax && feature.ChargeState >= chargeMin
                                select feature;
            List<UMC> filteredUMCs = filteredQuery.ToList();

            UpdateStatus("Finding Aligned Matches and correcting drift times.");
            DriftTimeAlignmentResults<UMC, UMC> alignedResults =
                            DriftTimeAlignment<UMC, UMC>.AlignObservedEnumerable(aligneeUMCs,
                                                                                filteredUMCs,
                                                                                baselineUMCs,
                                                                                options.MassPPMTolerance,
                                                                                options.NETTolerance);


            DriftTimeAlignmentResults<UMC, UMC> offsetResults = null;
            if (options.ShouldPerformOffset)
            {
                UpdateStatus("Adjusting drift time offsets.");
                List<UMC> aligneeData = aligneeUMCs;
                if (!options.ShouldUseAllObservationsForOffsetCalculation)
                {
                    UpdateStatus("Using only filtered matches for offset correction.");
                    aligneeData = filteredUMCs;
                }
                else
                {
                    UpdateStatus("Using all feature matches for offset correction.");
                }
                offsetResults = DriftTimeAlignment<UMC, UMC>.CorrectForOffset(aligneeData, baselineUMCs, options.MassPPMTolerance, options.NETTolerance, options.DriftTimeTolerance);
            }


            UpdateStatus("Remapping data structures for persistence to database.");

            foreach (UMC umc in aligneeUMCs)
            {
                featureIDMap[umc.ID].DriftTime = umc.DriftTimeAligned;
            }

            KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>> pair =
                            new KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>, DriftTimeAlignmentResults<UMC, UMC>>(alignedResults,
                                                                                                                       offsetResults);
            return pair;
        }

    }
}
