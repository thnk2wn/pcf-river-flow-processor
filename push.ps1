Param(
    [parameter(Mandatory=$false)]
    [ValidateSet("producer", "consumer", "api", "all")]
    [String]
    $target = "all",

    [parameter(Mandatory=$false)]
    [ValidateSet("Release", "Debug")]
    [String]
    $config = "Release",

    [parameter(Mandatory=$false)]
    [String]
    $runtime = "linux-x64",

    [parameter(Mandatory=$false)]
    [String]
    $framework = "netcoreapp2.2"
)

function PublishAndPush ($folder, $pushArgs)
{
    Push-Location $folder

    # cleanup
    $publishStageDir = "bin\publish"
    Get-ChildItem -Path $publishStageDir -Include *.* -File -Recurse | ForEach-Object { $_.Delete()}

    $projectItem = @(Get-ChildItem "*.csproj")[0]

    "Publishing $($projectItem.FullName) with runtime $runtime, configuration $config"
    dotnet publish $projectItem.FullName -r $runtime -c $config

    if (!$?)
    {
        Pop-Location
        return
    }

    # stage files in common known publish dir for manifest regardless of config, framework etc.
    $publishTempDir = "bin\$config\$framework\$runtime\publish"
    Copy-Item $publishTempDir "$publishStageDir\..\" -Recurse -Force
    Get-ChildItem $publishStageDir

    $projectName = $projectItem.Name.Replace(".csproj", "")

    "Pushing app for project $projectName w/args '$pushArgs' using $($projectItem.Directory.FullName)\manifest.yaml"
    cf push $pushArgs

    Pop-Location
}

"Running push with target $target, config $config, runtime $runtime"

if ($target -eq 'api' -or $target -eq 'all')
{
    PublishAndPush api
}

if ($target -eq 'consumer' -or $target -eq 'all')
{
    PublishAndPush consumer
}

if ($target -eq 'producer' -or $target -eq 'all')
{
    PublishAndPush producer "--no-start"
}