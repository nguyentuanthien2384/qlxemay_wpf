param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "[1/5] dotnet info" -ForegroundColor Cyan
dotnet --info

Write-Host "[2/5] restore" -ForegroundColor Cyan
dotnet restore .\QLXeMay.sln

Write-Host "[3/5] build" -ForegroundColor Cyan
dotnet build .\QLXeMay.sln -c $Configuration --no-restore

Write-Host "[4/5] tests" -ForegroundColor Cyan
dotnet run --project .\QLXeMay.Tests\QLXeMay.Tests.csproj -c $Configuration --no-build

Write-Host "[5/5] publish dry-run" -ForegroundColor Cyan
dotnet publish .\QLXeMay.csproj -c $Configuration -r win-x64 --self-contained true -o .\publish\win-x64

Write-Host "Source verification completed." -ForegroundColor Green
