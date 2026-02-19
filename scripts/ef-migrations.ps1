[CmdletBinding()]
param(
    [ValidateSet("add-update", "add", "update", "remove", "list")]
    [string]$Action = "add-update",

    [string]$MigrationName,

    [ValidateSet("all", "auth", "asset", "parameter")]
    [string]$Context = "all",

    [string]$StartupProject = "Server/Application/Api/Api.csproj",

    [string]$OutputDir = "Data/Migrations",

    [string]$TargetMigration,

    [switch]$NoBuild,

    [switch]$ContinueOnError,

    [switch]$Force,

    [switch]$Menu
)

$ErrorActionPreference = "Stop"

function Assert-Tooling {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "dotnet CLI not found."
    }

    & dotnet ef --version | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet-ef not available. Install with: dotnet tool install --global dotnet-ef"
    }
}

function Get-SelectedContexts {
    $all = @(
        [PSCustomObject]@{
            Key       = "auth"
            Project   = "Server/Modules/Auth/Auth/Auth.csproj"
            DbContext = "Auth.Data.AuthDbContext"
        },
        [PSCustomObject]@{
            Key       = "asset"
            Project   = "Server/Modules/Asset/Asset/Asset.csproj"
            DbContext = "Asset.Data.AssetDbContext"
        },
        [PSCustomObject]@{
            Key       = "parameter"
            Project   = "Server/Modules/Parameter/Parameter/Parameter.csproj"
            DbContext = "Parameter.Data.ParameterDbContext"
        }
    )

    if ($Context -eq "all") {
        return $all
    }

    return $all | Where-Object { $_.Key -eq $Context }
}

function Read-MenuChoice {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Title,

        [Parameter(Mandatory = $true)]
        [string[]]$Options,

        [int]$Default = 1
    )

    Write-Host ""
    Write-Host $Title -ForegroundColor Yellow
    for ($i = 0; $i -lt $Options.Count; $i++) {
        Write-Host ("[{0}] {1}" -f ($i + 1), $Options[$i])
    }

    while ($true) {
        $input = Read-Host "Select (1-$($Options.Count)) [default:$Default]"
        if ([string]::IsNullOrWhiteSpace($input)) {
            return ($Default - 1)
        }

        $choice = 0
        if ([int]::TryParse($input, [ref]$choice) -and $choice -ge 1 -and $choice -le $Options.Count) {
            return ($choice - 1)
        }

        Write-Host "Invalid selection. Please try again." -ForegroundColor Red
    }
}

function Read-YesNo {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Prompt,

        [bool]$Default = $false
    )

    $hint = if ($Default) { "Y/n" } else { "y/N" }

    while ($true) {
        $input = Read-Host "$Prompt [$hint]"
        if ([string]::IsNullOrWhiteSpace($input)) {
            return $Default
        }

        switch -Regex ($input.Trim()) {
            "^(y|yes)$" { return $true }
            "^(n|no)$" { return $false }
            default { Write-Host "Please answer y or n." -ForegroundColor Red }
        }
    }
}

function Read-RequiredText {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Prompt
    )

    while ($true) {
        $value = Read-Host $Prompt
        if (-not [string]::IsNullOrWhiteSpace($value)) {
            return $value.Trim()
        }

        Write-Host "This value is required." -ForegroundColor Red
    }
}

function Start-InteractiveMenu {
    Write-Host ""
    Write-Host "EF Migration Menu" -ForegroundColor Green

    $actionValues = @("add-update", "add", "update", "remove", "list")
    $actionLabels = @(
        "add-update (create migration + update database)",
        "add (create migration only)",
        "update (update database only)",
        "remove (remove latest migration)",
        "list (show migrations)"
    )

    $actionIndex = Read-MenuChoice -Title "Select action" -Options $actionLabels -Default 1
    $script:Action = $actionValues[$actionIndex]

    $contextValues = @("all", "auth", "asset", "parameter")
    $contextLabels = @(
        "all contexts",
        "auth",
        "asset",
        "parameter"
    )

    $contextIndex = Read-MenuChoice -Title "Select context" -Options $contextLabels -Default 1
    $script:Context = $contextValues[$contextIndex]

    if ($script:Action -eq "add" -or $script:Action -eq "add-update") {
        $script:MigrationName = Read-RequiredText -Prompt "Migration name"
    }

    if ($script:Action -eq "update") {
        $script:TargetMigration = Read-Host "Target migration (blank = latest, use 0 to rollback all)"
    }

    if ($script:Action -eq "remove") {
        $script:Force = Read-YesNo -Prompt "Use --force for remove?" -Default $false
    }

    $script:NoBuild = Read-YesNo -Prompt "Use --no-build?" -Default $false
    $script:ContinueOnError = Read-YesNo -Prompt "Continue when one context fails?" -Default $false

    Write-Host ""
    Write-Host "Selected: Action='$script:Action', Context='$script:Context'" -ForegroundColor Cyan
    if ($script:MigrationName) {
        Write-Host "MigrationName='$script:MigrationName'" -ForegroundColor Cyan
    }
    if ($script:TargetMigration) {
        Write-Host "TargetMigration='$script:TargetMigration'" -ForegroundColor Cyan
    }
}

