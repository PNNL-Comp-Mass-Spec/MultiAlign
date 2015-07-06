#region

using System;

#endregion

namespace MultiAlignCore.Drawing
{
    public static class HeatmapFactory
    {
        /// <summary>
        ///     Creates an LCMSWarp alignment heatmap.
        /// </summary>
        /// <param name="scores"></param>
        /// <returns></returns>
        public static PlotBase CreateAlignedHeatmap(float[,] scores)
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
            return CreateAlignedHeatmap(doubleScores);
        }

        /// <summary>
        ///     Creates an LCMSWarp alignment heatmap.
        /// </summary>
        /// <param name="scores"></param>
        /// <returns></returns>
        public static PlotBase CreateAlignedHeatmap(double[,] scores)
        {
            return new Heatmap("Alignment Heatmap", "BaselineNET", "Alignee NET", scores);
        }
    }
}