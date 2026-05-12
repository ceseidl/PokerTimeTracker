# Pipeline de empacotamento — gera o MSI a partir do publish self-contained.
#
# Pré-requisitos:
#   - .NET 10 SDK
#   - WiX Toolset >= 6.0  (`dotnet tool install --global wix`)
#   - Extensão UI:        `wix extension add WixToolset.UI.wixext`
#
# Uso:
#   pwsh -File installer/build-msi.ps1                    # publish + harvest + MSI
#   pwsh -File installer/build-msi.ps1 -SkipPublish       # só rebuild do MSI
#   pwsh -File installer/build-msi.ps1 -Version 1.0.0.0   # custom version

[CmdletBinding()]
param(
    [string]$Version = "1.0.1.0",
    [switch]$SkipPublish
)

$ErrorActionPreference = "Stop"

$root           = Resolve-Path "$PSScriptRoot\.."
$wpfProj        = Join-Path $root "src\TimePoker.Wpf\TimePoker.Wpf.csproj"
$publishDir     = Join-Path $root "installer\publish"
$harvestedFile  = Join-Path $PSScriptRoot "HarvestedFiles.wxs"
$productFile    = Join-Path $PSScriptRoot "Product.wxs"
$outputDir      = Join-Path $PSScriptRoot "out"
$msiOutput      = Join-Path $outputDir "TimePoker-$Version.msi"

Write-Host "==> Limpando saídas antigas..." -ForegroundColor Cyan
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
if (Test-Path $outputDir)  { Remove-Item $outputDir -Recurse -Force }
New-Item -ItemType Directory -Path $outputDir | Out-Null

if (-not $SkipPublish) {
    Write-Host "==> dotnet publish (self-contained, win-x64)..." -ForegroundColor Cyan
    dotnet publish $wpfProj `
        --configuration Release `
        --runtime win-x64 `
        --self-contained true `
        -p:PublishSingleFile=false `
        -p:DebugType=embedded `
        -p:Version=$Version `
        --output $publishDir
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish falhou (exit $LASTEXITCODE)" }
}

# Inclui pasta docs/ com MANUAL.md (e MANUAL.html se existir)
$docsSrc = Join-Path $root "docs"
$docsDst = Join-Path $publishDir "docs"
if (Test-Path $docsSrc) {
    Write-Host "==> Copiando docs/ pro publish..." -ForegroundColor Cyan
    Copy-Item -Path $docsSrc -Destination $docsDst -Recurse -Force
}

Write-Host "==> Harvest dos arquivos publicados..." -ForegroundColor Cyan
$xmlNs = "http://wixtoolset.org/schemas/v4/wxs"
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('<?xml version="1.0" encoding="UTF-8"?>')
[void]$sb.AppendLine("<Wix xmlns=`"$xmlNs`">")
[void]$sb.AppendLine('  <Fragment>')
[void]$sb.AppendLine('    <ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">')

$files = Get-ChildItem -Path $publishDir -Recurse -File
$idx = 0
foreach ($f in $files) {
    $idx++
    $relative = $f.FullName.Substring($publishDir.Length).TrimStart('\','/')
    # subpasta relativa ao INSTALLFOLDER (sem o nome do arquivo)
    $subdir = [System.IO.Path]::GetDirectoryName($relative)
    $compId = "C_" + ([System.Guid]::NewGuid().ToString('N'))
    $fileId = "F_" + ([System.Guid]::NewGuid().ToString('N'))
    $guid   = [System.Guid]::NewGuid().ToString().ToUpper()
    if ([string]::IsNullOrEmpty($subdir)) {
        [void]$sb.AppendLine("      <Component Id=`"$compId`" Guid=`"$guid`" Bitness=`"always64`">")
    } else {
        [void]$sb.AppendLine("      <Component Id=`"$compId`" Guid=`"$guid`" Bitness=`"always64`" Subdirectory=`"$subdir`">")
    }
    [void]$sb.AppendLine("        <File Id=`"$fileId`" Source=`"$($f.FullName)`" KeyPath=`"yes`" />")
    [void]$sb.AppendLine('      </Component>')
}

[void]$sb.AppendLine('    </ComponentGroup>')
[void]$sb.AppendLine('  </Fragment>')
[void]$sb.AppendLine('</Wix>')
[System.IO.File]::WriteAllText($harvestedFile, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "    $idx arquivos colhidos." -ForegroundColor Gray

Write-Host "==> wix build (gerando MSI)..." -ForegroundColor Cyan
Push-Location $PSScriptRoot
try {
    & wix build `
        $productFile $harvestedFile `
        -ext WixToolset.UI.wixext `
        -arch x64 `
        -d "ProductVersion=$Version" `
        -o $msiOutput
    if ($LASTEXITCODE -ne 0) { throw "wix build falhou (exit $LASTEXITCODE)" }
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "==> MSI gerado: $msiOutput" -ForegroundColor Green
Write-Host "    Tamanho: $([math]::Round(((Get-Item $msiOutput).Length / 1MB), 2)) MB"
