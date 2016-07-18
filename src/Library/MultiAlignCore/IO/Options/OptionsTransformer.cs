using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignCore.IO.Options
{
    /// <summary>
    /// Used to transform between <see cref="MultiAlignAnalysisOptions"/> and List of <see cref="OptionPair"/>
    /// </summary>
    /// <remarks>Will not work with Immutable types besides strings; the properties must have public set accessors for them to be properly set.
    /// This means that Tuples and KeyValuePairs (and any collection than uses them, including 'Dictionary's and 'SortedLists').
    ///
    /// This can store arrays to the database, but will not convert from collections to arrays; collections that need to be stored to the
    /// database should be "ignored", and have a separate get/set property that converts the collection to/from an array.
    /// This puts the control of how the collection is configured in the hands of the object.
    /// The restriction on this is that the size of the collections should be small, generally less than 20 items.
    /// If they are larger, it would probably be better to have them in their own table.</remarks>
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

                // Test to make sure the property has both get and set accessors
                if (!(property.CanRead && property.CanWrite))
                {
                    throw new NotSupportedException(
                        "Operation requires get and set accessors for all persisted properties. " +
                        "Add the 'IgnoreOptionProperty' attribute to ignore the failing property. Property info: Class '" +
                        optionsClass.GetType().FullName + "', property '" + property.Name + "'.");
                }

                // Array handling (only supported collection)
                if (propType.IsArray)
                {
                    var array = property.GetValue(optionsClass) as Array;
                    var nameBase = scope + property.Name + "#";
                    if (array != null && array.Length > 0)
                    {
                        // TODO: Should this be changed to support an array of "object"?
                        var arrayType = property.PropertyType.GetElementType();
                        var isValueOrEnumType = false;
                        var isMultiAlignClass = false;
                        if (arrayType.IsValueType || arrayType.IsEnum || arrayType == typeof(string))
                        {
                            isValueOrEnumType = true;
                        }
                        else if (arrayType.IsClass && propType.FullName.StartsWith(optType.FullName.Substring(0, 15)))
                        {
                            isMultiAlignClass = true;
                        }
                        // TODO: Could handle arrays of arrays

                        // Store the values of the array appropriately
                        for (var i = 0; i < array.Length; i++)
                        {
                            var name = nameBase + i;
                            if (isValueOrEnumType)
                            {
                                list.Add(new OptionPair(name, array.GetValue(i).ToString()));
                            }
                            else if (isMultiAlignClass)
                            {
                                if (array.GetValue(i) != null)
                                {
                                    GetProperties(list, array.GetValue(i), name + ".");
                                }
                            }
                        }
                    }
                }
                // basic types don't need additional special handling
                else if (propType.IsValueType || propType.IsEnum || propType == typeof(string))
                {
                    var name = scope + property.Name;
                    var value = property.GetValue(optionsClass);
                    var valueStr = value == null ? null : value.ToString();
                    list.Add(new OptionPair(name, valueStr));
                }
                // if it's a class in the namespace of "MultiAlignCore.xyz", we want to do special handling
                else if (propType.IsClass && propType.FullName.StartsWith(optType.FullName.Substring(0, 15)))
                {
                    if (property.GetValue(optionsClass) != null)
                    {
                        GetProperties(list, property.GetValue(optionsClass), scope + property.Name + ".");
                    }
                }
                // Notify the developer when they try to use a non-supported type, like a generic collection.
                else
                {
                    throw new NotSupportedException(
                        "Object type not supported. If it is a generic collection, provide access with a property returning an array. " +
                        "Add the 'IgnoreOptionProperty' attribute to ignore the failing property. Type info: Class '" +
                        optionsClass.GetType().FullName + "', property '" + property.Name + "'.");
                }
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
            var options = Activator.CreateInstance(optionsClass);

            // break the dictionary down into layers
            var objDict = new Dictionary<string, List<OptionPair>>();
            var valDict = new Dictionary<string, string>();
            var arrayValDict = new Dictionary<string, Dictionary<int, string>>();
            var arrayObjDict = new Dictionary<string, Dictionary<int, List<OptionPair>>>();
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

                    // Array of objects
                    if (basename.Contains("#"))
                    {
                        var hash = basename.IndexOf("#");
                        var indexStr = basename.Substring(hash + 1);
                        var index = Convert.ToInt32(indexStr);
                        var name = basename.Substring(0, hash);

                        if (!arrayObjDict.ContainsKey(name))
                        {
                            arrayObjDict.Add(name, new Dictionary<int, List<OptionPair>>());
                        }
                        if (!arrayObjDict[name].ContainsKey(index))
                        {
                            arrayObjDict[name].Add(index, new List<OptionPair>());
                        }
                        arrayObjDict[name][index].Add(new OptionPair(subkey, option.Value));
                    }
                    // object
                    else
                    {
                        // store it as a separate list of items for the specified prefix
                        if (!objDict.ContainsKey(basename))
                        {
                            objDict.Add(basename, new List<OptionPair>());
                        }
                        objDict[basename].Add(new OptionPair(subkey, option.Value));
                    }
                }
                // Not prefixed: is a basic type in this object
                else
                {
                    // Array of basic types
                    if (basename.Contains("#"))
                    {
                        var hash = basename.IndexOf("#");
                        var indexStr = basename.Substring(hash + 1);
                        var index = Convert.ToInt32(indexStr);
                        var name = basename.Substring(0, hash);

                        if (!arrayValDict.ContainsKey(name))
                        {
                            arrayValDict.Add(name, new Dictionary<int, string>());
                        }
                        if (!arrayValDict[name].ContainsKey(index))
                        {
                            arrayValDict[name][index] = option.Value;
                        }
                        else
                        {
                            arrayValDict[name].Add(index, option.Value);
                        }
                    }
                    // Basic type
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
                // Array of value/enum types
                else if (arrayValDict.ContainsKey(property.Name))
                {
                    var count = arrayValDict[property.Name].Count;
                    var type = property.PropertyType.GetElementType();
                    var array = Array.CreateInstance(type, count);

                    foreach (var item in arrayValDict[property.Name])
                    {
                        if (type.IsEnum)
                        {
                            array.SetValue(Enum.Parse(type, item.Value), item.Key);
                        }
                        else
                        {
                            array.SetValue(Convert.ChangeType(item.Value, type), item.Key);
                        }
                    }
                    property.SetValue(options, Convert.ChangeType(array, property.PropertyType));
                }
                // Array of custom MultiAlign objects
                else if (arrayObjDict.ContainsKey(property.Name))
                {
                    var count = arrayObjDict[property.Name].Count;
                    var type = property.PropertyType.GetElementType();
                    var array = Array.CreateInstance(type, count);

                    foreach (var item in arrayObjDict[property.Name])
                    {
                        array.SetValue(ParseProperties(type, item.Value), item.Key);
                    }
                    property.SetValue(options, Convert.ChangeType(array, property.PropertyType));
                }
            }

            return options;
        }
    }
}