[CmdletBinding()]
param(
    [string]$ApiBaseUrl = "http://localhost:5176",
    [string]$AdminEmail = "alex.morgan@swu-ams.local",
    [string]$AdminPassword = "P@ssw0rd",
    [string]$Password = "P@ssw0rd",
    [string]$AccessToken,
    [switch]$SkipBootstrapAdmin,
    [switch]$ContinueOnError
)

$ErrorActionPreference = "Stop"

$baseUrl = $ApiBaseUrl.TrimEnd("/")
$loginEndpoint = "$baseUrl/auth/login"
$logoutEndpoint = "$baseUrl/auth/logout"
$signupEndpoint = "$baseUrl/auth/signup/user"

function Escape-SqlLiteral {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    return $Value.Replace("'", "''")
}

function Invoke-BootstrapAdmin {
    if ($SkipBootstrapAdmin) {
        return
    }

    if ($AdminPassword -ne "P@ssw0rd") {
        throw "Bootstrap admin only supports the default password P@ssw0rd because the password hash is embedded in this script."
    }

    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        throw "Docker CLI not found. Start PostgreSQL and pass -AccessToken, or install Docker."
    }

    $adminEmailSql = Escape-SqlLiteral -Value $AdminEmail

    $sql = @"
BEGIN;

INSERT INTO auth."UserRole"
    ("Id", "RoleCode", "RoleName", "RoleDescription", "CreateBy")
VALUES
    ('1b4dc80d-c3e8-4e6d-a9d6-2bbd478d2d00', 'ADMIN', 'ADMIN', 'System administrator', 'SYSTEM'),
    ('b89541a9-8ce6-4e95-820d-7c7785f96f01', 'HOD', 'HOD', 'Department head', 'SYSTEM'),
    ('52400f2b-0eaf-4921-9ad7-e527fb52bb02', 'TEACHER', 'TEACHER', 'Lecturer', 'SYSTEM'),
    ('f801e95d-340b-4f34-8a6a-9deac4168003', 'STUDENT', 'STUDENT', 'Student', 'SYSTEM')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO auth."UserName"
    ("Id", "Username", "Email", "PasswordHash", "RoleId", "CreateBy")
SELECT
    '10000000-0000-0000-0000-000000000001'::uuid,
    'Alex Morgan',
    '$adminEmailSql',
    'AQAAAAIAAYagAAAAEAbpPJYygXkPW/wFI+Lq3s5EOPQIjrDrMHVaMiXOQPTehkLglhciIHWDr161BT+ciw==',
    roles."Id",
    'SYSTEM'
FROM auth."UserRole" roles
WHERE roles."RoleCode" = 'ADMIN'
  AND NOT EXISTS (
      SELECT 1
      FROM auth."UserName" existing
      WHERE existing."Email" = '$adminEmailSql'
         OR existing."Username" = 'Alex Morgan'
  );

COMMIT;
"@

    Write-Host "Bootstrapping first admin in PostgreSQL: $AdminEmail" -ForegroundColor Yellow

    $sql |
        docker compose exec -T postgres sh -c 'psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$POSTGRES_DB"'

    if ($LASTEXITCODE -ne 0) {
        throw "Bootstrap admin seed failed."
    }
}

function Invoke-ClearAdminSession {
    if ($SkipBootstrapAdmin) {
        return
    }

    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        throw "Docker CLI not found. Cannot clear admin session. Wait for the session to expire or pass -AccessToken."
    }

    $adminEmailSql = Escape-SqlLiteral -Value $AdminEmail

    $sql = @"
UPDATE auth."UserName"
SET "Session" = NULL,
    "SessionActiveOn" = NULL
WHERE "Email" = '$adminEmailSql'
   OR "Username" = '$adminEmailSql';
"@

    Write-Host "Clearing admin session in PostgreSQL: $AdminEmail" -ForegroundColor Yellow

    $sql |
        docker compose exec -T postgres sh -c 'psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$POSTGRES_DB"'

    if ($LASTEXITCODE -ne 0) {
        throw "Clear admin session failed."
    }
}

function Invoke-AdminLogin {
    Write-Host "POST $loginEndpoint"
    Write-Host "Logging in as admin: $AdminEmail"

    $loginBody = @{
        email = $AdminEmail
        password = $AdminPassword
    } | ConvertTo-Json -Depth 10

    $loginResponse = Invoke-RestMethod -Method Post -Uri $loginEndpoint -ContentType "application/json" -Body $loginBody
    return $loginResponse.accessToken
}

