namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MultiAlignCore.Algorithms.Regression;
    using MultiAlignCore.Data;

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
            this._numXBins = 100;
            this._numYBins = 100;
            this._numJumps = 30;
            this._tolerance = 0.8; // 5 standard devs
            this._outlierZScore = this._tolerance;
            this._minSectionPts = 5;
            this._matchScores = new List<double>();
            this._sectionMisMatchScore = new List<double>();
            this._sectionTolerance = new List<double>();
            this._alignmentScores = new List<double>();
            this._bestPreviousIndex = new List<int>();
            this._count = new List<int>();

            this._normUnifEm = new NormalUniformEm();

            this._stdY = new List<double>();
            this._alignmentFunction = new Dictionary<int, int>();

            this._pts = new List<RegressionPoint>();

            this.SetOptions(this._numXBins, this._numYBins, this._numJumps, this._outlierZScore);
        }

        /// <summary>
        /// Getter to return the list of regression points
        /// </summary>
        /// <returns></returns>
        public List<RegressionPoint> Points
        {
            get { return this._pts; }
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
            this._numXBins = numXBins;
            this._numYBins = numYBins;
            this._numJumps = numJumps;
            this._tolerance = zTolerance;
            this._outlierZScore = zTolerance;

            this._numSectionMatches = this._numYBins * (2 * numJumps + 1);

            this._matchScores.Clear();
            this._alignmentScores.Clear();
            this._bestPreviousIndex.Clear();
        }

        /// <summary>
        /// Method to set the outlier score for Central regression.
        /// </summary>
        /// <param name="outlierZScore"></param>
        public void SetOutlierZScore(double outlierZScore)
        {
            this._outlierZScore = outlierZScore;
        }

        /// <summary>
        /// Finds the min and max mass error and Net
        /// </summary>
        public void CalculateMinMax()
        {
            var numPts = this._pts.Count();

            this._minX = double.MaxValue;
            this._maxX = double.MinValue;

            this._minY = double.MaxValue;
            this._maxY = double.MinValue;

            for (var i = 0; i < numPts; i++)
            {
                var point = this._pts[i];
                this._minX = Math.Min(this._minX, point.X);
                this._maxX = Math.Max(this._maxX, point.X);
                this._minY = Math.Min(this._minY, point.Y);
                this._maxY = Math.Max(this._maxY, point.Y);
            }
        }

        /// <summary>
        /// Computers the Sections standard deviation and number in each match
        /// </summary>
        /// <param name="intervalNum"></param>
        private void CalculateSectionStdAndCount(int intervalNum)
        {
            var points = new List<double>();

            var numPoints = this._pts.Count();
            var xInterval = (this._maxX - this._minX) / this._numXBins;

            for (var ptNum = 0; ptNum < numPoints; ptNum++)
            {
                var pt = this._pts[ptNum];
                var sectionNum = Convert.ToInt32((pt.X - this._minX) / xInterval);
                if (sectionNum == this._numXBins)
                {
                    sectionNum = this._numXBins - 1;
                }
                if (intervalNum == sectionNum)
                {
                    this._count[sectionNum]++;
                    points.Add(pt.MassError);
                }
            }

            if (this._count[intervalNum] > this._minSectionPts)
            {
                this._normUnifEm.CalculateDistributions(points);
                this._stdY[intervalNum] = this._normUnifEm.StandDev;
                if (Math.Abs(this._stdY[intervalNum]) > double.Epsilon)
                {
                    var tolerance = this._stdY[intervalNum] * this._tolerance;
                    this._sectionTolerance[intervalNum] = tolerance;

                    var misMatchScore = (tolerance * tolerance) / (this._stdY[intervalNum] * this._stdY[intervalNum]);
                    this._sectionMisMatchScore[intervalNum] = misMatchScore;
                }
                else
                {
                    this._sectionMisMatchScore[intervalNum] = this._tolerance * this._tolerance;
                    this._sectionTolerance[intervalNum] = 0;
                }
            }
            else
            {
                this._stdY[intervalNum] = 0.1;
            }
        }

        /// <summary>
        /// Calculate standard deviations for all sections
        /// </summary>
        public void CalculateSectionsStd()
        {
            this._count.Capacity = this._numXBins;
            this._stdY.Capacity = this._numXBins;
            this._stdY.Clear();

            for (var interval = 0; interval < this._numXBins; interval++)
            {
                this._stdY.Add(0);
                this._count.Add(0);
                this._sectionMisMatchScore.Add(0);
                this._sectionTolerance.Add(0);
                this.CalculateSectionStdAndCount(interval);
            }
        }

        /// <summary>
        /// Removes all the residual data from prior regressions
        /// </summary>
        public void Clear()
        {
            this._matchScores.Clear();
            this._alignmentFunction.Clear();
            this._alignmentScores.Clear();
            this._bestPreviousIndex.Clear();
            this._count.Clear();
            this._pts.Clear();
            this._stdY.Clear();
            this._sectionMisMatchScore.Clear();
            this._sectionTolerance.Clear();
        }

        private void SetUnmatchedScoreMatrix()
        {
            // Assigns each section's score to the minimum for that section
            // for each possible matching sections, the minimum score would correspond
            // to the situation that all points in the section lie outside the tolerance
            this._matchScores.Clear();

            // At the moment, assumes that the tolerance is in zscore units
            for (var xSection = 0; xSection < this._numXBins; xSection++)
            {
                var sectionMismatchScore = this._sectionMisMatchScore[xSection] * this._count[xSection];

                for (var sectionMatchNum = 0; sectionMatchNum < this._numSectionMatches; sectionMatchNum++)
                {
                    this._matchScores.Add(sectionMismatchScore);
                }
            }
        }

        private void CalculateScoreMatrix()
        {
            this._matchScores.Capacity = this._numXBins * this._numSectionMatches;

            // Neww to calculate the score matrix for all possible score blocks.
            // For every x section, all y secments between y_interest - m_num_jumps
            // to y_interest + m_num_jumps are scored.

            // First set the unmatched score.
            this.SetUnmatchedScoreMatrix();

            var yIntervalSize = (this._maxY - this._minY) / this._numYBins;
            var xIntervalSize = (this._maxX - this._minX) / this._numXBins;

            // For each point that is seen, add the supporting score to the appropriate section.
            var numPts = this._pts.Count();
            if ((yIntervalSize > 0.000001) && (xIntervalSize > 0.000001))
            {
                for (var pointNum = 0; pointNum < numPts; pointNum++)
                {
                    var point = this._pts[pointNum];
                    var xSection = Convert.ToInt32((point.X - this._minX) / xIntervalSize);
                    if (xSection == this._numXBins)
                    {
                        xSection = this._numXBins - 1;
                    }

                    // If the point belongs to a section where the num # of points is not met, ignore it
                    if (this._count[xSection] < this._minSectionPts || Math.Abs(this._stdY[xSection]) < double.Epsilon)
                    {
                        continue;
                    }

                    var yTolerance = this._sectionTolerance[xSection];

                    var yInterval = Convert.ToInt32((0.0001 + (point.MassError - this._minY) / yIntervalSize));

                    if (yInterval == this._numYBins)
                    {
                        yInterval = this._numYBins - 1;
                    }

                    // Matches to the section that the point would contribute to.
                    var minYStart = Convert.ToInt32(yInterval - yTolerance / yIntervalSize);
                    var maxYStart = Convert.ToInt32(yInterval + yTolerance / yIntervalSize);

                    var sectionMismatchScore = this._sectionMisMatchScore[xSection];

                    var xFraction = (point.X - this._minX) / xIntervalSize - xSection;

                    for (var yFrom = minYStart; yFrom <= maxYStart; yFrom++)
                    {
                        if (yFrom < 0)
                        {
                            continue;
                        }
                        if (yFrom >= this._numYBins)
                        {
                            break;
                        }
                        for (var yTo = yFrom - this._numJumps; yTo <= yFrom + this._numJumps; yTo++)
                        {
                            if (yTo < 0)
                            {
                                continue;
                            }
                            if (yTo >= this._numYBins)
                            {
                                break;
                            }

                            //Assumes linear piecewise transform to calculate the estimated y
                            var yEstimated = (yFrom + (yTo - yFrom) * xFraction) * yIntervalSize + this._minY;
                            var yDelta = point.MassError - yEstimated;

                            //Make sure the point is in the linear range to effect the score
                            if (Math.Abs(yDelta) > yTolerance)
                            {
                                continue;
                            }

                            var matchScore = (yDelta * yDelta) / (this._stdY[xSection] * this._stdY[xSection]);
                            var jump = yTo - yFrom + this._numJumps;
                            var sectionIndex = xSection * this._numSectionMatches + yFrom * (2 * this._numJumps + 1) + jump;
                            var currentMatchScore = this._matchScores[sectionIndex];
                            this._matchScores[sectionIndex] = currentMatchScore - sectionMismatchScore + matchScore;
                        }
                    }
                }
            }
        }

        private void CalculateAlignmentMatrix()
        {
            this._bestPreviousIndex.Clear();
            this._alignmentScores.Clear();
            this._alignmentScores.Capacity = (this._numXBins + 1) * this._numYBins;
            this._bestPreviousIndex.Capacity = (this._numXBins + 1) * this._numYBins;

            for (var ySection = 0; ySection < this._numYBins; ySection++)
            {
                this._bestPreviousIndex.Add(-2);
                this._alignmentScores.Add(0);
            }

            for (var xSection = 1; xSection <= this._numXBins; xSection++)
            {
                for (var ySection = 0; ySection < this._numYBins; ySection++)
                {
                    this._bestPreviousIndex.Add(-1);
                    this._alignmentScores.Add(double.MaxValue);
                }
            }

            for (var xSection = 1; xSection <= this._numXBins; xSection++)
            {
                for (var ySection = 0; ySection < this._numYBins; ySection++)
                {
                    var index = xSection * this._numYBins + ySection;
                    var bestAlignmentScore = double.MaxValue;

                    for (var jump = this._numJumps; jump < 2 * this._numJumps + 1; jump++)
                    {
                        var ySectionFrom = ySection - jump + this._numJumps;
                        if (ySectionFrom < 0)
                        {
                            break;
                        }
                        var previousAlignmentIndex = (xSection - 1) * this._numYBins + ySectionFrom;
                        var previousMatchIndex = (xSection - 1) * this._numSectionMatches +
                                                 ySectionFrom * (2 * this._numJumps + 1) + jump;
                        var previousAlignmentScore = this._alignmentScores[previousAlignmentIndex];
                        var previousMatchScore = this._matchScores[previousMatchIndex];
                        if (previousAlignmentScore + previousMatchScore < bestAlignmentScore)
                        {
                            bestAlignmentScore = previousMatchScore + previousAlignmentScore;
                            this._bestPreviousIndex[index] = previousAlignmentIndex;
                            this._alignmentScores[index] = bestAlignmentScore;
                        }
                    }

                    for (var jump = 0; jump < this._numJumps; jump++)
                    {
                        var ySectionFrom = ySection - jump + this._numJumps;
                        if (ySectionFrom < 0)
                        {
                            break;
                        }
                        var previousAlignmentIndex = (xSection - 1) * this._numYBins + ySectionFrom;
                        var previousMatchIndex = (xSection - 1) * this._numSectionMatches +
                                                 ySectionFrom * (2 * this._numJumps + 1) + jump;
                        if ((previousAlignmentIndex > this._alignmentScores.Count - 1) ||
                            (previousMatchIndex > this._matchScores.Count - 1))
                        {
                            break;
                        }
                        var previousAlignmentScore = this._alignmentScores[previousAlignmentIndex];
                        var previousMatchScore = this._matchScores[previousMatchIndex];
                        if (previousAlignmentScore + previousMatchScore < bestAlignmentScore)
                        {
                            bestAlignmentScore = previousMatchScore + previousAlignmentScore;
                            this._bestPreviousIndex[index] = previousAlignmentIndex;
                            this._alignmentScores[index] = bestAlignmentScore;
                        }
                    }
                }
            }
        }

        private void CalculateRegressionFunction()
        {
            this._alignmentFunction.Clear();
            //Start at the last section best score and trace backwards
            var bestScore = double.MaxValue;
            var bestPreviousIndex = 0;
            var bestYShift = this._numYBins / 2;
            var xSection = this._numXBins;

            while (xSection >= 1)
            {
                if (this._count[xSection - 1] >= this._minSectionPts)
                {
                    for (var ySection = 0; ySection < this._numYBins; ySection++)
                    {
                        var index = xSection * this._numYBins + ySection;
                        var ascore = this._alignmentScores[index];
                        if (ascore < bestScore)
                        {
                            bestScore = ascore;
                            bestYShift = ySection;
                            bestPreviousIndex = this._bestPreviousIndex[index];
                        }
                    }
                    break;
                }
                xSection--;
            }

            for (var i = xSection; i <= this._numXBins; i++)
            {
                this._alignmentFunction.Add(i, bestYShift);
            }
            while (xSection > 0)
            {
                xSection--;
                var yShift = bestPreviousIndex % this._numYBins;
                this._alignmentFunction.Add(xSection, yShift);
                bestPreviousIndex = this._bestPreviousIndex[bestPreviousIndex];
            }
        }

        /// <summary>
        /// Calculates Central regression for the matches found and passed in
        /// </summary>
        /// <param name="calibMatches"></param>
        public void CalculateRegressionFunction(List<RegressionPoint> calibMatches)
        {
            this.Clear();
            foreach (var point in calibMatches)
            {
                this._pts.Add(point);
            }

            // First find the boundaries
            this.CalculateMinMax();

            // For if it's constant answer
            if (Math.Abs(this._minY - this._maxY) < double.Epsilon)
            {
                for (var xSection = 0; xSection < this._numXBins; xSection++)
                {
                    this._alignmentFunction.Add(xSection, 0);
                }
                return;
            }

            this.CalculateSectionsStd();
            this.CalculateScoreMatrix();
            this.CalculateAlignmentMatrix();
            this.CalculateRegressionFunction();
        }

        /// <summary>
        /// Goes through all the regression points and only retains the ones that are within the outlier z score
        /// </summary>
        public void RemoveRegressionOutliers()
        {
            var xIntervalSize = (this._maxX - this._minX) / this._numXBins;
            var tempPts = new List<RegressionPoint>();
            var numPts = this._pts.Count;

            for (var pointNum = 0; pointNum < numPts; pointNum++)
            {
                var point = this._pts[pointNum];
                var intervalNum = Convert.ToInt32((point.X - this._minX) / xIntervalSize);
                if (intervalNum == this._numXBins)
                {
                    intervalNum = this._numXBins - 1;
                }
                var stdY = this._stdY[intervalNum];
                var val = this.GetPredictedValue(point.X);
                var delta = (val - point.MassError) / stdY;
                if (Math.Abs(delta) < this._outlierZScore)
                {
                    tempPts.Add(point);
                }
            }

            this._pts.Clear();

            foreach (var point in tempPts)
            {
                this._pts.Add(point);
            }
        }

        /// <summary>
        /// Given a value x, finds the appropriate y value that would correspond to it in the regression function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetPredictedValue(double x)
        {
            var yIntervalSize = (this._maxY - this._minY) / this._numYBins;
            var xIntervalSize = (this._maxX - this._minX) / this._numXBins;

            var xSection = Convert.ToInt32((x - this._minX) / xIntervalSize);
            int ySectionFrom;
            if (xSection >= this._numXBins)
            {
                xSection = this._numXBins - 1;
            }
            if (xSection < 0)
            {
                xSection = 0;
                ySectionFrom = this._alignmentFunction.ElementAt(xSection).Value;
                return this._minY + ySectionFrom * yIntervalSize;
            }

            var xFraction = (x - this._minX) / xIntervalSize - xSection;
            ySectionFrom = this._alignmentFunction.ElementAt(xSection).Value;
            var ySectionTo = ySectionFrom;

            if (xSection < this._numXBins - 1)
            {
                ySectionTo = this._alignmentFunction.ElementAt(xSection + 1).Value;
            }

            var yPred = xFraction * yIntervalSize * (ySectionTo - ySectionFrom)
                        + ySectionFrom * yIntervalSize + this._minY;

            return yPred;
        }
    }
}