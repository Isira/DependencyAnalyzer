:run.bat
:
@echo off
@setlocal enableextensions
@cd /d "%~dp0"

:runs Starting up Server 2
"DependencyAnalyzer/ServerExecutive/bin/Debug/ServerExecutive.exe" "Server2.xml"