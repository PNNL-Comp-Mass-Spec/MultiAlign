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
                        object[] customAttributes = prop.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                        for (int i = 0; i < customAttributes.Length; i++)
                        {
                            ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;
                            if (attr != null && attr.Name != "")
                            {
                                try
                                {
                                    object val;
                                    val = node.GetValue(attr.Name);
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
                    object[] customAttributes = field.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;
                        if (attr != null)
                        {
                            try
                            {
                                object val = node.GetValue(attr.Name);
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
            AnalysisOptions options = analysis.Options;
            ReadParameterFile(parameterFilePath, ref options);
        }

        public void ReadParameterFile(string parameterFilePath, ref AnalysisOptions options)
        {            
            MetaData metaData = new MetaData("PNNLProteomics");
            metaData.ReadFile(parameterFilePath);

            MetaNode msOptions = metaData.OpenChild("MSnLinker", false);
            if (msOptions != null)
            {
                LoadParameterOptions(options.MSLinkerOptions, msOptions);
            }
            LoadParameterOptions(options.MassTagDatabaseOptions, metaData.OpenChild("MassTagDatabase"));
            LoadParameterOptions(options.FeatureFindingOptions, metaData.OpenChild("LCMSFeatureFinding"));
            msOptions = metaData.OpenChild("LCMSFeatureFilters", false);
            if (msOptions != null)
            {
                LoadParameterOptions(options.FeatureFilterOptions, msOptions);
            }
            List<classAlignmentMZBoundary> boundaries                       = options.AlignmentOptions.MZBoundaries;     
            LoadParameterOptions(options.AlignmentOptions, metaData.OpenChild("Alignment"));
            if (options.AlignmentOptions.DriftTimeBinSize <= 0)
            {
                options.AlignmentOptions.DriftTimeBinSize = 1;
            }
            options.AlignmentOptions.MZBoundaries                  = boundaries;
            MetaNode node = metaData.OpenChild("DriftTimeAlignment", false);
            if (node != null)
            {
                LoadParameterOptions(options.DriftTimeAlignmentOptions, node);
            }
            LoadParameterOptions(options.ClusterOptions, metaData.OpenChild("LCMSFeatureClustering"));       
            MetaNode stacNode = metaData.OpenChild("STAC", false);
            if (stacNode != null)
            {
                LoadParameterOptions(options.STACOptions, stacNode);
            }

            MetaNode consolidatorNode = metaData.OpenChild("FeatureConsolidator", false);
            if (consolidatorNode != null)
            {
                LoadParameterOptions(options.ConsolidationOptions, consolidatorNode);
            }
        }
    }
}
