Push-Location $PSScriptRoot

try {
    .\docker-rabbitmq-start.ps1
}
catch {
    Write-Warning $_.Exception.Message
    "Failed starting first container, will try restarting Docker"
    .\docker-restart.ps1 -wait
    .\docker-rabbitmq-start.ps1
}

.\docker-mysql-start.ps1
.\producer-init.ps1

.\start-eureka.ps1
Pop-Location