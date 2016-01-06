@echo off
set retryNumber=0
set maxRetries=5

:RESTORE

nuget restore Core2D.sln -source "https://www.nuget.org/api/v2/;https://ci.appveyor.com/nuget/portable-xaml;https://www.myget.org/F/perspex-nightly/api/v2;https://www.myget.org/F/xamlbehaviors-nightly/api/v2"

IF NOT ERRORLEVEL 1 GOTO :EOF

@echo Nuget restore exited with code %ERRORLEVEL%!
set /a retryNumber=%retryNumber%+1
IF %reTryNumber% LSS %maxRetries% (GOTO :RESTORE)

@echo Restoring nuget packages %maxRetries% times was unsuccessful!

EXIT /B 1
