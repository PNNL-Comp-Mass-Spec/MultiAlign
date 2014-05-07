#include "umccreator.h"
#include "MemMappedReader.h"
#include <stdlib.h> 
#include <algorithm>
#include <iostream> 
#pragma warning(disable : 4996)


namespace MultiAlignEngine
{
	namespace UMCCreation
	{

		bool SortIsotopesByMonoMassAndScan(IsotopePeak &a, IsotopePeak &b) 
		{
			return (a.mdouble_mono_mass < b.mdouble_mono_mass);
		}

		UMCCreator::UMCCreator(void)
		{
			mflt_wt_mono_mass		= 0.01F;	// using ppms 10 ppm = lenght of 0.1 ppm
			mflt_wt_average_mass	= 0.01F; // using ppms 10 ppm = lenght of 0.1 ppm
			mflt_wt_log_abundance	= 0.1F; 
			mflt_wt_scan			= 0.01F; 
			mflt_wt_fit				= 0.1F; 
			mflt_wt_net				= 15; 

			mflt_constraint_mono_mass	 = 10.0F; // is in ppm
			mflt_constraint_average_mass = 10.0F; // is in ppm. 
			mdouble_max_distance		 = 0.1; 			
			mint_max_scan				 = INT_MIN; 
			mint_min_scan				 = INT_MAX; 
			mshort_percent_complete		 = 0;
		}

		UMCCreator::~UMCCreator(void)
		{
		}


