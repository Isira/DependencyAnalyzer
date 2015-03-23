:run.bat
:
@echo off
@setlocal enableextensions
@cd /d "%~dp0"

:runs Starting up Server 1
"DependencyAnalyzer/ServerExecutive/bin/Debug/ServerExecutive.exe" "Server1.xml"