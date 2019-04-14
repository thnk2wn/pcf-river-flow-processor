param([switch]$all, [int]$top)

Push-Location $PSScriptRoot

.\docker-rabbitmq-start.ps1
.\docker-mysql-start.ps1 -mount
.\producer-init.ps1 -all:$all -top:$top

# start in new window, lots of continual Eureka output
Invoke-Expression 'cmd /c start powershell -Command { .\start-eureka.ps1 }'
Pop-Location