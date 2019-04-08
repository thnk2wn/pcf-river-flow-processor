param ([switch]$mount)

$name = "river-mysql"

# https://stackoverflow.com/questions/54217076/docker-port-bind-fails-why-a-permission-denied

$attempts = 0
$startSuccess = $false

do {
    docker ps -a -f name=$name -q | % { docker stop $_; docker rm $_ }

    # https://hub.docker.com/_/mysql

    if ($mount) {
        docker run -p 3306:3306 --name $name -e MYSQL_ROOT_PASSWORD=pwd -d -v c:/temp/river-data:/var/lib/mysql mysql:latest
    }
    else {
        docker run -p 3306:3306 --name $name -e MYSQL_ROOT_PASSWORD=pwd -d mysql:latest
    }

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1
    "Waiting on docker run success. Attempts: $attempts..."
    Start-Sleep 2
} while ($attempts -lt 3)

if (!$startSuccess) {
    throw "Failed to start Rabbit MQ container."
}

$attempts = 0
"Checking MySQL status..."

do {
    Start-Sleep ($attempts + 1)
    $conns = Get-NetTCPConnection -LocalPort 3306 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    if ($conns -and $conns.Length -gt 0) {
        "MySQL started"
        break;
    }

    $attempts = $attempts + 1
    "MySQL not fully started. Attempts: $attempts. Waiting..."
} while ($attempts -lt 10)