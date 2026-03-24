@echo off
echo ========================================
echo  TalentFlow - Recruitment Manager
echo ========================================
echo.
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET 8 SDK not found.
    echo Download from: https://dotnet.microsoft.com/download
    pause & exit /b 1
)
echo Restoring packages...
dotnet restore
echo.
echo Starting app on http://localhost:5000
echo Press Ctrl+C to stop.
echo.
start "" http://localhost:5000
dotnet run
pause
