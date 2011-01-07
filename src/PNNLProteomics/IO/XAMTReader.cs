using System;
using System.Collections.Generic;

using MultiAlignEngine.MassTags;

namespace PNNLProteomics.IO
{
    /// <summary>
    /// Reads XAMT database files and returns a mass tag database.
    /// </summary>
    public class XAMTReader
    {
        private const string CONST_MASS_TAG_ID           = "Mass_Tag_ID";
        private const string CONST_PEPTIDE               = "Peptide	";
        private const string CONST_CHARGE_STATE          = "Charge State";
        private const string CONST_PREDICTED_DRIFT_TIME  = "PredDriftTime";
        private const string CONST_MONO_MASS             = "MonoIsotopic Mass";
        private const string CONST_NET                   = "NET";
        private const string CONST_PRED_DRIFT_TIME_PLUS2 = "PredDTPlus2Model";
        private const string CONST_PRED_DRIFT_TIME_PLUS3 = "PredDTPlus3Modl";
        private const char CONST_CHAR_DELIMITER = '\t';

        /// <summary>
        /// Column header lookup.
        /// </summary>
        private Dictionary<string, int> m_headerDictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        public XAMTReader()
        {
            m_headerDictionary = new Dictionary<string,int>();
        }

        /// <summary>
        /// Constructs a column name lookup.
        /// </summary>
        /// <param name="header"></param>
        private void ConstructHeaderMapping(string line)
        {
            string[] headers = line.Split(CONST_CHAR_DELIMITER);
            

            m_headerDictionary = new Dictionary<string,int>();
            int i = 0;
            foreach (string header in headers)
            {
                switch (header)
                {
                    case CONST_CHARGE_STATE :
                        m_headerDictionary.Add(CONST_CHARGE_STATE, i);
                        break;
                    case CONST_MASS_TAG_ID:
                        m_headerDictionary.Add(CONST_MASS_TAG_ID, i);
                        break;
                    case CONST_MONO_MASS:
                        m_headerDictionary.Add(CONST_MONO_MASS, i);
                        break;
                    case CONST_NET:
                        m_headerDictionary.Add(CONST_NET, i);
                        break;
                    case CONST_PEPTIDE:
                        m_headerDictionary.Add(CONST_PEPTIDE, i);
                        break;
                    case CONST_PREDICTED_DRIFT_TIME:
                        m_headerDictionary.Add(CONST_PREDICTED_DRIFT_TIME, i);
                        break;
                    case CONST_PRED_DRIFT_TIME_PLUS2:
                        m_headerDictionary.Add(CONST_PRED_DRIFT_TIME_PLUS2, i);
                        break;
                    case CONST_PRED_DRIFT_TIME_PLUS3:
                        m_headerDictionary.Add(CONST_PRED_DRIFT_TIME_PLUS3, i);
                        break;
                }
                i++;
            }
        }

        /// <summary>
        /// Parses a line of data from the XAMT File and returns a mass tag.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private clsMassTag ParseLine(string line)
        {
            string[] data = line.Split(CONST_CHAR_DELIMITER);           

            clsMassTag tag      = new clsMassTag();
            tag.Id              = Convert.ToInt32(data[m_headerDictionary[CONST_MASS_TAG_ID]]);
            tag.Mass            = Convert.ToDouble(data[m_headerDictionary[CONST_MONO_MASS]]);
            tag.NetAverage      = Convert.ToDouble(data[m_headerDictionary[CONST_NET]]);
            tag.NetPredicted    = Convert.ToDouble(data[m_headerDictionary[CONST_NET]]);
            tag.DriftTime       = Convert.ToDouble(data[m_headerDictionary[CONST_PREDICTED_DRIFT_TIME]]);
            tag.mstrPeptide     = data[m_headerDictionary[CONST_PEPTIDE]];
            return tag;
        }

        /// <summary>
        /// Reads the XAMT file.
        /// </summary>
        /// <param name="filepath">Path of XAMT in flat file form.</param>
        /// <returns>Mass Tag Database.  Null if file does not exist.</returns>
        public clsMassTagDB ReadXAMTDatabase(string filepath)
        {            
            if (System.IO.File.Exists(filepath) == false)
                return null;

            /// 
            /// Create a new database.
            /// 
            clsMassTagDB xamt = new clsMassTagDB();

            /// 
            /// Read the file  - We could use a read line by line 
            /// method here, but the files are small.
            /// 
            string[] lines = System.IO.File.ReadAllLines(filepath);

            /// 
            /// Read the column headers
            /// 
            string header = lines[0];
            ConstructHeaderMapping(header);

            /// 
            /// Parse the rest of the file now.
            /// 
            List<clsMassTag> massTags = new List<clsMassTag>();
            for(int i = 1; i < lines.Length; i++)                
            {
                string line = lines[i];

                massTags.Add(ParseLine(line));
            }

            /// 
            /// Transfer the data to the XAMT.
            /// 
            clsMassTag[] tags = new clsMassTag[massTags.Count];
            massTags.CopyTo(tags);
            xamt.AddMassTags(tags);
            
            return xamt;
        }
    }
}
