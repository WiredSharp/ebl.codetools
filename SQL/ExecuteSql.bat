@ECHO off
COLOR 0F

REM Check inputs
IF "%~1" == "" GOTO help
IF "%~2" == "" GOTO help

:twoparam
SET database=(%~1)
SET script=%2
GOTO run

REM Execute script
:run
FOR %%i IN %database% DO (

	ECHO Executing script %script% on %%i database...
	sqlcmd -S sds-titan\dmd_ds9 -U titandbo -P titandbodev0 -d %%i -i %script% -b
	
	IF ERRORLEVEL 1 (
		COLOR 0C
		ECHO FAILED: execution stopped !
		EXIT /B 1
	)
	ECHO SUCCESS
)
GOTO EOF

:help
ECHO.
ECHO Usage: %0 [Database] ^<SQL Script^>
ECHO.
ECHO Example: Execute a script on database
ECHO - %0 MY_DATABASE my_script.sql
ECHO.
GOTO EOF

:EOF

REM HELPER:
REM %~1 ? >  Remove "" from parameter
REM See more here: http://technet.microsoft.com/en-us/library/bb490909.aspx
