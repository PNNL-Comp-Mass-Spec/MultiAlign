using System;
using System.Collections;
using System.Collections.Generic;

namespace PNNLProteomics.Data.Factors
{
    public class classFactorDefinition: ICloneable
    {
        #region Constants
        /// <summary>
        /// Returned when a factor value is added succesfully.
        /// </summary>
        public const int CONST_FACTOR_VALUE_ADDED               = 0;
        /// <summary>
        /// Returned when a factor value is added and the factor value exists.
        /// </summary>
        public const int CONST_FACTOR_VALUE_EXISTS              = 1;
        /// <summary>
        /// Returned when the factor value adding is in the incorrect format.
        /// </summary>
        public const int CONST_FACTOR_VALUE_FORMAT_INCORRECT    = 2;
        /// <summary>
        /// Returned when the factor is added succesfully.
        /// </summary>
        public const int CONST_FACTOR_ADDED             = 3;
        /// <summary>
        /// Returned when the factor exists while adding.
        /// </summary>
        public const int CONST_FACTOR_EXISTS            = 4;
        /// <summary>
        /// Returned when the factor being added is in the incorrect format.
        /// </summary>
        public const int CONST_FACTOR_FORMAT_INCORRECT  = 5;
        /// <summary>
        /// Returned when renaming a factor.  The rename was successful.
        /// </summary>
        public const int CONST_FACTOR_RENAMED = 6;
        /// <summary>
        /// Returned when renaming a factor value.  Successful rename.
        /// </summary>
        public const int CONST_FACTOR_VALUE_RENAMED = 7;
        /// <summary>
        /// Returned when renaming a factor or factor value.
        /// The old factor does not exist.
        /// </summary>
        public const int CONST_FACTOR_DOES_NOT_EXIST = 8;
        /// <summary>
        /// Returned when renaming a factor value.  The old factor value
        /// does not exist.
        /// </summary>
        public const int CONST_FACTOR_VALUE_DOES_NOT_EXIST  = 9;
        /// <summary>
        /// Returned when renaming a factor.  The new factor already exists.
        /// </summary>
        public const int CONST_FACTOR_NEW_EXISTS            = 10;
        /// <summary>
        /// Returned when renaming a factor value.  The new factor value
        /// already exists.
        /// </summary>
        public const int CONST_FACTOR_VALUE_NEW_EXISTS      = 11;
        #endregion

        /// <summary>
        /// List of available factors.
        /// </summary>
        private Dictionary<string, List<string>> mdict_factors;
        		
        /// <summary>
        /// Default constructor for a factor definition class.
        /// </summary>
		public classFactorDefinition()
		{			
            mdict_factors = new Dictionary<string,List<string>>();
        }

