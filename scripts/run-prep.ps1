param([switch]$all, [int]$top)

Push-Location $PSScriptRoot

# Eureka takes longest. start first in new window, takes a while, lots of continual Eureka output
"Launching Eureka startup in another process"
Invoke-Expression 'cmd /c start powershell -NoProfile -Command { .\eureka-start.ps1 }'

""
# Start Spring Cloud Config server for Producer and Consumer configuration.
.\docker-cloud-config-start.ps1

""
# Start RabbitMQ container for Producer and Consumer
.\docker-rabbitmq-start.ps1

""
# Start MySQL container for API. Mount to keep any existing data and not rebuild DB.
.\docker-mysql-start.ps1 -mount

""
# Setup queue and seed with message(s) for consumer processing.
.\producer-init.ps1 -all:$all -top:$top

""
# See if Eureka has fully started up yet, if not wait.
.\eureka-wait.ps1

"Run prep complete`n"

docker ps -a --format 'table {{.Names}}\t{{.Image}}\t{{.Ports}}'

Pop-Location