		void UMCCreator::ReadCSVFile(char *fileName)
		{
			Reset(); 
			MemMappedReader mappedReader; 
			mappedReader.Load(fileName); 
			__int64 file_len = mappedReader.FileLength(); 

			// Setup header information
			char *startTag					= "scan_num,charge,abundance,mz,fit,average_mw,monoisotopic_mw,mostabundant_mw,fwhm,signal_noise,mono_abundance,mono_plus2_abundance"; 
			char *imsStartTag				= "frame_num,ims_scan_num,charge,abundance,mz,fit,average_mw,monoisotopic_mw,mostabundant_mw,fwhm,signal_noise,mono_abundance,mono_plus2_abundance,orig_intensity,TIA_orig_intensity,drift_time";
			// "frame_num,ims_scan_num,charge,abundance,mz,fit,average_mw,monoisotopic_mw,mostabundant_mw,fwhm,signal_noise,mono_abundance,mono_plus2_abundance,drift_time,cumulative_drift_time";
			int startTagLength				= (int)strlen(startTag); 
			int startIMSTagLength			= (int) strlen(imsStartTag);

			// For reading lines
			char *stopTag					= "Blah"; 
			int stopTagLen					= (int)strlen(stopTag); 

			// Flags indicating status.
			bool passedFilter				= true;
			bool reading					= false; 
			bool is_first_scan				= true; 

			// Temporary Isotopic peak value for reading.
			IsotopePeak pk; 
			short charge					= 0; 
			double abundance				= 0;

			// Initialize variables used during reading.
			float fit						= 0;
			float mz						= 0;
			float averageMass				= 0;
			float monoMass					= 0;
			float maxMass					= 0;  
			int numPeaks					= 0; 

			mshort_percent_complete			= 0;

			// Initialize the peak class.
			pk.mdouble_abundance			= 0; 
			pk.mdouble_i2_abundance			= 0; 
			pk.mdouble_average_mass			= 0; 
			pk.mflt_fit						= 0; 
			pk.mdouble_max_abundance_mass	= 0; 
			pk.mdouble_mono_mass			= 0; 
			pk.mdouble_mz					= 0; 
			pk.mshort_charge				= 0; 

			/// Scan information 
			mint_min_scan = INT_MAX; 
			mint_max_scan = 0; 

			/// File Pointer and Index variables.
			int pos							= 0; 
			const int MAX_BUFFER_LEN		= 1024; 
			char buffer[MAX_BUFFER_LEN];

			// Percentages for status
			mshort_percent_complete = (short)((100.0 * mappedReader.CurrentPosition()) / file_len); 
			if (mshort_percent_complete > 99)
				mshort_percent_complete = 99; 

			/// Test to see if we read the IMS version or the older Decon2ls Version.
			char testBuffer[16];
			bool imsData = false;
			mappedReader.FillBuffer(testBuffer, 0, 16);			
			imsData = (strncmp(testBuffer, imsStartTag, 16) == 0);
			
			long featuresFound = 0;
			if (imsData == false)
			{
				bool success = mappedReader.SkipToAfterLine(startTag, buffer, startTagLength, MAX_BUFFER_LEN, ' '); 			
				if (!success)
				{
					/// Read the ims data format
					/// Move the file pointer back to the first byte.				
					throw "Incorrect header for file"; 
				}

				double fwhm = 0, s2n = 0;
				while(!mappedReader.eof())					
				{					
					/// Make sure the memmapped reader works.
					///   - Catch exceptions (corrupt files)
					///   - Make sure lines are read.
					try
					{
						bool readLine = mappedReader.GetNextLine(buffer, MAX_BUFFER_LEN, stopTag, stopTagLen);
						if (readLine == false)
							break;
					}
					catch(...)
					{
						break;
					}

					mshort_percent_complete = (short)((100.0 * mappedReader.CurrentPosition()) / file_len); 
					if (mshort_percent_complete > 99)
						mshort_percent_complete = 99; 
					char *stopPtr; 
					char *stopPtrNext; 
					pk.mint_scan = strtol(buffer, &stopPtr, 10); 
					stopPtr++; 
					pk.mshort_charge = (short) strtol(stopPtr, &stopPtrNext, 10); 
					stopPtr = ++stopPtrNext; 
					pk.mdouble_abundance = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mdouble_mz = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mflt_fit = (float)strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mdouble_average_mass = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mdouble_mono_mass = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mdouble_max_abundance_mass = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					fwhm = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					s2n = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mdouble_mono_abundance = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mdouble_i2_abundance = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					pk.mint_original_index = numPeaks; 

					if (FilterIsotopicPeak(&pk))
					{
						/// Add into global array of peaks.
						mvect_isotope_peaks.push_back(pk); 

						// Calculate max scan and min scan for this file
						if (pk.mint_scan > mint_max_scan)
							mint_max_scan = pk.mint_scan; 
						if (pk.mint_scan < mint_min_scan)
							mint_min_scan = pk.mint_scan; 

						numPeaks++; 
					}

					featuresFound++;
				}
			}
			else
			{
				bool success = mappedReader.SkipToAfterLine(imsStartTag, buffer, startIMSTagLength, MAX_BUFFER_LEN, ' '); 			
				if (!success)
				{
					/// Read the ims data format
					/// Move the file pointer back to the first byte.				
					throw "Incorrect header for file"; 
				}

				double fwhm = 0;
				double s2n	= 0;
				long line = 0;

				while(!mappedReader.eof()  )
				{
					/// Make sure the memmapped reader works.
					///   - Catch exceptions (corrupt files)
					///   - Make sure lines are read.
					try
					{
						bool readLine = mappedReader.GetNextLine(buffer, MAX_BUFFER_LEN, stopTag, stopTagLen);
						line++;
						if (readLine == false)
							break;
					}
					catch(...)//(char * s)
					{
						line++;
						break;
					}

					mshort_percent_complete = (short)((100.0 * mappedReader.CurrentPosition()) / file_len); 
					if (mshort_percent_complete > 99)
						mshort_percent_complete = 99; 
					char *stopPtr; 
					char *stopPtrNext; 

					// LC Scan
					pk.mint_scan = strtol(buffer, &stopPtr, 10); 
					stopPtr++; 
					// IMS Scan
					pk.mint_imsFrame = (int)strtol(stopPtr, &stopPtrNext, 10);
					stopPtr = ++stopPtrNext; 
					// Charge 
					pk.mshort_charge = (short) strtol(stopPtr, &stopPtrNext, 10); 
					stopPtr = ++stopPtrNext; 
					// Abundance
					pk.mdouble_abundance = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// Mass Charge 
					pk.mdouble_mz = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// Fit
					pk.mflt_fit = (float)strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// Average Mass 
					pk.mdouble_average_mass = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// Monoisotopic Mass
					pk.mdouble_mono_mass = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// Max Abundance
					pk.mdouble_max_abundance_mass	= strtod(stopPtr, &stopPtrNext); 
					stopPtr							= ++stopPtrNext; 
					// FWHM
					fwhm = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// Signal To noise Ratio
					s2n = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// MonoIsotopic Abundance
					pk.mdouble_mono_abundance = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext;
					// MonoIsostopic Abundance Plus 2
					pk.mdouble_i2_abundance = strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					//Orig intensity
					strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					//TIA_orig_intensity
					strtod(stopPtr, &stopPtrNext); 
					stopPtr = ++stopPtrNext; 
					// IMS Drift Time
					pk.mdouble_driftTime = strtod(stopPtr, &stopPtrNext);
					stopPtr = ++stopPtrNext; 
					// Count the peak number here.
					pk.mint_original_index = numPeaks; 

					
					if (FilterIsotopicPeak(&pk))
					{					
						/// Add into global array of peaks.
						mvect_isotope_peaks.push_back(pk); 

						// Calculate max scan and min scan for this file
						if (pk.mint_scan > mint_max_scan)
							mint_max_scan = pk.mint_scan; 
						if (pk.mint_scan < mint_min_scan)
							mint_min_scan = pk.mint_scan; 

						numPeaks++; 
					}			
					featuresFound++;			
				}			
			}				

			mint_featuresFound = featuresFound;
			mint_numberOfPeaks = numPeaks;
		}

