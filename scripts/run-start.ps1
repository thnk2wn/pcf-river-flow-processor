Push-Location (Join-Path $PSScriptRoot "..\")

dotnet build

if (!$?)
{
    Pop-Location
    return
}

Push-Location api
Invoke-Expression 'cmd /c start powershell -NoProfile -Command { dotnet run --no-build }'

Pop-Location

$attempts = 0
do {
    Start-Sleep ($attempts + 1)
    $status = -1

    try {
        $status = Invoke-WebRequest 'https://localhost:5001/swagger/index.html' | ForEach-Object {$_.StatusCode}
    }
    catch {
        Write-Warning "$($_.Exception.Message). Waiting on API to be ready"
    }

    if ($status -eq 200) {
        Start-Sleep 3
        "API ready. Launching consumer"
        break;
    }

    # can take a while, especially 1st attempt if DB needs to get created w/data seeding
    $attempts = $attempts + 1
    "API not fully started. $attempts. Waiting..."
} while ($attempts -lt 20)


Push-Location consumer

# consumer instance #1
Invoke-Expression 'cmd /c start powershell -NoProfile -Command { dotnet run --no-build }'

# consumer instance #2
Invoke-Expression 'cmd /c start powershell -NoProfile -Command { dotnet run --no-build }'

Pop-Location

Pop-Location