param([switch]$fullQueue)

Push-Location $PSScriptRoot

.\docker-rabbitmq-start.ps1
.\docker-mysql-start.ps1 -mount
.\producer-init.ps1 -all:$fullQueue

# start in new window, lots of continual Eureka output
Invoke-Expression 'cmd /c start powershell -Command { .\start-eureka.ps1 }'
Pop-Location