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
    public class ThermoRawDataFileReader: IRawDataFileReader, ISpectraProvider , IDisposable
    {
        /// <summary>
        /// Readers for each dataset.
        /// </summary>
        private Dictionary<int, XRawFileIO> m_readers;

        private Dictionary<int, string> m_dataFiles;

        public ThermoRawDataFileReader()
        {
            m_dataFiles = new Dictionary<int, string>();
            m_readers   = new Dictionary<int,XRawFileIO>();
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


        /// <summary>
        /// Adds a dataset file to the reader map so when in use the application
        /// knows where to get raw data from.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupID"></param>
        public void AddDataFile(string path, int groupID)
        {
            m_dataFiles.Add(groupID, path);
        }

        #region IRawDataFileReader Members
        /// <summary>
        /// Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> ReadMSMSSpectra(string file)
        {
            List<MSSpectra> spectra = new List<MSSpectra>();


            XRawFileIO rawReader = new XRawFileIO();
            bool opened          = rawReader.OpenRawFile(file);

            if (!opened)
            {                
                throw new IOException("Could not open the Thermo raw file " + file);
            }

            int numberOfScans = rawReader.GetNumScans();
            for (int i = 0; i < numberOfScans; i++)
            {

                FinniganFileReaderBaseClass.udtScanHeaderInfoType header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
                rawReader.GetScanInfo(i, ref header);

                if (header.MSLevel > 1)
                {
                    MSSpectra spectrum          = new MSSpectra();
                    spectrum.MSLevel            = header.MSLevel;
                    spectrum.RetentionTime      = header.RetentionTime;
                    spectrum.Scan               = i;
                    spectrum.PrecursorMZ        = header.ParentIonMZ;
                    spectrum.TotalIonCurrent    = header.TotalIonCurrent;

                    // Need to make this a standard type of collision based off of the data.
                    spectrum.CollisionType      = CollisionType.Other;

                    spectra.Add(spectrum);
                }
            }
            rawReader.CloseRawFile();

            return spectra;
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
            if (!m_dataFiles.ContainsKey(group))
            {
                throw new System.Exception("The group-dataset ID provided was not found.");
            }
            // If we dont have a reader, then create one for this group 
            // next time, it will be available and we won't have to waste time
            // opening the file.
            if (!m_readers.ContainsKey(group))
            {
                string path = m_dataFiles[group];
                XRawFileIO reader = new XRawFileIO();
                m_readers.Add(group, reader);
                bool opened = reader.OpenRawFile(path);

                if (!opened)
                {
                    throw new IOException("Could not open the Thermo raw file " + m_dataFiles[group]);
                }
            }

            List<MSSpectra> spectra = new List<MSSpectra>();
            XRawFileIO rawReader    = m_readers[group];
            
            FinniganFileReaderBaseClass.udtScanHeaderInfoType header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
            
            rawReader.GetScanInfo(scan, ref header);
            int N                   = header.NumPeaks;            
            
            double [] mz            = new double[N];
            double [] intensities   = new double[N];

            double minMass  = 0;    //header.LowMass;
            double maxMass  = 2000; //header.HighMass;
            int total       = System.Convert.ToInt32((maxMass - minMass) / BinSize);

            rawReader.GetScanData(scan, ref mz, ref intensities, ref header);                   

            // construct the array.
            List<XYData> data = new List<XYData>(total);                        
            for (int i = 0; i < total; i++)
            {
                double mass  = (BinSize * i) + minMass;
                XYData datum = new XYData(mass, 0);
                data.Add(datum);
            }

            
            for (int i = 0; i < mz.Length; i++)
            {
                double  intensity    = intensities[i];
                int     bin          = Math.Min(total - 1, System.Convert.ToInt32((mz[i] - minMass) / BinSize));
                try
                {
                    data[bin].Y += intensity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return data;
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
