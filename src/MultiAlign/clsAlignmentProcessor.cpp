
#include "clsalignmentprocessor.h"
#include "classAlignmentMZBoundary.h"

#include <fstream> 
#using <mscorlib.dll>
#include <float.h> 

namespace MultiAlignEngine
{
	namespace Alignment
	{
		clsAlignmentProcessor::clsAlignmentProcessor(void)
		{
			mobjLCMSWarp				= __nogc new MultiAlignEngine::Alignment::LCMSWarp(); 
			mobjAlignmentOptions		= new MultiAlignEngine::Alignment::clsAlignmentOptions();
			mstrMessage					= new System::String(S""); 
			ApplyAlignmentOptions(); 

			mblnAligningToMassTagDB		= false; 
			mblnMassTagDBLoaded			= false; 
			mblnClusterAlignee			= false; 
		}

		clsAlignmentProcessor::~clsAlignmentProcessor(void)
		{
			if (mobjLCMSWarp != NULL)
				delete mobjLCMSWarp; 
		}

		void clsAlignmentProcessor::ApplyAlignmentOptions()
		{
			mobjLCMSWarp->SetTolerances(mobjAlignmentOptions->get_MassTolerance(), 
				mobjAlignmentOptions->get_NETTolerance()); 

			mobjLCMSWarp->SetNETOptions(mobjAlignmentOptions->get_NumTimeSections(), 
				mobjAlignmentOptions->get_ContractionFactor(), mobjAlignmentOptions->get_MaxTimeJump(), 
				mobjAlignmentOptions->get_MaxPromiscuity()); 

			mobjLCMSWarp->SetMassCalOptions(mobjAlignmentOptions->get_MassCalibrationWindow(), 
				mobjAlignmentOptions->get_MassCalibrationNumMassDeltaBins(), 
				mobjAlignmentOptions->get_MassCalibrationNumXSlices(), 
				mobjAlignmentOptions->get_MassCalibrationMaxJump(), 
				mobjAlignmentOptions->get_MassCalibrationMaxZScore(),
				mobjAlignmentOptions->get_MassCalibrationUseLSQ()); 

			mobjLCMSWarp->SetMassCalLSQOptions(mobjAlignmentOptions->get_MassCalibrationLSQNumKnots(),
				mobjAlignmentOptions->get_MassCalibrationLSQZScore()); 

			mobjLCMSWarp->SetCalibrationType((MultiAlignEngine::Alignment::enmLCMSWarpCalibrationType)
				mobjAlignmentOptions->get_RecalibrationType()); 
		}


		void clsAlignmentProcessor::ApplyNETMassFunctionToAligneeDatasetFeatures(MultiAlignEngine::Features::clsClusterData* &clusterData)
		{
			mintPercentDone = 0; 
			std::vector<int>	vectClusterIndices; 
			std::vector<double> vectClusterCalibratedMasses; 
			std::vector<double> vectClusterAlignedNETs; 
			std::vector<int>	vectClusterAlignedScans; 
			std::vector<double>	vectClusterAlignedDriftTimes; 

			mobjLCMSWarp->GetFeatureCalibratedMassesAndAlignedNETs( vectClusterIndices,
																	vectClusterCalibratedMasses, 
																	vectClusterAlignedNETs,
																	vectClusterAlignedDriftTimes); 

			clusterData->SetClusterCalibratedMassedAndNETS( vectClusterIndices, 
															vectClusterCalibratedMasses,
															vectClusterAlignedNETs,
															vectClusterAlignedDriftTimes); 
		}