		bool UMCCreator::FilterIsotopicPeak(IsotopePeak *peak)
		{
			bool passed = true;

			if (mbool_useFitFilter)
			{
				/// RAPID  has a fit filter whose good fits are low and bad are high.
				/// THRASH has a fit filter whose good fits are high and bad are low.
				if (mbool_invertFitFilter == true)
				{
					passed = peak->mflt_fit >= mdouble_fitFilter;
				}
				else				
				{				
					passed = peak->mflt_fit <= mdouble_fitFilter;
				}				
			}

			if (mbool_useIntensityFilter)
			{
				passed = (passed && (peak->mdouble_abundance >= mdouble_intensityFilter));
			}

			return passed;
		}

		void UMCCreator::CalculateUMCs()
		{			
			std::vector<double> vect_mass; 
			int num_umcs;
			UMC new_umc; 			
			
			num_umcs				= mvect_umcs.size(); 			
			mshort_percent_complete = 0; 

			mvect_umcs.clear(); 
			mvect_umcs.reserve(mvect_umc_num_members.size()); 
			
			int highestChargeState	= 0;

			for (std::multimap<int,int>::iterator iter = mmultimap_umc_2_peak_index.begin(); iter != mmultimap_umc_2_peak_index.end(); )
			{
				vect_mass.clear(); 

				int umc_index			= (*iter).first; 
				mshort_percent_complete = (short) ((100.0 * umc_index) / num_umcs); 
				int numMembers			= mvect_umc_num_members[umc_index]; 
				int minScan				= INT_MAX; 
				int maxScan				= INT_MIN; 				

				double	minMass			= DBL_MAX; 
				double	maxMass			= -1 * DBL_MAX; 
				double	maxAbundance	= -1 * DBL_MAX; 
				double	sumAbundance	= 0; 
				double	sumMonoMass		= 0; 
				int		maxAbundanceScan= 0; 
				short	classRepCharge	= 0; 
				double  classRepMz		= 0; 

				/// Set the array of charge state abundaces for this umc to zero.
				memset(new_umc.marray_chargeStateAbundances, 0, sizeof(double)*NUM_CHARGE_STATES_LENGTH);

				/// The iterator operates over the given umc found
				/// Here we pull out the min and max scans, along with the
				/// abundances and representative charge state from the
				/// isotopic peak information
				while(iter != mmultimap_umc_2_peak_index.end() && (*iter).first == umc_index)
				{
					IsotopePeak pk = mvect_isotope_peaks[(*iter).second]; 
					vect_mass.push_back(pk.mdouble_mono_mass); 

					if (pk.mint_scan > maxScan)
						maxScan = pk.mint_scan; 
					if (pk.mint_scan < minScan )
						minScan = pk.mint_scan; 

					if (pk.mdouble_mono_mass > maxMass)
						maxMass = pk.mdouble_mono_mass; 
					if (pk.mdouble_mono_mass < minMass )
						minMass = pk.mdouble_mono_mass; 

					if (pk.mdouble_abundance > maxAbundance)
					{
						maxAbundance		= pk.mdouble_abundance; 
						maxAbundanceScan	= pk.mint_scan; 
						classRepCharge		= pk.mshort_charge; 
						classRepMz			= pk.mdouble_mz; 
					}

					sumAbundance += pk.mdouble_abundance; 
					sumMonoMass  += pk.mdouble_mono_mass; 

					/// Find the highest charge state to report.
					if (pk.mshort_charge > highestChargeState && pk.mshort_charge < NUM_CHARGE_STATES_LENGTH)
						highestChargeState = pk.mshort_charge;

					/// Save the total abundances for each charge state in it's own array.
					if (pk.mshort_charge > 0 && pk.mshort_charge < NUM_CHARGE_STATES_LENGTH)
					{
						new_umc.marray_chargeStateAbundances[pk.mshort_charge - 1] += pk.mdouble_abundance;
					}			

					iter++; 
				}

				sort(vect_mass.begin(), vect_mass.end()); 
				new_umc.mint_umc_index				 = umc_index; 
				new_umc.min_num_members				 = numMembers; 
				new_umc.mint_start_scan				 = minScan; 
				new_umc.mint_stop_scan				 = maxScan; 
				new_umc.mint_max_abundance_scan		 = maxAbundanceScan; 
				new_umc.mdouble_max_abundance		 = maxAbundance; 
				new_umc.mdouble_sum_abundance		 = sumAbundance;
				new_umc.mdouble_min_mono_mass		 = minMass; 
				new_umc.mdouble_max_mono_mass		 = maxMass; 
				new_umc.mdouble_average_mono_mass	 = sumMonoMass/numMembers; 
				new_umc.mdouble_class_rep_mz		 = classRepMz; 
				new_umc.mshort_class_rep_charge		 = classRepCharge; 
				new_umc.mshort_class_highest_charge  = highestChargeState;

				if (numMembers % 2 == 1)
				{
					new_umc.mdouble_median_mono_mass = vect_mass[numMembers/2]; 
				}
				else
				{
					new_umc.mdouble_median_mono_mass = 0.5 * (vect_mass[numMembers/2-1] + vect_mass[numMembers/2]); 
				}
				mvect_umcs.push_back(new_umc); 
			}
		}

