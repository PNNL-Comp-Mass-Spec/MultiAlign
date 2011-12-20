import os
from   datetime import date
import sys
import subprocess
import shutil


def BuildProjects():
    pathX64      = "MultiAlignConsole\bin\x64\Release"
    pathX86      = "MultiAlignConsole\bin\x86\Release"
    outPath      = "..\\..\\candidates"
    solutionFile = "MultiAlignWin.sln"

    if (os.path.exists(outPath) == False):
        print "Creating output directory for builds"
        os.mkdir(outPath)
    
    compiler  = "devenv"
    projects  = ["MultiAlignConsole"]
    configs   = [("Release|x64", pathX64, "x64"), ("Release|x86", pathX86, "x86")]

    for project in projects:
        for config in configs:
            path     = config[1]            
            platform = config[0]
            platformConfigName = config[2]
            
            cleanCommand  = "%s %s /project %s /projectConfig %s /clean " % (compiler, solutionFile, project, platform)
            buildCommand  = "%s %s /project %s /projectConfig %s /build " % (compiler, solutionFile, project, platform)

            print "Cleaning: %s - %s " % (project, platform)
            pe = subprocess.Popen(cleanCommand)            
            pe.wait()

            print "Building: %s - %s " % (project, platform)            
            pe = subprocess.Popen(buildCommand)            
            pe.wait()
                                    
            d = date.today()
            x = d.strftime("%m_%d_%y")
            p = outPath + "\\MultiAlignConsole-" + x + "-" + platformConfigName 
            p = os.path.abspath(p)
            print p
            
            

if (__name__ == "__main__"):
    BuildProjects()
