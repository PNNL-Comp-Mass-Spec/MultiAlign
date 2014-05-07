using MultiAlignCore.IO.InputFiles;
using System.Collections.Generic;

namespace MultiAlign.Data
{
    /// <summary>
    /// Creates a file browser filter based on the input file types.
    /// </summary>
    public class DatasetFilterFactory
    {
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
                    fileFilter = AppendExtension(fileFilter, "MSGF+ TSV", "_msgf.tsv");
                    break;
                case InputFileType.Features:
                    fileFilter = AppendExtension(fileFilter, "DeconTools Features Files", "_isos.csv");
                    fileFilter = AppendExtension(fileFilter, "LCMS Feature Finder Files", "LCMSFeatures.txt");
                    break;
                case InputFileType.Raw:
                    fileFilter = AppendExtension(fileFilter, "Thermo Raw",  ".raw");
                    fileFilter = AppendExtension(fileFilter, "MZ XML",      ".mzxml");
                    break;                
            }

            return fileFilter;
        }
    }    
}
