@echo off
setlocal

set ReleaseDate=YYYYMMDD
set ProjectPath=.
set Releases=RELEASES.TXT

FOR /F "tokens=*" %%i IN (%Releases%) DO (
@call package-release.cmd %%i %ReleaseDate% %ProjectPath%
)

endlocal
pause