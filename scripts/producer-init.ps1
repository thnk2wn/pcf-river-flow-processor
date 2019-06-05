param([switch]$all, [int]$top)

Push-Location ..\producer

$runtime = "linux-x64"
$framework = "netcoreapp2.2"
$config = "Debug"

$attempts = 0

do {
    Start-Sleep 1

    if (!$all -and $top -eq 0) {
        "Starting producer with single queue message"
        dotnet run -c $config -f $framework --runtime $runtime --no-launch-profile -v n -- --gauges "03539600"
    }
    elseif ($top -gt 0) {
        "Starting producer with top $top queue messages"
        dotnet run -c $config -f $framework --runtime $runtime --no-launch-profile -v n -- --top $top
    }
    elseif ($all) {
        "Starting producer with full queue population"
        dotnet run -c $config -f $framework --runtime $runtime --no-launch-profile -v n -- --all
    }
    else {
        dotnet run -c $config -f $framework --runtime $runtime --no-launch-profile -v n
    }

    if ($?) {
        "Producer ran okay"
        break;
    }

    $attempts = $attempts + 1
    "Trying producer again. Attempts: $attempts. Waiting..."
} while ($attempts -lt 2)


Pop-Location