		void UMCCreator::RemoveShortUMCs(int min_length)
		{
			std::vector<int> vectNewUMCLength; 

			// first reset all isotope peak umc indices to -1. 
			int numIsotopePeaks = mvect_isotope_peaks.size();
			for (int peakNum = 0; peakNum < numIsotopePeaks; peakNum++)
			{
				mvect_isotope_peaks[peakNum].mint_umc_index = -1; 
			}

			int numUmcsSoFar		= 0; 
			mshort_percent_complete = 0; 

			int num_umcs			= mmultimap_umc_2_peak_index.size(); 
			for (std::multimap<int,int>::iterator iter = mmultimap_umc_2_peak_index.begin(); iter != mmultimap_umc_2_peak_index.end(); )
			{
				int currentOldUmcNum	= (*iter).first; 
				mshort_percent_complete = (short) ((100.0 * currentOldUmcNum) / num_umcs); 

				int numMembers			= mvect_umc_num_members[currentOldUmcNum]; 
				while(iter != mmultimap_umc_2_peak_index.end() && (*iter).first == currentOldUmcNum)
				{
					if (numMembers >= min_length)
					{
						mvect_isotope_peaks[(*iter).second].mint_umc_index = numUmcsSoFar; 
					}
					iter++; 
				}
				if (numMembers >= min_length)
				{
					mvect_umc_num_members[numUmcsSoFar] = numMembers; 
					numUmcsSoFar++; 
				}
			}
			mvect_umc_num_members.resize(numUmcsSoFar); 

			// now set the map object. 
			mmultimap_umc_2_peak_index.clear(); 
			for (int pkNum = 0; pkNum < numIsotopePeaks; pkNum++)
			{
				IsotopePeak pk = mvect_isotope_peaks[pkNum]; 
				if (pk.mint_umc_index != -1)
				{
					mmultimap_umc_2_peak_index.insert(std::pair<int,int>(pk.mint_umc_index, pkNum)); 
				}
			}
		}