function Invoke-AdminLogout {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Token
    )

    Write-Host "POST $logoutEndpoint"
    Write-Host "Logging out seeded admin session"

    try {
        Invoke-RestMethod `
            -Method Post `
            -Uri $logoutEndpoint `
            -Headers @{ Authorization = "Bearer $Token" } `
            -ContentType "application/json" | Out-Null
    }
    catch {
        Write-Host "Admin logout failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

$users = @(
    @{
        username = "Olivia Carter"
        email = "olivia.carter@swu-ams.local"
        password = $Password
        roleCode = "HOD"
    },
    @{
        username = "Benjamin Reed"
        email = "benjamin.reed@swu-ams.local"
        password = $Password
        roleCode = "TEACHER"
    },
    @{
        username = "Charlotte Hayes"
        email = "charlotte.hayes@swu-ams.local"
        password = $Password
        roleCode = "TEACHER"
    },
    @{
        username = "Daniel Brooks"
        email = "daniel.brooks@swu-ams.local"
        password = $Password
        roleCode = "TEACHER"
    },
    @{
        username = "Emily Foster"
        email = "emily.foster@swu-ams.local"
        password = $Password
        roleCode = "TEACHER"
    },
    @{
        username = "Henry Collins"
        email = "henry.collins@swu-ams.local"
        password = $Password
        roleCode = "TEACHER"
    },
    @{
        username = "Sophia Bennett"
        email = "sophia.bennett@swu-ams.local"
        password = $Password
        roleCode = "TEACHER"
    },
    @{
        username = "Liam Anderson"
        email = "liam.anderson@student.swu-ams.local"
        password = $Password
        roleCode = "STUDENT"
    },
    @{
        username = "Emma Thompson"
        email = "emma.thompson@student.swu-ams.local"
        password = $Password
        roleCode = "STUDENT"
    },
    @{
        username = "Noah Mitchell"
        email = "noah.mitchell@student.swu-ams.local"
        password = $Password
        roleCode = "STUDENT"
    },
    @{
        username = "Ava Richardson"
        email = "ava.richardson@student.swu-ams.local"
        password = $Password
        roleCode = "STUDENT"
    },
    @{
        username = "Ethan Cooper"
        email = "ethan.cooper@student.swu-ams.local"
        password = $Password
        roleCode = "STUDENT"
    },
    @{
        username = "Mia Sullivan"
        email = "mia.sullivan@student.swu-ams.local"
        password = $Password
        roleCode = "STUDENT"
    },
    @{
        username = "Lucas Parker"
        email = "lucas.parker@student.swu-ams.local"
        password = $Password
        roleCode = "STUDENT"
    }
)

if ([string]::IsNullOrWhiteSpace($AccessToken)) {
    try {
        $AccessToken = Invoke-AdminLogin
    }
    catch {
        Write-Host ""
        Write-Host "Admin login failed. Trying one-file bootstrap and session cleanup for the first admin..." -ForegroundColor Yellow
        Invoke-BootstrapAdmin
        Invoke-ClearAdminSession
        $AccessToken = Invoke-AdminLogin
    }
}

if ([string]::IsNullOrWhiteSpace($AccessToken)) {
    throw "Access token is required. Login did not return accessToken."
}

$headers = @{
    Authorization = "Bearer $AccessToken"
}

$results = foreach ($user in $users) {
    $body = $user | ConvertTo-Json -Depth 10

    Write-Host "POST $signupEndpoint"
    Write-Host "Creating user: $($user.username) <$($user.email)> [$($user.roleCode)]"

    try {
        $response = Invoke-RestMethod -Method Post -Uri $signupEndpoint -Headers $headers -ContentType "application/json" -Body $body

        [PSCustomObject]@{
            Username = $user.username
            Email = $user.email
            Role = $user.roleCode
            Succeeded = $true
            Response = $response
            Error = $null
        }
    }
    catch {
        $errorMessage = $_.Exception.Message

        if (-not $ContinueOnError) {
            throw
        }

        [PSCustomObject]@{
            Username = $user.username
            Email = $user.email
            Role = $user.roleCode
            Succeeded = $false
            Response = $null
            Error = $errorMessage
        }
    }
}

Write-Host ""
Write-Host "Processed $($results.Count) users."

Invoke-AdminLogout -Token $AccessToken

$results
