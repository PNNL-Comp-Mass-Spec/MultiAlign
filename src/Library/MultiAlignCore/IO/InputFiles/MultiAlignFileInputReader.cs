#region

using System;
using System.IO;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.IO.InputFiles
{
    /// <summary>
    ///     Reads a multi-align input file.
    /// </summary>
    public class MultiAlignFileInputReader
    {
        #region Input file header constants

        /// <summary>
        ///     Indicates the start of the file input specifications.
        /// </summary>
        private const string FILE_HEADER = "[files]";

        /// <summary>
        ///     Indicates the start of the database specification header.
        /// </summary>
        private const string DATABASE_HEADER = "[database]";

        /// <summary>
        ///     Indicates the start of the scans files.
        /// </summary>
        private const string SCANS_HEADER = "[scans]";

        /// <summary>
        ///     Header for sequence related data.
        /// </summary>
        private const string SEQUENCE_HEADER = "[sequence]";

        /// <summary>
        ///     Header for peaks files.
        /// </summary>
        private const string PEAKS_HEADER = "[peaks]";

        /// <summary>
        ///     Indicates the start of the raw files.
        /// </summary>
        private const string RAW_HEADER = "[raw]";

        #endregion

        /// <summary>
        ///     Indicates a baseline dataset.
        /// </summary>
        private const char BASELINE_INDICATOR = '*';


        /// <summary>
        ///     Reads the input paths file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static InputAnalysisInfo ReadInputFile(string path)
        {
            var info = new InputAnalysisInfo();
            var lines = File.ReadAllLines(path);
            var readType = FileReadMode.None;

            foreach (var line in lines)
            {
                var fixedLine = line.ToLower();
                fixedLine = fixedLine.Trim();
                var containsHeaderOpen = fixedLine.Contains("[");
                var containsHeaderClose = fixedLine.Contains("]");

                if (containsHeaderClose && containsHeaderOpen)
                {
                    fixedLine = fixedLine.Replace(" ", "");
                }

                if (string.IsNullOrEmpty(fixedLine) || string.IsNullOrWhiteSpace(fixedLine))
                {
                    continue;
                }

                // If wasModeChanged = true, then the current 
                // line is not data, but a tag to say change how read the next section.
                var wasModeChanged = false;
                switch (fixedLine)
                {
                    case FILE_HEADER:
                        wasModeChanged = true;
                        readType = FileReadMode.Files;
                        break;
                    case DATABASE_HEADER:
                        wasModeChanged = true;
                        readType = FileReadMode.Database;
                        break;
                    case RAW_HEADER:
                        wasModeChanged = true;
                        readType = FileReadMode.RawFiles;
                        break;
                    case SCANS_HEADER:
                        wasModeChanged = true;
                        readType = FileReadMode.ScanFiles;
                        break;
                    case PEAKS_HEADER:
                        wasModeChanged = true;
                        readType = FileReadMode.Peaks;
                        break;
                    case SEQUENCE_HEADER:
                        wasModeChanged = true;
                        readType = FileReadMode.Sequence;
                        break;
                    default:
                        if (containsHeaderClose && containsHeaderOpen)
                        {
                            wasModeChanged = true;
                            readType = FileReadMode.Unknown;
                            throw new Exception(fixedLine + " is not a recognized data tag.");
                        }
                        wasModeChanged = false;
                        break;
                }

                if (!wasModeChanged)
                {
                    switch (readType)
                    {
                        case FileReadMode.Files:
                            var baselineCheck = fixedLine.Split(BASELINE_INDICATOR);
                            if (baselineCheck.Length == 2 && !string.IsNullOrEmpty(baselineCheck[0]))
                            {
                                var newFile = new InputFile();
                                newFile.Path = baselineCheck[0];
                                newFile.FileType = InputFileType.Features;
                                info.BaselineFile = newFile;
                                info.Files.Add(newFile);
                            }
                            else if (!string.IsNullOrEmpty(baselineCheck[0]))
                            {
                                var newFile = new InputFile();
                                newFile.Path = baselineCheck[0];
                                newFile.FileType = InputFileType.Features;
                                info.Files.Add(newFile);
                            }
                            break;
                        case FileReadMode.Database:
                            var keys = fixedLine.Split('=');
                            if (keys.Length > 1)
                            {
                                keys[0] = keys[0].Replace('\t', ' ').Trim();

                                switch (keys[0].ToLower())
                                {
                                    case "database":
                                        info.Database.DatabaseFormat = MassTagDatabaseFormat.MassTagSystemSql;
                                        info.Database.DatabaseName = keys[1];
                                        break;
                                    case "server":
                                        info.Database.DatabaseFormat = MassTagDatabaseFormat.MassTagSystemSql;
                                        info.Database.DatabaseServer = keys[1];
                                        break;
                                    case "sqlite":
                                        info.Database.DatabaseFormat = MassTagDatabaseFormat.Sqlite;
                                        info.Database.LocalPath = keys[1];
                                        break;
                                    case "metasample":
                                        info.Database.DatabaseFormat = MassTagDatabaseFormat.DelimitedTextFile;
                                        info.Database.LocalPath = keys[1];
                                        break;
                                    case "ims":
                                        info.Database.DatabaseFormat = MassTagDatabaseFormat.SkipAlignment;
                                        info.Database.LocalPath = keys[1];
                                        break;
                                }
                            }
                            break;
                        case FileReadMode.RawFiles:
                            var rawFile = new InputFile();
                            rawFile.Path = fixedLine;
                            rawFile.FileType = InputFileType.Raw;
                            info.Files.Add(rawFile);
                            break;
                        case FileReadMode.ScanFiles:
                            var scanFile = new InputFile();
                            scanFile.Path = fixedLine;
                            scanFile.FileType = InputFileType.Scans;
                            info.Files.Add(scanFile);
                            break;
                        case FileReadMode.Unknown:
                            // do nothing!
                            break;
                        case FileReadMode.Sequence:
                            var sequenceFile = new InputFile();
                            sequenceFile.Path = fixedLine;
                            sequenceFile.FileType = InputFileType.Sequence;
                            info.Files.Add(sequenceFile);
                            break;
                    }
                }
            }

            return info;
        }
    }
}