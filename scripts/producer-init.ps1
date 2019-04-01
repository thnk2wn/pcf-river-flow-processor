param([switch]$all)

Push-Location ..\producer
$env:QUEUE_NAME = "river-flow"

$attempts = 0

$singleOnly = $false

if ($env:ASPNETCORE_ENVIRONMENT -eq 'Local') {
    if (!$all) {
        $singleOnly = $true
    }
}

do {
    Start-Sleep 1

    if ($singleOnly) {
        "Start producer with initialization and single queue message"
        # put one sample guage into queue for testing
        dotnet run -- -i -g "03539600"
    }
    else {
        "Start producer with initialization and full queue population"
        # init and full queue populate
        dotnet run -- -i
    }

    if ($?) {
        "Procucer ran okay"
        break;
    }

    $attempts = $attempts + 1
    "Tring producer init again. Attempts: $attempts. Waiting..."
} while ($attempts -lt 3)


Pop-Location