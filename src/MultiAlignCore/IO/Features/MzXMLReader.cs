﻿using System.Collections.Generic;
using System.IO;
using MSDataFileReader;
using PNNLOmics.Data;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Adapter for reading mzXML files from a reader by Matt Monroe.
    /// </summary>
    public class MzXMLReader : ISpectraProvider
    {
        
        /// <summary>
        /// Readers for each dataset.
        /// </summary>
        private Dictionary<int, clsMzXMLFileReader> m_readers;

        private Dictionary<int, string> m_dataFiles;

        public MzXMLReader()
        {
            m_dataFiles = new Dictionary<int, string>();
            m_readers   = new Dictionary<int,clsMzXMLFileReader>();
            BinSize     = .5; 
        }
       
        /// <summary>
        /// Gets or sets the bin size for mass spectra
        /// </summary>
        public double BinSize
        {
            get;
            set;
        }

        #region ISpectraProvider Members
        /// <summary>
        /// Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> ReadMSMSSpectra(string file)
        {
            List<MSSpectra> spectra     = new List<MSSpectra>();
            clsMzXMLFileReader reader   = new clsMzXMLFileReader();
            reader.SkipBinaryData       = true;
            bool opened                 = reader.OpenFile(file);

            if (!opened)
            {
                throw new IOException("Could not open the mzXML file " + file);
            }

            int totalScans = reader.ScanCount;

            for (int i = 0; i < totalScans; i++)
            {
                
                clsSpectrumInfo info = new clsSpectrumInfo();
                reader.GetSpectrumByScanNumber(i, ref info);
                if (info.MSLevel > 1)
                {
                    MSSpectra spectrum          = new MSSpectra();
                    spectrum.MSLevel            = info.MSLevel;
                    spectrum.RetentionTime      = info.RetentionTimeMin;
                    spectrum.Scan               = i;
                    spectrum.PrecursorMZ        = info.ParentIonMZ;
                    spectrum.TotalIonCurrent    = info.TotalIonCurrent;

                    // Need to make this a standard type of collision based off of the data.
                    spectrum.CollisionType = CollisionType.Other;                    
                    spectra.Add(spectrum);
                }
            }
            reader.CloseFile();
            return spectra;
        }

        public List<XYData> GetRawSpectra(int scan, int group)
        {
            List<XYData> spectrum = null;

            if (!m_dataFiles.ContainsKey(group))
            {
                throw new System.Exception("The group-dataset ID provided was not found.");
            }
            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.
            if (!m_readers.ContainsKey(group))
            {
                string path                 = m_dataFiles[group];
                clsMzXMLFileReader reader   = new clsMzXMLFileReader();
                reader.SkipBinaryData       = true;
                m_readers.Add(group, reader);

                bool opened = reader.OpenFile(path);
                if (!opened)
                {
                    throw new IOException("Could not open the mzXML file " + path);
                }
            }
            clsMzXMLFileReader rawReader = m_readers[group];

            int totalScans = rawReader.ScanCount;            
            clsSpectrumInfo info = new clsSpectrumInfo();
            rawReader.GetSpectrumByScanNumber(scan, ref info);
            if (info.MSLevel > 1)
            {

                spectrum = new List<XYData>();
                for (int j = 0; j < info.MZList.Length; j++)
                {
                    spectrum.Add(new XYData(info.MZList[j], info.IntensityList[j]));
                }
            }
            
            return spectrum;
        }
        #endregion

        #region ISpectraProvider Members
        public void AddDataFile(string path, int groupID)
        {
            
        }
        public List<MSSpectra> GetMSMSSpectra(int group)
        {
            return null;
        }
        public List<MSSpectra> GetMSMSSpectra(int group, Dictionary<int, int> excludeMap)
        {
            return null;
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            
        }
        #endregion

        #region ISpectraProvider Members


        public List<XYData> GetRawSpectra(int scan, int group, int scanLevel)
        {
            return GetRawSpectra(scan, group);
        }

        #endregion
    }
}