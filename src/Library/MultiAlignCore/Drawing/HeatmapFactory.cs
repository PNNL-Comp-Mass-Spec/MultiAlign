#region

using System;

#endregion

namespace MultiAlignCore.Drawing
{
    public static class HeatmapFactory
    {
        /// <summary>
        /// Creates an LCMSWarp alignment heatmap.
        /// </summary>
        /// <param name="scores"></param>
        /// <returns></returns>
        public static PlotBase CreateAlignedHeatmap(float[,] scores, bool baselineIsAmtDB)
        {
            int x = scores.GetLength(0);
            int y = scores.GetLength(1);

            var doubleScores = new double[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    doubleScores[i, j] = Convert.ToDouble(scores[i, j]);
                }
            }
            return CreateAlignedHeatmap(doubleScores, baselineIsAmtDB);
        }

        /// <summary>
        /// Creates an LCMSWarp alignment heatmap.
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="baselineIsAmtDB">True if the baseline data is from an AMT tag database</param>
        /// <returns></returns>
        public static PlotBase CreateAlignedHeatmap(double[,] scores, bool baselineIsAmtDB)
        {
            string yAxisLabel;
            if (baselineIsAmtDB)
            {
                yAxisLabel = "AMT tag NET";
            }
            else
            {
                yAxisLabel = "Baseline NET";
            }

            return new Heatmap("Alignment Heatmap", "Alignee NET", yAxisLabel, scores);

        }
    }
}