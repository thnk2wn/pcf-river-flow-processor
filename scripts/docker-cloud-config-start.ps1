$name = "river-cloud-config"

$attempts = 0
$startSuccess = $false

$configPath = (Get-Item (Join-Path $PSScriptRoot "../../river-flow-config/local")).FullName.Replace("\", "/")
"Using config path $configPath"

do {
    docker ps -a -f name=$name -q | % { docker stop $_; docker rm $_ }
    # https://hub.docker.com/r/hyness/spring-cloud-config-server/

    $v = "$($configPath):/config"

    docker run -d -p 8888:8888 --rm --name cloud-config `
        -v $v `
        -e SPRING_PROFILES_ACTIVE=native `
        -e SPRING_CLOUD_CONFIG_SERVER_ACCEPT-EMPTY=false `
        hyness/spring-cloud-config-server

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1

    "Waiting on docker run success, attempts: $attempts"
    Start-Sleep 1
} while ($attempts -lt 3)

if (!$startSuccess) {
    throw "Failed to start Spring Cloud Config container."
}

$configServerTestUrl = "http://localhost:8888/river-flow-processor/local"

"Checking Cloud Config status..."
$attempts = 0
do {
    Start-Sleep ($attempts + 1)

    $status = -1

    try {
        $status = Invoke-WebRequest $configServerTestUrl | ForEach-Object {$_.StatusCode}
    }
    catch {
        Write-Warning "$($_.Exception.Message). Testing $configServerTestUrl"
    }

    if ($status -eq 200) {
        "Spring cloud config server started at $configServerTestUrl"
        break;
    }

    $attempts = $attempts + 1
    "Cloud config not fully started. Attempts: $attempts. Waiting..."
} while ($attempts -lt 10)