		void clsAlignmentProcessor::ApplyNETMassFunctionToAligneeDatasetFeatures(MultiAlignEngine::Features::clsUMCData* &umcData)
		{
			// if deserialized, lcmswarp object will be null. recreate it.
			if (mobjLCMSWarp == 0)
				mobjLCMSWarp = __nogc new MultiAlignEngine::Alignment::LCMSWarp(); 

			mintPercentDone = 0; 
			std::vector<int>	vectUMCIndices; 
			std::vector<double> vectUMCCalibratedMasses; 
			std::vector<double> vectUMCAlignedNETs; 
			std::vector<int>	vectUMCAlignedScans; 
			std::vector<double>	vectUMCDriftTimes; 

			if (mblnAligningToMassTagDB)
			{
				mobjLCMSWarp->GetFeatureCalibratedMassesAndAlignedNETs( vectUMCIndices,
																		vectUMCCalibratedMasses, 
																		vectUMCAlignedNETs,
																		vectUMCDriftTimes); 
				umcData->SetUMCCalibratedMassedAndNETS( vectUMCIndices,
														vectUMCCalibratedMasses,
														vectUMCAlignedNETs,
														vectUMCDriftTimes); 
			}
			else
			{
				mobjLCMSWarp->GetFeatureCalibratedMassesAndAlignedNETs( vectUMCIndices, 
																		vectUMCCalibratedMasses, 
																		vectUMCAlignedNETs,
																		vectUMCAlignedScans, 
																		vectUMCDriftTimes,
																		mintMinReferenceDatasetScan,
																		mintMaxReferenceDatasetScan); 

				umcData->SetUMCCalibratedMassedAndNETS( vectUMCIndices, 
														vectUMCCalibratedMasses, 
														vectUMCAlignedNETs, 
														vectUMCAlignedScans,
														vectUMCDriftTimes); 
			}
		}

