@echo off

sed "/^\s*$/d" %1 | wc -l > LineCount.tmp

for /f %%i in (LineCount.tmp) do set RESULT=%%i

echo %RESULT%	%1 >> CodeStats.txt

echo %1    %RESULT%

