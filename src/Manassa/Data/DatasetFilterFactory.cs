﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MultiAlignCore.IO.InputFiles;

namespace Manassa.Data
{
    /// <summary>
    /// Creates a file browser filter based on the input file types.
    /// </summary>
    public class DatasetFilterFactory
    {
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
                    break;
                case InputFileType.Features:
                    fileFilter = AppendExtension(fileFilter, "DeconTools", "_isos.csv");
                    fileFilter = AppendExtension(fileFilter, "LCMS Feature Finder Files", "LCMSFeatures.txt");
                    break;
                case InputFileType.Raw:
                    fileFilter = AppendExtension(fileFilter, "Thermo Raw", ".raw");
                    fileFilter = AppendExtension(fileFilter, "MZ XML", ".mzxml");
                    break;
                default:
                    break;
            }

            return fileFilter;
        }
        /// <summary>
        /// Determines what kind of file this is.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static InputFileType DetermineInputFileType(string path)
        {
            string newPath = path.ToLower();
            if (newPath.EndsWith("_isos.csv"))
            {
                return InputFileType.Features;
            }
            if (newPath.EndsWith("lcmsfeatures.txt"))
            {
                return InputFileType.Features;
            }

            InputFileType type = InputFileType.NotRecognized;
            string extension = Path.GetExtension(newPath).ToLower();            
            switch (extension)
            {
                case ".raw":
                    type = InputFileType.Raw;
                    break;
                case ".mzxml":
                    type = InputFileType.Raw;
                    break;
                case ".syn":
                    type = InputFileType.Sequence;
                    break;
            }
            return type;            
        }
    }    
}
