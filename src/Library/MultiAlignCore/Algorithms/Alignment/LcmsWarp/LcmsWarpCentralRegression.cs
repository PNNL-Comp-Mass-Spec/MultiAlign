using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms.Regression;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Object which performs Central Regression for LCMSWarp
    /// </summary>
    public class LcmsWarpCentralRegression
    {
        private int _numYBins;
        private int _numJumps;

        // number of matches for each x section
        private int _numSectionMatches;
        // Minimum number of points to be present in a section
        // for it to be considered in computing function
        private readonly int _minSectionPts;

        private readonly List<double> _matchScores;
        private readonly List<double> _sectionMisMatchScore;
        private readonly List<double> _sectionTolerance;
        private readonly List<double> _alignmentScores;
        private readonly List<int> _bestPreviousIndex;
        private readonly List<int> _count;

        private readonly NormalUniformEm _normUnifEm;

        private double _minY;
        private double _maxY;
        //the tolerance to apply
        private double _tolerance;
        //outlier zscore
        private double _outlierZScore;

        //Storage for standard deviations at each slice
        private readonly List<double> _stdY;
        //Storage for alignment
        private readonly Dictionary<int, int> _alignmentFunction;

        private double _minX;
        private double _maxX;

        private int _numXBins;
        private readonly List<RegressionPoint> _pts;

        /// <summary>
        /// Default constructor the Central regression, sets parameters to default
        /// values and allocates memory space for the lists that will be used
        /// </summary>
        public LcmsWarpCentralRegression()
        {
            _numXBins = 100;
            _numYBins = 100;
            _numJumps = 30;
            _tolerance = 0.8; // 5 standard devs
            _outlierZScore = _tolerance;
            _minSectionPts = 5;
            _matchScores = new List<double>();
            _sectionMisMatchScore = new List<double>();
            _sectionTolerance = new List<double>();
            _alignmentScores = new List<double>();
            _bestPreviousIndex = new List<int>();
            _count = new List<int>();

            _normUnifEm = new NormalUniformEm();

            _stdY = new List<double>();
            _alignmentFunction = new Dictionary<int, int>();

            _pts = new List<RegressionPoint>();

            SetOptions(_numXBins, _numYBins, _numJumps, _outlierZScore);
        }

        /// <summary>
        /// Getter to return the list of regression points
        /// </summary>
        /// <returns></returns>
        public List<RegressionPoint> Points
        {
            get { return _pts; }
        }

        /// <summary>
        /// Sets up all the options for the Central Regression
        /// </summary>
        /// <param name="numXBins"></param>
        /// <param name="numYBins"></param>
        /// <param name="numJumps"></param>
        /// <param name="zTolerance"></param>
        public void SetOptions(int numXBins, int numYBins, int numJumps, double zTolerance)
        {
            _numXBins = numXBins;
            _numYBins = numYBins;
            _numJumps = numJumps;
            _tolerance = zTolerance;
            _outlierZScore = zTolerance;

            _numSectionMatches = _numYBins * (2 * numJumps + 1);

            _matchScores.Clear();
            _alignmentScores.Clear();
            _bestPreviousIndex.Clear();
        }

        /// <summary>
        /// Method to set the outlier score for Central regression.
        /// </summary>
        /// <param name="outlierZScore"></param>
        public void SetOutlierZScore(double outlierZScore)
        {
            _outlierZScore = outlierZScore;
        }

        /// <summary>
        /// Finds the min and max mass error and Net
        /// </summary>
        public void CalculateMinMax()
        {
            var numPts = _pts.Count();

            _minX = double.MaxValue;
            _maxX = double.MinValue;

            _minY = double.MaxValue;
            _maxY = double.MinValue;

            for (var i = 0; i < numPts; i++)
            {
                var point = _pts[i];
                _minX = Math.Min(_minX, point.X);
                _maxX = Math.Max(_maxX, point.X);
                _minY = Math.Min(_minY, point.Y);
                _maxY = Math.Max(_maxY, point.Y);
            }
        }

        /// <summary>
        /// Computers the Sections standard deviation and number in each match
        /// </summary>
        /// <param name="intervalNum"></param>
        private void CalculateSectionStdAndCount(int intervalNum)
        {
            var points = new List<double>();

            var numPoints = _pts.Count();
            var xInterval = (_maxX - _minX) / _numXBins;

            for (var ptNum = 0; ptNum < numPoints; ptNum++)
            {
                var pt = _pts[ptNum];
                var sectionNum = Convert.ToInt32((pt.X - _minX) / xInterval);
                if (sectionNum == _numXBins)
                {
                    sectionNum = _numXBins - 1;
                }
                if (intervalNum == sectionNum)
                {
                    _count[sectionNum]++;
                    points.Add(pt.MassError);
                }
            }

            if (_count[intervalNum] > _minSectionPts)
            {
                _normUnifEm.CalculateDistributions(points);
                _stdY[intervalNum] = _normUnifEm.StandDev;
                if (Math.Abs(_stdY[intervalNum]) > double.Epsilon)
                {
                    var tolerance = _stdY[intervalNum] * _tolerance;
                    _sectionTolerance[intervalNum] = tolerance;

                    var misMatchScore = (tolerance * tolerance) / (_stdY[intervalNum] * _stdY[intervalNum]);
                    _sectionMisMatchScore[intervalNum] = misMatchScore;
                }
                else
                {
                    _sectionMisMatchScore[intervalNum] = _tolerance * _tolerance;
                    _sectionTolerance[intervalNum] = 0;
                }
            }
            else
            {
                _stdY[intervalNum] = 0.1;
            }
        }

        /// <summary>
        /// Calculate standard deviations for all sections
        /// </summary>
        public void CalculateSectionsStd()
        {
            _count.Capacity = _numXBins;
            _stdY.Capacity = _numXBins;
            _stdY.Clear();

            for (var interval = 0; interval < _numXBins; interval++)
            {
                _stdY.Add(0);
                _count.Add(0);
                _sectionMisMatchScore.Add(0);
                _sectionTolerance.Add(0);
                CalculateSectionStdAndCount(interval);
            }
        }

        /// <summary>
        /// Removes all the residual data from prior regressions
        /// </summary>
        public void Clear()
        {
            _matchScores.Clear();
            _alignmentFunction.Clear();
            _alignmentScores.Clear();
            _bestPreviousIndex.Clear();
            _count.Clear();
            _pts.Clear();
            _stdY.Clear();
            _sectionMisMatchScore.Clear();
            _sectionTolerance.Clear();
        }

        private void SetUnmatchedScoreMatrix()
        {
            // Assigns each section's score to the minimum for that section
            // for each possible matching sections, the minimum score would correspond 
            // to the situation that all points in the section lie outside the tolerance
            _matchScores.Clear();

            // At the moment, assumes that the tolerance is in zscore units
            for (var xSection = 0; xSection < _numXBins; xSection++)
            {
                var sectionMismatchScore = _sectionMisMatchScore[xSection] * _count[xSection];

                for (var sectionMatchNum = 0; sectionMatchNum < _numSectionMatches; sectionMatchNum++)
                {
                    _matchScores.Add(sectionMismatchScore);
                }
            }
        }

        private void CalculateScoreMatrix()
        {
            _matchScores.Capacity = _numXBins * _numSectionMatches;

            // Neww to calculate the score matrix for all possible score blocks.
            // For every x section, all y secments between y_interest - m_num_jumps
            // to y_interest + m_num_jumps are scored.

            // First set the unmatched score.
            SetUnmatchedScoreMatrix();

            var yIntervalSize = (_maxY - _minY) / _numYBins;
            var xIntervalSize = (_maxX - _minX) / _numXBins;

            // For each point that is seen, add the supporting score to the appropriate section.
            var numPts = _pts.Count();
            if ((yIntervalSize > 0.000001) && (xIntervalSize > 0.000001))
            {
                for (var pointNum = 0; pointNum < numPts; pointNum++)
                {
                    var point = _pts[pointNum];
                    var xSection = Convert.ToInt32((point.X - _minX) / xIntervalSize);
                    if (xSection == _numXBins)
                    {
                        xSection = _numXBins - 1;
                    }

                    // If the point belongs to a section where the num # of points is not met, ignore it
                    if (_count[xSection] < _minSectionPts || Math.Abs(_stdY[xSection]) < double.Epsilon)
                    {
                        continue;
                    }

                    var yTolerance = _sectionTolerance[xSection];

                    var yInterval = Convert.ToInt32((0.0001 + (point.MassError - _minY) / yIntervalSize));

                    if (yInterval == _numYBins)
                    {
                        yInterval = _numYBins - 1;
                    }

                    // Matches to the section that the point would contribute to.
                    var minYStart = Convert.ToInt32(yInterval - yTolerance / yIntervalSize);
                    var maxYStart = Convert.ToInt32(yInterval + yTolerance / yIntervalSize);

                    var sectionMismatchScore = _sectionMisMatchScore[xSection];

                    var xFraction = (point.X - _minX) / xIntervalSize - xSection;

                    for (var yFrom = minYStart; yFrom <= maxYStart; yFrom++)
                    {
                        if (yFrom < 0)
                        {
                            continue;
                        }
                        if (yFrom >= _numYBins)
                        {
                            break;
                        }
                        for (var yTo = yFrom - _numJumps; yTo <= yFrom + _numJumps; yTo++)
                        {
                            if (yTo < 0)
                            {
                                continue;
                            }
                            if (yTo >= _numYBins)
                            {
                                break;
                            }

                            //Assumes linear piecewise transform to calculate the estimated y
                            var yEstimated = (yFrom + (yTo - yFrom) * xFraction) * yIntervalSize + _minY;
                            var yDelta = point.MassError - yEstimated;

                            //Make sure the point is in the linear range to effect the score
                            if (Math.Abs(yDelta) > yTolerance)
                            {
                                continue;
                            }

                            var matchScore = (yDelta * yDelta) / (_stdY[xSection] * _stdY[xSection]);
                            var jump = yTo - yFrom + _numJumps;
                            var sectionIndex = xSection * _numSectionMatches + yFrom * (2 * _numJumps + 1) + jump;
                            var currentMatchScore = _matchScores[sectionIndex];
                            _matchScores[sectionIndex] = currentMatchScore - sectionMismatchScore + matchScore;
                        }
                    }
                }
            }
        }

        private void CalculateAlignmentMatrix()
        {
            _bestPreviousIndex.Clear();
            _alignmentScores.Clear();
            _alignmentScores.Capacity = (_numXBins + 1) * _numYBins;
            _bestPreviousIndex.Capacity = (_numXBins + 1) * _numYBins;

            for (var ySection = 0; ySection < _numYBins; ySection++)
            {
                _bestPreviousIndex.Add(-2);
                _alignmentScores.Add(0);
            }

            for (var xSection = 1; xSection <= _numXBins; xSection++)
            {
                for (var ySection = 0; ySection < _numYBins; ySection++)
                {
                    _bestPreviousIndex.Add(-1);
                    _alignmentScores.Add(double.MaxValue);
                }
            }

            for (var xSection = 1; xSection <= _numXBins; xSection++)
            {
                for (var ySection = 0; ySection < _numYBins; ySection++)
                {
                    var index = xSection * _numYBins + ySection;
                    var bestAlignmentScore = double.MaxValue;

                    for (var jump = _numJumps; jump < 2 * _numJumps + 1; jump++)
                    {
                        var ySectionFrom = ySection - jump + _numJumps;
                        if (ySectionFrom < 0)
                        {
                            break;
                        }
                        var previousAlignmentIndex = (xSection - 1) * _numYBins + ySectionFrom;
                        var previousMatchIndex = (xSection - 1) * _numSectionMatches +
                                                 ySectionFrom * (2 * _numJumps + 1) + jump;
                        var previousAlignmentScore = _alignmentScores[previousAlignmentIndex];
                        var previousMatchScore = _matchScores[previousMatchIndex];
                        if (previousAlignmentScore + previousMatchScore < bestAlignmentScore)
                        {
                            bestAlignmentScore = previousMatchScore + previousAlignmentScore;
                            _bestPreviousIndex[index] = previousAlignmentIndex;
                            _alignmentScores[index] = bestAlignmentScore;
                        }
                    }

                    for (var jump = 0; jump < _numJumps; jump++)
                    {
                        var ySectionFrom = ySection - jump + _numJumps;
                        if (ySectionFrom < 0)
                        {
                            break;
                        }
                        var previousAlignmentIndex = (xSection - 1) * _numYBins + ySectionFrom;
                        var previousMatchIndex = (xSection - 1) * _numSectionMatches +
                                                 ySectionFrom * (2 * _numJumps + 1) + jump;
                        if ((previousAlignmentIndex > _alignmentScores.Count - 1) ||
                            (previousMatchIndex > _matchScores.Count - 1))
                        {
                            break;
                        }
                        var previousAlignmentScore = _alignmentScores[previousAlignmentIndex];
                        var previousMatchScore = _matchScores[previousMatchIndex];
                        if (previousAlignmentScore + previousMatchScore < bestAlignmentScore)
                        {
                            bestAlignmentScore = previousMatchScore + previousAlignmentScore;
                            _bestPreviousIndex[index] = previousAlignmentIndex;
                            _alignmentScores[index] = bestAlignmentScore;
                        }
                    }
                }
            }
        }

        private void CalculateRegressionFunction()
        {
            _alignmentFunction.Clear();
            //Start at the last section best score and trace backwards
            var bestScore = double.MaxValue;
            var bestPreviousIndex = 0;
            var bestYShift = _numYBins / 2;
            var xSection = _numXBins;

            while (xSection >= 1)
            {
                if (_count[xSection - 1] >= _minSectionPts)
                {
                    for (var ySection = 0; ySection < _numYBins; ySection++)
                    {
                        var index = xSection * _numYBins + ySection;
                        var ascore = _alignmentScores[index];
                        if (ascore < bestScore)
                        {
                            bestScore = ascore;
                            bestYShift = ySection;
                            bestPreviousIndex = _bestPreviousIndex[index];
                        }
                    }
                    break;
                }
                xSection--;
            }

            for (var i = xSection; i <= _numXBins; i++)
            {
                _alignmentFunction.Add(i, bestYShift);
            }
            while (xSection > 0)
            {
                xSection--;
                var yShift = bestPreviousIndex % _numYBins;
                _alignmentFunction.Add(xSection, yShift);
                bestPreviousIndex = _bestPreviousIndex[bestPreviousIndex];
            }
        }

        /// <summary>
        /// Calculates Central regression for the matches found and passed in
        /// </summary>
        /// <param name="calibMatches"></param>
        public void CalculateRegressionFunction(List<RegressionPoint> calibMatches)
        {
            Clear();
            foreach (var point in calibMatches)
            {
                _pts.Add(point);
            }

            // First find the boundaries
            CalculateMinMax();

            // For if it's constant answer
            if (Math.Abs(_minY - _maxY) < double.Epsilon)
            {
                for (var xSection = 0; xSection < _numXBins; xSection++)
                {
                    _alignmentFunction.Add(xSection, 0);
                }
                return;
            }

            CalculateSectionsStd();
            CalculateScoreMatrix();
            CalculateAlignmentMatrix();
            CalculateRegressionFunction();
        }

        /// <summary>
        /// Goes through all the regression points and only retains the ones that are within the outlier z score
        /// </summary>
        public void RemoveRegressionOutliers()
        {
            var xIntervalSize = (_maxX - _minX) / _numXBins;
            var tempPts = new List<RegressionPoint>();
            var numPts = _pts.Count;

            for (var pointNum = 0; pointNum < numPts; pointNum++)
            {
                var point = _pts[pointNum];
                var intervalNum = Convert.ToInt32((point.X - _minX) / xIntervalSize);
                if (intervalNum == _numXBins)
                {
                    intervalNum = _numXBins - 1;
                }
                var stdY = _stdY[intervalNum];
                var val = GetPredictedValue(point.X);
                var delta = (val - point.MassError) / stdY;
                if (Math.Abs(delta) < _outlierZScore)
                {
                    tempPts.Add(point);
                }
            }

            _pts.Clear();

            foreach (var point in tempPts)
            {
                _pts.Add(point);
            }
        }

        /// <summary>
        /// Given a value x, finds the appropriate y value that would correspond to it in the regression function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetPredictedValue(double x)
        {
            var yIntervalSize = (_maxY - _minY) / _numYBins;
            var xIntervalSize = (_maxX - _minX) / _numXBins;

            var xSection = Convert.ToInt32((x - _minX) / xIntervalSize);
            int ySectionFrom;
            if (xSection >= _numXBins)
            {
                xSection = _numXBins - 1;
            }
            if (xSection < 0)
            {
                xSection = 0;
                ySectionFrom = _alignmentFunction.ElementAt(xSection).Value;
                return _minY + ySectionFrom * yIntervalSize;
            }

            var xFraction = (x - _minX) / xIntervalSize - xSection;
            ySectionFrom = _alignmentFunction.ElementAt(xSection).Value;
            var ySectionTo = ySectionFrom;

            if (xSection < _numXBins - 1)
            {
                ySectionTo = _alignmentFunction.ElementAt(xSection + 1).Value;
            }

            var yPred = xFraction * yIntervalSize * (ySectionTo - ySectionFrom)
                        + ySectionFrom * yIntervalSize + _minY;

            return yPred;
        }
    }
}