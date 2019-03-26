$name = "river-queue"
docker ps -a -f name=$name -q | % { docker stop $_; docker rm $_ }

# https://hub.docker.com/_/rabbitmq
docker run -d --hostname local-rabbit --name $name -p 5672:5672 -p 8080:15672 rabbitmq:3-management

# give rabbit a bit to start up. ideally would monitor in loop
# Start-Sleep -Seconds 10

# loging as guest/guest
# Start-Process 'http://localhost:8080'