		void UMCCreator::CreateUMCsSinglyLinkedWithAll()
		{

			mshort_percent_complete = 0; 
			mmultimap_umc_2_peak_index.clear(); 
			int numPeaks			= mvect_isotope_peaks.size(); 
			for (int pkNum = 0; pkNum < numPeaks; pkNum++)
			{
				mvect_isotope_peaks[pkNum].mint_umc_index = -1; 
			}

			std::vector<IsotopePeak> vectTempPeaks; 
			vectTempPeaks.insert(vectTempPeaks.begin(), mvect_isotope_peaks.begin(), mvect_isotope_peaks.end()); 

			// basically take all umcs sorted in mass and perform single linkage clustering. 
			sort(vectTempPeaks.begin(), vectTempPeaks.end(), &SortIsotopesByMonoMassAndScan); 


			// now we are sorted. Start with the first index and move rightwards.
			// For each index, 
			// 1. If it already belongs to a UMC, move right comparing to all umcs in mass tolerance. For each match
			//		a. If match is not in a UMC, calculate distance. If within tolerance add it to current umc.
			//		b. If match is in the same UMC skip.
			//		c. If match is in a different UMC, calculate distance. If within tolerance, merge umcs, update current guys UMC number.
			// 2. If it does not belong to a UMC, move right comparing to all umcs in mass tolerance. For each match, calculate distance.
			//		a. Create new UMC and follow 1.

			int currentIndex		= 0; 

			IsotopePeak currentPeak; 
			IsotopePeak matchPeak; 
			int numUmcsSoFar		= 0; 
			double currentDistance	= 0; 
			std::multimap<int, int>::iterator iter; 
			std::multimap<int, int>::iterator deleteIter; 
			std::vector<int> tempIndices; // used to store indices of isotope peaks that are moved from one umc to another. 
			tempIndices.reserve(128); 


			mshort_percent_complete = 0; 
			while(currentIndex < numPeaks)
			{
				mshort_percent_complete = (short)((100.0 * currentIndex)/numPeaks); 
				currentPeak = vectTempPeaks[currentIndex]; 
				if (currentPeak.mint_umc_index == -1)
				{
					// create UMC
					mmultimap_umc_2_peak_index.insert(std::pair<int,int>(numUmcsSoFar, currentIndex)); 
					currentPeak.mint_umc_index = numUmcsSoFar; 
					vectTempPeaks[currentIndex].mint_umc_index = numUmcsSoFar; 
					numUmcsSoFar++; 
				}
				int matchIndex = currentIndex + 1; 
				if (matchIndex == numPeaks)
					break; 

				double massTolerance = currentPeak.mdouble_mono_mass * mflt_constraint_mono_mass / 1000000.0; 
				double maxMass = currentPeak.mdouble_mono_mass + massTolerance; 
				matchPeak = vectTempPeaks[matchIndex]; 
				while (matchPeak.mdouble_mono_mass < maxMass)
				{
					if (matchPeak.mint_umc_index != currentPeak.mint_umc_index)
					{		
						currentDistance = PeakDistance(currentPeak, matchPeak); 
						if (currentDistance < mdouble_max_distance)
						{
							if (matchPeak.mint_umc_index == -1)
							{
								mmultimap_umc_2_peak_index.insert(std::pair<int,int>(currentPeak.mint_umc_index, matchIndex)); 
								vectTempPeaks[matchIndex].mint_umc_index = currentPeak.mint_umc_index; 
							}
							else
							{
								tempIndices.clear(); 
								int numPeaksMerged = 0; 
								// merging time. Merge this guy's umc into the next guys UMC.
								for (iter = mmultimap_umc_2_peak_index.find(currentPeak.mint_umc_index); iter != mmultimap_umc_2_peak_index.end()
									&& (*iter).first == currentPeak.mint_umc_index; )
								{
									deleteIter			= iter; 
									int deletePeakIndex = (*iter).second; 
									tempIndices.push_back(deletePeakIndex); 
									vectTempPeaks[deletePeakIndex].mint_umc_index = matchPeak.mint_umc_index; 
									iter++; 
									mmultimap_umc_2_peak_index.erase(deleteIter); 
									numPeaksMerged++; 
								}
								for (int mergedPeakNum = 0; mergedPeakNum < numPeaksMerged; mergedPeakNum++)
								{
									mmultimap_umc_2_peak_index.insert(std::pair<int,int>(matchPeak.mint_umc_index, tempIndices[mergedPeakNum])); 
								}
								currentPeak.mint_umc_index = matchPeak.mint_umc_index; 
							}
						}
					}
					matchIndex++;
					if (matchIndex < numPeaks)
					{
						matchPeak = vectTempPeaks[matchIndex]; 
					}
					else
						break; 
				}
				currentIndex++; 
			}

			// At the end of all of this. The mapping from mmultimap_umc_2_peak_index is from umc_index to index in sorted stuff. 
			// Also, several of the umc indices are no longer valid. So lets step through the map, get new umc indices, renumber them,
			// and set the umc indices in the original vectors.
			numUmcsSoFar = 0; 
			for (std::multimap<int,int>::iterator iter = mmultimap_umc_2_peak_index.begin(); iter != mmultimap_umc_2_peak_index.end(); )
			{
				int currentOldUmcNum = (*iter).first; 
				int numMembers = 0; 
				while(iter != mmultimap_umc_2_peak_index.end() && (*iter).first == currentOldUmcNum)
				{
					IsotopePeak pk = vectTempPeaks[(*iter).second]; 
					mvect_isotope_peaks[pk.mint_original_index].mint_umc_index = numUmcsSoFar; 
					iter++; 
					numMembers++; 
				}
				mvect_umc_num_members.push_back(numMembers); 
				numUmcsSoFar++; 
			}
			// now set the map object. 
			mmultimap_umc_2_peak_index.clear(); 
			for (int pkNum = 0; pkNum < numPeaks; pkNum++)
			{
				IsotopePeak pk = mvect_isotope_peaks[pkNum]; 
				mmultimap_umc_2_peak_index.insert(std::pair<int,int>(pk.mint_umc_index, pkNum)); 
			}
			// DONE!! 
		}

