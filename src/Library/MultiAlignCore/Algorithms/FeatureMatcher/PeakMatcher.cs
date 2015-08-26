using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    /// Matches features to a list of mass tags.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PeakMatcher<T, U>
        where T : FeatureLight, new()
        where U : FeatureLight, new ()
    {
        /// <summary>
        /// Matches a list of features to a list of mass tags.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public List<FeatureMatch<T, U>> MatchFeatures(    List<T>               features,
                                                          List<U>               tags,
                                                          PeakMatcherOptions    options)
        {
            var matches = new List<FeatureMatch<T, U>>();

            // Construct a large array of features so we can do searching in linear time.
            var allFeatures = new List<FeatureLight>();
            foreach (var copyFeature in features)
            {
                allFeatures.Add(copyFeature);
            }
            foreach (var copyTag in tags)
            {
                allFeatures.Add(copyTag);
            }

            // Sort by mass, gives us the best search time.
            allFeatures.Sort(FeatureLight.MassComparison);
            
            var netTolerance     = options.Tolerances.Net;
            var massTolerance    = options.Tolerances.Mass;
            var driftTolerance   = options.Tolerances.DriftTime;
            var shift            = options.DaltonShift;

            var N               = allFeatures.Count;
            var elementNumber   = 0;
 
            // This was a linear search, now O(N^2).  Need to improve.
			while(elementNumber < N)
			{
                var feature = allFeatures[elementNumber];
                var massTag            = feature as U;
                if (massTag == null)
                {

                    var lowerNET             = feature.Net - netTolerance;
                    var higherNET            = feature.Net + netTolerance;
                    var lowerDritfTime       = feature.DriftTime - driftTolerance;
                    var higherDriftTime      = feature.DriftTime + driftTolerance;
                    var currentMassTolerance = feature.MassMonoisotopicAligned * massTolerance / 1000000.0;
                    var lowerMass            = feature.MassMonoisotopicAligned - currentMassTolerance;
                    var higherMass           = feature.MassMonoisotopicAligned + currentMassTolerance;
                    var matchIndex              = elementNumber - 1;
                    while (matchIndex >= 0)
                    {
                        var toMatchFeature = allFeatures[matchIndex];
                        if (toMatchFeature.MassMonoisotopicAligned < lowerMass)
                        {
                            break;
                        }

                        var tag = toMatchFeature as U;
                        if (tag != null)
                        {                           
                            if (lowerNET <= tag.Net && tag.Net <= higherNET)
                            {
                                if (lowerDritfTime <= tag.DriftTime && tag.DriftTime <= higherDriftTime)
                                {
                                    var match   = new FeatureMatch<T, U>(feature as T, tag, false, false);
                                    matches.Add(match);
                                }
                            }                            
                        }
                        matchIndex--;
                    }

                    matchIndex = elementNumber + 1;
                    while(matchIndex < N)                    
                    {
                        var toMatchFeature = allFeatures[matchIndex];
                        if (toMatchFeature.MassMonoisotopicAligned > higherMass)
                        {
                            break;
                        }

                        var tag = toMatchFeature as U;
                        if (tag != null)
                        {
                            if (lowerNET <= tag.Net && tag.Net <= higherNET)
                            {
                                if (lowerDritfTime <= tag.DriftTime && tag.DriftTime <= higherDriftTime)
                                {
                                    var match = new FeatureMatch<T, U>(feature as T, tag, false, false);
                                    matches.Add(match);
                                }
                            }
                        }
                        matchIndex++;
                    }
                }
                elementNumber++;																						
			}			
            return matches;
        }
    }
}
