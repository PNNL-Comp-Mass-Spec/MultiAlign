from   	string 		import atoi, atof
from 	sys		import stderr
from 	Features 	import Cluster, Feature, PeptideMatch
	

class TreeNode:
	def __init__(self, name, evalFunc, trueNode, falseNode):	
		self.name	= name
		self.func    	= evalFunc
		self.trueNode   = trueNode
		self.falseNode  = falseNode
		self.trueData   = []
		self.falseData  = []	
	def evalCluster(self, cluster):	
		decision = self.func.__call__(cluster)		
		if (decision):
			if (self.trueNode is not None):
				self.trueNode.evalCluster(cluster)
			else:
				self.trueData.append(cluster)
		else:
			if (self.falseNode is not None):
				self.falseNode.evalCluster(cluster)
			else:
				self.falseData.append(cluster)
	def getCount(self):		
		'''
		Gets the count from the sub-tree
		'''
		count = len(self.trueData) + len(self.falseData)
		
		if (self.trueNode is not None):
			count = count + self.trueNode.getCount()
		
		if (self.falseNode is not None):
			count = count + self.falseNode.getCount()	
			
		return count 
	def clear(self):
                self.trueData  = []
                self.falseData = []
	def __str__(self):
		trueCount  = len(self.trueData)
		falseCount = len(self.falseData)
		
		if (self.trueNode is not None):
			trueCount = trueCount + self.trueNode.getCount() 
		
		if (self.falseNode is not None):
			falseCount = falseCount + self.falseNode.getCount()
		
		return "%s,True,%d,False,%d" % (self.name, trueCount, falseCount)
		
class BinaryDecisionClassifier:
	def __init__(self):
			
		# construct the tree deicion nodes.
		self.hasMsnNode		= TreeNode("has MS/MS?",                  self.msmsFunc,	None,           None)
		self.matchNode 		= TreeNode("has AMT matches > STAC UP?",  self.matchFunc,       None,           self.hasMsnNode)
		self.filterNode         = TreeNode("has enough dataset members?", self.filterFunc,      self.matchNode, None)
		self.tree 		= self.filterNode
		self.minMembers         = 0
		self.upscoreCutoff      = 0
		self.clusters           = {}

        def clear(self, tree):                                
		if (tree is None):			
			return
                tree.clear()                
                self.clear(tree.trueNode)
                self.clear(tree.falseNode)
        
	def addCluster(self, cluster, feature, msn, match):		
		''' 
		Adds partitioned cluster to the classifier.
		'''
		id 	= cluster.id
		hasKey 	= self.clusters.has_key(id)
		if (not hasKey):
			self.clusters[id] = cluster

		self.clusters[id].addFeatures(feature, msn)
		self.clusters[id].addMatch(match)
	
	def filterFunc(self, cluster):
                '''
                Filters clusters with not enough members.
                '''
                return cluster.datasetCount > self.minMembers
	def msmsFunc(self, cluster):
                '''
                Determines if a cluster has msms
                '''
		return cluster.hasMSMS
	
	def matchFunc(self, cluster):
                '''
                Determines if a cluster has an AMT match with an upscore above threshold.
                '''
		hasMatch = cluster.hasMatch		
		score    = cluster.highestScore
		upscore  = cluster.highestUpScore		
		matched  = hasMatch and upscore > self.upscoreCutoff
		
		return matched
		
	def processClusters(self):
		for id in self.clusters.keys():
			cluster = self.clusters[id]
			self.tree.evalCluster(cluster)
		
		
	def printTree(self, tree, tabs):		
		
		if (tree is None):			
			return
			
		print tabs  + str(tree)
		self.printTree(tree.trueNode,"\t" + tabs)
		self.printTree(tree.falseNode,"\t" + tabs)
		
	def writeTree(self, tree, f):                
		if (tree is None):			
			return			
		f.write(str(tree) + "\n")
		self.writeTree(tree.trueNode,  f)
		self.writeTree(tree.falseNode, f)													
