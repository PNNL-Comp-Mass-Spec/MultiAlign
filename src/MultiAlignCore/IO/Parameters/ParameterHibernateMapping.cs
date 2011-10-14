using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.IO.Parameters
{
    public class ParameterHibernateMapping
    {
        #region Properties
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        public string Parameter
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the option group.
        /// </summary>
        public string OptionGroup
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public string Value
        {
            get;
            set;
        }
        #endregion

        public override bool Equals(object obj)
        {
            ParameterHibernateMapping map = obj as ParameterHibernateMapping;
            if (map == null)
                return false;

            return Parameter == map.Parameter && Value == map.Value && OptionGroup == map.OptionGroup;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ Parameter.GetHashCode() ^ OptionGroup.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", OptionGroup, Parameter, Value);
        }
    }
}
