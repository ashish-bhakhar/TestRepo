#!/bin/bash
echo "========================================"
echo " TalentFlow - Recruitment Manager"
echo "========================================"
echo ""
if ! command -v dotnet &>/dev/null; then
    echo "ERROR: .NET 8 SDK not found."
    echo "Download from: https://dotnet.microsoft.com/download"
    exit 1
fi
dotnet restore
echo ""
echo "Starting app on http://localhost:5000"
echo "Press Ctrl+C to stop."
echo ""
open http://localhost:5000 2>/dev/null || xdg-open http://localhost:5000 2>/dev/null || true
dotnet run
