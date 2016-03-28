namespace MultiAlignCore.Data.Features
{
    using MultiAlignCore.Data;

    /// <summary>
    /// An enum that represents the names of the units that can be used to represent elution times.
    /// </summary>
    public enum ElutionUnitNames
    {
        /// <summary>
        /// The actual elution time in minutes.
        /// </summary>
        Minutes,

        /// <summary>
        /// The normalized elution time.
        /// </summary>
        Net,

        /// <summary>
        /// The number of scans.
        /// </summary>
        Scans,
    }

    public interface IElutionTimePoint
    {
        double Value { get; set; }

        /// <summary>
        /// Converts this elution time point from one unit to a scan number, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        /// <remarks>
        /// WARNING:
        /// When converting from NET or ElutionTime to a Scan, the converted value is only an approximation
        /// and should be treated as such.
        /// This feature was included for convenience of converting values when selecting time points through the GUI.
        /// They should NOT be used for actual computation.
        /// </remarks>
        ScanTimePoint ToScan(IScanSummaryProvider scanSummaryProvider);

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        NetTimePoint ToNet(IScanSummaryProvider scanSummaryProvider);

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        ElutionTimePoint ToElutionTime(IScanSummaryProvider scanSummaryProvider);
    }

    public class NetTimePoint : IElutionTimePoint
    {
        public NetTimePoint(double value = 0.0)
        {
            this.Value = value;
        }

        public double Value { get; set; }

        /// <summary>
        /// Converts this elution time point from one unit to a scan number, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        /// <remarks>
        /// WARNING:
        /// When converting from NET or ElutionTime to a Scan, the converted value is only an approximation
        /// and should be treated as such.
        /// This feature was included for convenience of converting values when selecting time points through the GUI.
        /// They should NOT be used for actual computation.
        /// </remarks>
        public ScanTimePoint ToScan(IScanSummaryProvider scanSummaryProvider)
        {
            // TODO: this assumes linearity in scan -> elution time, which is not necessarily the case if there are MS/MS spectra.
            // A better way would be to convert this to an approximate elution time,
            // find the closest scan: binary search by elution time on all scan summaries,
            //                        and then compare the scan summary to
            //                        the left and to the right to find the closest.
            var maxLcScan = scanSummaryProvider.MaxScan;
            return new ScanTimePoint((int)(this.Value * maxLcScan));
        }

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        public NetTimePoint ToNet(IScanSummaryProvider scanSummaryProvider)
        {
            return this;
        }

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        public ElutionTimePoint ToElutionTime(IScanSummaryProvider scanSummaryProvider)
        {
            var maxLcScan = scanSummaryProvider.MaxScan;
            var scanSummary = scanSummaryProvider.GetScanSummary(maxLcScan);
            var elutionTime = this.Value * scanSummary.Time;

            return new ElutionTimePoint(elutionTime);
        }
    }

    public class ElutionTimePoint : IElutionTimePoint
    {
        public ElutionTimePoint(double value = 0.0)
        {
            this.Value = value;
        }

        public double Value { get; set; }

        /// <summary>
        /// Converts this elution time point from one unit to a scan number, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        /// <remarks>
        /// WARNING:
        /// When converting from NET or ElutionTime to a Scan, the converted value is only an approximation
        /// and should be treated as such.
        /// This feature was included for convenience of converting values when selecting time points through the GUI.
        /// They should NOT be used for actual computation.
        /// </remarks>
        public ScanTimePoint ToScan(IScanSummaryProvider scanSummaryProvider)
        {
            return this.ToNet(scanSummaryProvider).ToScan(scanSummaryProvider);
        }

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        public NetTimePoint ToNet(IScanSummaryProvider scanSummaryProvider)
        {
            var maxScanSummary = scanSummaryProvider.GetScanSummary(scanSummaryProvider.MaxScan);
            var minScanSummary = scanSummaryProvider.GetScanSummary(scanSummaryProvider.MinScan);
            return new NetTimePoint((this.Value - minScanSummary.Time) / (maxScanSummary.Time - minScanSummary.Time));
        }

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        public ElutionTimePoint ToElutionTime(IScanSummaryProvider scanSummaryProvider)
        {
            return this;
        }
    }

    public class ScanTimePoint : IElutionTimePoint
    {
        public ScanTimePoint(int value = 0)
        {
            this.Value = value;
        }

        public int ScanValue { get { return (int)this.Value; } }

        public double Value { get; set; }

        /// <summary>
        /// Converts this elution time point from one unit to a scan number, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        public ScanTimePoint ToScan(IScanSummaryProvider scanSummaryProvider)
        {
            return this;
        }

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        NetTimePoint IElutionTimePoint.ToNet(IScanSummaryProvider scanSummaryProvider)
        {
            var scanSummary = scanSummaryProvider.GetScanSummary((int)this.Value);
            return new NetTimePoint(scanSummary.Net);
        }

        /// <summary>
        /// Converts this elution time point from one unit to a normalized elution time, keeping the value consistent between units.
        /// </summary>
        /// <param name="scanSummaryProvider">
        /// Provides scan information for the dataset that this is associated with.
        /// This is necessary because a conversion happens in the context of a single dataset.
        /// </param>
        /// <returns>The converted elution time point.</returns>
        public ElutionTimePoint ToElutionTime(IScanSummaryProvider scanSummaryProvider)
        {
            var scanSummary = scanSummaryProvider.GetScanSummary((int)this.Value);
            return new ElutionTimePoint(scanSummary.Time);
        }
    }

    /// <summary>
    /// A container for a range of values between minimum and maximum.
    /// This container requires both values to be of a certain type.
    /// </summary>
    public class ElutionTimeRange<T> where T : IElutionTimePoint
    {
        /// <summary>
        /// All possible elution time units.
        /// </summary>
        public static readonly System.Type[] ElutionTimePointTypes =
        {
            typeof(ScanTimePoint),
            typeof(NetTimePoint),
            typeof(ElutionTimePoint)
        };

        public ElutionTimeRange(T minValue, T maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        /// <summary>
        /// Gets or sets the minimum elution time point.
        /// </summary>
        public T MinValue { get; set; }
        
        /// <summary>
        /// Gets or sets the maximum elution time point.
        /// </summary>
        public T MaxValue { get; set; }
    }
}
