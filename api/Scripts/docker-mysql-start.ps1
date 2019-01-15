$name = "river-mysql"
docker ps -f name=$name -q | % { docker stop $_; docker rm $_ }
docker run --name $name -e MYSQL_ROOT_PASSWORD=pwd -d -p 3306:3306 mysql:latest