param([switch]$all)

pushd ..\producer
$env:QUEUE_NAME = "river-flow"

if ($env:ASPNETCORE_ENVIRONMENT -eq 'Local') {
	if (!$all) {
		# put one sample guage into queue for testing
		dotnet run -- -i -g "03539600"
		return
	}
}

# init and full queue populate
dotnet run -- -i

popd