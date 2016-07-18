using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Regression
{
    public sealed class LinearRegressionModel: IRegressorAlgorithm<LinearRegressionResult>
    {
        public LinearRegressionResult CalculateRegression(IEnumerable<XYData> xyDataList)
        {
            var linearEquation = new LinearRegressionResult();

            //TODO: Calculate an R^2 value.
            double sumX = 0;
            double sumY = 0;
            double sumXTimesY = 0;
            double sumXSquared = 0;

            var dataList        = xyDataList as XYData[] ?? xyDataList.ToArray();
            double numPoints    = dataList.Count();

            foreach (var xyData in dataList)
            {
                var xValue = xyData.X;
                var yValue = xyData.Y;

                sumX += xValue;
                sumY += yValue;
                sumXTimesY += (xValue * yValue);
                sumXSquared += (xValue * xValue);
            }

            var slope = ((numPoints * sumXTimesY) - (sumX * sumY)) / ((numPoints * sumXSquared) - (sumX * sumX));
            var intercept = ((sumY * sumXSquared) - (sumX * sumXTimesY)) / ((numPoints * sumXSquared) - (sumX * sumX));

            linearEquation.Intercept = intercept;
            linearEquation.Slope     = slope;

            return linearEquation;
        }

        public LinearRegressionResult CalculateRegression(IEnumerable<double> observed, IEnumerable<double> predicted)
        {
            var observedList = observed.ToList();
            var predictedList = predicted.ToList();

            var data = observedList.Select((t, i) => new XYData(t, predictedList[i])).ToList();
            return CalculateRegression(data);
        }


        public double Transform(LinearRegressionResult regressionFunction, double observed)
        {
            return (regressionFunction.Slope * observed) + regressionFunction.Intercept;
        }
    }
}
