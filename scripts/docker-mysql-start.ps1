param ([switch]$mount)

$image = "mysql"
$containerName = "river-mysql"
$hostPort = 3306
$containerPort = 3306

$attempts = 0
$maxAttempts = 3
$startSuccess = $false

do {
    docker ps -a -f name=$containerName -q | ForEach-Object {
        "Stopping $containerName container"
        docker stop $_ | Out-Null
        docker rm $_ | Out-Null
    }

    # https://hub.docker.com/_/mysql
    $ports = "$($hostPort):$($containerPort)"

    if ($mount) {
        $volume = "c:/temp/river-data:/var/lib/mysql"
        "Starting $image container using volume $volume"
        docker run -p $ports --name $containerName -e MYSQL_ROOT_PASSWORD=pwd -d -v $volume $image
    }
    else {
        "Starting $image container without volume"
        docker run -p $ports --name $containerName -e MYSQL_ROOT_PASSWORD=pwd -d $image
    }

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
$maxAttempts = 10
"Checking $image status..."

do {
    Start-Sleep ($attempts + 2)
    $conns = Get-NetTCPConnection -LocalPort $hostPort -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    if ($conns -and $conns.Length -gt 0) {
        # port may be open but mysql may not be fully started. test a command
        "Running $image connectivity test with execution of 'show databases'"
        docker exec $containerName mysql --user=root --password=pwd --execute='show databases;'

        if ($?) {
            "$image started"
            break;
        }
    }

    $attempts = $attempts + 1
    "$image not fully started. Attempts: $attempts of $maxAttempts. Waiting..."
} while ($attempts -lt $maxAttempts)