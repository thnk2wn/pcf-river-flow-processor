$PSScriptRoot
.\docker-restart.ps1 -wait
.\docker-mysql-start.ps1
.\docker-rabbitmq-start.ps1
.\producer-init.ps1

Push-Location $PSScriptRoot
.\start-eureka.ps1
