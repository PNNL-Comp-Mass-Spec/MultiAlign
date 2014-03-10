using System;
using System.Collections.Generic;
using System.Reflection;
using MultiAlignEngine;

namespace MultiAlignCore.IO.Parameters
{
    public static class ParameterUtility
    {
        /// <summary>
        /// Converts a parameter object into a string.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static List<ParameterHibernateMapping> ExtractParameterMapObjects(object o, string optionGroup)
        {
            List<ParameterHibernateMapping> values = new List<ParameterHibernateMapping>();
            if (o != null)
            {
                foreach (PropertyInfo prop in o.GetType().GetProperties())
                {
                    // Recurse to get parameters.
                    if (prop.CanRead)
                    {
                        object[] customAttributes = prop.GetCustomAttributes(typeof(ParameterFileAttribute), true);
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
                                ParameterHibernateMapping map = new ParameterHibernateMapping();
                                map.OptionGroup = optionGroup;
                                map.Parameter   = attr.Name;
                                map.Value       = potential.ToString();
                                values.Add(map);
                            }
                        }
                    }
                }
                foreach (FieldInfo field in o.GetType().GetFields())
                {
                    object[] customAttributes = field.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                    object objectValue = null;
                    if (customAttributes.Length > 0)
                        objectValue = field.GetValue(o);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;
                        if (objectValue != null && attr != null)
                        {
                            ParameterHibernateMapping map = new ParameterHibernateMapping();
                            map.OptionGroup = optionGroup;
                            map.Parameter   = attr.Name;
                            map.Value       = objectValue.ToString();
                            values.Add(map);                            
                        }
                    }
                }
            }
            return values;
        }
        /// <summary>
        /// Converts a parameter object into a string.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static List<string> ConvertParameterObjectToStrings(object o)
        {
            List<string> values = new List<string>();
            if (o != null)
            {
                foreach (var prop in o.GetType().GetProperties())
                {
                    // Recurse to get parameters.
                    if (prop.CanRead)
                    {
                        object[] customAttributes = prop.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                        object potential = null;
                        if (customAttributes.Length > 0)
                            potential = prop.GetValue(o,
                                                    BindingFlags.GetProperty,
                                                    null,
                                                    null,
                                                    null);
                        for (int i = 0; i < customAttributes.Length; i++)
                        {

                            
                            if (potential != null)
                            {
                                try
                                {
                                    string value = string.Format("{0} = {1}", prop.Name, potential);
                                    values.Add(value);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }
                }
                foreach (FieldInfo field in o.GetType().GetFields())
                {
                    object objectValue = field.GetValue(o);
                    if (objectValue != null)
                        {
                            try
                            {
                                string value = string.Format("{0} = {1}", field.Name, objectValue);
                                values.Add(value);
                            }
                            catch
                            {
                            }
                        }
                    }
                
            }
            return values;
        }
    }
}
