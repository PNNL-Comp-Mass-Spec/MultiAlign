#pragma once

#include "clsDataSummaryAttribute.h"
#include "clsParameterFileAttribute.h"
#include "classAlignmentMZBoundary.h"

using namespace System::Collections::Generic;

namespace MultiAlignEngine
{
	namespace Alignment
	{
		public __value enum enmAlignmentType { NET_WARP = 0, NET_MASS_WARP }; 
		public __value enum enmCalibrationType { MZ_CALIB = 0, SCAN_CALIB, HYBRID_CALIB }; 
		[System::Serializable]
		public __gc class clsAlignmentOptions
		{
			// if the dataset is aligned to a mass tag database. If false it is aligned to 
			// a dataset.
			bool mblnAlignToMassTagDatabase; 
			System::String* mstrAlignmentBaselineName; 

			/// <summary>
			/// number of time sections for LCMSWarp.
			/// </summary>
			int mintNumTimeSections; 
			/// <summary>
			/// contraction factor c
			/// </summary>
			short mshortContractionFactor; 
			/// <summary>
			///  max time jump (distortion in time alignment)
			/// </summary>
			short mshortMaxTimeDistortion; 
			/// <summary>
			/// maximum number of hits possible for a feature to still be considered in alignments.
			/// </summary>
			short mshortMaxPromiscuity; 
			/// <summary>
			// whether or not to consider features matching more than mshortMaxPromiscuity other features during alignment.
			/// </summary>
			bool mblnUsePromiscuousPoints; 
			/// <summary>
			/// whether or not to use least square fit in mass recalibration.
			/// </summary>
			bool mblnMassCalibUseLSQ; 
			/// <summary>
			/// number of ppm to consider in the original mass recalibrations.
			/// </summary>
			double mdblMassCalibrationWindow; 
			/// <summary>
			/// number of slices for the mass recalibration dynamic programming piece.
			/// </summary>
			short mshortMassCalibNumXSlices; 
			/// <summary>
			/// number of bins in the mass delta (Y) part for mass recalibration with dynamic programming.
			/// </summary>
			short mshortMassCalibNumYSlices; 
			/// <summary>
			/// maximum jumps for the mass recalibration with dynamic programming. 
			/// </summary>
			short mshortMassCalibMaxJump; 
			/// <summary>
			/// maximum ZScore to be considered for matches in dynamic programming.
			/// </summary>
			double mdblMassCalibMaxZScore; 
			/// <summary>
			/// maximum ZScore to be considered for matches in lsq .
			/// </summary>
			double mdblMassCalibLSQMaxZScore; 
			/// <summary>
			/// number of knots for lsq in mass recalibration;
			/// </summary>
			short mshortMassCalLSQNumKnots; 
			/// <summary>
			/// mass tolerance to be used for alignments
			/// </summary>
			double mdblMassTolerance; 
			/// Histogram binning option for net values.
			double mdouble_massBinSize;
			/// Histogram binning option for net values.
			double mdouble_driftTimeBinSize;
			/// <summary>
			/// NET tolerance to be used for aligments
			/// </summary>
			double mdblNETTolerance; 
			/// Histogram binning option for net values.
			double mdouble_netBinSize;
			/// <summary>
			/// type of alignment. 
			/// </summary>
			enmAlignmentType menmAlignmentType; 
			/// <summary>
			///  type of mass recalibration 
			/// </summary>
			enmCalibrationType menmCalibrationType;			
			/// <summary>
			/// Flag indicating whether to split in m/z space for alignment.
			/// </summary>
			bool   mbool_alignSplitMZs;
			
			/// <summary>
			/// Boundaries for m/z space to align certain sections of the code to first.
			/// </summary>
			List<classAlignmentMZBoundary*> *mlist_mzBoundaries;			

		public:
			clsAlignmentOptions(void);
			~clsAlignmentOptions(void);

			[clsDataSummaryAttribute("Number of Time Sections")]
			[clsParameterFileAttribute("NumTimeSections","AlignmentOptions")]
			__property int get_NumTimeSections()
			{
				return mintNumTimeSections; 
			}
			__property void set_NumTimeSections(int value)
			{
				mintNumTimeSections = value; 
			}

			/// Gets if to run the alignment by first splitting in M/Z space.			
			[clsDataSummaryAttribute("Split Alignment by M/Z")]
			[clsParameterFileAttribute("SplitAlignmentMZ","AlignmentOptions")]
			__property bool get_SplitAlignmentInMZ()
			{
				return mbool_alignSplitMZs;
			}
			/// Sets whether to split the alignment in M/Z space.
			__property void set_SplitAlignmentInMZ(bool value)
			{
				mbool_alignSplitMZs = value;
			}
			
			/// Gets if to run the alignment by first splitting in M/Z space.			
			[clsDataSummaryAttribute("Split Alignment M/Z Boundary")]
			__property List<classAlignmentMZBoundary*>* get_MZBoundaries()
			{
				return mlist_mzBoundaries;
			}
			/// Sets whether to split the alignment in M/Z space.
			__property void set_MZBoundaries(List<classAlignmentMZBoundary*> * value)
			{
				mlist_mzBoundaries = value;
			}

			[clsDataSummaryAttribute("Contraction Factor")] 
			[clsParameterFileAttribute("ContractionFactor","AlignmentOptions")]			
			__property short get_ContractionFactor()
			{
				return  mshortContractionFactor; 
			}
			
			__property void set_ContractionFactor(short value)
			{
				mshortContractionFactor = value; 
			}

			[clsDataSummaryAttribute("Max Time Jump")] 
			[clsParameterFileAttribute("MaxTimeJump", "AlignmentOptions")]			
			__property short get_MaxTimeJump()
			{
				return mshortMaxTimeDistortion; 
			}
			__property void set_MaxTimeJump(short value)
			{
				mshortMaxTimeDistortion = value; 
			}

			[clsDataSummaryAttribute("Max Promiscuity")] 
			[clsParameterFileAttribute("MaxPromiscuity","AlignmentOptions")]			
			__property short get_MaxPromiscuity()
			{
				return mshortMaxPromiscuity; 
			}
			__property void set_MaxPromiscuity(short value)
			{
				mshortMaxPromiscuity = value; 
			}

			[clsDataSummaryAttribute("Use Promiscuous Points")] 
			[clsParameterFileAttribute("UsePromiscuousPoints","AlignmentOptions")]
			__property bool get_UsePromiscuousPoints()
			{
				return mblnUsePromiscuousPoints; 
			}
			
			__property void set_UsePromiscuousPoints(bool value)
			{
				mblnUsePromiscuousPoints = value; 
			}

			
			[clsDataSummaryAttribute("Mass Calibration Use LSQ")] 
			[clsParameterFileAttribute("MassCalibrationUseLSQ","AlignmentOptions")]
			__property bool get_MassCalibrationUseLSQ()
			{
				return mblnMassCalibUseLSQ; 
			}
			
			__property void set_MassCalibrationUseLSQ(bool value)
			{
				mblnMassCalibUseLSQ = value; 
			}

			[clsDataSummaryAttribute("Mass Calibration Window")] 
			[clsParameterFileAttribute("MassCalibrationWindow","AlignmentOptions")]
			__property double get_MassCalibrationWindow()
			{
				return mdblMassCalibrationWindow; 
			}
			__property void set_MassCalibrationWindow(double value)
			{
				mdblMassCalibrationWindow = value; 
			}

			[clsDataSummaryAttribute("Mass Calibration Number of X Slices")]
			[clsParameterFileAttribute("MassCalibrationNumXSlices","AlignmentOptions")]
			__property short get_MassCalibrationNumXSlices()
			{
				return mshortMassCalibNumXSlices; 
			}
			
			__property void set_MassCalibrationNumXSlices(short value)
			{
				mshortMassCalibNumXSlices = value; 
			}

			
			[clsDataSummaryAttribute("Mass Calibration Number of Mass Delta Bins")] 
			[clsParameterFileAttribute("MassCalibrationNumMassDeltaBins","AlignmentOptions")]
			__property short get_MassCalibrationNumMassDeltaBins()
			{
				return mshortMassCalibNumYSlices; 
			}
			
			__property void set_MassCalibrationNumMassDeltaBins(short value)
			{
				mshortMassCalibNumYSlices = value; 
			}

			
			[clsDataSummaryAttribute("Mass Calibration Max Jump")] 
			[clsParameterFileAttribute("MassCalibrationMaxJump","AlignmentOptions")]
			__property short get_MassCalibrationMaxJump()
			{
				return mshortMassCalibMaxJump; 
			}
			
			__property void set_MassCalibrationMaxJump(short value)
			{
				mshortMassCalibMaxJump = value; 
			}

			
			[clsDataSummaryAttribute("Max Z-Score")] 
			[clsParameterFileAttribute("MassCalibrationMaxZScore","AlignmentOptions")]
			__property double get_MassCalibrationMaxZScore()
			{
				return mdblMassCalibMaxZScore; 
			}

			__property void set_MassCalibrationMaxZScore(double value)
			{
				mdblMassCalibMaxZScore = value; 
			}

			[clsDataSummaryAttribute("Mass Calibration LSQZScore")] 
			[clsParameterFileAttribute("MassCalibrationLSQZScore","AlignmentOptions")]
			__property double get_MassCalibrationLSQZScore()
			{
				return mdblMassCalibLSQMaxZScore;
			}
			
			__property void set_MassCalibrationLSQZScore(double value)
			{
				mdblMassCalibLSQMaxZScore = value; 
			}

			[clsDataSummaryAttribute("Mass Calibration LSQ Number Knots")]
			[clsParameterFileAttribute("MassCalibrationLSQNumKnots","AlignmentOptions")]
			__property short get_MassCalibrationLSQNumKnots()
			{
				return mshortMassCalLSQNumKnots; 
			}
			
			__property void set_MassCalibrationLSQNumKnots(short value)
			{
				mshortMassCalLSQNumKnots = value; 
			}

			[clsDataSummaryAttribute("Mass Tolerance")]
			[clsParameterFileAttribute("MassTolerance","AlignmentOptions")]
			__property double get_MassTolerance()
			{
				return mdblMassTolerance; 
			}
			__property void set_MassTolerance(double value)
			{
				mdblMassTolerance = value; 
			}

			[clsDataSummaryAttribute("Histogram Mass Bin Size")]
			[clsParameterFileAttribute("HistogramMassBinSize","AlignmentOptions")]
			__property double get_MassBinSize()
			{
				return mdouble_massBinSize;
			}
			__property void set_MassBinSize(double value)
			{
				mdouble_massBinSize = value; 
			}
			
			[clsDataSummaryAttribute("Net Tolerance")]
			[clsParameterFileAttribute("NETTolerance","AlignmentOptions")]
			__property double get_NETTolerance()
			{
				return mdblNETTolerance;
			}
			__property void set_NETTolerance(double value)
			{
				mdblNETTolerance = value; 
			}

			[clsDataSummaryAttribute("Histogram NET Bin Size")]
			[clsParameterFileAttribute("HistogramNETBinSize","AlignmentOptions")]
			__property double get_NETBinSize()
			{
				return mdouble_netBinSize;
			}
			__property void set_NETBinSize(double value)
			{
				mdouble_netBinSize = value; 
			}
			
			[clsDataSummaryAttribute("Histogram Drift Time Bin Size")]
			[clsParameterFileAttribute("HistogramDriftTimeBinSize","AlignmentOptions")]
			__property double get_DriftTimeBinSize()
			{
				return mdouble_driftTimeBinSize;
			}
			__property void set_DriftTimeBinSize(double value)
			{
				mdouble_driftTimeBinSize  = value; 
			}
			
			[clsDataSummaryAttribute("Apply Mass Recalibration")]
			[clsParameterFileAttribute("ApplyMassRecalibration","AlignmentOptions")]
			__property bool get_ApplyMassRecalibration()
			{
				return 	menmAlignmentType == NET_MASS_WARP; 

			}
			__property void set_ApplyMassRecalibration(bool value)
			{
				if (value)
					menmAlignmentType = NET_MASS_WARP; 
				else
					menmAlignmentType = NET_WARP; 
			}

			[clsDataSummaryAttribute("Recalibration Type")]
			[clsParameterFileAttribute("RecalibrationType","AlignmentOptions")]
			__property enmCalibrationType get_RecalibrationType()
			{
				return menmCalibrationType; 
			}

			__property void set_RecalibrationType(enmCalibrationType value)
			{
				menmCalibrationType = value; 
			}

			[clsDataSummaryAttribute("Alignment Type")]
			[clsParameterFileAttribute("AlignmentType","AlignmentOptions")]
			__property enmAlignmentType get_AlignmentType()
			{
				return menmAlignmentType; 
			}

			__property void set_AlignmentType(enmAlignmentType value)
			{
				menmAlignmentType = value; 
			}

			[clsDataSummaryAttribute("Alignment Baseline Name")]			
			__property System::String* get_AlignmentBaselineName()
			{
				return mstrAlignmentBaselineName; 
			}

			__property void set_AlignmentBaselineName(System::String *name)
			{
				mstrAlignmentBaselineName = name; 
			}

			[clsDataSummaryAttribute("Is Aligned to Mass Tag Database")]
			//[clsParameterFileAttribute("Is Aligned to Mass Tag Database")]
			__property bool get_IsAlignmentBaselineAMasstagDB()
			{
				return mblnAlignToMassTagDatabase; 
			}

			__property void set_IsAlignmentBaselineAMasstagDB(bool value)
			{
				mblnAlignToMassTagDatabase = value; 
			}
		};
	}
}