using System;
using System.Reflection;
using MultiAlignEngine;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Parameters
{
    /// <summary>
    /// Writes a MultiAlign XML Parameter File.
    /// </summary>
    public class XMLParameterFileWriter: IParameterFileWriter
    {
        /// <summary>
        /// Reflects the object o for parameter options to load.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="node"></param>
        private void ReflectParameterOptions(object o, MetaNode node)
		{		
			if (o != null)
			{			
				/// 
				/// Iterate all the properties
				/// 
				foreach(PropertyInfo prop in o.GetType().GetProperties())
				{					
					// Recurse to get parameters.
					if (prop.CanRead)
					{																						
						object[] customAttributes = prop.GetCustomAttributes(typeof(ParameterFileAttribute),true);
						object potential = null;
						if (customAttributes.Length > 0)
							potential = prop.GetValue(o,
								BindingFlags.GetProperty,
								null,
								null,
								null);
						for (int i = 0; i < customAttributes.Length; i++)
						{
                            
							ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;
                            
							if (potential != null && attr != null && attr.Name != "")
							{										
								try
								{
                                    
									node.SetValue(attr.Name, potential);
								}
								catch(Exception ex)
								{
                                    System.Diagnostics.Debug.WriteLine(ex.Message);									
								}
							}
						}						
					}
				}					
				
				/// 
				/// Iterate all the fields
				/// 
				foreach(FieldInfo field in o.GetType().GetFields())
				{					
					object[] customAttributes	= field.GetCustomAttributes(typeof(ParameterFileAttribute),true);
					object objectValue = null;
					if (customAttributes.Length > 0)
						objectValue	   = field.GetValue(o);											
					for (int i = 0; i < customAttributes.Length; i++)
					{
						ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute ;
						if (objectValue != null && attr != null)
						{		
							try
							{
								node.SetValue(attr.Name, objectValue);
							}
							catch
							{
								// what?
							}
						}
					}					
				}	
			}
		}       
        public void WriteParameterFile(string parameterFilePath, MultiAlignAnalysis analysis)
        {            				
			MetaData metaData = new MetaData("PNNLProteomics");
            ReflectParameterOptions(analysis.Options.MSLinkerOptions,           metaData.OpenChild("MSnLinker"));
            ReflectParameterOptions(analysis.Options.FeatureFindingOptions,     metaData.OpenChild("LCMSFeatureFinding"));
            ReflectParameterOptions(analysis.Options.FeatureFilterOptions,      metaData.OpenChild("LCMSFeatureFilters"));
            ReflectParameterOptions(analysis.Options.MassTagDatabaseOptions,    metaData.OpenChild("MassTagDatabase"));
            ReflectParameterOptions(analysis.Options.AlignmentOptions,          metaData.OpenChild("Alignment"));
            ReflectParameterOptions(analysis.Options.DriftTimeAlignmentOptions, metaData.OpenChild("DriftTimeAlignment"));
            ReflectParameterOptions(analysis.Options.ClusterOptions,            metaData.OpenChild("LCMSFeatureClustering"));
            ReflectParameterOptions(analysis.Options.STACOptions,               metaData.OpenChild("STAC"));
            metaData.WriteFile(parameterFilePath);
        }        
    }
}
