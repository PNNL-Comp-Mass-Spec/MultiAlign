using System;

namespace MultiAlignCore.IO.Options
{
    public class OptionPair
    {
        /// <summary>
        /// The name of the option
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The value of the option
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Initialize an OptionPair class with the specified values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public OptionPair(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Private constructor for NHibernate
        /// </summary>
        private OptionPair()
        { }

        /// <summary>
        /// Overridden ToString() for debugging views
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("\"{0}\",\"{1}\"", Name, Value);
        }
    }
}
