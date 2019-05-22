$image = "rabbitmq"
$containerName = "river-queue"

$attempts = 0
$maxAttempts = 3
$startSuccess = $false

# https://stackoverflow.com/questions/54217076/docker-port-bind-fails-why-a-permission-denied

do {
    docker ps -a -f name=$containerName -q | ForEach-Object {
        "Stopping $containerName container"
        docker stop $_ | Out-Null
        docker rm $_ | Out-Null
    }

    # https://hub.docker.com/_/rabbitmq
    $imageTag = "$($image):3-management"
    docker run -d --hostname local-rabbit --name $containerName -p 5672:5672 -p 8080:15672 $imageTag

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1

    "Waiting on $image docker run success, attempts: $attempts of $maxAttempts"
    Start-Sleep 1
} while ($attempts -lt $maxAttempts)

if (!$startSuccess) {
    throw "Failed to start $image container."
}

$webMgtUrl = "http://localhost:8080"

"Checking $image status. Test url: $webMgtUrl"
$attempts = 0
$maxAttempts = 10

do {
    Start-Sleep ($attempts + 3)
    $conns5672 = Get-NetTCPConnection -LocalPort 5672 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue
    $conns8080 = Get-NetTCPConnection -LocalPort 8080 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    $status = -1

    try {
        $status = Invoke-WebRequest $webMgtUrl | ForEach-Object {$_.StatusCode}
    }
    catch {
        Write-Warning "$($_.Exception.Message)"
    }

    if ($conns5672 -and $conns5672.Length -gt 0 -and $conns8080 -and $conns8080.Length -gt 0 -and $status -eq 200) {
        "$image started. Launching $webMgtUrl"
        # login as guest/guest
        Start-Process $webMgtUrl -WindowStyle Minimized
        break;
    }

    $attempts = $attempts + 1
    "$image not fully started. Attempts: $attempts of $maxAttempts. Waiting..."
} while ($attempts -lt $maxAttempts)