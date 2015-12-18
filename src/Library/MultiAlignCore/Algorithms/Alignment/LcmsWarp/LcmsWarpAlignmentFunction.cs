using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration;

    /// <summary>
    /// Object to house the Alignment Function from LCMS warping, including the
    /// calibration and alignment type
    /// </summary>
    public sealed class LcmsWarpAlignmentFunction
    {
        // TODO: What accessibility do these need, and are the names really accurate?
        private readonly LcmsWarpCalibrationType _calibrationType;
        private readonly LcmsWarpAlignmentType _alignmentType;

        private readonly List<double> _netFuncNetInput = new List<double>();
        private readonly List<double> _netFuncNetOutput = new List<double>();

        private readonly List<double> _netFuncEstScanOutput = new List<double>();
        private readonly List<double> _netFuncTimeOutput = new List<double>();

        private readonly List<double> _massFuncNetInput = new List<double>();
        private readonly List<double> _massFuncNetPpmOutput = new List<double>();

        private readonly List<double> _massFuncMzInput = new List<double>();
        private readonly List<double> _massFuncMzPpmOutput = new List<double>();

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

            if (_calibrationType == LcmsWarpCalibrationType.NetRegression)
            {
                throw new InvalidOperationException(
                    "Attempting to set MZ calibration of masses when option chosen was SCAN_CALIBRATION");
            }

            _massFuncMzInput.AddRange(mz);

            // ReSharper disable once PossibleMultipleEnumeration
            _massFuncMzPpmOutput.AddRange(ppm);
        }

        /// <summary>
        /// Sets up Net function using alignee times and reference times, function requires initialized lists
        /// </summary>
        /// <param name="aligneeNets"></param>
        /// <param name="referenceNets"></param>
        public void SetNetFunction(IEnumerable<double> aligneeNets, IEnumerable<double> referenceNets)
        {
            _netFuncNetInput.AddRange(aligneeNets);
            _netFuncNetOutput.AddRange(referenceNets);
        }

        /// <summary>
        /// Sets up Net function using alignee times and reference times as well as the reference scans in the 
        /// time output, function requires initialized lists
        /// </summary>
        /// <param name="aligneeNets"></param>
        /// <param name="referenceNets"></param>
        /// <param name="referenceScans"></param>
        /// <param name="referenceTimes"></param>
        public void SetNetFunction(IEnumerable<double> aligneeNets, IEnumerable<double> referenceNets,
            IEnumerable<double> referenceScans, IEnumerable<double> referenceTimes)
        {
            _netFuncNetInput.AddRange(aligneeNets);
            _netFuncNetOutput.AddRange(referenceNets);
            _netFuncEstScanOutput.AddRange(referenceScans);
            _netFuncTimeOutput.AddRange(referenceTimes);
        }

        /// <summary>
        /// Sets up Mass Calibration with respect to time, function requires initialized lists
        /// </summary>
        /// <param name="aligneeNets"></param>
        /// <param name="ppmShifts"></param>
        public void SetMassCalibrationFunctionWithNet(IEnumerable<double> aligneeNets, IEnumerable<double> ppmShifts)
        {
            _massFuncNetInput.AddRange(aligneeNets);
            _massFuncNetPpmOutput.AddRange(ppmShifts);
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