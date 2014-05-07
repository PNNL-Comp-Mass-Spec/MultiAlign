using System;

namespace MultiAlignCore.IO.Parameters
{
    /// <summary>
    /// Attribute class for persisting primitive type properties to a parameter file.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ParameterFileAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        public ParameterFileAttribute(string name, string groupName)
        {
            Name            = name;
            GroupName       = groupName;            
        }
        /// <summary>
        /// Gets the name of the Parameter
        /// </summary>
        public string Name 
        { 
            get; 
            private set; 
        }
        /// <summary>
        /// Gets the group the parameter belongs to.
        /// </summary>
        public string GroupName 
        { 
            get; 
            private set; 
        }
    }
}
