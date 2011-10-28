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

            List<classAlignmentMZBoundary> boundaries                       = analysis.Options.DefaultAlignmentOptions.MZBoundaries;                        
            LoadParameterOptions(analysis.Options.ClusterOptions,           metaData.OpenChild("ClusterOptions"));
            LoadParameterOptions(analysis.Options.PeakMatchingOptions,      metaData.OpenChild("PeakMatchingOptions"));
            LoadParameterOptions(analysis.Options.DefaultAlignmentOptions,  metaData.OpenChild("DefaultAlignmentOptions"));

            if (analysis.Options.DefaultAlignmentOptions.DriftTimeBinSize <= 0)
            {
                analysis.Options.DefaultAlignmentOptions.DriftTimeBinSize = 1;
            }

            LoadParameterOptions(analysis.Options.MassTagDBOptions, metaData.OpenChild("MassTagDBOptions"));
            LoadParameterOptions(analysis.Options.UMCFindingOptions, metaData.OpenChild("UMCFindingOptions"));

            MetaNode msOptions = metaData.OpenChild("MSnLinkerOptions", false);
            if (msOptions != null)
            {
                LoadParameterOptions(analysis.Options.MSLinkerOptions, msOptions);
            }

            msOptions = metaData.OpenChild("FeatureFilters", false);
            if (msOptions != null)
            {
                LoadParameterOptions(analysis.Options.FeatureFilterOptions, msOptions);
            }

            MetaNode node = metaData.OpenChild("DriftTimeAlignmentOptions", false);
            if (node != null)
            {
                LoadParameterOptions(analysis.Options.DriftTimeAlignmentOptions, node);
            }
            analysis.Options.DefaultAlignmentOptions.MZBoundaries = boundaries;


            MetaNode stacNode = metaData.OpenChild("STACOptions", false);
            if (stacNode != null)
            {
                LoadParameterOptions(analysis.Options.STACOptions, stacNode);
            }
        }
    }
}
