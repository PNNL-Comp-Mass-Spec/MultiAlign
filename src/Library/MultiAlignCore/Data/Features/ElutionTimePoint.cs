namespace MultiAlignCore.Data.Features
{
    using System;

    using MultiAlignCore.Data;

    public interface IElutionTimePoint
    {
        double Value { get; set; }

        FeatureLight.SeparationTypes SeparationType { get; set; }

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
        public double Value { get; set; }

        public FeatureLight.SeparationTypes SeparationType { get; set; }

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
            return new ScanTimePoint
            {
                Value = (int)(this.Value * maxLcScan),
                SeparationType = this.SeparationType
            };
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
            if (this.SeparationType == FeatureLight.SeparationTypes.DriftTime)
            {
                throw new ArgumentException("Conversions on IMS time points not currently supported.");
            }

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

            return new ElutionTimePoint
            {
                Value = elutionTime,
                SeparationType = this.SeparationType
            };
        }
    }

    public class ElutionTimePoint : IElutionTimePoint
    {
        public double Value { get; set; }

        public FeatureLight.SeparationTypes SeparationType { get; set; }

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
            if (this.SeparationType == FeatureLight.SeparationTypes.DriftTime)
            {
                throw new ArgumentException("Conversions on IMS time points not currently supported.");
            }

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
            if (this.SeparationType == FeatureLight.SeparationTypes.DriftTime)
            {
                throw new ArgumentException("Conversions on IMS time points not currently supported.");
            }

            var maxScanSummary = scanSummaryProvider.GetScanSummary(scanSummaryProvider.MaxScan);
            var minScanSummary = scanSummaryProvider.GetScanSummary(scanSummaryProvider.MinScan);
            return new NetTimePoint
            {
                Value = (this.Value - minScanSummary.Time) / (maxScanSummary.Time - minScanSummary.Time),
                SeparationType = this.SeparationType
            };
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
            if (this.SeparationType == FeatureLight.SeparationTypes.DriftTime)
            {
                throw new ArgumentException("Conversions on IMS time points not currently supported.");
            }

            return this;
        }
    }

    public class ScanTimePoint : IElutionTimePoint
    {
        public double Value { get; set; }

        public FeatureLight.SeparationTypes SeparationType { get; set; }

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
            if (this.SeparationType == FeatureLight.SeparationTypes.DriftTime)
            {
                throw new ArgumentException("Conversions on IMS time points not currently supported.");
            }

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
            if (this.SeparationType == FeatureLight.SeparationTypes.DriftTime)
            {
                throw new ArgumentException("Conversions on IMS time points not currently supported.");
            }

            var scanSummary = scanSummaryProvider.GetScanSummary((int)this.Value);
            return new NetTimePoint
            {
                Value = scanSummary.Net,
                SeparationType = this.SeparationType

            };
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
            if (this.SeparationType == FeatureLight.SeparationTypes.DriftTime)
            {
                throw new ArgumentException("Conversions on IMS time points not currently supported.");
            }

            var scanSummary = scanSummaryProvider.GetScanSummary((int)this.Value);
            return new ElutionTimePoint
            {
                Value = scanSummary.Time,
                SeparationType = this.SeparationType

            };
        }
    }
}