function Invoke-Ef {
    param(
        [Parameter(Mandatory = $true)]
        [PSCustomObject]$Ctx,

        [Parameter(Mandatory = $true)]
        [string[]]$EfArgs
    )

    Write-Host "[$($Ctx.Key)] dotnet ef $($EfArgs -join ' ')" -ForegroundColor Cyan
    & dotnet ef @EfArgs

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet ef failed for context '$($Ctx.Key)'."
    }
}

function Build-CommonArgs {
    param(
        [Parameter(Mandatory = $true)]
        [PSCustomObject]$Ctx
    )

    $args = @(
        "--project", $Ctx.Project,
        "--startup-project", $StartupProject,
        "--context", $Ctx.DbContext
    )

    if ($NoBuild) {
        $args += "--no-build"
    }

    return $args
}

function Run-AddMigration {
    param([PSCustomObject]$Ctx)

    $args = @("migrations", "add", $MigrationName, "--output-dir", $OutputDir)
    $args += Build-CommonArgs -Ctx $Ctx
    Invoke-Ef -Ctx $Ctx -EfArgs $args
}

function Run-UpdateDatabase {
    param([PSCustomObject]$Ctx)

    $args = @("database", "update")
    if (-not [string]::IsNullOrWhiteSpace($TargetMigration)) {
        $args += $TargetMigration
    }

    $args += Build-CommonArgs -Ctx $Ctx
    Invoke-Ef -Ctx $Ctx -EfArgs $args
}

function Run-RemoveMigration {
    param([PSCustomObject]$Ctx)

    $args = @("migrations", "remove")
    if ($Force) {
        $args += "--force"
    }

    $args += Build-CommonArgs -Ctx $Ctx
    Invoke-Ef -Ctx $Ctx -EfArgs $args
}

function Run-ListMigrations {
    param([PSCustomObject]$Ctx)

    $args = @("migrations", "list")
    $args += Build-CommonArgs -Ctx $Ctx
    Invoke-Ef -Ctx $Ctx -EfArgs $args
}

if ($Menu -or $PSBoundParameters.Count -eq 0) {
    Start-InteractiveMenu
}

if (($Action -eq "add" -or $Action -eq "add-update") -and [string]::IsNullOrWhiteSpace($MigrationName)) {
    throw "-MigrationName is required when Action is 'add' or 'add-update'."
}

Assert-Tooling

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $repoRoot

$selectedContexts = Get-SelectedContexts
$failedContexts = @()

foreach ($ctx in $selectedContexts) {
    try {
        switch ($Action) {
            "add-update" {
                Run-AddMigration -Ctx $ctx
                Run-UpdateDatabase -Ctx $ctx
                break
            }
            "add" {
                Run-AddMigration -Ctx $ctx
                break
            }
            "update" {
                Run-UpdateDatabase -Ctx $ctx
                break
            }
            "remove" {
                Run-RemoveMigration -Ctx $ctx
                break
            }
            "list" {
                Run-ListMigrations -Ctx $ctx
                break
            }
        }
    }
    catch {
        Write-Host "[$($ctx.Key)] FAILED: $($_.Exception.Message)" -ForegroundColor Red
        $failedContexts += $ctx.Key
        if (-not $ContinueOnError) {
            break
        }
    }
}

if ($failedContexts.Count -gt 0) {
    throw "Finished with errors. Failed contexts: $($failedContexts -join ', ')"
}

Write-Host "Done. Action '$Action' completed for context '$Context'." -ForegroundColor Green
