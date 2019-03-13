$name = "river-queue"
docker ps -a -f name=$name -q | % { docker stop $_; docker rm $_ }

# https://hub.docker.com/_/rabbitmq
docker run -d --hostname local-rabbit --name $name -p 8080:15672 rabbitmq:3-management
# http://localhost:8080 as guest/guest