using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using MultiAlignEngine;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.PeakMatching;

using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Parameters
{

    /// <summary>
    /// Reads MultiAlign paramter information from file.
    /// </summary>
    public class MultiAlignParameterIniFileReader
    {
        #region Tag Constants 
        private const string ALIGNMENT_TAG          = "Alignment";
        private const string CLUSTER_TAG            = "Clustering";
        private const string PEAK_MATCH_TAG         = "PeakMatching";
        private const string GLOBAL_TAG             = "Global";
        private const string FEATURE_FINDING_TAG    = "FeatureFinding";
        private const string MASS_TAG_DATABASE_TAG  = "MassTagDatabase";
        #endregion
        


        /// <summary>
        /// Maps header points to the data itself.
        /// </summary>
        private Dictionary<string, List<string>> m_headerMaps;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiAlignParameterIniFileReader()
        {
            m_headerMaps = new Dictionary<string, List<string>>();
        }


        #region Extraction and File Parsing
        /// <summary>
        /// Parses each subsection into key-value pairs.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, string> ProcessSubSectionData(string groupName)
        {
            List<string> data               = m_headerMaps[groupName];
            Dictionary<string, string> map  = new Dictionary<string, string>();
            foreach (string key in data)
            {
                string[] keyvalue = key.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (keyvalue.Length == 2)
                {
                    keyvalue[0] = keyvalue[0].TrimEnd(new char[] { ' ' });
                    keyvalue[0] = keyvalue[0].TrimStart(new char[] { ' ' });
                    keyvalue[1] = keyvalue[1].TrimEnd(new char[] { ' ' });
                    keyvalue[1] = keyvalue[1].TrimStart(new char[] { ' ' });
                    map.Add(keyvalue[0], keyvalue[1]);
                }
            }
            return map;
        }
        /// <summary>
        /// Extracts group names from the ini file.
        /// </summary>
        /// <param name="lines"></param>
        private void ExtractGroups(string[] lines)
        {
            int start = -1;
            int i = 0;
            List<string> data = new List<string>();
            foreach (string dataLine in lines)
            {
                string line = dataLine;
                line.Replace("\n", "");
                line.Replace("\r", "");
                line = line.TrimEnd(new char[] { ' ' });
                line = line.TrimStart(new char[] { ' ' });

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (start >= 0 && i > 0)
                    {
                        string header = lines[start];                        
                        header = header.Replace("]", "");
                        header = header.Replace("[", "");
                        m_headerMaps.Add(header, data);

                        data.Clear();
                    }
                    start = i;
                }
                else
                {
                    data.Add(line);
                }
                i++;
            }
            if (data.Count > 0)
            {
                string header = lines[start];
                header = header.Replace("]", "");
                header = header.Replace("[", "");
                m_headerMaps.Add(header, data);
            }
        }
        #endregion

        #region Parameter Options Reflection and manual loading.
        /// <summary>
        /// Loads the global options manually.
        /// </summary>
        /// <param name="analysis"></param>
        private void LoadGlobalOptions(ref MultiAlignAnalysis analysis)
        {            
            Dictionary<string, string> map  = ProcessSubSectionData(GLOBAL_TAG);
            analysis.Options.UseMassTagDBAsBaseline = Convert.ToBoolean(map["Use Mass Tag DB As Baseline"]);            
        }
        /// <summary>
        /// Load the parameters from the object
        /// </summary>
        /// <param name="o"></param>
        /// <param name="node"></param>
        void LoadParameterOptions(Dictionary<string, string> map, object o)
        {
            foreach (PropertyInfo prop in o.GetType().GetProperties())
            {
                if (prop.CanWrite)
                {
                    object[] customAttributes = prop.GetCustomAttributes(typeof(clsParameterFileAttribute), true);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        clsParameterFileAttribute attribute = customAttributes[i] as clsParameterFileAttribute;
                        if (attribute != null)
                        {
                            if (map.ContainsKey(attribute.Description))
                            {
                                object value = map[attribute.Description];
                                prop.SetValue(o, value, BindingFlags.SetProperty, null, null, null);
                            }
                        }
                    }
                }
            }
            foreach (FieldInfo field in o.GetType().GetFields())
            {
                object[] customAttributes = field.GetCustomAttributes(typeof(clsParameterFileAttribute), true);
                for (int i = 0; i < customAttributes.Length; i++)
                {
                    clsParameterFileAttribute attribute = customAttributes[i] as clsParameterFileAttribute;
                    if (attribute != null)
                    {
                        if (map.ContainsKey(attribute.Description))
                        {
                            object value = map[attribute.Description];
                            field.SetValue(o, value);
                        }                                                    
                    }
                }
            }            
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterFilePath"></param>
        /// <returns></returns>
        public void ReadParameterFile(string parameterFilePath, ref MultiAlignAnalysis analysis)
        {
            if (!File.Exists(parameterFilePath))
                throw new FileNotFoundException("The parameter file was not found. " + parameterFilePath);

            // Parse the data.
            string[] lines = File.ReadAllLines(parameterFilePath);
            ExtractGroups(lines);

            // Load options.
            clsAlignmentOptions alignmentOptions            = analysis.Options.DefaultAlignmentOptions;
            LoadParameterOptions(ProcessSubSectionData(ALIGNMENT_TAG), alignmentOptions);            
            LoadParameterOptions(ProcessSubSectionData(FEATURE_FINDING_TAG),     analysis.Options.UMCFindingOptions);
            LoadParameterOptions(ProcessSubSectionData(PEAK_MATCH_TAG),          analysis.Options.PeakMatchingOptions);
            LoadParameterOptions(ProcessSubSectionData(MASS_TAG_DATABASE_TAG),   analysis.Options.MassTagDatabaseOptions);
            LoadParameterOptions(ProcessSubSectionData(CLUSTER_TAG),             analysis.Options.ClusterOptions);
            LoadGlobalOptions(ref analysis);                 
        }
    }
}
