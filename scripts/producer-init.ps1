param([switch]$all, [int]$top)

Push-Location ..\producer
$env:QUEUE_NAME = "river-flow"

$attempts = 0

do {
    Start-Sleep 1

    if (!$all -and $top -eq 0) {
        "Starting producer with single queue message"
        dotnet run -- --gauges "03539600"
    }
    elseif ($top -gt 0) {
        "Starting producer with top $top queue messages"
        dotnet run -- --top $top
    }
    elseif ($all) {
        "Starting producer with full queue population"
        dotnet run -- --all
    }

    if ($?) {
        "Producer ran okay"
        break;
    }

    $attempts = $attempts + 1
    "Trying producer again. Attempts: $attempts. Waiting..."
} while ($attempts -lt 3)


Pop-Location