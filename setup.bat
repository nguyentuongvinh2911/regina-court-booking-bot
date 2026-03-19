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
echo    (Keep appsettings.json for booking preferences like court/time.)
echo.
echo  Then double-click run.bat to start the bot.
echo.
pause
