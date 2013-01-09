ECHO OFF
REM This DOS script deploys a new version of MultiAlign to the sandbox site before being pushed out the main location
REM and ultimately website.
CLS

SET /P "major=Please Enter Major Number: "
SET /P "minor=Please Enter Minor Number: "
SET /P "build=Please Enter Build Number: "
SET "version=%major%.%minor%.%build%"

SET "vDir=MultiAlign-v"
SET "finalVersion=%vDir%%version%"
SET "localpath=m:\software\proteomics\MultiAlign\%finalVersion%"

ECHO %outpath%

mkdir %localpath%
explorer %localpath%

SET "consolex86=%localpath%\%finalVersion%-Console-x86"
SET "consolex64=%localpath%\%finalVersion%-Console-x64"
SET "guix86=%localpath%\%finalVersion%-GUI-x86"
SET "guix64=%localpath%\%finalVersion%-GUI-x64"

mkdir %consolex86%
mkdir %consolex64%
mkdir %guix86%
mkdir %guix64%

mkdir %consolex86%\MultiAlignParameterFileEditor
mkdir %consolex86%\MultiAlignConsole
mkdir %guix86%\MultiAlignParameterFileEditor
mkdir %guix86%\MultiAlign

mkdir %consolex64%\MultiAlignParameterFileEditor
mkdir %consolex64%\MultiAlignConsole
mkdir %guix64%\MultiAlignParameterFileEditor
mkdir %guix64%\MultiAlign

xcopy /S .\MultiAlignParameterFileEditor\bin\x86\release\*  	%consolex86%\MultiAlignParameterFileEditor
xcopy /S .\MultiAlignConsole\bin\x86\release\*  		%consolex86%\MultiAlignConsole
xcopy /S .\RevisionHistory.txt 					%consolex86%

xcopy /S .\Manassa\bin\x86\release\*  				%guix86%\MultiAlign
xcopy /S .\MultiAlignParameterFileEditor\bin\x86\release\*  	%guix86%\MultiAlignParameterFileEditor
xcopy /S .\RevisionHistory.txt 					%guix86%

xcopy /S .\MultiAlignParameterFileEditor\bin\x64\release\*  	%consolex64%\MultiAlignParameterFileEditor
xcopy /S .\MultiAlignConsole\bin\x64\release\*  		%consolex64%\MultiAlignConsole
xcopy /S .\RevisionHistory.txt 					%consolex64%

xcopy /S .\MultiAlignParameterFileEditor\bin\x64\release\*  	%guix64%\MultiAlignParameterFileEditor
xcopy /S .\Manassa\bin\x64\release\*  				%guix64%\MultiAlign
xcopy /S .\RevisionHistory.txt 					%guix64%

ECHO Cleaning up directories
del /F /S %consolex86%\MultiAlignParameterFileEditor\*.vshost.exe*
del /F /S %consolex86%\MultiAlignParameterFileEditor\*.pdb
del /F /S %consolex86%\MultiAlignConsole\*.vshost.exe*
del /F /S %consolex86%\MultiAlignConsole\*.pdb
del /F /S %guix86%\MultiAlignParameterFileEditor\*.vshost.exe*
del /F /S %guix86%\MultiAlignParameterFileEditor\*.pdb
del /F /S %guix86%\MultiAlign\*.vshost.exe*
del /F /S %guix86%\MultiAlign\*.pdb

rmdir /Q /S %consolex86%\MultiAlignConsole\examples
rmdir /Q /S %consolex86%\MultiAlignConsole\sic
rmdir /Q /S %consolex86%\MultiAlignConsole\scripts


del /F /S %consolex64%\MultiAlignParameterFileEditor\*.vshost.exe*
del /F /S %consolex64%\MultiAlignParameterFileEditor\*.pdb
del /F /S %consolex64%\MultiAlignConsole\*.vshost.exe*
del /F /S %consolex64%\MultiAlignConsole\*.pdb
del /F /S %guix64%\MultiAlignParameterFileEditor\*.vshost.exe*
del /F /S %guix64%\MultiAlignParameterFileEditor\*.pdb
del /F /S %guix64%\MultiAlign\*.vshost.exe*
del /F /S %guix64%\MultiAlign\*.pdb

ECHO Cleaning up Old Deprecated Directories
rmdir /Q /S %consolex64%\MultiAlignConsole\examples
rmdir /Q /S %consolex64%\MultiAlignConsole\sic
rmdir /Q /S %consolex64%\MultiAlignConsole\scripts
rmdir /Q /S %consolex64%\MultiAlignConsole\zh-cn

ECHO Running Tests

