# ============================================================
#  publish.ps1  —  Build a distributable package for end users
# ============================================================
#  Usage (from repo root):
#    .\publish.ps1
#
#  Output:
#    - dist\ folder
#    - ReginaCourtBookingBot-win-x64.zip (ready for GitHub Release upload)
#
#  The receiving user only needs to:
#    1. Unzip
#    2. Run setup.bat  (one-time, installs Chromium)
#    3. Fill in Username / Password in secrets.json
#    4. Use run-once.bat or run-service.bat
# ============================================================

$ErrorActionPreference = "Stop"

$project  = "src\ReginaCourtBookingBot\ReginaCourtBookingBot.csproj"
$outDir   = "dist"
$zipName  = "ReginaCourtBookingBot-win-x64.zip"
$zipPath  = Join-Path (Get-Location) $zipName

Write-Host "Cleaning previous build..." -ForegroundColor Cyan
if (Test-Path $outDir) { Remove-Item $outDir -Recurse -Force }

Write-Host "Publishing self-contained win-x64 build..." -ForegroundColor Cyan
dotnet publish $project `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    --output $outDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed."
    exit 1
}

# Copy the end-user helper scripts into the dist folder
Copy-Item "setup.bat" $outDir -Force
Copy-Item "run.bat"   $outDir -Force
Copy-Item "run-once.bat" $outDir -Force
Copy-Item "run-service.bat" $outDir -Force
Copy-Item "README.md" $outDir -Force

Write-Host "Creating release zip..." -ForegroundColor Cyan
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Compress-Archive -Path "$outDir\*" -DestinationPath $zipPath -Force

Write-Host ""
Write-Host "Done! Package is ready in: $((Resolve-Path $outDir).Path)" -ForegroundColor Green
Write-Host "Release zip: $zipPath" -ForegroundColor Green
Write-Host ""
Write-Host "Before sharing, verify secrets.json has:"
Write-Host "  Username  = \"\"" -ForegroundColor Yellow
Write-Host "  Password  = \"\"" -ForegroundColor Yellow
Write-Host "(each user fills in their own credentials after unzipping)"
