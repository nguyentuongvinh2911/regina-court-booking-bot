@echo off
setlocal

if not exist logs mkdir logs
for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"') do set "TS=%%i"
set "LOGFILE=logs\run-once-%TS%.log"

echo.
echo  Starting Regina Court Booking Bot ^(one-time mode^)...
echo  Settings file: appsettings.once.json
echo  Log file: %LOGFILE%
echo.

echo [%date% %time%] Starting ReginaCourtBookingBot.exe --once > "%LOGFILE%"
ReginaCourtBookingBot.exe --once >> "%LOGFILE%" 2>&1

set "EXITCODE=%ERRORLEVEL%"
echo [%date% %time%] Exit code: %EXITCODE%>> "%LOGFILE%"

echo.
if %EXITCODE% NEQ 0 (
    echo  Bot exited with an error. Check this log file:
    echo    %LOGFILE%
) else (
    echo  Bot completed. Log saved to:
    echo    %LOGFILE%
)

echo.
pause
endlocal
exit /b %EXITCODE%
