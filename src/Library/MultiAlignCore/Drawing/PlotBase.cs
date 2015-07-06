#region

using OxyPlot;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Drawing
{
    public abstract class PlotBase
    {
        /// <summary>
        ///     Empty constructor that does not uniquely name the plot.
        /// </summary>
        protected PlotBase() : this("")
        {
        }

        /// <summary>
        ///     Constructor that names the plot.
        /// </summary>
        /// <param name="name"></param>
        protected PlotBase(string name)
        {
            Model = new PlotModel
            {
                Background = OxyColors.White,
            };
            Name = name;
        }

        /// <summary>
        ///     Gets or sets the underlying model of the plot.
        /// </summary>
        [UsedImplicitly]
        public PlotModel Model { get; protected set; }

        /// <summary>
        ///     Gets or sets the name of the plot
        /// </summary>
        public string Name { get; set; }
    }
}