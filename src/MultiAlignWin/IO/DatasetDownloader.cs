using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using PNNLProteomics.Data;

namespace MultiAlignWin.IO
{

    /// <summary>
    /// Downloads files from a given path.
    /// </summary>
    public class DatasetDownloader
    {
        /// <summary>
        /// Fired when a file is copied.
        /// </summary>
        public event EventHandler<FileCopyEventArgs> FileCopied;
        /// <summary>
        /// Fired when copying is finished.
        /// </summary>
        public event EventHandler CopyingComplete;

        /// <summary>
        /// Delegate definition so we can pass a list of files to the thread to copy them for us.
        /// </summary>
        /// <param name="sourceLocations"></param>
        private delegate void DelegatedCopyFilesThreaded(List<string> paths, string outputPath);
        /// <summary>
        /// Flag indicating whether files are finished copying.
        /// </summary>
        private volatile bool m_shouldCopyFiles;
        /// <summary>
        /// Copying thread.
        /// </summary>
        private Thread m_copyThread;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DatasetDownloader()
        {
            m_shouldCopyFiles   = true;
        }

        #region Copying of Data Files
        /// <summary>
        /// Threaded copy function.
        /// </summary>
        /// <param name="o"></param>
        private void CopyFiles(object o)
        {
            CopyThreadParameters parameters = o as CopyThreadParameters;
            if (parameters == null)
                throw new InvalidCastException("The parameters are not correct.");

            List<DatasetInformation> information = parameters.Paths;
            string destinationPath               = parameters.DestinationPath;

            foreach(DatasetInformation info in information)
            {
                string dataFilePath = info.ArchivePath;
                string fileName = Path.GetFileName(dataFilePath);
                string newPath  = Path.Combine(destinationPath, fileName); 
                bool shouldCopy = false;
                // Only copy if the file does not exist, or the file sizes 
                // of the destination and source are not the same.
                if (!File.Exists(newPath))
                {
                    shouldCopy = true;
                }
                else
                {
                    FileInfo sourceFile      = new FileInfo(dataFilePath);
                    FileInfo destinationFile = new FileInfo(newPath);
                    if (sourceFile.Length != destinationFile.Length)
                    {
                        shouldCopy = true;
                    }                    
                }

                if (shouldCopy)
                {                    
                    File.Copy(dataFilePath, newPath, true);
                    if (FileCopied != null)
                    {
                        FileCopied(this, new FileCopyEventArgs(dataFilePath, newPath));
                    }
                }

                info.mstrLocalPath = newPath;

                // Check  to see if the user has told us to stop copying data files.
                if (!m_shouldCopyFiles)
                    break;
            }

            if (CopyingComplete != null)
                CopyingComplete(this, null);
        }
        /// <summary>
        /// Copies all of the files from the given list of file paths to the directory provided.
        /// </summary>
        /// <param name="filePaths">Data file paths to copy.</param>
        /// <param name="directoryPath">Path to copy data files to.</param>
        /// <returns></returns>
        public void CopyFiles(List<DatasetInformation> filePaths, string destinationPath)
        {
            if (filePaths.Count <= 0)
                throw new Exception("No file paths were provided to copy.");
            
            if (!Directory.Exists(destinationPath))
                throw new DirectoryNotFoundException("The directory" + destinationPath + " does not exist.");

            m_shouldCopyFiles    = true;
            
            CopyThreadParameters parameters = new CopyThreadParameters();
            parameters.DestinationPath      = destinationPath;
            parameters.Paths                = filePaths;
            
            ParameterizedThreadStart start = new ParameterizedThreadStart(CopyFiles);
            m_copyThread = new Thread(start);            
            m_copyThread.Start(parameters);             
        }
        /// <summary>
        /// Aborts any copying operations.
        /// </summary>
        public void Abort()
        {
            if (m_copyThread == null)
                return;

            if (!m_copyThread.IsAlive)
                return;

            try
            {                
                m_shouldCopyFiles = false;
                Thread.Sleep(100);
                m_copyThread.Abort();
            }
            catch (ThreadAbortException)
            {
                // Do Nothing! 
            }
        }
        /// <summary>
        /// Holds information on where to copy data to.
        /// </summary>
        internal class CopyThreadParameters
        {
            public List<DatasetInformation> Paths;
            public string DestinationPath;
        }
        #endregion 	
    }
}