        #region Properties 
        /// <summary>
        /// Gets whether the factors are fully defined or not.
        /// </summary>
        /// <returns>True if all factors have at least 2 factor values.  False if one does not.</returns>
        public bool FullyDefined
        {
            get
            {                
                foreach (string key in mdict_factors.Keys)
                {
                    List<string> list = mdict_factors[key];
                    if (list != null)
                    {
                        if (list.Count < 2) 
                            return false; 
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }                        
        }
        /// <summary>
        /// Gets the dictionary of factor-factor value pairs.
        /// </summary>
        public Dictionary<string, List<string>> Factors
        {
            get            
            {
                return mdict_factors;
            }
        }
        #endregion

        #region Adding Factors/Factor Values
        /// <summary>
        /// Adds a factor to the tree view.
        /// </summary>
        /// <param name="factor">Name of the factor to add.</param>
        /// <returns>0 if the factor was added.  1 if the factor exists. 
        /// 2 if the format was incorrect.</returns>
        public int AddFactor(string factor)
        {
            if (factor != null && factor.Trim() != "")
            {
                /// 
                /// Make sure that we have a list of factors, and that
                /// it does not contain 
                /// the factor name already.                
                /// 
                if (mdict_factors.ContainsKey(factor) == true)
                {
                    return CONST_FACTOR_EXISTS;
                }                                
                /// 
                /// Add a factor list for the factor key.
                /// 
                mdict_factors.Add(factor, new List<string>());
                return CONST_FACTOR_ADDED;
            }
            return CONST_FACTOR_FORMAT_INCORRECT;
        }
        /// <summary>
        /// Add the factor value to the factor node selected.
        /// </summary>
        /// <param name="value">Value to add to the currently selected value.</param>
        /// <returns>
        /// 0 if the value was added.
        /// 1 of the value was already present.
        /// 2 if the value was incorrect.  </returns>
        public int AddFactorValue(string factor, string value)
        {
            value = value.Replace(" ", "");
            if (value != null && value != "")
            {
                if (mdict_factors.ContainsKey(factor) == false)
                    return CONST_FACTOR_DOES_NOT_EXIST;

                List<string> list = mdict_factors[factor];
                if (list == null)
                    list = new List<string>();

                if (list.Contains(value) == false)
                {
                    list.Add(value);
                    return CONST_FACTOR_VALUE_ADDED;
                }
                else
                {
                    return CONST_FACTOR_VALUE_EXISTS;
                }
            }
            return CONST_FACTOR_VALUE_FORMAT_INCORRECT;
        }
        #endregion

        #region Deleting Factors/Factor Values
        /// <summary>
        /// Deletes the currently selected tree node from the tree view.
        /// </summary>
        public bool DeleteFactor(string factor)
        {
            bool result = false;
            factor = factor.Replace(" ", "");
            if (factor != null && factor != "")
            {
                /// 
                /// See if we need to delete to factor
                /// 
                if (mdict_factors.ContainsKey(factor) == true)
                {
                    mdict_factors.Remove(factor);
                    result = true;
                }                
            }
            return result;
        }
        /// <summary>
        /// Removes the factor value from the specified factor key.
        /// </summary>
        /// <returns></returns>
        public bool DeleteFactorValue(string factor, string factorValue)
        {
            bool result = false;
            factor = factor.Replace(" ", "");
            if (factor != null && factor != "")
            {
                if (factorValue != null && factorValue.Trim() != "")
                /// 
                /// See if we need to delete to factor
                /// 
                if (mdict_factors.ContainsKey(factor) == true)
                {
                    List<string> list = mdict_factors[factor];
                    if (list != null)
                    {
                        if (list.Contains(factorValue) == true)
                        {
                            list.Remove(factorValue);
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
        #endregion  

        #region Re-naming
        /// <summary>
        /// Renames the factor name.
        /// </summary>
        /// <param name="factorName">Name of the factor to rename.</param>
        /// <returns>CONST_FACTOR_RENAMED if rename is successful.</returns>
        public int RenameFactor(string oldFactorName, string newFactorName)
        {
            /// 
            /// Make sure the factor exists, 
            /// and that the new name does not exist already.
            /// 
            if (mdict_factors.ContainsKey(oldFactorName) == false)
            {
                return CONST_FACTOR_DOES_NOT_EXIST;
            }

            if (mdict_factors.ContainsKey(newFactorName) == true)
            {
                return CONST_FACTOR_NEW_EXISTS;
            }

            /// 
            /// Create a new deep list reference of the values
            /// 
            List<string> values = mdict_factors[oldFactorName];
            List<string> copy   = new List<string>();
            copy.AddRange(values.ToArray());

            /// 
            /// Remove the old, add the new
            /// 
            mdict_factors.Remove(oldFactorName);
            mdict_factors.Add(newFactorName, copy);

            return CONST_FACTOR_RENAMED;
        }
        /// <summary>
        /// Renames the factor value.
        /// </summary>
        /// <param name="factorName">Name of the factor the value 
        /// is contained in.</param>
        /// <param name="factorValue">Name of the factor value to 
        /// rename.</param>
        /// <returns>CONST_FACTOR_VALUE_RENAMED if factor value rename is successful.</returns>
        public int RenameFactorValue(string factorName,
                            string oldFactorValue, string newFactorValue)
        {
            /// 
            /// Make sure the factor exists, 
            /// and that: 
            ///     1.  the old factor value exists
            ///     2.  the new factor value is not used.
            /// 
            if (mdict_factors.ContainsKey(factorName) == false)
            {
                return CONST_FACTOR_DOES_NOT_EXIST;
            }

            List<string> values = mdict_factors[factorName];
            if (values != null)
            {
                if (values.Contains(oldFactorValue) == false)
                {
                    return CONST_FACTOR_VALUE_DOES_NOT_EXIST;
                }
                if (values.Contains(newFactorValue) == true)
                {
                    return CONST_FACTOR_VALUE_NEW_EXISTS;
                }

                values.Remove(oldFactorValue);
                values.Add(newFactorValue);
            }

            return CONST_FACTOR_VALUE_RENAMED;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clones all factors and their definitions.
        /// </summary>
        /// <returns>Reference to a new copy of this object.</returns>
        public object Clone()
        {
            /// Create a new factor definition object.
            classFactorDefinition factors = new classFactorDefinition();
            foreach (string key in mdict_factors.Keys)
            {
                /// 
                /// Create a new factor
                /// 
                factors.AddFactor(key);
                
                /// 
                /// Then add all its values as deep copies
                /// 
                List<string> values = mdict_factors[key];
                if (values != null)
                {

                    foreach (string value in values)
                    {
                        string newValue = value.Clone() as string;
                        factors.AddFactorValue(key, newValue);    
                    }
                }
            }
            return factors;
        }

        #endregion
    }      
}
