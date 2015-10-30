using System;
using System.Collections.Generic;
using InformedProteomics.Backend.Data.Spectrometry;

namespace MultiAlignCore.Data.Features
{
	/// <summary>
	/// MS Feature class that describes a raw or deisotoped feature.
	/// </summary>
    public class MSFeatureLight : FeatureLight, IComparable<MSFeatureLight>, IChildFeature<UMCLight>
	{

	    public MSFeatureLight()
	    {	        
            MSnSpectra  = new List<MSSpectra>();
            MsPeakList  = new List<Peak>();
            MassMonoisotopicMostAbundant = 0;
            MsMsCount   = 0;
            Umc         = null;
	    }

        #region AutoProperties
        /// <summary>
		/// The list of MSPeaks that make up the MSFeature.  This would be the isotopic distribution.
        /// </summary>
        public List<Peak> MsPeakList { get; set; }        
		/// <summary>
		/// The UMC associated with this MS Feature.
		/// </summary>
        public UMCLight Umc { get; set; }
        /// <summary>
        /// Gets or sets the average monoisotopic mass. 
        /// </summary>
        public double MassMonoisotopicAverage { get; set; }
        /// <summary>
        /// Gets or sets the most abundant monoisotopic mass from the isotopic distribution.
        /// </summary>
        public double MassMonoisotopicMostAbundant { get; set; }
        /// <summary>
        /// Gets or sets the list of potential MS/MS (MSn) spectra associated with this feature.
        /// </summary>
        public override List<MSSpectra> MSnSpectra { get; set; }
		#endregion

        public override double MassMonoisotopicAligned
        {
            get
            {
                return MassMonoisotopic;
            }
            set
            {
                MassMonoisotopic = value;
            }
        }


		#region IComparable<MSFeature> Members
		/// <summary>
		/// Default Comparer used for the MSFeature class. Sorts by Monoisotopic Mass descending.
		/// </summary>
		public int CompareTo(MSFeatureLight other)
		{
			return other.MassMonoisotopic.CompareTo(MassMonoisotopic);
		}
		#endregion
     
        #region IChildFeature<UMCLight> Members

        public void SetParentFeature(UMCLight parentFeature)
        {
            Umc   = parentFeature;
            UmcId = Umc.Id;
        }

	    public UMCLight GetParentFeature()
	    {
	        return Umc;
	    }

        public int UmcId
        {
            get;
            set;
        }
        #endregion

	    public override double GetSeparationValue(SeparationTypes type)
	    {
	        throw new NotImplementedException();
	    }

	    public override void SetSeparationValue(SeparationTypes type, double value)
	    {
	        throw new NotImplementedException();
	    }

	    public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + GroupId.GetHashCode();
            hash = hash * 23 + MassMonoisotopicMostAbundant.GetHashCode();
            hash = hash * 23 + MassMonoisotopic.GetHashCode();
            hash = hash * 23 + Net.GetHashCode();

            return hash;
        }
    }
}
