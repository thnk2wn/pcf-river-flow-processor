param ([switch]$wait)

"Starting / restarting Docker. Checking status..."
$processes = Get-Process "*docker for windows*"

if ($processes.Count -gt 0)
{
	"Stopping containers"
	docker ps -q | % { docker stop $_ }
    docker ps -a -q | % { docker rm $_ }
	"Stopping docker..."
	$processes[0].Kill()
	$processes[0].WaitForExit()
}

"Starting docker..."
Start-Process "C:\Program Files\Docker\Docker\Docker for Windows.exe" -Verb RunAs

if ($wait) {
	$attempts = 0
	"Checking Docker status..."

	do {
		docker ps -a #| Out-Null

		if ($?) {
			break;
		}

		$attempts++
		"Docker not fully ready, waiting..."
		Start-Sleep 2
	} while ($attemps -le 10)

	"Pausing until initialized..."
	Start-Sleep 4
}

"Docker started"
