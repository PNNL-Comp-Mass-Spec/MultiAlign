using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.Windows.Plots
{
    public static class UnitSingletons
    {
        static UnitSingletons()
        {
            MassDimension        = Plots.MassDimension.Monoisotopic;
            ClusterTimeDimension = Plots.TimeDimension.NET;
            FeatureTimeDimension = Plots.TimeDimension.NET;
        }

        public static MassDimension MassDimension { get; set; }
        public static TimeDimension ClusterTimeDimension { get; set; }
        public static TimeDimension FeatureTimeDimension { get; set; }
    }

    public enum MassDimension
    {
        Monoisotopic,
        Mz
    }

    public enum TimeDimension
    {
        Scan,
        NET,
        RetentionTime
    }
}
