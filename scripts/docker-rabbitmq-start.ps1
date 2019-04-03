$name = "river-queue"

$attempts = 0
$startSuccess = $false

do {
    docker ps -a -f name=$name -q | % { docker stop $_; docker rm $_ }
    # https://hub.docker.com/_/rabbitmq
    docker run -d --hostname local-rabbit --name $name -p 5672:5672 -p 8080:15672 rabbitmq:3-management

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1

    "Waiting on docker run success, attempts: $attempts"
    Start-Sleep 1
} while ($attempts -lt 3)

if (!$startSuccess) {
    throw "Failed to start Rabbit MQ container."
}

# give rabbit a bit to start up. ideally would monitor in loop
# Start-Sleep -Seconds 10

# login as guest/guest
# Start-Process 'http://localhost:8080'

"Checking RabbitMQ status..."
$attempts = 0
do {
    Start-Sleep 2
    $conns5672 = Get-NetTCPConnection -LocalPort 5672 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue
    $conns8080 = Get-NetTCPConnection -LocalPort 8080 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    if ($conns5672 -and $conns5672.Length -gt 0 -and $conns8080 -and $conns8080.Length -gt 0) {
        "RabbitMQ started"
        break;
    }

    $attempts = $attempts + 1
    "RabbitMQ not fully started. Attempts: $attempts. Waiting..."
} while ($attempts -lt 10)