		/*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
		bool clsAlignmentProcessor::ValidateAlignmentBoundary(	bool split, 
																double mz,
																classAlignmentMZBoundary* boundary)
		{
			if (split == true)
			{
				return (mz < boundary->HighBoundary && mz >= boundary->LowBoundary);				
			}
			return true;
		}
		/*////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			Function: Set Alignee Features 
			Description:
				For a given dataset of UMC's.  Set the alignee (who we are warping to the baseline) features.
				Above boundary will determine if we should allow features above the m/z range or below if the 
				splitMZBoundary flag is set in the alignment options to true				
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
		void clsAlignmentProcessor::SetAligneeDatasetFeatures(  MultiAlignEngine::Features::clsUMCData *umcData, 
																int datasetIndex,																
																classAlignmentMZBoundary* boundary)
		{
			mintPercentDone		= 0; 
			mblnClusterAlignee	= false;

			MultiAlignEngine::Features::clsUMC* arrUMCs __gc[] = umcData->GetUMCS(datasetIndex); 
			int numPts = arrUMCs->Length; 

			std::vector<MultiAlignEngine::Alignment::MassTimeFeature> vectMassTimeFeatures;
			vectMassTimeFeatures.reserve(numPts); 

			mintMinAligneeDatasetScan = INT_MAX; 
			mintMaxAligneeDatasetScan = INT_MIN; 
			mdblMinAligneeDatasetMZ   = DBL_MAX; 
			mdblMaxAligneeDatasetMZ   = -1 * DBL_MAX; 
			
			MultiAlignEngine::Alignment::MassTimeFeature ms_feature; 
			for (int index = 0; index < numPts; index++)
			{
				mintPercentDone							= (index *100)/numPts; 
				MultiAlignEngine::Features::clsUMC *umc = arrUMCs[index]; 
				ms_feature.mdouble_mono_mass			= umc->mdouble_mono_mass; 
				ms_feature.mdouble_mono_mass_calibrated = umc->mdouble_mono_mass_calibrated; 
				ms_feature.mdouble_mono_mass_original	= umc->mdouble_mono_mass; 
				ms_feature.mdouble_net					= (double) umc->mint_scan; 

				/*
					See if we want to split the alignment at some given m/z range.  If so then make sure that we do 
				*/				
				ms_feature.mdouble_mz					= umc->mdouble_class_rep_mz; 
				ms_feature.mdouble_abundance			= umc->mdouble_abundance;
				ms_feature.mint_id						= umc->mint_umc_index; 
				ms_feature.mdouble_driftTime			= umc->mfloat_drift_time;

				/*
					Only allow the feature to be aligned if we are splitting the alignment in m/z 
					and then if we are within the boundary the user specified.
				*/
				if (ValidateAlignmentBoundary(	mobjAlignmentOptions->SplitAlignmentInMZ,
												ms_feature.mdouble_mz,
												boundary))
				{
					vectMassTimeFeatures.push_back(ms_feature); 

					if (umc->mint_scan > mintMaxAligneeDatasetScan)
						mintMaxAligneeDatasetScan	= umc->mint_scan; 
					if (umc->mint_scan < mintMinAligneeDatasetScan)
						mintMinAligneeDatasetScan	= umc->mint_scan; 
					if (umc->mdouble_class_rep_mz > mdblMaxAligneeDatasetMZ)
						mdblMaxAligneeDatasetMZ		= umc->mdouble_class_rep_mz; 
					if (umc->mdouble_class_rep_mz < mdblMinAligneeDatasetMZ)
						mdblMinAligneeDatasetMZ		= umc->mdouble_class_rep_mz; 
				}
			}
			mobjLCMSWarp->SetFeatures(vectMassTimeFeatures); 
		}
		/*////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			Function: Set Alignee Features 
			Description:
				For a given dataset of UMC's.  Set the alignee (who we are warping to the baseline) features.
				Above boundary will determine if we should allow features above the m/z range or below if the 
				splitMZBoundary flag is set in the alignment options to true				
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
		void clsAlignmentProcessor::SetAligneeDatasetFeatures(	MultiAlignEngine::Features::clsClusterData *clusterData,																
																classAlignmentMZBoundary* boundary)
		{
			mintPercentDone = 0; 
			mblnClusterAlignee = true;

			int numClusters = clusterData->NumClusters; 
			std::vector<MultiAlignEngine::Alignment::MassTimeFeature> vectMassTimeFeatures;
			vectMassTimeFeatures.reserve(numClusters); 

			mintMinAligneeDatasetScan = INT_MAX; 
			mintMaxAligneeDatasetScan = INT_MIN; 
			mdblMinAligneeDatasetMZ = DBL_MAX; 
			mdblMaxAligneeDatasetMZ = -1 * DBL_MAX; 
			
			MultiAlignEngine::Alignment::MassTimeFeature ms_feature; 
			for (int clusterIndex = 0; clusterIndex < numClusters; clusterIndex++)
			{
				mintPercentDone									= (clusterIndex *100)/numClusters; 
				MultiAlignEngine::Features::clsCluster *cluster = clusterData->GetCluster(clusterIndex); 
				
				// for clusters use the calibrated mass as the mono mass to calibrate with. 
				// as this is the mass used in the clustering process.
				ms_feature.mdouble_mono_mass			= cluster->mdouble_mass_calibrated; 
				ms_feature.mdouble_mono_mass_calibrated = cluster->mdouble_mass_calibrated; 
				ms_feature.mdouble_mono_mass_original	= cluster->mdouble_mass; 
				
				// for clusters the time value to use is the net value. because clustering is performed on a 
				// net level. 
				ms_feature.mdouble_net	= (double) cluster->mdouble_net; 
				ms_feature.mdouble_mz	= cluster->mdouble_mass_calibrated/cluster->mshort_charge + 1.00782; 
				ms_feature.mint_id		= clusterIndex; 
				

				vectMassTimeFeatures.push_back(ms_feature); 

				if (cluster->mint_scan > mintMaxAligneeDatasetScan)
					mintMaxAligneeDatasetScan = cluster->mint_scan; 
				if (cluster->mint_scan < mintMinAligneeDatasetScan)
					mintMinAligneeDatasetScan = cluster->mint_scan; 
				if (ms_feature.mdouble_mz > mdblMaxAligneeDatasetMZ)
					mdblMaxAligneeDatasetMZ = ms_feature.mdouble_mz; 
				if (ms_feature.mdouble_mz < mdblMinAligneeDatasetMZ)
					mdblMinAligneeDatasetMZ = ms_feature.mdouble_mz; 
			}
			mobjLCMSWarp->SetFeatures(vectMassTimeFeatures); 
		}

		// Use the net values of the UMCs as the value to align to, i.e. the predictor variable. 
		void clsAlignmentProcessor::SetReferenceDatasetFeatures(MultiAlignEngine::Features::clsUMCData *umcData, 
			int referenceDatasetIndex)
		{
			mblnAligningToMassTagDB = false; 
			mblnMassTagDBLoaded = false; 
			mintPercentDone = 0; 

			MultiAlignEngine::Features::clsUMC* arrUMCs __gc[] = umcData->GetUMCS(referenceDatasetIndex); 
			int numPts = arrUMCs->Length; 

			std::vector<MultiAlignEngine::Alignment::MassTimeFeature> vectMassTimeFeatures;
			vectMassTimeFeatures.reserve(numPts); 

			mintMinReferenceDatasetScan = INT_MAX; 
			mintMaxReferenceDatasetScan = INT_MIN; 

			MultiAlignEngine::Alignment::MassTimeFeature ms_feature; 
			for (int index = 0; index < numPts; index++)
			{
				mintPercentDone = (index *100)/numPts; 
				MultiAlignEngine::Features::clsUMC *umc = arrUMCs[index]; 
				ms_feature.mdouble_mono_mass = umc->mdouble_mono_mass ; 
				ms_feature.mdouble_mono_mass_calibrated = umc->mdouble_mono_mass_calibrated; 
				ms_feature.mdouble_mono_mass_original = umc->mdouble_mono_mass  ; 
				ms_feature.mdouble_net = umc->mdouble_net; 
				ms_feature.mdouble_mz = umc->mdouble_class_rep_mz; 
				ms_feature.mdouble_abundance = umc->mdouble_abundance;
				ms_feature.mdouble_driftTime = umc->mfloat_drift_time;
				ms_feature.mint_id = umc->mint_umc_index; 
				vectMassTimeFeatures.push_back(ms_feature); 
				if (umc->mint_scan > mintMaxReferenceDatasetScan)
					mintMaxReferenceDatasetScan = umc->mint_scan; 
				if (umc->mint_scan < mintMinReferenceDatasetScan)
					mintMinReferenceDatasetScan = umc->mint_scan; 
				if (umc->mdouble_class_rep_mz > mdblMaxAligneeDatasetMZ)
					mdblMaxAligneeDatasetMZ = umc->mdouble_class_rep_mz; 
				if (umc->mdouble_class_rep_mz < mdblMinAligneeDatasetMZ)
					mdblMinAligneeDatasetMZ = umc->mdouble_class_rep_mz; 
			}
			mobjLCMSWarp->SetReferenceFeatures(vectMassTimeFeatures); 
		}

		void clsAlignmentProcessor::SetReferenceDatasetFeatures(MultiAlignEngine::MassTags::clsMassTagDB *massTagDB)
		{
			mintPercentDone = 0; 
			mblnAligningToMassTagDB = true; 

			std::vector<MultiAlignEngine::Alignment::MassTimeFeature> vectMassTimeFeatures;
			massTagDB->GetMassTagsAsMassTimeFeatures(vectMassTimeFeatures); 
			mobjLCMSWarp->SetReferenceFeatures(vectMassTimeFeatures); 
			mblnMassTagDBLoaded = true; 
		}

		void clsAlignmentProcessor::SetDataForAlignmentToMSFeatures(MultiAlignEngine::Features::clsUMCData *umcData, 
			System::String *aligneeDataset, System::String *referenceDataset)
		{
			menmState = SETTING_VARIABLES; 
			int aligneeDatasetIndex = umcData->GetDatasetIndex(aligneeDataset);
			int referenceDatasetIndex = umcData->GetDatasetIndex(referenceDataset);
			SetDataForAlignmentToMSFeatures(umcData, aligneeDatasetIndex, referenceDatasetIndex); 
		}

		void clsAlignmentProcessor::SetDataForAlignmentToMSFeatures(MultiAlignEngine::Features::clsUMCData *umcData, 
																	int aligneeDatasetIndex,
																	int referenceDatasetIndex)
		{
			menmState = SETTING_VARIABLES; 			
			

			SetAligneeDatasetFeatures(umcData, 
									  aligneeDatasetIndex,
									  mobjAlignmentOptions->MZBoundaries->Item[0]);

			SetReferenceDatasetFeatures(umcData, referenceDatasetIndex); 
			menmState = MS_ALIGNMENT_INITIALIZED; 
		}

		void clsAlignmentProcessor::SetDataForAlignmentToMassTagDatabase(MultiAlignEngine::Features::clsUMCData *umcData, 
			int aligneeDatasetIndex, MultiAlignEngine::MassTags::clsMassTagDB *massTagDB)
		{
			menmState = SETTING_VARIABLES; 

			SetAligneeDatasetFeatures(  umcData, 
										aligneeDatasetIndex,
										mobjAlignmentOptions->MZBoundaries->Item[0]);

			SetReferenceDatasetFeatures(massTagDB); 
			menmState = MASSTAG_ALIGNMENT_INITIALIZED; 
		}

		void clsAlignmentProcessor::SetDataForAlignmentToMassTagDatabase(MultiAlignEngine::Features::clsUMCData *umcData, 
			System::String* aligneeDataset, MultiAlignEngine::MassTags::clsMassTagDB *massTagDB)
		{
			menmState = SETTING_VARIABLES; 
			int aligneeDatasetIndex = umcData->GetDatasetIndex(aligneeDataset);
			SetDataForAlignmentToMassTagDatabase(umcData, aligneeDatasetIndex, massTagDB); 
			menmState = MASSTAG_ALIGNMENT_INITIALIZED; 
		}

		void clsAlignmentProcessor::PerformAlignmentToMSFeatures()
		{
			if (mobjAlignmentOptions == 0)
				throw new System::NullReferenceException(S"Alignment Options were not set in clsAlignmentProcessor"); 

			menmState = ALIGNING_TO_MS; 
			if(!mobjAlignmentOptions->get_ApplyMassRecalibration())
			{
				PerformNetWarp(); 
			}
			else
			{
				PerformNetMassWarp(); 
			}
			menmState = DONE; 
		}

		void clsAlignmentProcessor::PerformAlignmentToMassTagDatabase()
		{
			if (mobjAlignmentOptions == 0)
				throw new System::NullReferenceException(S"Alignment Options were not set in clsAlignmentProcessor"); 

			menmState = ALIGNING_TO_MASSTAG; 
			if(!mobjAlignmentOptions->get_ApplyMassRecalibration())
			{
				PerformNetWarp(); 
			}
			else
			{
				PerformNetMassWarp(); 
			}
			menmState = DONE; 
		}

		clsAlignmentFunction* clsAlignmentProcessor::GetAlignmentFunction()
		{
			// Remember that: 
			//		When Alignee dataset is cluster data then input time is net of cluster
			//		When Alignee dataset is umc data then input time is scan
			//		When Baseline dataset is umc data, then input baseline time is net
			//		When baseline dataset is mass tag database then input baseline time is avgGANET

			clsAlignmentFunction *fnc = new	clsAlignmentFunction(mobjAlignmentOptions->RecalibrationType, 
																 mobjAlignmentOptions->AlignmentType); 

			// Lets get the alignment functions that are needed. 
			// First the time alignment function. 
			std::vector<double> vectAligneeNETs;  
			std::vector<double> vectAligneeScans;  
			std::vector<double> vectReferenceNETS;  
			mobjLCMSWarp->GetAlignmentFunction(vectAligneeNETs, vectReferenceNETS); 

			if (mblnAligningToMassTagDB)
			{
				fnc->SetNETFunction(vectAligneeNETs, vectReferenceNETS); 
			}
			else
			{
				//that means that the baseline was a umc dataset. Which means that its 
				// times are in nets. lets make a copy of scans.
				std::vector<double> vectReferenceScans;
				int numSections		= (int) vectReferenceNETS.size(); 
				for (int sectionNum = 0; sectionNum < numSections; sectionNum++)
				{
					vectReferenceScans.push_back(mintMinReferenceDatasetScan + vectReferenceNETS[sectionNum] * (mintMaxReferenceDatasetScan-mintMinReferenceDatasetScan)); 
				}
				fnc->SetNETFunction(vectAligneeNETs, vectReferenceNETS, vectReferenceScans); 
			}

			if (mobjAlignmentOptions->AlignmentType == enmAlignmentType::NET_WARP)
				return fnc; 

			double minAligneeNET = 0 , maxAligneeNET = 0, minBaselineNET = 0 , maxBaselineNET = 0; 
			mobjLCMSWarp->GetBounds(&minBaselineNET, &maxBaselineNET, &minAligneeNET, &maxAligneeNET); 

			if (mobjAlignmentOptions->get_RecalibrationType() == MultiAlignEngine::Alignment::enmCalibrationType::SCAN_CALIB
				|| mobjAlignmentOptions->get_RecalibrationType() == MultiAlignEngine::Alignment::enmCalibrationType::HYBRID_CALIB)
			{
				// get the mass calibration function with time. 
				short numXKnots = mobjAlignmentOptions->MassCalibrationNumXSlices; 
				std::vector<double> vectAligneeNETMassFnc; 
				std::vector<double> vectAligneePPMShiftMassFnc; 
				// get the ppm for each. 
				for (short knotNum = 0; knotNum < numXKnots; knotNum++)
				{
					double net = minAligneeNET + ((maxAligneeNET - minAligneeNET) * knotNum)/numXKnots; 
					double ppm = mobjLCMSWarp->GetPPMShiftFromNET(net);
					vectAligneeNETMassFnc.push_back(net); 
					vectAligneePPMShiftMassFnc.push_back(ppm); 
				}
				fnc->SetMassCalibrationFunctionWithTime(vectAligneeNETMassFnc, vectAligneePPMShiftMassFnc);
			}

			if (mobjAlignmentOptions->get_RecalibrationType() == MultiAlignEngine::Alignment::enmCalibrationType::MZ_CALIB
				|| mobjAlignmentOptions->get_RecalibrationType() == MultiAlignEngine::Alignment::enmCalibrationType::HYBRID_CALIB)
			{
				// get the mass calibration function with time. 
				short numXKnots = mobjAlignmentOptions->MassCalibrationNumXSlices; 
				std::vector<double> vectAligneeMZMassFnc; 
				std::vector<double> vectAligneePPMShiftMassFnc; 
				// get the ppm for each. 
				for (short knotNum = 0; knotNum < numXKnots; knotNum++)
				{
					double net = knotNum*1.0/numXKnots; 
					double mz  = mdblMinAligneeDatasetMZ + (int) ((mdblMaxAligneeDatasetMZ - mdblMinAligneeDatasetMZ) * net); 
					double ppm = mobjLCMSWarp->GetPPMShiftFromMZ(mz);
					vectAligneeMZMassFnc.push_back(mz); 
					vectAligneePPMShiftMassFnc.push_back(ppm); 
				}
				fnc->SetMassCalibrationFunctionWithMZ(vectAligneeMZMassFnc, vectAligneePPMShiftMassFnc);
			}
			return fnc; 
		}

		void clsAlignmentProcessor::PerformNetWarp()
		{
			mstrMessage = new System::String (S"Performing Alignment between UMCs and Mass Tag Database");
			mstrMessage = new System::String(S"Generating Candidate Matches");

			mobjLCMSWarp->GenerateCandidateMatches();
			if (mobjLCMSWarp->GetNumCandidateMatches() <10)
			{
				menmState = ERROR; 
				throw new System::ApplicationException(S"Insufficient number of candidate matches by mass alone"); 
			}

			mstrMessage = new System::String("Get Match Scores Between Sections");
			mobjLCMSWarp->GetMatchProbabilities();

			mstrMessage = new System::String("Calculating Alignment Scores Between Sections");
			mobjLCMSWarp->CalculateAlignmentMatrix();

			mstrMessage = new System::String("Calculating Alignment Function");
			mobjLCMSWarp->CalculateAlignmentFunction(); 

			mstrMessage = new System::String("Getting Transformed Scans");
			mobjLCMSWarp->GetTransformedNets(); 

			mstrMessage = new System::String("Calculating Matches");
			mobjLCMSWarp->CalculateAlignmentMatches(); 

			mstrMessage = new System::String("Done with Alignment");

		}
		void clsAlignmentProcessor::PerformNetMassWarp()
		{
			// first perform a net calibration using a wide mass tolerance which is the same as the 
			// mass window parameter.
			double mass_tolerance = mobjLCMSWarp->GetMassTolerance(); 
			mobjLCMSWarp->SetMassTolerance(mobjLCMSWarp->GetMassCalibrationWindow()); 
			mobjLCMSWarp->UseMassAndNetScore(false); 

			PerformNetWarp(); 

			mstrMessage = new System::String("Performing Mass Recalibration");
			mobjLCMSWarp->PerformMassCalibration(); 

			mstrMessage = new System::String("Calculate Standard Deviations");
			mobjLCMSWarp->CalculateStandardDeviations();
			
			mobjLCMSWarp->SetMassTolerance(mass_tolerance); 
			mobjLCMSWarp->UseMassAndNetScore(true); 
			PerformNetWarp(); 
		}
		void clsAlignmentProcessor::GetAlignmentHeatMap(float (&alignmentScores) __gc[,], 
														float (&xIntervals) __gc[], 
														float (&yIntervals) __gc[])
		{
			// if deserialized, lcmswarp object will be null. recreate it.
			if (mobjLCMSWarp == 0)
				mobjLCMSWarp = __nogc new MultiAlignEngine::Alignment::LCMSWarp(); 
			std::vector<float> vectAlignmentScores; 
			std::vector<float> vectAligneeIntervals; 
			std::vector<float> vectBaselineIntervals; 
			mobjLCMSWarp->GetSubsectionMatchScores(vectAlignmentScores, vectAligneeIntervals, vectBaselineIntervals); 
			// since in the heat map we are going to want x axis to be alignee dataset, we are going to have to 
			// transpose this score vector here, by chaning baseline and reference.
			int numBaselineSections = vectBaselineIntervals.size(); 
			int numAligneeSections = vectAligneeIntervals.size(); 
			alignmentScores = new float __gc[numBaselineSections, numAligneeSections]; 

			int numTotalSections = vectAlignmentScores.size(); 
			if (numTotalSections != numBaselineSections * numAligneeSections)
				throw new System::ApplicationException(S"Error in alignment heatmap scores. Total section is not as expected"); 

			int aligneeSection = 0; 
			int baselineSection = 0; 
			for (int i = 0; i < numTotalSections; i++)
			{
				alignmentScores[baselineSection, aligneeSection] = vectAlignmentScores[i]; 
				baselineSection++;
				if (baselineSection == numBaselineSections)
				{
					baselineSection = 0; 
					aligneeSection++; 
				}
			}

			xIntervals = new float __gc [numAligneeSections];
			for (int i = 0; i < numAligneeSections; i++)
			{
				xIntervals[i] = vectAligneeIntervals[i]; 
			}

			yIntervals = new float __gc [numBaselineSections];
			for (int i = 0; i < numBaselineSections; i++)
			{
				yIntervals[i] = vectBaselineIntervals[i]; 
			}

		}

		void clsAlignmentProcessor::GetReferenceNETRange(float __gc & minRefNET, float __gc & maxRefNET)
		{
			double minAligneeNET = 0 , maxAligneeNET = 0, minBaselineNET = 0 , maxBaselineNET = 0; 
			mobjLCMSWarp->GetBounds(&minBaselineNET, &maxBaselineNET, &minAligneeNET, &minAligneeNET); 
			minRefNET = (float) minBaselineNET;
			maxRefNET = (float) maxBaselineNET;
		}

		
		
		/*////////////////////////////////////////////////////////////////////////////////
			Function:	GetErrorHistograms
			Note:		Calculates the mass and net error histograms.
			Arguments:  N/A
			Returns:	void
		////////////////////////////////////////////////////////////////////////////////*/
		void clsAlignmentProcessor::GetErrorHistograms( double  massBin,
														double  netBin, 
														double (&massErrorHistogram) __gc[,], 
														double (&netErrorHistogram) __gc[,])
		{
			std::vector<double> massErrorBin,  netErrorBin;
			std::vector<int>	massErrorFreq, netErrorFreq;

			mobjLCMSWarp->GetErrorHistograms(		massBin,
													netBin,
													massErrorBin,
													massErrorFreq,
													netErrorBin,
													netErrorFreq);

			massErrorHistogram = new double __gc[massErrorBin.size(), 2]; 
			netErrorHistogram  = new double __gc[netErrorBin.size(),  2]; 

			for(std::vector<double>::size_type i = 0; i < massErrorBin.size(); i++)
			{
				massErrorHistogram[i,0] = massErrorBin[i];
				massErrorHistogram[i,1] = massErrorFreq[i];
			}

			for(std::vector<double>::size_type i = 0; i < netErrorBin.size(); i++)
			{
				netErrorHistogram[i,0] = netErrorBin[i];
				netErrorHistogram[i,1] = netErrorFreq[i];
			}
		}
		double clsAlignmentProcessor::GetMassStandardDeviation()
		{
			double massStd, netStd, netMu, massMu;
			mobjLCMSWarp->GetStatistics(&massStd, &netStd, &massMu, &netMu);
			return massStd;			
		}
		double clsAlignmentProcessor::GetNETStandardDeviation()
		{
			double massStd, netStd, netMu, massMu;
			mobjLCMSWarp->GetStatistics(&massStd, &netStd, &massMu, &netMu);
			return netStd;			
		}
		double clsAlignmentProcessor::GetMassMean()
		{
			double massStd, netStd, netMu, massMu;
			mobjLCMSWarp->GetStatistics(&massStd, &netStd, &massMu, &netMu);
			return massMu;			
		}
		double clsAlignmentProcessor::GetNETMean()
		{
			double massStd, netStd, netMu, massMu;
			mobjLCMSWarp->GetStatistics(&massStd, &netStd, &massMu, &netMu);
			return netMu;			
		}		
		/*////////////////////////////////////////////////////////////////////////////////
			Function:	GetResiduals
			Note:		Calculates the all of the residual data for the alignment.
			Arguments:  None
			Returns:	Object holding all the residual data stored in respective arrays.
		////////////////////////////////////////////////////////////////////////////////*/
		classAlignmentResidualData*  clsAlignmentProcessor::GetResidualData()
		{			
			
			
			std::vector<double> vectNet;
			std::vector<double> vectMZ;
			std::vector<double> vectLinearNet;
			std::vector<double> vectCustomNet;
			std::vector<double> vectLinearCustomNet;
			std::vector<double> vectMassError;
			std::vector<double> vectMassErrorCorrected;
					
			mobjLCMSWarp->GetResiduals(vectNet,
									   vectMZ,
									   vectLinearNet,
									   vectCustomNet,
									   vectLinearCustomNet,
									   vectMassError,
									   vectMassErrorCorrected);

			int count			= vectNet.size();		

			float mz					__gc[]		= new float __gc[count]; 
			float scans					__gc[]		= new float __gc[count]; 
			float linearNet				__gc[]		= new float __gc[count]; 
			float customNet				__gc[]		= new float __gc[count]; 
			float linearCustomNet		__gc[]		= new float __gc[count]; 
			float massError				__gc[]		= new float __gc[count]; 
			float massErrorCorrected	__gc[]		= new float __gc[count]; 
			float mzMassError			__gc[]		= new float __gc[count]; 
			float mzMassErrorCorrected	__gc[]		= new float __gc[count]; 	

			// Copy the data from the vectors to the managed arrays.
			for(int i = 0; i < count; i++)
			{
				scans				[i] = Convert::ToSingle(vectNet[i]);
				mz					[i] = Convert::ToSingle(vectMZ[i]);
				linearNet			[i] = Convert::ToSingle(vectLinearNet[i]);
				customNet			[i] = Convert::ToSingle(vectCustomNet[i]);
				linearCustomNet		[i] = Convert::ToSingle(vectLinearCustomNet[i]);
				massError			[i] = Convert::ToSingle(vectMassError[i]);
				massErrorCorrected	[i] = Convert::ToSingle(vectMassErrorCorrected[i]);				
				mzMassError			[i] = Convert::ToSingle(vectMassError[i]);				
				mzMassErrorCorrected[i] = Convert::ToSingle(vectMassErrorCorrected[i]);
			}

			classAlignmentResidualData* residualData = new classAlignmentResidualData();
			residualData->scans					= scans;
			residualData->mz					= mz;	
			residualData->linearCustomNet		= linearCustomNet;
			residualData->customNet				= customNet;
			residualData->linearNet				= linearNet;
			residualData->massError				= massError;
			residualData->massErrorCorrected	= massErrorCorrected;
			residualData->mzMassError			= mzMassError;
			residualData->mzMassErrorCorrected	= mzMassErrorCorrected;

			return residualData;
		}		
	}
}