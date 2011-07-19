using System;
using System.Collections.Generic;

using PNNLProteomics.Data;
using MultiAlignEngine;

namespace PNNLProteomics.IO
{
    /// <summary>
    /// Class that holds filter information for searching DMS.
    /// </summary>
    public class DMSDatasetSearchOptions
    {
        #region Members
        //TODO: add field comments.
        private string m_instrumentName;
        private List<PNNLProteomics.Data.DeisotopingTool> m_toolIDList;        
        private DateTime m_dateTime;
        private string m_datasetName;
        private string m_datasetID;
        private string m_fileExtension;
        /// <summary>
        /// Name of parameter file used to process the data.
        /// </summary>
        private string m_parameterFileName;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DMSDatasetSearchOptions()
        {
            Clear();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the name of the parameter file used.
        /// </summary>
        public string ParameterFileName
        {
            get
            {
                return m_parameterFileName;
            }
            set
            {
                m_parameterFileName = value;
            }
        }
        public string FileExtension
        {
            get
            {
                return m_fileExtension;
            }
            set
            {
                m_fileExtension = value;
            }
        }
        //TODO: Add Property comments.
        public string InstrumentName
        {
            get { return m_instrumentName; }
            set { m_instrumentName = value; }
        }
        public string DatasetName
        {
            get { return m_datasetName; }
            set { m_datasetName = value; }
        }
        public string DatasetID
        {
            get { return m_datasetID; }
            set { m_datasetID = value; }
        }
        public List<DeisotopingTool> ToolIDs
        {
            get { return m_toolIDList; }
            set { m_toolIDList = value; }
        }
        public DateTime DateTime
        {
            get { return m_dateTime; }
            set { m_dateTime = value; }
        }
        #endregion

        /// <summary>
        /// Sets values of all data to defaults.
        /// </summary>
        public void Clear()
        {
            m_datasetID         = "";
            m_datasetName       = "";
            m_dateTime          = new DateTime(2000, 1,1);
            m_instrumentName    = "";
            m_fileExtension     = "";
            
            m_toolIDList        = new List<DeisotopingTool>();

            // By default add all tool id's to the list of available.
            Array tools = Enum.GetValues(typeof(DeisotopingTool));
            foreach (object o in tools)
            {
                DeisotopingTool id = (DeisotopingTool)o;
                m_toolIDList.Add(id);
            }
        }

        /// <summary>
        ///  Map the tool ID to the enumeration value.
        /// </summary>
        private static Dictionary<int, DeisotopingTool> m_toolMap;
        private static bool mappedData = false;
        public static  DeisotopingTool MapToolIDToDeisotopingTool(int id)
        {            
            if (mappedData == false)
            {
                m_toolMap   = new Dictionary<int,DeisotopingTool>();
                Array tools = Enum.GetValues(typeof(DeisotopingTool));
                foreach (object o in tools)
                {
                    DeisotopingTool idTool = (DeisotopingTool)o;
                    m_toolMap.Add((int)idTool, idTool);
                }
                mappedData = true;
            }
            if (m_toolMap.ContainsKey(id) == false)
                return DeisotopingTool.NA;

            return m_toolMap[id];
        }
    }
}
