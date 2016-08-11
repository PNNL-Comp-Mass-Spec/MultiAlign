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
        private readonly LcmsWarpCalibrationType _calibrationType;
        private readonly LcmsWarpAlignmentType _alignmentType;

        /// <summary>
        /// NET values before and after alignment
        /// Keys are input NET (often scan number), values are aligned NET
        /// </summary>
        /// <remarks>Sorted ascending on input NET</remarks>
        private readonly List<KeyValuePair<double, double>> _netAlignmentFunction = new List<KeyValuePair<double, double>>();

        /// <summary>
        /// This list is used to find the closest matching data point in _netAlignmentFunction based on an input NET value
        /// Keys are input NET (often scan number), values are an index in _netAlignmentFunction
        /// </summary>
        private readonly SortedList<double, int> _netLookupKeys = new SortedList<double, int>();

        /// <summary>
        /// Scan number to time mapping as a list of KeyValuePairs
        /// Keys are scan number, values are aligned NET
        /// </summary>
        /// <remarks>Sorted ascending on scan number</remarks>
        private readonly List<KeyValuePair<double, double>> _scanToTimeMapping = new List<KeyValuePair<double, double>>();

        /// <summary>
        /// Mass calibration function as a list of KeyValuePairs
        /// Keys are time, values are ppm
        /// </summary>
        /// <remarks>Sorted ascending on time</remarks>
        private readonly List<KeyValuePair<double, double>> _massCalibrationFunction = new List<KeyValuePair<double, double>>();

        /// <summary>
        /// m/z calibration function as a list of KeyValuePairs
        /// Keys are m/z, values are ppm
        /// </summary>
        /// <remarks>Sorted ascending on m/z</remarks>
        private readonly List<KeyValuePair<double, double>> _mzCalibrationFunction = new List<KeyValuePair<double, double>>();


        /// <summary>
        /// NET Alignment function as a list of KeyValuePairs
        /// Keys are input NET, values are aligned NET
        /// </summary>
        /// // ReSharper disable once UnusedMember.Global
        public List<KeyValuePair<double, double>> NetAlignmentFunction
        {
            get { return _netAlignmentFunction; }
        }

        /// <summary>
        /// Scan number to time mapping as a list of KeyValuePairs
        /// Keys are scan number, values are aligned NET
        /// </summary>
        /// // ReSharper disable once UnusedMember.Global
        public List<KeyValuePair<double, double>> ScanToTimeMapping
        {
            get { return _scanToTimeMapping; }
        }

        /// <summary>
        /// Mass calibration function as a list of KeyValuePairs
        /// Keys are time, values are ppm
        /// </summary>
        /// // ReSharper disable once UnusedMember.Global
        public List<KeyValuePair<double, double>> MassCalibrationFunction
        {
            get { return _massCalibrationFunction; }
        }

        /// <summary>
        /// m/z calibration function as a list of KeyValuePairs
        /// Keys are m/z, values are ppm
        /// </summary>
        /// // ReSharper disable once UnusedMember.Global
        public List<KeyValuePair<double, double>> MzCalibrationFunction
        {
            get { return _mzCalibrationFunction; }
        }

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

        private IEnumerable<KeyValuePair<double, double>> ConvertParallelLists(IReadOnlyList<double> list1, IReadOnlyList<double> list2)
        {
            var dataPoints = new List<KeyValuePair<double, double>>();
            for (var i = 0; i < list1.Count; i++)
            {
                if (i >= list2.Count)
                    break;
                dataPoints.Add(new KeyValuePair<double, double>(list1[i], list2[i]));
            }

            return dataPoints;
        }

        /// <summary>
        /// Sets up the mass calibration function using ppm and mz.
        /// Throws exceptions if there is no PPM data, alignment type is not of NET-Mass warping or if calibration type is
        /// not of Scan calibration type.
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="ppm"></param>
        public void SetMassCalibrationFunctionWithMz(IReadOnlyList<double> mz, IReadOnlyList<double> ppm)
        {
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

            if (mz.Count != ppm.Count)
            {
                throw new InvalidOperationException(
                    "Length of mz and ppm should be identical; " +
                    "currently " + mz.Count + " and " + ppm.Count);
            }

            // Convert the parallel lists to a single list
            var mzFunction = ConvertParallelLists(mz, ppm);

            // Assure that mz is sorted ascending
            var query = (from item in mzFunction orderby item.Key select item);

            _mzCalibrationFunction.Clear();
            _mzCalibrationFunction.AddRange(query);

        }

        /// <summary>
        /// Sets up NET function using alignee times and reference times, function requires initialized lists
        /// </summary>
        /// <param name="aligneeNets">NET values before alignment</param>
        /// <param name="referenceNets">NET values after alignment</param>
        public void SetNetFunction(IReadOnlyList<double> aligneeNets, IReadOnlyList<double> referenceNets)
        {
            if (aligneeNets.Count != referenceNets.Count)
            {
                throw new InvalidOperationException(
                    "Length of aligneeNets and referenceNets should be identical; " +
                    "currently " + aligneeNets.Count + " and " + referenceNets.Count);
            }

            // Convert the parallel lists to a single list
            var netFunction = ConvertParallelLists(aligneeNets, referenceNets);

            // Assure that aligneeNets is sorted ascending
            var query = (from item in netFunction orderby item.Key select item);

            _netAlignmentFunction.Clear();
            _netAlignmentFunction.AddRange(query);

            // Populate _netLookupKeys, which is used by GetInterpolatedNet
            _netLookupKeys.Clear();

            var lastValue = double.MinValue;

            for (var i = 0; i < _netAlignmentFunction.Count; i++)
            {
                var valueToAdd = _netAlignmentFunction[i].Key;

                if (Math.Abs(valueToAdd - lastValue) < float.Epsilon)
                    continue;

                _netLookupKeys.Add(valueToAdd, i);

                lastValue = valueToAdd;
            }

        }

        /// <summary>
        /// Sets up NET function using alignee times and reference times as well as the reference scans in the
        /// time output; function requires initialized lists
        /// </summary>
        /// <param name="aligneeNets"></param>
        /// <param name="referenceNets"></param>
        /// <param name="referenceScans"></param>
        /// <param name="referenceTimes"></param>
        public void SetNetFunction(IReadOnlyList<double> aligneeNets, IReadOnlyList<double> referenceNets,
            IReadOnlyList<double> referenceScans, IReadOnlyList<double> referenceTimes)
        {
            SetNetFunction(aligneeNets, referenceNets);


            if (referenceScans.Count != referenceTimes.Count)
            {
                throw new InvalidOperationException(
                    "Length of referenceScans and referenceTimes should be identical; " +
                    "currently " + referenceScans.Count + " and " + referenceTimes.Count);
            }

            // Convert the parallel lists to a single list
            var scanToTimeMapping = ConvertParallelLists(referenceScans, referenceTimes);

            // Assure that referenceScans is sorted ascending
            var query = (from item in scanToTimeMapping orderby item.Key select item);

            _scanToTimeMapping.Clear();
            _scanToTimeMapping.AddRange(query);

        }

        /// <summary>
        /// Sets up Mass Calibration with respect to time, function requires initialized lists
        /// </summary>
        /// <param name="aligneeNets"></param>
        /// <param name="ppmShifts"></param>
        public void SetMassCalibrationFunctionWithNet(IReadOnlyList<double> aligneeNets, IReadOnlyList<double> ppmShifts)
        {
            if (aligneeNets.Count != ppmShifts.Count)
            {
                throw new InvalidOperationException(
                    "Length of aligneeNets and ppmShifts should be identical; " +
                    "currently " + aligneeNets.Count + " and " + ppmShifts.Count);
            }

            // Convert the parallel lists to a single list
            var massCalibrationFunction = ConvertParallelLists(aligneeNets, ppmShifts);

            // Assure that aligneeNets is sorted ascending
            var query = (from item in massCalibrationFunction orderby item.Key select item);

            _massCalibrationFunction.Clear();
            _massCalibrationFunction.AddRange(query);
        }

        /// <summary>
        /// Given the alignee NET (often scan number) compute the warped NET value
        /// Interpolates between adjacent points in _netAlignmentFunction
        /// </summary>
        /// <param name="aligneeNet">The alignee NET to warp.</param>
        /// <returns>Warped NET.</returns>
        public double GetInterpolatedNet(double aligneeNet)
        {
            // Find the closest data point in _netAlignmentFunction

            double alignedNET;
            var keys = new List<double>(_netLookupKeys.Keys);

            var matchIndex = keys.BinarySearch(aligneeNet);

            if (matchIndex >= 0)
            {
                // Exact match was found
                var resultIndex = _netLookupKeys[keys[matchIndex]];
                alignedNET = _netAlignmentFunction[resultIndex].Value;
            }
            else
            {
                if (_netLookupKeys.Count < 1)
                {
                    throw new Exception("Cannot interpolate the X value since _netLookupKeys does not contain multiple entries");
                }

                var closestMatch = ~matchIndex - 1;
                if (closestMatch < 0)
                    closestMatch = 0;

                if (closestMatch == _netLookupKeys.Count - 1)
                {
                    // Matched the last entry in _netLookupKeyIndices
                    // Decrement by one so that we can interpolate
                    closestMatch--;
                }
                var resultIndex1 = _netLookupKeys[keys[closestMatch]];
                var resultIndex2 = _netLookupKeys[keys[closestMatch + 1]];

                var x1 = _netAlignmentFunction[resultIndex1].Key;
                var x2 = _netAlignmentFunction[resultIndex2].Key;

                var y1 = _netAlignmentFunction[resultIndex1].Value;
                var y2 = _netAlignmentFunction[resultIndex2].Value;

                alignedNET = InterpolateY(x1, x2, y1, y2, aligneeNet);
            }

            return alignedNET;
        }

        /// <summary>
        /// Given two X,Y coordinates interpolate or extrapolate to determine the Y value that would be seen for a given X value
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="xValueToInterpolate"></param>
        /// <returns></returns>
        private double InterpolateY(double x1, double x2, double y1, double y2, double xValueToInterpolate)
        {

            var xDifference = x2 - x1;

            if (Math.Abs(xDifference) < double.Epsilon)
                throw new ArgumentException("x1 and x2 are identical; cannot interpolate");

            var interpolatedValue = y1 + (y2 - y1) * ((xValueToInterpolate - x1) / xDifference);
            return interpolatedValue;
        }
    }
}