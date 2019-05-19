Push-Location $PSScriptRoot

# start in new window, takes a while, lots of continual Eureka output
Invoke-Expression 'cmd /c start powershell -NoProfile -Command { .\eureka-start.ps1 }'

$eurekaUrl = "http://localhost:8761/eureka/"

"Checking Eureka status..."
$attempts = 0
$maxAttempts = 20

do {
    Start-Sleep ($attempts + 2)

    $status = -1
    "Testing $eurekaUrl"

    try {
        $status = Invoke-WebRequest $eurekaUrl | ForEach-Object {$_.StatusCode}
    }
    catch {
        Write-Warning "$($_.Exception.Message)"
    }

    if ($status -eq 200) {
        "Eureka started at $eurekaUrl"
        break;
    }

    $attempts = $attempts + 1
    "Eureka not fully started. Attempts: $attempts of $maxAttempts. Waiting..."
} while ($attempts -lt $maxAttempts)

Pop-Location