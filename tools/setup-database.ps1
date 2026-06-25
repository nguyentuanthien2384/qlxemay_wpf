param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$ScriptPath = "$PSScriptRoot\..\Database\CreateDatabase_Complete.sql"
)

Write-Host "QLXeMay database setup" -ForegroundColor Cyan
Write-Host "Server: $Server"
Write-Host "Script: $ScriptPath"

$resolvedScriptPath = Resolve-Path $ScriptPath -ErrorAction SilentlyContinue
if (-not $resolvedScriptPath) {
    Write-Error "Không tìm thấy file SQL: $ScriptPath"
    exit 1
}

$sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmd) {
    Write-Error "Không tìm thấy sqlcmd. Hãy cài SQL Server Command Line Utilities hoặc chạy script bằng SSMS."
    exit 1
}

& sqlcmd -S $Server -E -b -i $resolvedScriptPath.Path
if ($LASTEXITCODE -ne 0) {
    Write-Error "Thiết lập database thất bại. Kiểm tra log lỗi phía trên."
    exit $LASTEXITCODE
}

Write-Host "Database btl đã sẵn sàng." -ForegroundColor Green
