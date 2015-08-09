@echo off
setlocal

set ReleaseName=Test2d.UI.Wpf-YYYYMMDD-AnyCPU

set ProjectPath=.
set ReleasePath=%ProjectPath%\Test2d.UI.Wpf\bin\Release
set WinRAR=c:\Program Files\WinRAR\WinRAR.exe

mkdir "%ReleaseName%"

copy "%ProjectPath%\*.txt" "%ReleaseName%"
copy "%ProjectPath%\*.md" "%ReleaseName%"
copy "%ReleasePath%\*.dll" "%ReleaseName%"
copy "%ReleasePath%\Test2d.UI.Wpf.exe" "%ReleaseName%"

"%WinRAR%" a -ep1 -m5 -r -t "%ReleaseName%.zip" "%ReleaseName%\*"
rmdir /S /Q "%ReleaseName%"

endlocal