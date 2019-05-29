cf create-service p-rabbitmq standard queue-svc
cf create-service p.mysql db-small river-db
cf create-service app-autoscaler standard autoscale-svc
cf cups usgs-iv -p '{\"uri\":\"https://waterservices.usgs.gov/nwis/iv\"}'
cf create-service p-service-registry standard registry-svc
cf create-service p-config-server standard river-config `
    -c '{\"git\": { \"uri\": \"https://github.com/thnk2wn/river-flow-config\", \"label\": \"master\", \"searchPaths\": \"Development/*\" } }'