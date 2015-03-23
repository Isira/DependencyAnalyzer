:run.bat
:
@echo off
@setlocal enableextensions
@cd /d "%~dp0"

start client1.bat
start client2.bat

start server1.bat
start server2.bat

PAUSE

