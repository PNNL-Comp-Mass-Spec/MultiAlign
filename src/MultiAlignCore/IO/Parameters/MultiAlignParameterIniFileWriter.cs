using System;
using System.IO;
using System.Reflection;

using MultiAlignEngine;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Parameters
{
    /// <summary>
    /// Writes an analysis' parameters to file in the INI format.
    /// </summary>
    public class MultiAlignParameterIniFileWriter
    {
        #region Tag Constants
        private const string ALIGNMENT_TAG                      = "Alignment";
        private const string CLUSTER_TAG                        = "Clustering";
        private const string PEAK_MATCH_TAG                     = "PeakMatching";
        private const string GLOBAL_TAG                         = "Global";
        private const string FEATURE_FINDING_TAG                = "FeatureFinding";
        private const string MASS_TAG_DATABASE_TAG              = "MassTagDatabase";
        private const string GLOBAL_OPTION_USE_STAC             = "Use STAC";
        private const string GLOBAL_OPTION_USE_MTDB_AS_BASELINE = "Use Mass Tag DB As Baseline";
        #endregion

        #region Writing
        /// <summary>
        /// Writes the option and it's data to the text stream.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="optionName"></param>
        /// <param name="value"></param>
        private void WriteOption(TextWriter writer, string optionName, object value)
        {
            writer.WriteLine(string.Format("{0} = {1}", optionName, value));
        }
        /// <summary>
        /// Writes an option object to the parameter file.
        /// </summary>
        /// <param name="writer">Text stream to file object.</param>
        /// <param name="o">Options object.</param>
        /// <param name="optionSetName">Name of option group</param>
        private void WriteOptionGroup(TextWriter writer, string optionGroupName, object o)
		{
            writer.WriteLine("[" + optionGroupName + "]");
			foreach(PropertyInfo prop in o.GetType().GetProperties())
			{					
				if (prop.CanRead)
				{																						
					object[] customAttributes   = prop.GetCustomAttributes(typeof(clsParameterFileAttribute),true);
					object data                 = null;
					if (customAttributes.Length > 0)
                    {
						data = prop.GetValue(o,BindingFlags.GetProperty, null, null, null);
                    }
					for (int i = 0; i < customAttributes.Length; i++)
					{                        
						clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute;                        
						if (data != null && attr != null && attr.Description != "")
						{
                            WriteOption(writer, attr.Description, data);
						}
					}						
				}
			}								
			foreach(FieldInfo field in o.GetType().GetFields())
			{
                object[] customAttributes	= field.GetCustomAttributes(typeof(clsParameterFileAttribute),true);
				object data                 = null;
				if (customAttributes.Length > 0)
                {
					data	   = field.GetValue(o);											
                }
				for (int i = 0; i < customAttributes.Length; i++)
				{
					clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute ;
                    if (data != null && attr != null && attr.Description != "")
                    {
                        WriteOption(writer, attr.Description, data);
                    }
				}					
			}
            writer.WriteLine();
		}
	    #endregion

        /// <summary>
        /// Saves the analysis parameters to file.
        /// </summary>
        /// <param name="filename">File to save to.</param>
        /// <param name="analysis">Analysis with options to save.</param>
		public void WriteParametersToFile(string filename, MultiAlignAnalysis analysis)
		{            
            using (TextWriter writer = File.CreateText(filename))
            {

                WriteOptionGroup(writer, ALIGNMENT_TAG,         analysis.Options.DefaultAlignmentOptions);
                WriteOptionGroup(writer, FEATURE_FINDING_TAG,   analysis.Options.UMCFindingOptions);
                WriteOptionGroup(writer, MASS_TAG_DATABASE_TAG, analysis.Options.MassTagDBOptions);
                WriteOptionGroup(writer, CLUSTER_TAG,           analysis.Options.ClusterOptions);
                WriteOptionGroup(writer, PEAK_MATCH_TAG,        analysis.Options.PeakMatchingOptions);

                writer.WriteLine("[" + GLOBAL_TAG + "]" );
                WriteOption(writer, GLOBAL_OPTION_USE_MTDB_AS_BASELINE, analysis.Options.UseMassTagDBAsBaseline);
                writer.WriteLine();
            }
		} 
    }
}
