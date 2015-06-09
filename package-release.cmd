@echo off
setlocal

set ReleaseName=%3-%5-%4

set ProjectPath=%6
set ReleasePath=%6\%1
set WinRAR=c:\Program Files\WinRAR\WinRAR.exe

mkdir "%ReleaseName%"
mkdir "%ReleaseName%\Scripts"

copy "%ProjectPath%\*.txt" "%ReleaseName%"
copy "%ProjectPath%\*.md" "%ReleaseName%"
xcopy "%ProjectPath%\Scripts" "%ReleaseName%\Scripts" /E

copy "%ReleasePath%\*.dll" "%ReleaseName%"
copy "%ReleasePath%\%2" "%ReleaseName%"

"%WinRAR%" a -ep1 -m5 -r -t "%ReleaseName%.zip" "%ReleaseName%\*"
rmdir /S /Q "%ReleaseName%"

endlocal