@echo off
setlocal

set ProjectPath=Test\bin\Release
set ReleaseName=Test-2015-05-11
set WinRAR=c:\Program Files\WinRAR\WinRAR.exe

mkdir "%ProjectPath%\%ReleaseName%"
mkdir "%ProjectPath%\%ReleaseName%\Scripts"
copy *.txt "%ProjectPath%\%ReleaseName%"
copy *.md "%ProjectPath%\%ReleaseName%"
xcopy "Scripts" "%ProjectPath%\%ReleaseName%\Scripts" /E

cd "%ProjectPath%"
copy *.dll "%ReleaseName%"
copy Test.exe "%ReleaseName%"

"%WinRAR%" a -ep1 -m5 -r -t "%ReleaseName%.zip" "%ReleaseName%\*"
rmdir /S /Q "%ReleaseName%"

endlocal
pause