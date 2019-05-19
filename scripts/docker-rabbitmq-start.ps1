$name = "river-queue"

$attempts = 0
$maxAttempts = 3
$startSuccess = $false

# https://stackoverflow.com/questions/54217076/docker-port-bind-fails-why-a-permission-denied

do {
    docker ps -a -f name=$name -q | ForEach-Object { "Stopping $name container"; docker stop $_; docker rm $_ }
    # https://hub.docker.com/_/rabbitmq
    docker run -d --hostname local-rabbit --name $name -p 5672:5672 -p 8080:15672 rabbitmq:3-management

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1

    "Waiting on RabbitMQ docker run success, attempts: $attempts of $maxAttempts"
    Start-Sleep 1
} while ($attempts -lt $maxAttempts)

if (!$startSuccess) {
    throw "Failed to start Rabbit MQ container."
}

$webMgtUrl = "http://localhost:8080"

"Checking RabbitMQ status..."
$attempts = 0
$maxAttempts = 10

do {
    Start-Sleep ($attempts + 1)
    $conns5672 = Get-NetTCPConnection -LocalPort 5672 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue
    $conns8080 = Get-NetTCPConnection -LocalPort 8080 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    $status = -1

    try {
        "Testing $webMgtUrl"
        $status = Invoke-WebRequest $webMgtUrl | ForEach-Object {$_.StatusCode}
    }
    catch {
        Write-Warning "$($_.Exception.Message)"
    }

    if ($conns5672 -and $conns5672.Length -gt 0 -and $conns8080 -and $conns8080.Length -gt 0 -and $status -eq 200) {
        "RabbitMQ started. Launching $webMgtUrl"
        # login as guest/guest
        Start-Process $webMgtUrl -WindowStyle Minimized
        break;
    }

    $attempts = $attempts + 1
    "RabbitMQ not fully started. Attempts: $attempts of $maxAttempts. Waiting..."
} while ($attempts -lt $maxAttempts)