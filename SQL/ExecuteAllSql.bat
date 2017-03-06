@ECHO off
COLOR 0F

REM How to use:
REM ExecuteAllSql.bat [DATABASE] [DIRECTORY]
REM Example: ExecuteAllSql.bat MY_DATABASE 7.2
 
REM Special case for generating script order list file
IF "%~1" == "-order" (
	SET onlyOrder=1
	IF "%~2" == "" GOTO help
	SET directory=%~2
	GOTO order
)

REM Check inputs
IF "%~1" == "" GOTO help
IF "%~2" == "" GOTO oneparam
GOTO twoparam

REM Set local parameters
:oneparam
SET database=
SET directory=%~1
SET onlyOrder=0
GOTO order

:twoparam
SET database=%~1
SET directory=%~2
SET onlyOrder=0
GOTO order

:run
FOR /f %%f IN (%fileOrderName%) DO (

	IF [%database%] == [] (CALL ExecuteSql.bat "%directory%\%%f") ELSE (CALL ExecuteSql.bat "%database%" "%directory%\%%f")
	
	IF ERRORLEVEL 1 (
		COLOR 0C
		ECHO FAILED: Stop execution of all scripts !
		EXIT /B %ERRORLEVEL%
	)
)
GOTO EOF

:help
ECHO.
ECHO Usage: %0 [Database] ^<Folder^>
ECHO Usage: %0 [Folder]
ECHO Usage: %0 -order [Folder]
ECHO.
ECHO Example: Execute all scripts just on one database
ECHO - %0 MY_DATABASE 7.2
ECHO.
GOTO EOF

:order
SET fileOrderName=ordre.cfg
IF EXIST %fileOrderName% DEL /F %fileOrderName%
FOR /f "usebackq delims=|" %%f IN (`dir /a-d /b "%directory%" ^| findstr /r "^d_.*"`) DO ECHO %%f > %fileOrderName%
FOR /f "usebackq delims=|" %%f IN (`dir /a-d /b "%directory%" ^| findstr /r "^a_.*"`) DO ECHO %%f > %fileOrderName%
FOR /f "usebackq delims=|" %%f IN (`dir /a-d /b "%directory%" ^| findstr /r "^c_.*"`) DO ECHO %%f >> %fileOrderName%
FOR /f "usebackq delims=|" %%f IN (`dir /a-d /b "%directory%" ^| findstr /r "^i_.*"`) DO ECHO %%f >> %fileOrderName%
FOR /f "usebackq delims=|" %%f IN (`dir /a-d /b "%directory%" ^| findstr /r "^u_.*"`) DO ECHO %%f >> %fileOrderName%
FOR /f "usebackq delims=|" %%f IN (`dir /a-d /b "%directory%" ^| findstr /r "^[0-9]+*"`) DO ECHO %%f >> %fileOrderName%
ECHO File "%fileOrderName%" generated!

ECHo %onlyOrder%
IF %onlyOrder% == 1 GOTO EOF
GOTO run

:EOF

REM HELPER:
REM %~1 ? >  Remove "" from parameter
REM old way: FOR /f "usebackq delims=|" %%f IN (`dir /a-d /b "%directory%" ^| findstr /r "^d_.*"`)
REM See more here: http://technet.microsoft.com/en-us/library/bb490909.aspx
