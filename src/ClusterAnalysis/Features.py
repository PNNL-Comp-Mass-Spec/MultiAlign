class Cluster:	
	def __init__(self, id, mass, net, memberCount, datasetCount):
		self.id		  = id		
		self.mass	  = mass
		self.net  	  = net
		self.memberCount  = memberCount
		self.datasetCount = datasetCount	
		
		self.features  	  = []
		self.matches 	  = []
		self.hasMSMS	  = False
		self.hasMatch 	  = False
		
		self.highestUpScore = 0
		self.highestScore   = 0
			
	def addFeatures(self, feature, msn):
		''' 
		Adds a feature to the features list as a tuple of feature to msn.
		hasMSMS is set to true if msn is not None..		
		'''
		if (feature is not None):
			self.features.append((feature, msn))
		if (msn is not None):
			self.hasMSMS = True				
	def addMatch(self, match):		
		''' 
		Adds a match to the match list if it's not None.
		Determines highest seen score for each match.
		'''
		if (match is not None):
			self.matches.append(match)
			self.hasMatch = True	
			if (match.score > self.highestScore):
				self.highestScore = match.score
			if (match.upscore > self.highestUpScore):
				self.highestUpScore = match.upscore
				
	def __str__(self):
		return "%d, %.4f, %4f, %d, %d" % (self.id, self.mass, self.net, self.datasetCount, self.memberCount)
	
class Feature:
	def __init__(self, id, datasetId, mz, mass, level, charge, scan, scanMsn, net, rt, intensity):	
		self.id		= id
		self.datasetId	= datasetId
		self.mz		= mz
		self.mass	= mass
		self.net	= net
		self.scan	= scan
		self.scanMsn	= scanMsn
		self.rt		= rt
		self.msLevel 	= level
		self.charge  	= charge
		self.intensity  = intensity
	def __str__(self):
		return "%d, %d, %f, %f, %d, %d, %f, %d " % (self.id, self.datasetId, self.mz, self.mass, self.scan, self.scanMsn, self.rt, self.intensity)

class PeptideMatch:		
	def __init__(self, peptide, score, upscore, tagId, mass, modCount, net, priorProb, xcorr, msgf):
		self.peptide 	= peptide
		self.score	= score
		self.upscore 	= upscore
		self.tagId	= tagId
		self.mass	= mass
		self.mods	= modCount
		self.net	= net
		self.priorProb 	= priorProb
		self.xcorr	= xcorr
		self.msgf	= msgf
	def __str__(self):
		return "%d, %s, %f, %f" % (self.tagId, self.peptide, self.score, self.upscore)	
