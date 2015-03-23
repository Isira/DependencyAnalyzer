:run.bat
:
@echo off
@setlocal enableextensions
@cd /d "%~dp0"

:runs Starting up Client 1
"DependencyAnalyzer/ClientExecutive/bin/Debug/ClientExecutive.exe" "Client1.xml"