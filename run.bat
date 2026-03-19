@echo off
setlocal

set "MODE=%~1"
if /I "%MODE%"=="once" goto run_once
if /I "%MODE%"=="service" goto run_service

echo.
echo  =====================================================
echo   Regina Court Booking Bot ^| Choose Run Mode
echo  =====================================================
echo.
echo    1. One-time run
echo       Uses: appsettings.once.json
echo.
echo    2. Scheduled background run
echo       Uses: appsettings.service.json
echo.
choice /C 12 /N /M "Choose 1 or 2: "
if errorlevel 2 goto run_service
goto run_once

:run_once
call "%~dp0run-once.bat"
endlocal
exit /b %ERRORLEVEL%

:run_service
call "%~dp0run-service.bat"
endlocal
exit /b %ERRORLEVEL%
