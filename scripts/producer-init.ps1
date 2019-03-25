pushd ..\producer
$env:QUEUE_NAME = "river-flow"

if ($env:ASPNETCORE_ENVIRONMENT -eq 'Local') {
    # put one sample guage into queue for testing
    dotnet run -- -i -g "03539600"
}
else {
    dotnet run -- -i
}
popd