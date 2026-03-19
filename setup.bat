@echo off
:: ============================================================
::  setup.bat  —  Run ONCE on a new computer before using run.bat
:: ============================================================
echo.
echo  =====================================================
echo   Regina Court Booking Bot  ^|  First-time Setup
echo  =====================================================
echo.
echo  This will download the Chromium browser the bot needs.
echo  (Internet connection required, ~150 MB, takes ~1 min)
echo.
pause

powershell -ExecutionPolicy Bypass -File playwright.ps1 install chromium

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo  ERROR: Browser install failed. Check your internet connection and try again.
    pause
    exit /b 1
)

echo.
echo  =================================================
echo   Setup complete!
echo  =================================================
echo.
echo  NEXT STEP:
echo    Open secrets.json in Notepad and fill in:
echo      "Username": "your-login-name"
echo      "Password": "your-password"
echo.
echo    Then edit the settings file you want to use:
echo      appsettings.once.json     ^(manual one-time run^)
echo      appsettings.service.json  ^(scheduled background run^)
echo.
echo  Then start the bot using one of these:
echo    run-once.bat
echo    run-service.bat
echo.
echo  Or double-click run.bat to choose a mode from a menu.
echo.
pause
