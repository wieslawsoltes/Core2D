@echo off
set PATH=%WINDIR%\Microsoft.NET\Framework\v4.0.30319;%PATH%
MSBuild Test.sln /m /t:Build /p:Configuration=Debug;TargetFrameworkVersion=v4.5 /nologo /verbosity:normal
pause
