using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public class AnchorPointErrorMeasurement
    {
        /// <summary>
        /// Computes errors for mass and retention time given a set of linked and matched features.
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public Tuple<AlignmentMeasurement<double>, AlignmentMeasurement<double>> MeasureErrors(IEnumerable<AnchorPointMatch> matches)
        {
            AlignmentMeasurement<double> netError  = new AlignmentMeasurement<double>();
            AlignmentMeasurement<double> massError = new AlignmentMeasurement<double>();

            Tuple<AlignmentMeasurement<double>, AlignmentMeasurement<double>> errors =
                                    new Tuple<AlignmentMeasurement<double>, AlignmentMeasurement<double>>
                                        (netError, massError);

            // Baseline features
            Dictionary<int, UMCLight> featureMap = new Dictionary<int, UMCLight>();
            foreach (AnchorPointMatch match in matches)
            {
                AnchorPoint x           = match.AnchorPointX;
                AnchorPoint y           = match.AnchorPointX;
                MSFeatureLight featureX = x.Spectrum.ParentFeature;
                MSFeatureLight featureY = y.Spectrum.ParentFeature;

                if (featureX != null && featureY != null)
                {
                    UMCLight umcX = featureX.ParentFeature;
                    UMCLight umcY = featureY.ParentFeature;

                    netError.PreAlignment.Add(umcX.NET  - umcY.NET);
                    netError.PostAlignment.Add(umcX.NET - umcY.NETAligned);

                    massError.PreAlignment.Add(Feature.ComputeMassPPMDifference(umcX.MassMonoisotopic, umcY.MassMonoisotopic));
                    massError.PostAlignment.Add(Feature.ComputeMassPPMDifference(umcX.MassMonoisotopic, umcY.MassMonoisotopicAligned));
                }
            }

            return errors;
        }
    }
}
