using System.Collections.Generic;
using PNNLOmics.Data;
using System.IO;
using System;
using PNNLOmics.Algorithms.Alignment;
using ThermoRawFileReaderDLL.FinniganFileIO;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Adapter for the Thermo Finnigan file format reader made by Matt Monroe.
    /// </summary>
    public class ThermoRawDataFileReader: ISpectraProvider, IDisposable
    {
        /// <summary>
        /// Readers for each dataset.
        /// </summary>
        private Dictionary<int, XRawFileIO> m_readers;

        private Dictionary<int, string> m_dataFiles;
        private Dictionary<int, bool> m_opened;

        public ThermoRawDataFileReader()
        {
            m_dataFiles = new Dictionary<int, string>();
            m_readers   = new Dictionary<int,XRawFileIO>();
            m_opened    = new Dictionary<int,bool>();
        }
       
        /// <summary>
        /// Adds a dataset file to the reader map so when in use the application
        /// knows where to get raw data from.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupID"></param>
        public void AddDataFile(string path, int groupID)
        {
            if (m_opened.ContainsKey(groupID))
            {
                try
                {
                    if (m_readers.ContainsKey(groupID))
                    {
                        m_readers[groupID].CloseRawFile();
                    }
                }
                catch
                {
                }

                m_opened[groupID]    = false;
                m_dataFiles[groupID] = path;

                return;
            }
            m_dataFiles.Add(groupID, path);
            m_opened.Add(groupID, false);
        }        

        #region IRawDataFileReader Members

        /// <summary>
        /// Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <param name="excludeMap">Dictionary indicating which scans and related feature ID's to ignore.</param>
        /// <returns>List of MSMS spectra data</returns>        
        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap)
        {
            List<MSSpectra> spectra = new List<MSSpectra>();

            if (!m_readers.ContainsKey(group))
            {
                string path       = m_dataFiles[group];
                XRawFileIO reader = new XRawFileIO();
                m_readers.Add(group, reader);
                
            }

            XRawFileIO rawReader = m_readers[group];
            if (!m_opened[group])
            {
                bool opened = rawReader.OpenRawFile(m_dataFiles[group]);
                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }


            int numberOfScans = rawReader.GetNumScans(); 
            for (int i = 0; i < numberOfScans; i++)
            {
                // This scan is not to be used.
                bool isInExcludeMap = excludeMap.ContainsKey(i);
                if (isInExcludeMap)
                    continue;

                FinniganFileReaderBaseClass.udtScanHeaderInfoType header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
                rawReader.GetScanInfo(i, out header);
                
                if (header.MSLevel > 1)
                {
                    MSSpectra spectrum       = new MSSpectra();
                    spectrum.MSLevel         = header.MSLevel;
                    spectrum.RetentionTime   = header.RetentionTime;
                    spectrum.Scan            = i;
                    spectrum.PrecursorMZ     = header.ParentIonMZ;                    
                    spectrum.TotalIonCurrent = header.TotalIonCurrent;
                    spectrum.CollisionType   = CollisionType.Other;

                    switch(header.CollisionMode)
                    {
                        case "cid":
                            spectrum.CollisionType = CollisionType.CID;
                            break;
                        default:
                            break;
                    }

                    // Need to make this a standard type of collision based off of the data.
                    
                    spectra.Add(spectrum);
                }
            }
            rawReader.CloseRawFile();

            return spectra;
        }
        /// <summary>
        /// Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> GetMSMSSpectra(int group)
        {
            return GetMSMSSpectra(group, new Dictionary<int, int>());
        }
        #endregion

        #region IMSMSDataSource Members
        /// <summary>
        /// Gets the raw data from the data file.
        /// </summary>
        /// <param name="scan"></param>
        /// <returns></returns>        
        public List<XYData> GetRawSpectra(int scan, int group)
        {
            return GetRawSpectra(scan, group, -1);
        }
        
        /// <summary>
        /// Gets the raw data from the data file.
        /// </summary>
        /// <param name="scan"></param>
        /// <returns></returns>
        public List<XYData> GetRawSpectra(int scan, int group, int msLevel)
        {
            if (!m_dataFiles.ContainsKey(group))
            {
                throw new System.Exception("The group-dataset ID provided was not found.");
            }
            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.
            if (!m_readers.ContainsKey(group))
            {
                string path         = m_dataFiles[group];
                XRawFileIO reader   = new XRawFileIO();

                m_readers.Add(group, reader);
                bool opened         = reader.OpenRawFile(path);

                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }

            List<MSSpectra> spectra  = new List<MSSpectra>();
            XRawFileIO rawReader     = m_readers[group];
            
            FinniganFileReaderBaseClass.udtScanHeaderInfoType header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();

            rawReader.GetScanInfo(scan, out header);

            if (header.MSLevel != msLevel && msLevel != -1)
                return null;

            int N                   = header.NumPeaks;

            double[] mz             = new double[N];
            double[] intensities    = new double[N];            
            rawReader.GetScanData(scan, ref mz, ref intensities, ref header);
            
            //rawReader.CloseRawFile();
            // construct the array.
            List<XYData> data = new List<XYData>(mz.Length);            
            for (int i = 0; i < mz.Length; i++)
            {
                double intensity = intensities[i];
                data.Add(new XYData(mz[i], intensity));                
            }
            return data;
        }
        /// <summary>
        /// Determines if the scan is a precursor scan or not.
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool IsPrecursorScan(int scan, int group)
        {
            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.
            if (!m_readers.ContainsKey(group))
            {
                string path         = m_dataFiles[group];
                XRawFileIO reader   = new XRawFileIO();
                m_readers.Add(group, reader);

                bool opened = reader.OpenRawFile(path);
                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }
            
            XRawFileIO rawReader                                     = m_readers[group];
            FinniganFileReaderBaseClass.udtScanHeaderInfoType header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
            rawReader.GetScanInfo(scan, out header);

            return header.MSLevel == 1;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Cleanup closing each of the raw file readers if they were read.
        /// </summary>
        public void Dispose()
        {
            foreach (int key in m_readers.Keys)
            {
                try
                {
                    m_readers[key].CloseRawFile();
                }
                catch
                {
                    // We don't want it to fail.  Just go with it.
                }
            }
            m_readers.Clear();
        }

        #endregion
    }
}