		void UMCCreator::SetPeks(std::vector<IsotopePeak> &vectPks)
		{
			mvect_isotope_peaks.clear(); 
			mvect_isotope_peaks.insert(mvect_isotope_peaks.begin(), vectPks.begin(), vectPks.end()); 

			int numPeaks = mvect_isotope_peaks.size(); 
			for (int i = 0; i < numPeaks ; i++)
			{
				IsotopePeak pk = mvect_isotope_peaks[i]; 
				if (pk.mint_scan > mint_max_scan)
					mint_max_scan = pk.mint_scan; 
				if (pk.mint_scan < mint_min_scan)
					mint_min_scan = pk.mint_scan; 
			}
		}

		void UMCCreator::ReadPekFileMemoryMapped(char *fileName)
		{
			Reset(); 
			MemMappedReader mappedReader; 
			mappedReader.Load(fileName); 
			__int64 file_len = mappedReader.FileLength(); 

			char *fileNameTag				= "Filename:"; 
			int fileNameTagLength			= (int)strlen(fileNameTag); 
			char *startTag					= "CS,  Abundance,   m/z,   Fit,    Average MW, Monoisotopic MW,    Most abundant MW"; 
			char *startTagIsotopicLabeled	= "CS,  Abundance,   m/z,   Fit,    Average MW, Monoisotopic MW,    Most abundant MW,   Imono,   I+2"; 
			int startTagLength				= (int)strlen(startTag); 
			char *stopTag					= "Processing stop time:"; 
			int stopTagLen					= (int)strlen(stopTag); 
			bool reading					= false; 
			bool isotopically_labeled		= false;
			bool is_first_scan				= true; 
			bool is_pek_file_from_wiff		= false; 

			IsotopePeak pk; 

			float fit = 0, mz	= 0 , averageMass = 0 , monoMass = 0 , maxMass = 0;  
			short charge					= 0; 
			double abundance				= 0;

			int numPeaks					= 0; 
			mshort_percent_complete			= 0;

			pk.mdouble_abundance			= 0; 
			pk.mdouble_i2_abundance			= 0; 
			pk.mdouble_average_mass			= 0; 
			pk.mflt_fit						= 0; 
			pk.mdouble_max_abundance_mass	= 0; 
			pk.mdouble_mono_mass			= 0; 
			pk.mdouble_mz					= 0; 
			pk.mshort_charge				= 0; 

			mint_min_scan					= INT_MAX; 
			mint_max_scan					= 0; 

			int pos							= 0; 
			const int MAX_HEADER_BUFFER_LEN = 1024; 
			char headerBuffer[MAX_HEADER_BUFFER_LEN];
			const int MAX_BUFFER_LEN = 1024; 
			char buffer[MAX_BUFFER_LEN];
	
			bool passedFilter = true;
			long featuresFound = 0;
			while (!mappedReader.eof())
			{
				mshort_percent_complete = (short)((100.0 * mappedReader.CurrentPosition()) / file_len); 
				if (mshort_percent_complete > 99)
					mshort_percent_complete = 99; 

				bool success = mappedReader.SkipToAfterLine(fileNameTag, headerBuffer, fileNameTagLength, MAX_HEADER_BUFFER_LEN); 
				if (!success)
				{
					break; 
				}
				else
				{
					// found file name. start at the end. 
					int index = (int)strlen(headerBuffer); 
					while (index > 0 && headerBuffer[index] != '.' && headerBuffer[index] != ' ')
					{
						index--; 
					}
					index++; 
					// wiff file pek files have a wierd format. Another ICR2LS-ism.
					if (is_first_scan)
					{
						if(_strnicmp(&headerBuffer[index], "wiff", 4) ==0)
						{
							is_pek_file_from_wiff = true; 
							// REMEMBER TO DELETE WHEN PARAMETERS ARE SET ELSEWHERE
							mflt_wt_mono_mass = 0.0025F; // using ppms 10 ppm = length of 0.1 ppm
							mflt_wt_average_mass = 0.0025F; // using ppms 10 ppm = length of 0.1 ppm
							mflt_wt_log_abundance = 0.1F; 
							mflt_wt_scan = 0.01F; 
							mflt_wt_fit = 0.1F; 

							mflt_constraint_mono_mass = 25.0F; // is in ppm
							mflt_constraint_average_mass = 25.0F; // is in ppm. 
							mdouble_max_distance = 0.1; 

							mshort_percent_complete = 0;
						}
					}
					if (is_pek_file_from_wiff)
						index += 5; 
					pk.mint_scan = atoi(&headerBuffer[index]); 
					if (pk.mint_scan > mint_max_scan)
						mint_max_scan = pk.mint_scan; 
					if (pk.mint_scan < mint_min_scan)
						mint_min_scan = pk.mint_scan; 
				}

				if (is_first_scan)
				{
					bool success = mappedReader.SkipToAfterLine(startTag, headerBuffer, startTagLength, MAX_HEADER_BUFFER_LEN); 
					if (!success)
						break; 
					if (strncmp(headerBuffer, startTagIsotopicLabeled, strlen(startTagIsotopicLabeled)) == 0)
					{
						isotopically_labeled = true; 
					}
					is_first_scan = false; 
				}
				else
				{
					bool success = mappedReader.SkipToAfterLine(startTag, startTagLength); 
					if (!success)
						break; 
				}
				while(!mappedReader.eof() && mappedReader.GetNextLine(buffer, MAX_BUFFER_LEN, stopTag, stopTagLen))
				{
					if (isotopically_labeled)
					{
						char *stopPtr; 
						char *stopPtrNext; 
						pk.mshort_charge = (short) strtol(buffer, &stopPtr, 10); 
						pk.mdouble_abundance = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_mz = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mflt_fit = (float)strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_average_mass = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_mono_mass = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_max_abundance_mass = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_mono_abundance = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_i2_abundance = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
					}
					else
					{
						char *stopPtr; 
						char *stopPtrNext; 
						pk.mshort_charge = (short) strtol(buffer, &stopPtr, 10); 
						pk.mdouble_abundance = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_mz = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mflt_fit = (float)strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_average_mass = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_mono_mass = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
						pk.mdouble_max_abundance_mass = strtod(stopPtr, &stopPtrNext); 
						stopPtr = stopPtrNext; 
					}
					pk.mint_original_index = numPeaks; 
					
					featuresFound++;
					if (FilterIsotopicPeak(&pk))
					{					
						/// Add into global array of peaks.
						mvect_isotope_peaks.push_back(pk); 
						numPeaks++; 
					}		
				}
			}
			mappedReader.Close(); 	
			
			mint_featuresFound = featuresFound;
			mint_numberOfPeaks = numPeaks;
		}


