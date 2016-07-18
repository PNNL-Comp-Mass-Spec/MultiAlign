using System;

namespace MultiAlignCore.IO.Options
{
    /// <summary>
    /// Attribute for properties that should not be persisted to the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreOptionProperty : Attribute
    {
    }
}