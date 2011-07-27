using System.Collections.Generic;
using System.Reflection;
using MultiAlignCore.Data;
using MultiAlignEngine;
using MultiAlignEngine.Alignment;

namespace MultiAlignCore.IO.Parameters
{

    /// <summary>
    /// Reads a MultiAlign XML Parameter File.
    /// </summary>
    public class XMLParamterFileReader: IParameterFileReader
    {
        private void LoadParameterOptions(object o, MetaNode node)
        {
            if (o != null)
            {
                foreach (PropertyInfo prop in o.GetType().GetProperties())
                {
                    // Recurse to get parameters.
                    if (prop.CanWrite)
                    {
                        object[] customAttributes = prop.GetCustomAttributes(typeof(clsParameterFileAttribute), true);
                        for (int i = 0; i < customAttributes.Length; i++)
                        {
                            clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute;
                            if (attr != null && attr.Description != "")
                            {
                                try
                                {
                                    object val;
                                    val = node.GetValue(attr.Description);
                                    prop.SetValue(o, val, BindingFlags.SetProperty, null, null, null);
                                }
                                catch
                                {
                                    //System.Windows.Forms.MessageBox.Show("Could not load " + attr.Description + " parameter. " + ex.Message);
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
                        clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute;
                        if (attr != null)
                        {
                            try
                            {
                                object val = node.GetValue(attr.Description);
                                if (val != null)
                                {
                                    field.SetValue(o, val);
                                }
                            }
                            catch
                            {
                                //System.Windows.Forms.MessageBox.Show("Could not load " + attr.Description + " parameter. " + ex.Message);
                            }
                        }
                    }
                }
            }
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterFilePath"></param>
        /// <returns></returns>
        public void ReadParameterFile(string parameterFilePath, ref MultiAlignAnalysis analysis)
        {
            MetaData metaData = new MetaData("PNNLProteomics");
            metaData.ReadFile(parameterFilePath);

            List<classAlignmentMZBoundary> boundaries       = analysis.DefaultAlignmentOptions.MZBoundaries;            
            LoadParameterOptions(analysis.AlignmentOptions,         metaData.OpenChild("DefaultAlignmentOptions"));
            LoadParameterOptions(analysis.ClusterOptions,           metaData.OpenChild("ClusterOptions"));
            LoadParameterOptions(analysis.PeakMatchingOptions,      metaData.OpenChild("PeakMatchingOptions"));
            LoadParameterOptions(analysis.DefaultAlignmentOptions,  metaData.OpenChild("DefaultAlignmentOptions"));            
            LoadParameterOptions(analysis.MassTagDBOptions,         metaData.OpenChild("MassTagDBOptions"));
            LoadParameterOptions(analysis.UMCFindingOptions,        metaData.OpenChild("UMCFindingOptions"));

            MetaNode msOptions = metaData.OpenChild("MSnLinkerOptions", false);
            if (msOptions != null)
            {
                LoadParameterOptions(analysis.MSLinkerOptions, msOptions);
            }

            MetaNode node = metaData.OpenChild("DriftTimeAlignmentOptions", false);
            if (node != null)
            {
                LoadParameterOptions(analysis.DriftTimeAlignmentOptions, node);
            }            
            analysis.DefaultAlignmentOptions.MZBoundaries   = boundaries;
        }
    }
}
