cf create-service p-rabbitmq standard queue-svc
cf create-service p.mysql db-small river-db
cf create-service app-autoscaler standard autoscale-svc
cf cups usgs-iv -p '{\"uri\":\"https://waterservices.usgs.gov/nwis/iv\"}'