using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{
    public class AnchorPointErrorMeasurement
    {
        /// <summary>
        /// Computes errors for mass and retention time given a set of linked and matched features.
        /// </summary>
        public Tuple<AlignmentMeasurement<double>, AlignmentMeasurement<double>> MeasureErrors(IEnumerable<SpectralAnchorPointMatch> matches)
        {
            var netError  = new AlignmentMeasurement<double>();
            var massError = new AlignmentMeasurement<double>();

            var errors    = new Tuple<AlignmentMeasurement<double>, AlignmentMeasurement<double>>
                                        (netError, massError);

              
            foreach (var match in matches)
            {
                var x   = match.AnchorPointX;
                var y   = match.AnchorPointY;
                var featureX = x.Spectrum.ParentFeature;
                var featureY = y.Spectrum.ParentFeature;

                if (featureX == null || featureY == null)
                    continue;

                var umcX = featureX.GetParentFeature();
                var umcY = featureY.GetParentFeature();

                netError.PreAlignment.Add(umcX.Net  - umcY.Net);
                netError.PostAlignment.Add(umcX.Net - umcY.NetAligned);

                massError.PreAlignment.Add(FeatureLight.ComputeMassPPMDifference(umcX.MassMonoisotopic, umcY.MassMonoisotopic));
                massError.PostAlignment.Add(FeatureLight.ComputeMassPPMDifference(umcX.MassMonoisotopic, umcY.MassMonoisotopicAligned));
            }

            return errors;
        }
    }
}
