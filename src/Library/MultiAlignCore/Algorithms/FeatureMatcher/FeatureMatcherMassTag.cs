using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Annotations;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    [UsedImplicitly]
    public class FeatureMatcherMassTag<TObserved, TTarget> :
        FeatureMatcher<TObserved, TTarget>               
        where TObserved : FeatureLight, new()
        where TTarget   : MassTagLight, new()
    {
        public FeatureMatcherMassTag(List<TObserved> observedFeatureList, List<TTarget> targetFeatureList, FeatureMatcherParameters matchParameters)
            : base(observedFeatureList, targetFeatureList, matchParameters)
        {
            
        }

        protected override void PerformStac(STACInformation stacInformation)
        {
            
            stacInformation.PerformStac(MatchList,
                MatchParameters.UserTolerances,
                MatchParameters.UseDriftTime,
                MatchParameters.UsePriors);
            
        }
    }
}