		void UMCCreator::ReadPekFile(char *fileName)
		{
			Reset(); 
			FILE *fp = fopen(fileName, "r");
			int success = fseek(fp, 0, SEEK_END); 
			int file_len = ftell(fp); 
			success = fseek(fp, 0, SEEK_SET); 

			const int MAX_LINE_LENGTH = 512; 
			char buffer[MAX_LINE_LENGTH]; 
			char *fileNameTag = "Filename:"; 
			int fileNameTagLength = strlen(fileNameTag); 
			char *startTag = "CS,  Abundance,   m/z,   Fit,    Average MW, Monoisotopic MW,    Most abundant MW"; 
			char *startTagIsotopicLabeled = "CS,  Abundance,   m/z,   Fit,    Average MW, Monoisotopic MW,    Most abundant MW,   Imono,   I+2"; 
			int startTagLength = strlen(startTag); 
			char *stopTag = "Processing stop time:"; 
			bool reading = false; 
			bool isotopically_labeled = false;
			bool is_first_scan = true; 

			IsotopePeak pk; 
			int stopTagLen = (int)strlen(stopTag); 

			short charge = 0; 
			double abundance = 0;
			float fit = 0, mz = 0 , averageMass = 0 , monoMass = 0 , maxMass = 0;  

			int numPeaks = 0; 

			mshort_percent_complete = 0;

			pk.mdouble_abundance = 0; 
			pk.mdouble_i2_abundance = 0; 
			pk.mdouble_average_mass = 0; 
			pk.mflt_fit = 0; 
			pk.mdouble_max_abundance_mass = 0; 
			pk.mdouble_mono_mass = 0; 
			pk.mdouble_mz = 0; 
			pk.mshort_charge = 0; 

			mint_min_scan = INT_MAX; 
			mint_max_scan = 0; 


			int pos = 0; 
			long featuresFound = 0;
			bool passedFilter  = true;
			while (!feof(fp))
			{
				mshort_percent_complete = (short)((100.0 * pos) / file_len); 
				if (!reading)
				{
					fgets(buffer, MAX_LINE_LENGTH, fp); 
					pos += (int) strlen(buffer); 
					if (strncmp(buffer, startTag, startTagLength) == 0)
					{
						reading = true; 
						if (is_first_scan)
						{
							if (strncmp(buffer, startTagIsotopicLabeled, strlen(startTagIsotopicLabeled)) == 0)
							{
								isotopically_labeled = true; 
							}
							is_first_scan = false; 
						}
					}
					else if (strncmp(buffer, fileNameTag, fileNameTagLength) == 0)
					{
						// found file name. start at the end. 
						int index = strlen(buffer); 
						while (index > 0 && buffer[index] != '.')
						{
							index--; 
						}
						index++; 
						pk.mint_scan = atoi(&buffer[index]); 
						if (pk.mint_scan > mint_max_scan)
							mint_max_scan = pk.mint_scan; 
						if (pk.mint_scan < mint_min_scan)
							mint_min_scan = pk.mint_scan; 
					}
				}
				else
				{
					//int numRead = fscanf(fp, "%hd\t%lg\t%g\t%g\t%g\t%g\t%g", &charge, &abundance, &mz, &fit, 
					//	&averageMass, &monoMass, &maxMass); 
					int numRead = 0; 
					if (isotopically_labeled)
					{
						numRead = fscanf(fp, "%hd\t%lg\t%lg\t%g\t%lg\t%lg\t%lg\t%lg\t%lg",  &pk.mshort_charge, 
																							&pk.mdouble_abundance, 
																							&pk.mdouble_mz, 
																							&pk.mflt_fit, 
																							&pk.mdouble_average_mass, 
																							&pk.mdouble_mono_mass, 
																							&pk.mdouble_max_abundance_mass, 
																							&pk.mdouble_mono_abundance, 
																							&pk.mdouble_i2_abundance); 
						pos += 71; // this is really approximate, but its better than using ftellp
					}
					else
					{
						numRead = fscanf(fp, "%hd\t%lg\t%lg\t%g\t%lg\t%lg\t%lg",&pk.mshort_charge,
																				&pk.mdouble_abundance,
																				&pk.mdouble_mz, 
																				&pk.mflt_fit, 
																				&pk.mdouble_average_mass,
																				&pk.mdouble_mono_mass,
																				&pk.mdouble_max_abundance_mass); 
						pos += 51; // this is really approximate, but its better than using ftellp
					}
					if (numRead == 0)
					{
						// at the end of the reading sections
						reading = false; 
						pos += stopTagLen; 
					}
					else
					{
						featuresFound++;
						pk.mint_original_index = numPeaks; 
						if (FilterIsotopicPeak(&pk))
						{					
							/// Add into global array of peaks.
							mvect_isotope_peaks.push_back(pk); 
							numPeaks++; 
						}		
					}
				}
			}
			
			mint_featuresFound = featuresFound;
			mint_numberOfPeaks = numPeaks;
		}
		

		

		void UMCCreator::Reset()
		{
			mvect_isotope_peaks.clear(); 
			mvect_umcs.clear(); 
			mvect_umc_num_members.clear();
			mmultimap_umc_2_peak_index.clear(); 
			mshort_percent_complete = 0; 
		}
	}
}