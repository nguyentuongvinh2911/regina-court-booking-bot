@echo off
setlocal

if not exist logs mkdir logs
for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"') do set "TS=%%i"
set "LOGFILE=logs\run-service-%TS%.log"

echo.
echo  Starting Regina Court Booking Bot ^(scheduled mode^)...
echo  Settings file: appsettings.service.json
echo  This process stays running and books on the configured days/time.
echo  Press Ctrl+C in this window to stop it.
echo  Log file: %LOGFILE%
echo.

echo [%date% %time%] Starting ReginaCourtBookingBot.exe --service > "%LOGFILE%"
ReginaCourtBookingBot.exe --service >> "%LOGFILE%" 2>&1

set "EXITCODE=%ERRORLEVEL%"
echo [%date% %time%] Exit code: %EXITCODE%>> "%LOGFILE%"

echo.
if %EXITCODE% NEQ 0 (
    echo  Scheduled mode exited with an error. Check this log file:
    echo    %LOGFILE%
) else (
    echo  Scheduled mode stopped. Log saved to:
    echo    %LOGFILE%
)

echo.
pause
endlocal
exit /b %EXITCODE%
