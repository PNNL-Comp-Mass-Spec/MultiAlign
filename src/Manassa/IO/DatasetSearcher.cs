using System;
using MultiAlignCore.IO;
using MultiAlignCore.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlign.Data;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlign.IO
{
    public class DatasetSearcher
    {
        /// <summary>
        /// Finds a list of input files within the folder provided.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static List<InputFile> FindDatasets(string folderPath, List<string> extensions, SearchOption options)
        {            
            List<InputFile> datasetList = new List<InputFile>();
            List<string> candidates = new List<string>();

            foreach(string extension in extensions)
            {
                string[] paths = Directory.GetFiles(folderPath, "*" + extension, options);
                candidates.AddRange(paths);
            }

            
            foreach (string path in candidates)
            {

                InputFileType type = DatasetInformation.GetInputFileType(path);
                
                if (type == InputFileType.NotRecognized)
                {
                    continue;
                }
                if (type == InputFileType.Scans)
                {
                    continue;
                }
                
                InputFile file  = new InputFile();
                file.Path       = path;
                file.FileType   = type;

                datasetList.Add(file);
            }

            return datasetList;
        }
    }
}
