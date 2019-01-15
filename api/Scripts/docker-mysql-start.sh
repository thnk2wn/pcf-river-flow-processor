#!/usr/bin/env bash
name="river-mysql"
docker stop $name || true && docker rm $name || true
docker run --name $name -e MYSQL_ROOT_PASSWORD=pwd -d -p 3306:3306 mysql:latest