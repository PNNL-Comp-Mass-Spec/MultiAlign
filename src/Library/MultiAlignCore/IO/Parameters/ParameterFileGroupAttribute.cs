#region

using System;

#endregion

namespace MultiAlignCore.IO.Parameters
{
    /// <summary>
    ///     Attribute to tag an options class for parameter file persistence.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ParameterFileGroupAttribute : Attribute
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="name">Name of the group.</param>
        /// <param name="fullDescription">Description of the group.</param>
        public ParameterFileGroupAttribute(string name, string fullDescription)
        {
            Name = name;
            FullDecription = fullDescription;
        }

        /// <summary>
        ///     Gets the description of the group.
        /// </summary>
        public string FullDecription { get; private set; }

        /// <summary>
        ///     Gets the name of the group.
        /// </summary>
        public string Name { get; private set; }
    }
}