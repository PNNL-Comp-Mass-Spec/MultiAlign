using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data.MetaData;

namespace MultiAlign.Data
{
    public class DatasetPlotLoader
    {
        /// <summary>
        ///     Loads dataset information from the path provided.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DatasetPlotInformation LoadDatasetPlots(string[] files, DatasetInformation info)
        {
            var plotInfo = new DatasetPlotInformation();

            var fileList = new List<string>();
            fileList.AddRange(files);

            var datasetFiles = new List<string>();
            var name = info.DatasetName.ToLower();

            foreach (var filename in fileList)
            {
                var file = filename.ToLower();
                if (file.Contains(name))
                {
                    if (file.Contains("_features"))
                    {
                        plotInfo.Features = file;
                    }
                    else if (file.Contains("_heatmap"))
                    {
                        plotInfo.Alignment = file;
                    }
                    else if (file.Contains("_masshistogram"))
                    {
                        plotInfo.MassErrorHistogram = file;
                    }
                    else if (file.Contains("_nethistogram"))
                    {
                        plotInfo.NetErrorHistogram = file;
                    }
                    else if (file.Contains("_massmzresidual"))
                    {
                        plotInfo.MassMzResidual = file;
                    }
                    else if (file.Contains("_massscanresidual"))
                    {
                        plotInfo.MassScanResidual = file;
                    }
                    else if (file.Contains("_netresidual"))
                    {
                        plotInfo.NetResiduals = file;
                    }
                }
            }
            info.PlotData = plotInfo;
            return plotInfo;
        }

        public void LoadDatasetPlots(string path, List<DatasetInformation> datasets)
        {
            var files = Directory.GetFiles(path);
            datasets.ForEach(x => LoadDatasetPlots(files, x));
        }
    }
}