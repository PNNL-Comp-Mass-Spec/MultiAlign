#pragma once
#include "clsAlignmentOptions.h" 
#include "clsUtilities.h"
#using <mscorlib.dll>
#include <vector>
namespace MultiAlignEngine
{
	namespace Alignment
	{
		inline float Interpolate(float (&x) __gc[], float (&y) __gc[], float valX, bool interpolateLast)
		{
			int index = System::Array::BinarySearch(x, __box(valX)); 
			if (index < 0)
			{
				// the next higher value. 
				index = ~index; 
				if (index == x->Length)
				{
					if (!interpolateLast)
						return y[index-1]; 
					index--; 
				}
				float xUp = x[index]; 
				float xDown = x[index-1]; 
				float yUp = y[index]; 
				float yDown = y[index-1];
				// do a linear interpolation between the two.
				float interpVal = (valX-xDown)/(xUp - xDown) * yUp + (xUp-valX)/(xUp - xDown) * yDown;
				return interpVal; 
			}
			// exact match. return value.
			return y[index]; 				
		}

		[System::Serializable]
		public __gc class clsAlignmentFunction
		{
		public:
			System::String *mstrDataset; 
			System::String *mstrReference; 

			enmCalibrationType menmCalibrationType; 
			enmAlignmentType menmAlignmentType; 

			float marrNETFncTimeInput __gc[]; 
			float marrNETFncNETOutput __gc[]; 

			// in the case that alignment was performed to a MS dataset, there can be a mapping to scan numbers.
			// in that case this variable is set. However, if the net of the MS dataset used as baseline was computed
			// by aligning it to a different database, remember that the marrNETFncNETOutput will be in the scale of 
			// that MS dataaset, while this TimeOutput will be in the scale of the current dataset and probably not at all
			// valid. 
			float marrNETFncTimeOutput __gc[]; 

			float marrMassFncTimeInput __gc[]; 
			float marrMassFncTimePPMOutput __gc[]; 

			float marrMassFncMZInput __gc[]; 
			float marrMassFncMZPPMOutput __gc[]; 

			clsAlignmentFunction(System::String* alignee, System::String* reference, enmCalibrationType calibType,
				enmAlignmentType alignmentType); 
			clsAlignmentFunction(enmCalibrationType calibType, enmAlignmentType alignmentType);  
			~clsAlignmentFunction(); 

			void SetNETFunction(float (&time) __gc[], float (&net) __gc[])
			{
				if(time->Length == 0)
				{
					throw new System::ArgumentException("Input NET Calibration Function with time has no time data.", "time"); 
				}
				if(net->Length == 0)
				{
					throw new System::ArgumentException("Input NET Calibration Function with time has no net data.", "net"); 
				}

				marrNETFncTimeInput = dynamic_cast<float __gc[]> (time->Clone() ); 
				marrNETFncNETOutput = dynamic_cast<float __gc[]> (net->Clone() ); 
			}
			void SetMassCalibrationFunctionWithTime(float (&time) __gc[], float (&ppm) __gc[])
			{
				//if (ppm == NULL)
				//{
				//	throw new System::ArgumentNullException("Mass Calibration function is null", "ppm"); 
				//}
				if(ppm->Length == 0)
				{
					throw new System::ArgumentException("Input Mass Calibration Function with time has no data.", "ppm"); 
				}

				if (menmAlignmentType != NET_MASS_WARP)
				{
					throw new System::InvalidOperationException(S"Recalibration of mass not enabled with NET_WARP alignment type. PPM shift cannot be retrieved. Use NET_MASS_WARP for aligment type."); 
				}
				if(menmCalibrationType == MZ_CALIB)
				{
					throw new System::InvalidOperationException(S"Attempting to set time calibration of masses when option chosen was MZ_CALIB"); 
				}
				System::Array::Copy(time, marrMassFncTimeInput, time->Length); 
				System::Array::Copy(ppm, marrMassFncTimePPMOutput, ppm->Length); 
			}
			void SetMassCalibrationFunctionWithMZ(float (&mz) __gc[], float (&ppm) __gc[])
			{
				//if (ppm == NULL)
				//{
				//	throw new System::ArgumentNullException("Mass Calibration function is null", "ppm"); 
				//}
				if(ppm->Length == 0)
				{
					throw new System::ArgumentException("Input Mass Calibration Function with time has no data.", "ppm"); 
				}

				if (menmAlignmentType != NET_MASS_WARP)
				{
					throw new System::InvalidOperationException(S"Recalibration of mass not enabled with NET_WARP alignment type. PPM shift cannot be retrieved. Use NET_MASS_WARP for aligment type."); 
				}
				if(menmCalibrationType == SCAN_CALIB)
				{
					throw new System::InvalidOperationException(S"Attempting to set mz calibration of masses when option chosen was SCAN_CALIB"); 
				}
				System::Array::Copy(mz, marrMassFncMZInput, mz->Length); 
				System::Array::Copy(ppm, marrMassFncMZPPMOutput, ppm->Length); 
			}

			float GetNETFromTime(float time)
			{
				return Interpolate(marrNETFncTimeInput, marrNETFncNETOutput, time, false); 
			}

			float GetPPMShiftFromTime(float time)
			{
				if (menmAlignmentType != NET_MASS_WARP)
				{
					throw new System::InvalidOperationException(S"Recalibration of mass not enabled with NET_WARP aligment type. PPM shift cannot be retrieved. Use NET_MASS_WARP for alignment type."); 
				}
				if(menmCalibrationType == MZ_CALIB)
				{
					throw new System::InvalidOperationException(S"Attempting to get time calibration of masses when option chosen was MZ_CALIB"); 
				}
				return Interpolate(marrMassFncTimeInput, marrMassFncTimePPMOutput, time, false); 
			}
			float GetPPMShiftFromMZ(float mz)
			{
				if (menmAlignmentType != NET_MASS_WARP)
				{
					throw new System::InvalidOperationException(S"Recalibration of mass not enabled with NET_WARP alignment type. PPM shift cannot be retrieved. Use NET_MASS_WARP for alignment type"); 
				}
				if(menmCalibrationType == SCAN_CALIB)
				{
					throw new System::InvalidOperationException(S"Attempting to get mz calibration of masses when option chosen was SCAN_CALIB"); 
				}
				return Interpolate(marrMassFncMZInput, marrMassFncMZPPMOutput, mz, false); 
			}
			float GetPPMShiftFromTimeMZ(float time, float mz)
			{
				if (menmAlignmentType != NET_MASS_WARP)
				{
					throw new System::InvalidOperationException(S"Recalibration of mass not enabled with NET_WARP alignment type. PPM shift cannot be retrieved. Use NET_MASS_WARP for alignment type"); 
				}
				if(menmCalibrationType != HYBRID_CALIB)
				{
					throw new System::InvalidOperationException(S"Attempting to get hybrid calibration of masses when option chosen was not HYBRID_CALIB"); 
				}
				return GetPPMShiftFromMZ(mz) + GetPPMShiftFromTime(time); 
			}

			void SetNETFunction(std::vector<double> &vectAligneeTimes, std::vector<double> &vectReferencesNETs)
			{
				clsUtilities::Copy(vectAligneeTimes, marrNETFncTimeInput);  
				clsUtilities::Copy(vectReferencesNETs, marrNETFncNETOutput);  
			}
			void SetNETFunction(std::vector<double> &vectAligneeTimes, std::vector<double> &vectReferenceNETs, 
				std::vector<double> &vectReferenceScans)
			{
				clsUtilities::Copy(vectAligneeTimes, marrNETFncTimeInput);  
				clsUtilities::Copy(vectReferenceNETs, marrNETFncNETOutput);  
				clsUtilities::Copy(vectReferenceScans, marrNETFncTimeOutput);  
			}
			void SetMassCalibrationFunctionWithTime(std::vector<double> &vectAligneeTimes, std::vector<double> &vectPPMShifts)
			{
				clsUtilities::Copy(vectAligneeTimes, marrMassFncTimeInput);  
				clsUtilities::Copy(vectPPMShifts, marrMassFncTimePPMOutput);  
			}
			void SetMassCalibrationFunctionWithMZ(std::vector<double> &vectAligneeMZs, std::vector<double> &vectPPMShifts)
			{
				clsUtilities::Copy(vectAligneeMZs, marrMassFncMZInput);  
				clsUtilities::Copy(vectPPMShifts, marrMassFncMZPPMOutput);  
			}
		};
	}
}