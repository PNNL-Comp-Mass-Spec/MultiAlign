using System;
using System.Collections.Generic;

namespace MultiAlignCore.Data.Factors
{
	[Serializable]
	public class FactorInformation
	{
		//TODO: REFACTOR Encapsulate	
		//TODO: REFACTOR Bring up to coding standard

		/// <summary>
		/// Name of the factor.
		/// </summary>
		private string m_factorName;
		/// <summary>
		/// List of factor values.
		/// </summary>
		private List<string> m_factorValues;

		/// <summary>
		/// Default construtor.
		/// </summary>
		public FactorInformation()
		{
			m_factorName = "";
			m_factorValues = new List<string>();
		}

		/// <summary>
		/// Gets if a factor is fully defined.
		/// </summary>
		public bool IsFullyDefined
		{
			get
			{
				return m_factorValues.Count > 1;
			}
		}

		/// <summary>
		/// Gets or sets the name of the factor.
		/// </summary>
		public string FactorName
		{
			get
			{
				return m_factorName;
			}
			set
			{
				m_factorName = value;
			}
		}
		/// <summary>
		/// Gets or sets the list of factor values associated with this factor.
		/// </summary>
		public List<string> FactorValues
		{
			get
			{
				return m_factorValues;
			}
			set
			{
				m_factorValues = value;
			}
		}
	}
}
