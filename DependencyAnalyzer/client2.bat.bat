:run.bat
:
@echo off
@setlocal enableextensions
@cd /d "%~dp0"

:runs Starting up Client 2
"DependencyAnalyzer/ClientExecutive/bin/Debug/ClientExecutive.exe" "Client2.xml"
PAUSE