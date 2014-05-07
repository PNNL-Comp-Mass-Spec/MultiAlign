using System;

namespace PNNLProteomics.IO
{

    /// <summary>
    /// Event Arguments for file copy operations.
    /// </summary>
    public class FileCopyEventArgs : EventArgs
    {
        /// <summary>
        /// Source path for the file.
        /// </summary>
        string m_sourceFilePath;
        /// <summary>
        /// Destination path for the file.
        /// </summary>
        string m_destinationFilePath;

        /// <summary>
        /// Creates a new event argument storing the source and destination file paths.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public FileCopyEventArgs(string sourcePath, string destinationPath)
        {
            m_sourceFilePath = sourcePath;
            m_destinationFilePath = destinationPath;
        }
        /// <summary>
        /// Gets the path from where the file was copied.
        /// </summary>
        public string SourcePath
        {
            get
            {
                return m_sourceFilePath;
            }
            set
            {
                m_sourceFilePath = value;
            }
        }
        /// <summary>
        /// Gets the path to where the file was copied.
        /// </summary>
        public string DestinationPath
        {
            get
            {
                return m_destinationFilePath;
            }
        }
    }
}
