using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignCore.IO.Options
{
    /// <summary>
    /// Used to transform between <see cref="MultiAlignAnalysisOptions"/> and List of <see cref="OptionPair"/>
    /// </summary>
    public static class OptionsTransformer
    {
        /// <summary>
        /// Reads all non-ignored properties from options and contained classes into a list of <see cref="OptionPair"/>
        /// </summary>
        /// <param name="options">Options to parse into a List of <see cref="OptionPair"/></param>
        /// <returns></returns>
        /// <remarks>Ignore a property using a <see cref="IgnoreOptionProperty"/> attribute</remarks>
        public static IList<OptionPair> PropertiesToList(MultiAlignAnalysisOptions options)
        {
            var list = new List<OptionPair>();
            GetProperties(list, options);
            return list;
        }

        /// <summary>
        /// Parses a list of <see cref="OptionPair"/> into a new MultiAlignAnalysisOptions
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static MultiAlignAnalysisOptions ListToProperties(IList<OptionPair> options)
        {
            return ParseProperties(typeof(MultiAlignAnalysisOptions), options) as MultiAlignAnalysisOptions;
        }

        /// <summary>
        /// Copy all non-ignored properties from optionsClass into list, with the name prefix of scope
        /// </summary>
        /// <param name="list">where to store the processed properties</param>
        /// <param name="optionsClass">object from which to process properties</param>
        /// <param name="scope">name prefix, dot-separated, ending with a dot.</param>
        private static void GetProperties(IList<OptionPair> list, object optionsClass, string scope = "")
        {
            var optType = optionsClass.GetType();

            // Iterate through all of the properties
            foreach (var property in optType.GetProperties())
            {
                // Check for a custom attribute
                //if (property.CustomAttributes.Any(x => x.AttributeType.Equals(typeof(IgnoreOptionProperty))))
                if (Attribute.IsDefined(property, typeof(IgnoreOptionProperty)))
                {
                    // Ignore the property
                    continue;
                }
                
                // Get the type of the property
                var propType = property.PropertyType;

                // basic types don't need additional special handling
                if (propType.IsValueType || propType.IsEnum)
                {
                    var name = scope + property.Name;
                    var value = property.GetValue(optionsClass).ToString();
                    list.Add(new OptionPair(name, value));
                }
                // if it's a class in the namespace of "MultiAlignCore.xyz", we want to do special handling
                else if (propType.IsClass && propType.FullName.StartsWith(optType.FullName.Substring(0, 15)))
                {
                    if (property.GetValue(optionsClass) != null)
                    {
                        GetProperties(list, property.GetValue(optionsClass), scope + property.Name + ".");
                    }
                }
                // TODO: add handling for collections of basic types.
            }
        }

        /// <summary>
        /// Parses list into an object of type optionsClass
        /// </summary>
        /// <param name="optionsClass"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static object ParseProperties(Type optionsClass, IList<OptionPair> list)
        {
            // create a new object of type optionsClass
            var options = optionsClass.Assembly.CreateInstance(optionsClass.FullName);

            // break the dictionary down into layers
            var objDict = new Dictionary<string, List<OptionPair>>();
            var valDict = new Dictionary<string, string>();
            foreach (var option in list)
            {
                // break off the first prefix, if there is one
                string basename = option.Name;
                string subkey = string.Empty;
                var dot = basename.IndexOf(".");
                // prefixed: is part of an object that needs to be handled separately
                if (dot > 0)
                {
                    subkey = basename.Substring(dot + 1);
                    basename = basename.Substring(0, dot);

                    // store it as a separate list of items for the specified prefix
                    if (!objDict.ContainsKey(basename))
                    {
                        objDict.Add(basename, new List<OptionPair>());
                    }
                    objDict[basename].Add(new OptionPair(subkey, option.Value));
                }
                // Not prefixed: is a basic type in this object
                else
                {
                    if (valDict.ContainsKey(basename))
                    {
                        valDict[basename] = option.Value;
                    }
                    else
                    {
                        valDict.Add(basename, option.Value);
                    }
                }
            }

            // iterate through all properties in the object, and modify with those that have a match in the dictionaries
            foreach (var property in optionsClass.GetProperties())
            {
                // Enums: must use Enum.Parse to translate from string back into Enum 
                if (property.PropertyType.IsEnum && valDict.ContainsKey(property.Name))
                {
                    var val = valDict[property.Name];
                    property.SetValue(options, Enum.Parse(property.PropertyType, val));
                }
                // Other basic types: Convert.ChangeType can convert the values from string to the necessary value
                else if (valDict.ContainsKey(property.Name))
                {
                    var val = valDict[property.Name];
                    property.SetValue(options, Convert.ChangeType(val, property.PropertyType));
                }
                // Objects: Call this function iteratively
                else if (objDict.ContainsKey(property.Name))
                {
                    property.SetValue(options, ParseProperties(property.PropertyType, objDict[property.Name]));
                }
                // TODO: add handling for collections of basic types.
            }

            return options;
        }
    }
}