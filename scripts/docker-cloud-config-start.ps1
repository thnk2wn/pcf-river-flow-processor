$image = "hyness/spring-cloud-config-server"
$name = "river-cloud-config"

$attempts = 0
$startSuccess = $false
$maxAttempts = 3

$configPath = (Get-Item (Join-Path $PSScriptRoot "../../river-flow-config/local")).FullName.Replace("\", "/")

do {
    docker ps -a -f name=$name -q | ForEach-Object {
        "Stopping $name container"
        docker stop $_ | Out-Null
        docker rm $_ | Out-Null
    }

    # https://hub.docker.com/r/hyness/spring-cloud-config-server/

    $v = "$($configPath):/config"
    "Starting $image container using config path $configPath"

    docker run -d -p 8888:8888 --rm --name $name `
        -v $v `
        -e SPRING_PROFILES_ACTIVE=native `
        -e SPRING_CLOUD_CONFIG_SERVER_ACCEPT-EMPTY=false `
        $image

    if ($?) {
        $startSuccess = $true
        break;
    }

    $attempts = $attempts + 1

    "Waiting on $image docker run success, attempts $attempts of $maxAttempts"
    Start-Sleep 1
} while ($attempts -lt $maxAttempts)

if (!$startSuccess) {
    throw "Failed to start $image container."
}

$configServerTestUrl = "http://localhost:8888/river-flow-processor/local"

"Checking Cloud Config status. Test url: $configServerTestUrl"
$attempts = 0
$maxAttempts = 10

do {
    Start-Sleep ($attempts + 3)

    $status = -1

    try {
        $status = Invoke-WebRequest $configServerTestUrl | ForEach-Object {$_.StatusCode}
    }
    catch {
        Write-Warning "$($_.Exception.Message)"
    }

    if ($status -eq 200) {
        "$image started. Launching $configServerTestUrl"
        Start-Process $configServerTestUrl -WindowStyle Minimized
        break;
    }

    $attempts = $attempts + 1
    "$image not fully started. Attempts $attempts of $maxAttempts. Waiting..."
} while ($attempts -lt $maxAttempts)

# if you get to this point and it can't seem to find the app, might need to reset docker drive creds if pwd changed
if ($attempts -eq $maxAttempts) {
    Write-Warning "Starting $image appeared to fail"
}