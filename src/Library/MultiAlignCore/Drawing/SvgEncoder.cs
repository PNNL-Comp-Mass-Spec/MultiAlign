using System.Drawing;
using System.IO;
using OxyPlot;

namespace MultiAlignCore.Drawing
{
    public sealed class SvgEncoder: IPlotModelEncoder<string>
    {

        public string CreateImage(PlotModel model)
        {
            var exporter = new SvgExporter();
            return exporter.ExportToString(model);
        }

        public void SaveImage(PlotModel model, string path)
        {
            var exporter = new SvgExporter();
            var svgData  = exporter.ExportToString(model);
            File.WriteAllText(path, svgData);
        }

        public void SaveImage(PlotModel model, string path, Size size)
        {
            SaveImage(model, path);
        }
    }
}