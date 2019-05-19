param ([switch]$mount)

$name = "river-mysql"

# https://stackoverflow.com/questions/54217076/docker-port-bind-fails-why-a-permission-denied

$attempts = 0
$maxAttempts = 3
$startSuccess = $false

do {
    docker ps -a -f name=$name -q | ForEach-Object {
        "Stopping $name container"
        docker stop $_ | Out-Null
        docker rm $_ | Out-Null
    }

    # https://hub.docker.com/_/mysql

    if ($mount) {
        $volume = "c:/temp/river-data:/var/lib/mysql"
        "Starting MySQL container using volume $volume"
        docker run -p 3306:3306 --name $name -e MYSQL_ROOT_PASSWORD=pwd -d -v $volume mysql:latest
    }
    else {
        "Starting MySQL container without volume"
        docker run -p 3306:3306 --name $name -e MYSQL_ROOT_PASSWORD=pwd -d mysql:latest
    }

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1
    "Waiting on MySql docker run success. Attempts: $attempts of $maxAttempts..."
    Start-Sleep 2
} while ($attempts -lt $maxAttempts)

if (!$startSuccess) {
    throw "Failed to start MySQL container."
}

$attempts = 0
$maxAttempts = 10
"Checking MySQL status..."

do {
    Start-Sleep ($attempts + 2)
    $conns = Get-NetTCPConnection -LocalPort 3306 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    if ($conns -and $conns.Length -gt 0) {
        # port may be open but mysql may not be fully started. test a command
        "Running mysql connectivity test with execution of 'show databases'"
        docker exec river-mysql mysql --user=root --password=pwd --execute='show databases;'

        if ($?) {
            "MySQL started"
            break;
        }
    }

    $attempts = $attempts + 1
    "MySQL not fully started. Attempts: $attempts of $maxAttempts. Waiting..."
} while ($attempts -lt $maxAttempts)