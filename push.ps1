Param(
    [parameter(Mandatory=$true)]
    [ValidateSet("producer", "consumer", "all", "p", "c", "a")]
    [String]
    $target
)

Set-Variable runtime -option Constant -value "linux-x64"
Set-Variable config -option Constant -value "Release"

function PublishAndPush ($project, $app)
{
    "Publishing $project with runtime $runtime, configuration $config"
    dotnet publish $project -r $runtime -c $config

    if (!$?) { return }

    if ($target -like 'a*') 
    {
        "Pushing consumer and producer" 
        cf push 
    }
    else 
    {
        "Pushing $app"
        cf push $app 
    }
}

if ($target -like 'c*' -or $target -like 'a*')
{
    PublishAndPush "consumer/river-flow-processor.csproj" "river-flow-processor"
}
elseif ($target -like 'p*' -or $target -like 'a*')
{
    PublishAndPush "producer/river-flow-producer.csproj" "river-flow-producer"
}