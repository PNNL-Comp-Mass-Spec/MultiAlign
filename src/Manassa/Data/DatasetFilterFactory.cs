using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlign.Data
{
    /// <summary>
    /// Creates a file browser filter based on the input file types.
    /// </summary>
    public class DatasetFilterFactory
    {
        private static Dictionary<string, InputFileType> fileTypes = new Dictionary<string, InputFileType>();
        DatasetFilterFactory()
        {

        }

        private static string AppendExtension(string baseName, string extension, string description)
        {
            return string.Format("{0}|{1}", CreateFilterName(extension, description), baseName);
        }
        private static string CreateFilterName(string description, string extension)
        {
            return string.Format("{0} ({1})|*{1}", description, extension);
        }

        public static string BuildFileFilters(InputFileType type)
        {
            string fileFilter = "All Files (*.*)|*.*";

            switch (type)
            {
                case InputFileType.Sequence:
                    fileFilter = AppendExtension(fileFilter, "SEQUEST Synopsis", ".syn");
                    fileFilter = AppendExtension(fileFilter, "MSGF+ FHT", "fht_msgf.txt");
                    break;
                case InputFileType.Features:
                    fileFilter = AppendExtension(fileFilter, "DeconTools Features Files", "_isos.csv");
                    fileFilter = AppendExtension(fileFilter, "LCMS Feature Finder Files", "LCMSFeatures.txt");
                    break;
                case InputFileType.Peaks:
                    fileFilter = AppendExtension(fileFilter, "DeconTools Peaks Files", "peaks.txt");                    
                    break;
                case InputFileType.Raw:
                    fileFilter = AppendExtension(fileFilter, "Thermo Raw",  ".raw");
                    fileFilter = AppendExtension(fileFilter, "MZ XML",      ".mzxml");
                    break;
                default:
                    break;
            }

            return fileFilter;
        }
    }    
}
