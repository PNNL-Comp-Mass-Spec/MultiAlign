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
        readonly LcmsWarpCalibrationType m_calibrationType;
        readonly AlignmentType m_alignmentType;

        readonly List<double> m_netFuncTimeInput = new List<double>();
        readonly List<double> m_netFuncNetOutput = new List<double>();

        readonly List<double> m_netFuncTimeOutput = new List<double>();

        readonly List<double> m_massFuncTimeInput = new List<double>();
        readonly List<double> m_massFuncTimePpmOutput = new List<double>();

        readonly List<double> m_massFuncMzInput = new List<double>();
        readonly List<double> m_massFuncMzppmOutput = new List<double>();

                
        /// <summary>
        /// Constructs the Alignment function data members, used when there isn't a specified name for the
        /// alignee or reference names, sets up the calibration type and alignment type.
        /// </summary>
        /// <param name="calibType"></param>
        /// <param name="alignmentType"></param>
        public LcmsWarpAlignmentFunction(LcmsWarpCalibrationType calibType,
                                 AlignmentType alignmentType)
        {
            m_calibrationType = calibType;
            m_alignmentType = alignmentType;
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

            if (m_alignmentType != AlignmentType.NET_MASS_WARP)
            {
                throw new InvalidOperationException("Recalibration of mass not enabled with NET_WARP alignment type. PPM shift cannot be retrieved. Used NET_MASS_WARP as alignment type instead");
            }

            if (m_calibrationType == LcmsWarpCalibrationType.ScanRegression)
            {
                throw new InvalidOperationException("Attempting to set MZ calibration of masses when option chosen was SCAN_CALIBRATION");
            }

            foreach (var value in mz)
            {
                m_massFuncMzInput.Add(value);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var value in ppm)
            {
                m_massFuncMzppmOutput.Add(value);
            }
        }




        /// <summary>
        /// Sets up Net function using alignee times and reference times, function requires initialized lists
        /// </summary>
        /// <param name="aligneeTimes"></param>
        /// <param name="referenceNets"></param>
        public void SetNetFunction(IEnumerable<double> aligneeTimes, IEnumerable<double> referenceNets)
        {
            foreach (var value in aligneeTimes)
            {
                m_netFuncTimeInput.Add(value);
            }
            foreach (var value in referenceNets)
            {
                m_netFuncNetOutput.Add(value);
            }
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
            foreach (var value in aligneeTimes)
            {
                m_netFuncTimeInput.Add(value);
            }
            foreach (var value in referenceNets)
            {
                m_netFuncNetOutput.Add(value);
            }
            foreach (var value in referenceScans)
            {
                m_netFuncTimeOutput.Add(value);
            }
        }

        /// <summary>
        /// Sets up Mass Calibration with respect to time, function requires initialized lists
        /// </summary>
        /// <param name="aligneeTimes"></param>
        /// <param name="ppmShifts"></param>
        public void SetMassCalibrationFunctionWithTime(IEnumerable<double> aligneeTimes, IEnumerable<double> ppmShifts)
        {
            foreach (var value in aligneeTimes)
            {
                m_massFuncTimeInput.Add(value);
            }
            foreach (var value in ppmShifts)
            {
                m_massFuncTimePpmOutput.Add(value);
            }
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
