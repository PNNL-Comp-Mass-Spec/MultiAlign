#region

using System.Windows;
using OxyPlot;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Drawing
{
    public interface IPlotModelEncoder<T>
    {
        [UsedImplicitly]
        T CreateImage(PlotModel model);

        /// <summary>
        ///     Saves the model data to the path provided assuming format T.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="path"></param>
        void SaveImage(PlotModel model, string path);

        /// <summary>
        ///     Saves the model data to the path provided assuming T format, with the size provided.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="path"></param>
        /// <param name="size"></param>
        void SaveImage(PlotModel model, string path, Size size);
    }
}