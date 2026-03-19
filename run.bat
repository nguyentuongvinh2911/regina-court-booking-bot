@echo off
:: ============================================================
::  run.bat  —  Launch the Regina Court Booking Bot
:: ============================================================
echo.
echo  Starting Regina Court Booking Bot...
echo  (The browser will open automatically)
echo.

ReginaCourtBookingBot.exe

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo  Bot exited with an error. Check the output above for details.
)

echo.
pause
