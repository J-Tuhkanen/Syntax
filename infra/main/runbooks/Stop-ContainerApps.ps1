<#
.SYNOPSIS
    Stops every container app in a resource group. Triggered by the budget
    Action Group when actual spend reaches 100 % of the budget.

.DESCRIPTION
    Uses the Automation account's system-assigned managed identity and plain
    ARM REST calls, so it needs no Az modules. Stopping an app removes all of
    its replicas; Postgres receives its stop signal (SIGINT, fast shutdown) and
    writes a shutdown checkpoint, so no data is lost. Postgres is stopped last.

    The webhook passes SubscriptionId and ResourceGroupName as fixed
    parameters. Anything in the request body is ignored.

    Undo with infra/scripts/start-apps.sh or by re-running the deploy pipeline.
#>
param(
    [Parameter(Mandatory = $false)]
    [object] $WebhookData,

    [Parameter(Mandatory = $true)]
    [string] $SubscriptionId,

    [Parameter(Mandatory = $true)]
    [string] $ResourceGroupName
)

$ErrorActionPreference = 'Stop'
$arm = 'https://management.azure.com'
$apiVersion = '2025-01-01'

# Access token for the system-assigned managed identity.
$tokenHeaders = @{ 'X-IDENTITY-HEADER' = $env:IDENTITY_HEADER; 'Metadata' = 'True' }
$token = Invoke-RestMethod -Method Get -Headers $tokenHeaders `
    -Uri ($env:IDENTITY_ENDPOINT + '?resource=https://management.azure.com/')
$headers = @{ Authorization = "Bearer $($token.access_token)" }

$listUri = "$arm/subscriptions/$SubscriptionId/resourceGroups/$ResourceGroupName/providers/Microsoft.App/containerApps?api-version=$apiVersion"
$apps = @((Invoke-RestMethod -Method Get -Uri $listUri -Headers $headers).value)

if ($apps.Count -eq 0) {
    Write-Output "No container apps in $ResourceGroupName."
    return
}

# Everything else first, Postgres last.
$ordered = @($apps | Where-Object { $_.name -notlike '*-postgres-*' }) +
           @($apps | Where-Object { $_.name -like '*-postgres-*' })

$failed = @()
foreach ($app in $ordered) {
    try {
        Write-Output "Stopping $($app.name)"
        Invoke-RestMethod -Method Post -Headers $headers -ContentType 'application/json' -Body '{}' `
            -Uri "$arm$($app.id)/stop?api-version=$apiVersion" | Out-Null

        if ($app.name -notlike '*-postgres-*') {
            # Give clients a moment to disconnect before the database goes down.
            Start-Sleep -Seconds 20
        }
    }
    catch {
        Write-Error "Failed to stop $($app.name): $($_.Exception.Message)" -ErrorAction Continue
        $failed += $app.name
    }
}

if ($failed.Count -gt 0) {
    throw "Could not stop: $($failed -join ', ')"
}

Write-Output "Stopped $($ordered.Count) container app(s) in $ResourceGroupName."
