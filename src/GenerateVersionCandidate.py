import os
from   datetime import date
import sys
import subprocess
import shutil


def BuildProjects(version, testPath, resultPath):
    pathX64      = "MultiAlignConsole\\bin\\x64\\Release"
    pathX86      = "MultiAlignConsole\\bin\\x86\\Release"
    outPath      = "..\\..\\candidates"
    solutionFile = "MultiAlignWin.sln"
    tool         = "MultiAlignConsole.exe"
    expectedInputPath  = "input.txt"
    expectedParamsPath = "params.xml"

    failed_flag  = "ANALYSIS FAILED"
 

    if (os.path.exists(outPath) == False):
        print "Creating output directory for builds"
        os.mkdir(outPath)
    
    compiler  = "devenv"
    projects  = ["MultiAlignConsole"]
    configs   = [("Release|x64", pathX64, "x64"), ("Release|x86", pathX86, "x86")]

    # batch commands to start running the tests.
    commands = []            
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

            # create a new output path
            configPath  = "MultiAlignConsole-v" + version + "-" + platformConfigName            
            p = outPath + "\\" + configPath
            p = os.path.abspath(p)

            print "Creating new path", p
            if (os.path.exists(p)):
                print "removing older copy of this version."
                shutil.rmtree(p)
                
            shutil.copytree(path, p)         

            if (os.path.exists(testPath) == False):
                raise Exception, "The test path provided was invalid." + testPath

            # then build a directory to run a batch file for Multalign for each test data
            testData = os.listdir(testPath)

            if (os.path.exists(resultPath) == False):
                os.mkdir(resultPath)
                
            xpath    = os.path.join(resultPath, configPath)
            print "creating final path for this config", xpath
            if (os.path.exists(xpath) == False):
                os.mkdir(xpath)
            
            for dirPath in testData:
                fp = os.path.join(testPath, dirPath)                    
                np = os.path.join(xpath, dirPath)

                if (os.path.exists(np) == False):
                    os.mkdir(np)

                # copy the parameter and input file over
                command = {"name": dirPath + "-test",
                           "path": np + "-test"
                          }
                
                for f in os.listdir(fp):
                    if (f == expectedInputPath):
                        newName = dirPath + "-input.txt"
                        command["files"] = os.path.join(np, newName)
                    elif (f == expectedParamsPath):
                        newName = dirPath + "-params.txt"
                        command["params"] = os.path.join(np, newName)
                        
                    shutil.copy(os.path.join(fp, f), os.path.join(np, newName))

                command["exe"] = os.path.join(p, tool)
                commands.append(command)

    print "Creating commands and executing tests"
    print "-"*50
    shells = []
    for command in commands:
        print command.keys()
        print command["name"]
        tool   = command["exe"]
        files  = command["files"]
        params = command["params"]
        name   = command["name"]
        path   = command["path"]

        commandLineText = "%s -files %s -params %s -name %s -path %s" % (tool, files, params, name, path)
        print commandLineText
        print "-"*50

        pe = subprocess.Popen(commandLineText)
        shells.append((pe, name))
        
    print "waiting on all jobs to finish."
    for shell in shells:
        shell[0].wait()
        output = shell.stdout.read()
        if (failedFlag in output):
            print "FAILED", name
        else:
            print "PASSED", name
        
if (__name__ == "__main__"):
    try:
        BuildProjects(sys.argv[1], sys.argv[2], sys.argv[3])
    except Exception, e:
        print "MultiAlign is not ready."
        print e
        
