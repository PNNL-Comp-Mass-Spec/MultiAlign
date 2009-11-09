using System;

namespace MultiAlignWin.UMCFinding
{
	/// <summary>
	/// Summary description for clsUMCFindingOptions.
	/// </summary>
	public class clsUMCFindingOptions
	{		
		float mfltWtMonoMass = 0.01F ; 
		float mfltWtAveMass = 0.01F ; 
		float mfltWtLogDistance = 0.1F ; 
		float mfltWtScan = 0.01F ; 
		float mfltWtFit =0.1F ; 
		float mfltWtNET =0.1F ; 

		float mfltConstraintMonoMass = 10.0F ; // in ppm 
		float mfltConstraintAveMass = 10.0F ; // in ppm; 

		double mdblMaxDistance = 0.1 ; 
		bool mblnUseNET = true ; 

		public clsUMCFindingOptions()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public float MonoMassWeight
		{
			get
			{
				return mfltWtMonoMass ; 
			}
			set
			{
				mfltWtMonoMass = value ; 
			}
		}

		public float AveMassWeight
		{
			get
			{
				return mfltWtAveMass ; 
			}
			set
			{
				mfltWtAveMass = value ; 
			}
		}

		public float LogDistanceWeight
		{
			get
			{
				return mfltWtLogDistance ; 
			}
			set
			{
				mfltWtLogDistance = value ; 
			}
		}

		public float ScanWeight
		{
			get
			{
				return mfltWtScan ; 
			}
			set
			{
				mfltWtScan = value ; 
			}
		}

		public float FitWeight
		{
			get
			{
				return mfltWtFit ; 
			}
			set
			{
				mfltWtFit = value ; 
			}
		}

		public float NETWeight
		{
			get
			{
				return mfltWtNET ; 
			}
			set
			{
				mfltWtNET = value ; 
			}
		}


		public float ConstraintMonoMass
		{
			get
			{
				return mfltConstraintMonoMass ; 
			}
			set
			{
				mfltConstraintMonoMass = value ; 
			}
		}
		public float ConstraintAveMass
		{
			get
			{
				return mfltConstraintAveMass ; 
			}
			set
			{
				mfltConstraintAveMass = value ; 
			}
		}


		public double MaxDistance
		{
			get
			{
				return mdblMaxDistance ; 
			}
			set
			{
				mdblMaxDistance = value ; 
			}
		}
		public bool UseNET
		{
			get
			{
				return mblnUseNET ; 
			}
			set
			{
				mblnUseNET = value ; 
			}
		}
	}
}
