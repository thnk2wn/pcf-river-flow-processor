$image = "redis"
$containerName = "river-cache"
$hostPort = 6379
$containerPort = 6379

$attempts = 0
$maxAttempts = 3
$startSuccess = $false

do {
    docker ps -a -f name=$containerName -q | ForEach-Object {
        "Stopping $containerName container"
        docker stop $_ | Out-Null
        docker rm $_ | Out-Null
    }

    # https://hub.docker.com/_/redis
    "Starting $image"
    $ports = "$($hostPort):$($containerPort)"
    docker run -d -p $ports --name $containerName $image

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1
    "Waiting on $image docker run success. Attempts: $attempts of $maxAttempts..."
    Start-Sleep 2
} while ($attempts -lt $maxAttempts)

if (!$startSuccess) {
    throw "Failed to start $image container."
}

$attempts = 0
$maxAttempts = 5
"Checking $image status..."

do {
    Start-Sleep ($attempts + 2)
    $conns = Get-NetTCPConnection -LocalPort $hostPort -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    if ($conns -and $conns.Length -gt 0) {
        # port may be open but mysql may not be fully started. test a command
        "Running $image connectivity test"
        docker exec $containerName redis-cli

        if ($?) {
            "$image started"
            break;
        }
    }

    $attempts = $attempts + 1
    "$image not fully started. Attempts: $attempts of $maxAttempts. Waiting..."
} while ($attempts -lt $maxAttempts)