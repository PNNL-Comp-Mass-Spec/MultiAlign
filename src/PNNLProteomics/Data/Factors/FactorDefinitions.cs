using System;
using System.Collections;
using System.Collections.Generic;

namespace PNNLProteomics.Data.Factors
{
    /// <summary>
    /// Enumerates the possible ways a factor can be edited.
    /// </summary>
    public enum FactorEditResult
    {
        Added,
        Exists,
        Renamed,
        IncorrectFormat,
        DoesNotExist
    }

    /// <summary>
    /// Class that holds all factor definitions.
    /// </summary>
    public class FactorCollection: ICloneable<FactorCollection>
    {        
        /// <summary>
        /// List of available factors.
        /// </summary>
        private Dictionary<string, FactorInformation> m_factors;
        		
        /// <summary>
        /// Default constructor for a factor definition class.
        /// </summary>
		public FactorCollection()
		{
            m_factors = new Dictionary<string, FactorInformation>();
        }

        #region Properties 
        /// <summary>
        /// Gets whether the factors are fully defined or not.
        /// </summary>
        /// <returns>True if all factors have at least 2 factor values.  False if one does not.</returns>
        public bool AreAllFactorsDefined
        {
            get
            {                
                foreach (FactorInformation information in m_factors.Values)
                {
                    if (!information.IsFullyDefined)
                        return false;                    
                }
                return true;
            }                        
        }
        /// <summary>
        /// Gets the dictionary of factor-factor value pairs.
        /// </summary>
        public Dictionary<string, FactorInformation> Factors
        {
            get            
            {
                return m_factors;
            }
        }
        #endregion

        #region Adding Factors/Factor Values
        /// <summary>
        /// Creates a new factor.
        /// </summary>
        /// <param name="factor">Factor to create.</param>
        /// <returns>A enumerated result detailing if the factor was created or not.</returns>
        public FactorEditResult AddFactor(string factorName)
        {
            // Make sure the factor is in the right format.
            if (string.IsNullOrEmpty(factorName))
            {
                return FactorEditResult.IncorrectFormat;
            }
            
            // Make sure the factor does not already exist.
            if (m_factors.ContainsKey(factorName))
                return FactorEditResult.Exists;

            // Create the new factor.
            FactorInformation newFactor = new FactorInformation();
            newFactor.FactorName = factorName;
            m_factors.Add(factorName, newFactor);
            return FactorEditResult.Added;
        }
        /// <summary>
        /// Adds factor information to the collection.
        /// </summary>
        /// <param name="information"></param>
        /// <returns></returns>
        public FactorEditResult AddFactor(FactorInformation information)
        {
            if (m_factors.ContainsKey(information.FactorName))
                return FactorEditResult.Exists;
            m_factors.Add(information.FactorName, information);
            return FactorEditResult.Added;
        }

        /// <summary>
        /// Add the factor value to the factor name.
        /// </summary>
        /// <param name="value">Value to add to the currently selected value.</param>
        /// <returns>Result indicating whether the factor value was added or not. </returns>
        public FactorEditResult AddFactorValue(string factorName, string factorValue)
        {
            // Make sure the factor and factor value is in the correct format.
            if (string.IsNullOrEmpty(factorValue))            
                return FactorEditResult.IncorrectFormat;
            
            if (string.IsNullOrEmpty(factorName))            
                return FactorEditResult.IncorrectFormat;
            
            // Make sure the factor exists.
            if (!m_factors.ContainsKey(factorName))                        
                return FactorEditResult.DoesNotExist;
            
            // Make sure the factor value does not exist.
            List<string> values = m_factors[factorName].FactorValues;
            if (values.Contains(factorValue))            
                return FactorEditResult.Exists;
            
            // Finally add if no conflicts exist.
            values.Add(factorValue);
            return FactorEditResult.Added;
        }
        #endregion

        #region Deleting Factors/Factor Values
        /// <summary>
        /// Deletes the currently selected tree node from the tree view.
        /// </summary>
        public bool DeleteFactor(string factorName)
        {
            if (string.IsNullOrEmpty(factorName))
                return false;

            /// 
            /// See if we need to delete to factor
            /// 
            if (!m_factors.ContainsKey(factorName))
                return false;

            m_factors.Remove(factorName);
            return true;
        }
        /// <summary>
        /// Removes the factor value from the specified factor key.
        /// </summary>
        /// <returns></returns>
        public bool DeleteFactorValue(string factorName, string factorValue)
        {
            if (string.IsNullOrEmpty(factorName))
                return false;

            if (string.IsNullOrEmpty(factorValue))
                return false;

            if (!m_factors.ContainsKey(factorName))
                return false;

            FactorInformation information = m_factors[factorName];
            if (!information.FactorValues.Contains(factorValue))
                return false;

            information.FactorValues.Remove(factorValue);
            return true;
        }
        #endregion  

        #region Re-naming
        /// <summary>
        /// Renames the factor name.
        /// </summary>
        /// <param name="factorName">Name of the factor to rename.</param>
        /// <returns>CONST_FACTOR_RENAMED if rename is successful.</returns>
        public FactorEditResult RenameFactor(string oldFactorName, string newFactorName)
        {
            if (!m_factors.ContainsKey(oldFactorName))
                return FactorEditResult.DoesNotExist;

            if (m_factors.ContainsKey(newFactorName) == true)
                return FactorEditResult.Exists;
            
            // Create a new deep list reference of the values
            FactorInformation information = m_factors[oldFactorName];

            /// 
            /// Remove the old, add the new
            /// 
            m_factors.Remove(oldFactorName);
            m_factors.Add(newFactorName, information);

            return FactorEditResult.Renamed;
        }
        /// <summary>
        /// Renames the factor value.
        /// </summary>
        /// <param name="factorName">Name of the factor the value 
        /// is contained in.</param>
        /// <param name="factorValue">Name of the factor value to 
        /// rename.</param>
        /// <returns>CONST_FACTOR_VALUE_RENAMED if factor value rename is successful.</returns>
        public FactorEditResult RenameFactorValue(string factorName,
                            string oldFactorValue, string newFactorValue)
        {
            if (!m_factors.ContainsKey(factorName))
                return FactorEditResult.DoesNotExist;
         
            FactorInformation information = m_factors[factorName];
            if (!information.FactorValues.Contains(oldFactorValue))
                return FactorEditResult.DoesNotExist;
            if (information.FactorValues.Contains(newFactorValue))
                return FactorEditResult.Exists;

            information.FactorValues.Remove(oldFactorValue);
            information.FactorValues.Add(newFactorValue);

            return FactorEditResult.Renamed;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Returns a new object of a cloned type.
        /// </summary>
        /// <returns></returns>
        public FactorCollection Clone()
        {
            FactorCollection definitions = new FactorCollection();
            foreach (FactorInformation information in this.Factors.Values)
            {
                FactorInformation newInformation = new FactorInformation();
                newInformation.FactorName = information.FactorName;
                newInformation.FactorValues.AddRange(information.FactorValues);

                definitions.AddFactor(newInformation);
            }
            return definitions;
        }

        #endregion
    }      
}
