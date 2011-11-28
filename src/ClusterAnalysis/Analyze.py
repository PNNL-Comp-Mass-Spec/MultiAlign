from   string 	import atoi, atof
from   sys 	import argv, stderr, stdout
import os
from ParseClusterFile 	import *
from Features 		import Cluster, Feature, PeptideMatch
from DecisionTrees 	import BinaryDecisionClassifier
									
def pause():
	raw_input(":")

def printd(msg):
	stderr.write(msg + "\n")
	stdout.write(msg + "\n")

	
def parseClusters(path, tree, printClusterSizes=False):
	f = open(path, "r")
	lines = f.readlines()
	f.close()

	clusters  = {}			
	matches   = {}
	noMatches = {}	
	
	i 	= 0
	dots 	= 0	
	length  = 0	
	
	stderr.write("Processing Data.\n")
	stderr.write("\t00.00 : ")
	for line in lines: 		
		line = line.replace("\n","")
		try:
			(cluster, feature, msn, match) = parseCluster(line)				
			tree.addCluster(cluster, feature, msn, match)
		except Exception, e:			
			print repr(e), e			
			pause()
		i = i + 1
		if (i > 1000):
			stderr.write(".")
			i = 0
			dots = dots + 1
			if (dots == 50):
				stderr.write("\n")
				dots = 0
				stderr.write("\t%.2f : " % ( (length*100.0)  / float(len(lines)) ))
		length = length + 1
	
	stderr.write("\nParsing Complete.\n")
	
def run(path, numDatasets):
        scores  = map(lambda x: float(x)/10.0, range(5,11))
        members = range(numDatasets)

        bdt      = BinaryDecisionClassifier()
        clusters = parseClusters(path, bdt)

        print numDatasets
        f = open("tree.csv", "w")
        for score in scores:
                bdt.upscoreCutoff = score
                for minMembers in members:
                        bdt.clear(bdt.tree)
                        bdt.minMembers  = minMembers
                        f.write("%.2f,%d\n" % (score, minMembers))  
                        stderr.write("\n")
                        stderr.write("Building Binary Decision Trees %.2f-%d\n" % (score, minMembers))                
                        bdt.processClusters()                        
                        bdt.printTree(bdt.tree, "")
                        bdt.writeTree(bdt.tree, f)
        f.close()	
        	
	
if (__name__ == "__main__"):	
	if (len(argv) < 3):
		print "The input cluster file was missing"
		print "usage: analyzeClusters.py clusterFilePath.txt numberOfDatasets"
		exit(0)
		
	if (os.path.exists(argv[1]) == False):
		print "The input file does not exist."
		exit(0)
		
	if (os.path.isfile(argv[1]) == False):
		print "The file specified is not a valid file."
		exit(0)
		
	run(argv[1], int(argv[2]))
	
