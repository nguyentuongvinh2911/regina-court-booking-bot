@echo off
setlocal

:: ============================================================
::  run.bat  —  Launch the Regina Court Booking Bot
:: ============================================================
if not exist logs mkdir logs

for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"') do set "TS=%%i"
set "LOGFILE=logs\run-%TS%.log"

set "MODE=%~1"
if "%MODE%"=="" set "MODE=once"

if /I "%MODE%"=="once" goto run_once
if /I "%MODE%"=="service" goto run_service

echo.
echo  Usage: run.bat [once^|service]
echo.
echo    once     Runs the booking flow one time. This is the default.
echo    service  Runs the bot as a long-lived scheduled worker using
echo             AllowedRunDays + RunAtLocalTime from appsettings.json.
echo.
pause
exit /b 1

:run_once

echo.
echo  Starting Regina Court Booking Bot (one-time mode)...
echo  (The browser will open automatically)
echo  Log file: %LOGFILE%
echo.

echo [%date% %time%] Starting ReginaCourtBookingBot.exe --once > "%LOGFILE%"

ReginaCourtBookingBot.exe --once >> "%LOGFILE%" 2>&1

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
exit /b %EXITCODE%

:run_service
echo.
echo  Starting Regina Court Booking Bot (scheduled service mode)...
echo  This process stays running and will book on the configured days/time.
echo  Press Ctrl+C in this window to stop it.
echo  Log file: %LOGFILE%
echo.

echo [%date% %time%] Starting ReginaCourtBookingBot.exe --service > "%LOGFILE%"

ReginaCourtBookingBot.exe --service >> "%LOGFILE%" 2>&1

set "EXITCODE=%ERRORLEVEL%"
echo [%date% %time%] Exit code: %EXITCODE%>> "%LOGFILE%"

echo.
if %EXITCODE% NEQ 0 (
    echo  Scheduled service mode exited with an error. Check this log file:
    echo    %LOGFILE%
) else (
    echo  Scheduled service mode stopped. Log saved to:
    echo    %LOGFILE%
)

echo.
pause
endlocal
exit /b %EXITCODE%
