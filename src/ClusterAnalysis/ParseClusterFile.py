from string 	import atoi, atof
from Features 	import Cluster, Feature, PeptideMatch
		
							
def parseCluster(line):
	data = line.split(',')	
	if (len(data) > 1):	
		
		# the return objects 
		# may have clusters without matches/msn data		
		# so we start empty 
		cluster 	= None
		feature 	= None
		ms 		= None
		msn 		= None
		match		= None		

		# we should always have cluster data		
		clusterMatch    = data[0].lower()
		clusterType 	= data[1].lower()			
		clusterId   	= atoi(data[2])		
		clusterMass	= atof(data[3])
		clusterNet	= atof(data[4])
		clusterDS	= atoi(data[5])
		clusterMC	= atoi(data[6])
		
		cluster = Cluster(clusterId, clusterMass, clusterNet, clusterMC, clusterDS)
		k 	= 6

		# optionally we may have a cluster traceback for ms/ms		
		if (clusterType == "msn"):				
			featureDataset   = atoi(data[7])
			featureId 	 = atoi(data[8])
			featureMass 	 = atof(data[9])
			featureNet	 = atof(data[10])
			featureScan	 = atoi(data[11])
			featureAbundance = atoi(data[12])
						
			msFId	 	 = atoi(data[13])		
			msMz 	 	 = atof(data[14])
			msScan 		 = atoi(data[15])
			msIntensity	 = atoi(data[16])
			msCharge	 = atoi(data[17])
			feature 	 = Feature(msFId, featureDataset, msMz, featureMass, 1, msCharge, msScan, -1, featureNet, -1, featureAbundance);
									
			msnDataset	 = atoi(data[18])
			msnFeatureId	 = atoi(data[19])
			msnMz		 = atof(data[20])
			msnScan		 = atoi(data[21])
			msnRetention	 = atof(data[22])
			msn 		 = Feature(msnFeatureId,msnDataset, msnMz, -1, 2, msCharge, -1, msnScan, -1, msnRetention, -1)
			k 		 = 22

		if (clusterMatch == "matched"):
			tagId 		= atoi(data[k + 1])
			stac 		= atof(data[k + 2])
			stacup 		= atof(data[k + 3])
			tagMass 	= atof(data[k + 4])
			tagMods 	= atoi(data[k + 5])
			tagNet 		= atof(data[k + 6])
			peptide 	=      data[k + 7]
			tagPriors	= atof(data[k + 8])
			tagXcorr	= atof(data[k + 9])
			tagMsgf		= atof(data[k + 10])			
			match = PeptideMatch(peptide, stac, stacup, tagId, tagMass, tagMods, tagNet, tagPriors, tagXcorr, tagMsgf)
		
		return (cluster, feature, msn, match)
	
	return None
	
