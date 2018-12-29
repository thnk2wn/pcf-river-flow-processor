Param(
    [parameter(Mandatory=$false)]
    [ValidateSet("producer", "consumer", "all", "p", "c", "a")]
    [String]
    $target = "all",

    [parameter(Mandatory=$false)]
    [ValidateSet("Release", "Debug")]
    [String]
    $config = "Release",

    [parameter(Mandatory=$false)]
    [String]
    $runtime = "linux-x64"
)

function PublishAndPush ($folder, $pushArgs)
{
    Push-Location $folder
    $projectItem = @(Get-ChildItem "*.csproj")[0]

    "Publishing $($projectItem.FullName) with runtime $runtime, configuration $config"
    dotnet publish $projectItem.FullName -r $runtime -c $config

    if (!$?) 
    {
        Pop-Location
        return
    }

    $projectName = $projectItem.Name.Replace(".csproj", "")

    "Pushing app for project $projectName w/args '$pushArgs' using $($projectItem.Directory.FullName)\manifest.yaml"
    cf push $pushArgs

    Pop-Location
}

"Running push with target $target, config $config, runtime $runtime"

if ($target -like 'c*' -or $target -like 'a*')
{
    PublishAndPush consumer
}

if ($target -like 'p*' -or $target -like 'a*')
{
    PublishAndPush producer "--no-start"
}