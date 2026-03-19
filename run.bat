@echo off
setlocal

:: ============================================================
::  run.bat  —  Launch the Regina Court Booking Bot
:: ============================================================
if not exist logs mkdir logs

for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"') do set "TS=%%i"
set "LOGFILE=logs\run-%TS%.log"

echo.
echo  Starting Regina Court Booking Bot...
echo  (The browser will open automatically)
echo  Log file: %LOGFILE%
echo.

echo [%date% %time%] Starting ReginaCourtBookingBot.exe > "%LOGFILE%"

ReginaCourtBookingBot.exe >> "%LOGFILE%" 2>&1

set "EXITCODE=%ERRORLEVEL%"
echo [%date% %time%] Exit code: %EXITCODE%>> "%LOGFILE%"

if %EXITCODE% NEQ 0 (
    echo.
    echo  Bot exited with an error. Check this log file:
    echo    %LOGFILE%
) else (
    echo.
    echo  Bot completed. Log saved to:
    echo    %LOGFILE%
)

echo.
pause
endlocal
