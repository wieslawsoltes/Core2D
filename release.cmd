@echo off
setlocal

set ReleaseName=Core2D.UI.Wpf-YYYYMMDD-AnyCPU

set ProjectPath=.
set ReleasePath=%ProjectPath%\Core2D.UI.Wpf\bin\Release
set WinRAR=c:\Program Files\WinRAR\WinRAR.exe

mkdir "%ReleaseName%"

copy "%ProjectPath%\*.txt" "%ReleaseName%"
copy "%ProjectPath%\*.md" "%ReleaseName%"
copy "%ReleasePath%\*.dll" "%ReleaseName%"
copy "%ReleasePath%\Core2D.UI.Wpf.exe" "%ReleaseName%"

"%WinRAR%" a -ep1 -m5 -r -t "%ReleaseName%.zip" "%ReleaseName%\*"
rmdir /S /Q "%ReleaseName%"

endlocal