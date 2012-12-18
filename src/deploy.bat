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
SET "localpath=m:\software\proteomics\MultiAlign\deployed\%finalVersion%"

ECHO %outpath%

mkdir %localpath%
explorer %localpath%

SET "x86=%localpath%\x86"
SET "x64=%localpath%\x64"

mkdir %x86%
mkdir %x64%

mkdir %x86%\MultiAlignParameterFileEditor
mkdir %x86%\MultiAlign
mkdir %x86%\MultiAlignConsole

mkdir %x64%\MultiAlignParameterFileEditor
mkdir %x64%\MultiAlign
mkdir %x64%\MultiAlignConsole

xcopy /S .\MultiAlignParameterFileEditor\bin\x86\release\*  	%x86%\MultiAlignParameterFileEditor
xcopy /S .\Manassa\bin\x86\release\*  				%x86%\MultiAlign
xcopy /S .\MultiAlignConsole\bin\x86\release\*  		%x86%\MultiAlignConsole
xcopy /S .\RevisionHistory.txt 					%x86%

xcopy /S .\MultiAlignParameterFileEditor\bin\x64\release\*  	%x64%\MultiAlignParameterFileEditor
xcopy /S .\Manassa\bin\x64\release\*  				%x64%\MultiAlign
xcopy /S .\MultiAlignConsole\bin\x64\release\*  		%x64%\MultiAlignConsole
xcopy /S .\RevisionHistory.txt 					%x64%

ECHO Cleaning up directories
del /F /S %x86%\MultiAlignParameterFileEditor\*.vshost.exe*
del /F /S %x86%\MultiAlignParameterFileEditor\*.pdb
del /F /S %x86%\MultiAlign\*.vshost.exe*
del /F /S %x86%\MultiAlign\*.pdb

rmdir /Q /S %x86%\MultiAlignConsole\examples
rmdir /Q /S %x86%\MultiAlignConsole\sic
rmdir /Q /S %x86%\MultiAlignConsole\scripts

del /F /S %x86%\MultiAlignConsole\*.vshost.exe*
del /F /S %x86%\MultiAlignConsole\*.pdb

del /F /S %x64%\MultiAlignParameterFileEditor\*.vshost.exe*
del /F /S %x64%\MultiAlignParameterFileEditor\*.pdb
del /F /S %x64%\MultiAlign\*.vshost.exe*
del /F /S %x64%\MultiAlign\*.pdb
del /F /S %x64%\MultiAlignConsole\*.vshost.exe*
del /F /S %x64%\MultiAlignConsole\*.pdb

rmdir /Q /S %x64%\MultiAlignConsole\examples
rmdir /Q /S %x64%\MultiAlignConsole\sic
rmdir /Q /S %x64%\MultiAlignConsole\scripts
rmdir /Q /S %x64%\MultiAlignConsole\zh-cn

SET /P "isDone=DONE? "