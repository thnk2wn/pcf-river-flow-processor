@echo off

dotnet publish -r linux-x64 -c Release
if %errorlevel% neq 0 exit /b %errorlevel%

cf push