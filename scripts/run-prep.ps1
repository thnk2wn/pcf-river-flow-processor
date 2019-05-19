param([switch]$all, [int]$top)

Push-Location $PSScriptRoot

.\docker-rabbitmq-start.ps1
.\docker-mysql-start.ps1 -mount
.\docker-cloud-config-start.ps1
.\producer-init.ps1 -all:$all -top:$top
.\eureka-start-wait.ps1

Pop-Location