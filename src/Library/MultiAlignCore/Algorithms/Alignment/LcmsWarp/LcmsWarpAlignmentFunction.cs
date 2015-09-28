using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Object to house the Alignment Function from LCMS warping, including the
    /// calibration and alignment type
    /// </summary>
    public sealed class LcmsWarpAlignmentFunction
    {
        private readonly LcmsWarpCalibrationType _calibrationType;
        private readonly LcmsWarpAlignmentType _alignmentType;

        private readonly List<double> _netFuncTimeInput = new List<double>();
        private readonly List<double> _netFuncNetOutput = new List<double>();

        private readonly List<double> _netFuncTimeOutput = new List<double>();

        private readonly List<double> _massFuncTimeInput = new List<double>();
        private readonly List<double> _massFuncTimePpmOutput = new List<double>();

        private readonly List<double> _massFuncMzInput = new List<double>();
        private readonly List<double> _massFuncMzppmOutput = new List<double>();

        /// <summary>
        /// Constructs the Alignment function data members, used when there isn't a specified name for the
        /// alignee or reference names, sets up the calibration type and alignment type.
        /// </summary>
        /// <param name="calibType"></param>
        /// <param name="alignmentType"></param>
        public LcmsWarpAlignmentFunction(LcmsWarpCalibrationType calibType,
            LcmsWarpAlignmentType alignmentType)
        {
            _calibrationType = calibType;
            _alignmentType = alignmentType;
        }

        /// <summary>
        /// Sets up the mass calibration function using ppm and mz.
        /// Throws exceptions if there is no PPM data, alignment type is not of NET-Mass warping or if calibration type is
        /// not of Scan calibration type.
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="ppm"></param>
        public void SetMassCalibrationFunctionWithMz(IEnumerable<double> mz, IEnumerable<double> ppm)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (!ppm.Any())
            {
                throw new ArgumentException("Input Mass Calibration Function with time has no ppm data.");
            }

            if (_alignmentType != LcmsWarpAlignmentType.NET_MASS_WARP)
            {
                throw new InvalidOperationException(
                    "Recalibration of mass not enabled with NET_WARP alignment type. PPM shift cannot be retrieved. Used NET_MASS_WARP as alignment type instead");
            }

            if (_calibrationType == LcmsWarpCalibrationType.ScanRegression)
            {
                throw new InvalidOperationException(
                    "Attempting to set MZ calibration of masses when option chosen was SCAN_CALIBRATION");
            }

            _massFuncMzInput.AddRange(mz);

            // ReSharper disable once PossibleMultipleEnumeration
            _massFuncMzppmOutput.AddRange(ppm);
        }

        /// <summary>
        /// Sets up Net function using alignee times and reference times, function requires initialized lists
        /// </summary>
        /// <param name="aligneeTimes"></param>
        /// <param name="referenceNets"></param>
        public void SetNetFunction(IEnumerable<double> aligneeTimes, IEnumerable<double> referenceNets)
        {
            _netFuncTimeInput.AddRange(aligneeTimes);
            _netFuncNetOutput.AddRange(referenceNets);
        }

        /// <summary>
        /// Sets up Net function using alignee times and reference times as well as the reference scans in the 
        /// time output, function requires initialized lists
        /// </summary>
        /// <param name="aligneeTimes"></param>
        /// <param name="referenceNets"></param>
        /// <param name="referenceScans"></param>
        public void SetNetFunction(
            IEnumerable<double> aligneeTimes,
            IEnumerable<double> referenceNets,
            IEnumerable<double> referenceScans)
        {
            _netFuncTimeInput.AddRange(aligneeTimes);
            _netFuncNetOutput.AddRange(referenceNets);
            _netFuncTimeOutput.AddRange(referenceScans);
        }

        /// <summary>
        /// Sets up Mass Calibration with respect to time, function requires initialized lists
        /// </summary>
        /// <param name="aligneeTimes"></param>
        /// <param name="ppmShifts"></param>
        public void SetMassCalibrationFunctionWithTime(IEnumerable<double> aligneeTimes, IEnumerable<double> ppmShifts)
        {
            _massFuncTimeInput.AddRange(aligneeTimes);
            _massFuncTimePpmOutput.AddRange(ppmShifts);
        }

        /// <summary>
        /// Get warped NET. If warped net is not in alignment function, it will interpolate
        /// between points.
        /// </summary>
        /// <param name="aligneeNet">The alignee net to warp.</param>
        /// <returns>Warped NET.</returns>
        //public double GetInterpolatedNet(double aligneeNet)
        //{
        //    this.m_netFuncTimeInput.BinarySearch(aligneeNet);
        //}
    }
}