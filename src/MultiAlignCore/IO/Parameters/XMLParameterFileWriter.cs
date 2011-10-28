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
						object[] customAttributes = prop.GetCustomAttributes(typeof(clsParameterFileAttribute),true);
						object potential = null;
						if (customAttributes.Length > 0)
							potential = prop.GetValue(o,
								BindingFlags.GetProperty,
								null,
								null,
								null);
						for (int i = 0; i < customAttributes.Length; i++)
						{
                            
							clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute;
                            
							if (potential != null && attr != null && attr.Description != "")
							{										
								try
								{
                                    
									node.SetValue(attr.Description, potential);
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
					object[] customAttributes	= field.GetCustomAttributes(typeof(clsParameterFileAttribute),true);
					object objectValue = null;
					if (customAttributes.Length > 0)
						objectValue	   = field.GetValue(o);											
					for (int i = 0; i < customAttributes.Length; i++)
					{
						clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute ;
						if (objectValue != null && attr != null)
						{		
							try
							{
								node.SetValue(attr.Description, objectValue);
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
            ReflectParameterOptions(analysis.Options.MSLinkerOptions, metaData.OpenChild("MSnLinkerOptions"));
            ReflectParameterOptions(analysis.Options.UMCFindingOptions, metaData.OpenChild("UMCFindingOptions"));
            ReflectParameterOptions(analysis.Options.FeatureFilterOptions, metaData.OpenChild("FeatureFilters"));
            ReflectParameterOptions(analysis.Options.MassTagDBOptions, metaData.OpenChild("MassTagDBOptions"));
            ReflectParameterOptions(analysis.Options.DefaultAlignmentOptions, metaData.OpenChild("DefaultAlignmentOptions"));
            ReflectParameterOptions(analysis.Options.DriftTimeAlignmentOptions, metaData.OpenChild("DriftTimeAlignmentOptions"));
            ReflectParameterOptions(analysis.Options.ClusterOptions, metaData.OpenChild("ClusterOptions"));
            ReflectParameterOptions(analysis.Options.PeakMatchingOptions, metaData.OpenChild("PeakMatchingOptions"));
            ReflectParameterOptions(analysis.Options.SMARTOptions, metaData.OpenChild("SMARTOptions"));
            ReflectParameterOptions(analysis.Options.STACOptions, metaData.OpenChild("STACOptions"));
            metaData.WriteFile(parameterFilePath);
        }        
    }
}
