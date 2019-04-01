$name = "river-mysql"

$attempts = 0
$startSuccess = $false
do {
    docker ps -a -f name=$name -q | % { docker stop $_; docker rm $_ }
    # https://hub.docker.com/_/mysql
    docker run --name $name -e MYSQL_ROOT_PASSWORD=pwd -d -p 3306:3306 mysql:latest

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1
    "Waiting on docker run success. Attempts: $attempts..."
    Start-Sleep 1
} while ($attempts -lt 3)

if (!$startSuccess) {
    throw "Failed to start Rabbit MQ container."
}

$attempts = 0
"Checking MySQL status..."

do {
    Start-Sleep 2
    $conns = Get-NetTCPConnection -LocalPort 3306 -State Listen -ErrorVariable $err -ErrorAction SilentlyContinue

    if ($conns -and $conns.Length -gt 0) {
        "MySQL started"
        break;
    }

    $attempts = $attempts + 1
    "MySQL not fully started. Attempts: $attempts. Waiting..."
} while ($attempts -le 10)