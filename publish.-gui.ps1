Write-Host "Generating Build Number for project GUI"

#Get the version from the csproj file
$project =  "TsMap.Canvas"
$finalExeName = "TsMap"
$workingDirectory = Get-Location
$xml = [Xml] (Get-Content $workingDirectory\$project\$project.csproj)

$assemblyVersion = [System.Version]::Parse($xml.Project.PropertyGroup.Version)
$initialVersion = [Version] $assemblyVersion

#Get the build number (number of days since January 1, 2000)
$baseDate = [datetime]"01/01/2000"
$currentDate = $(Get-Date)
$interval = (NEW-TIMESPAN -Start $baseDate -End $currentDate)
$buildNumber = $interval.Days

#Get the revision number (number seconds (divided by two) into the day on which the compilation was performed)
$StartDate=[datetime]::Today
$EndDate=(GET-DATE)
$revisionNumber = [math]::Round((New-TimeSpan -Start $StartDate -End $EndDate).TotalSeconds / 2,0)

#Final version number
$finalBuildVersion = "$($initialVersion.Major).$($initialVersion.Minor).$($buildNumber).$($revisionNumber)"
Write-Host "Final build number: " $finalBuildVersion

#Publish
$publishDirectory = "$workingDirectory\$project\bin\Publish\"
Remove-Item "$publishDirectory*"

dotnet build TsMap.Canvas -f net47 -r win-x64 -c Publish
Start-Sleep -s 1

Rename-Item "$publishDirectory$project.exe" "$publishDirectory$finalExeName-$finalBuildVersion.exe"

Get-ChildItem -Path "$publishDirectory*.dll" | Compress-Archive -DestinationPath "$publishDirectory$finalExeName-$finalBuildVersion.zip" -CompressionLevel "Fastest"
Get-ChildItem -Path "$publishDirectory*.exe" | Compress-Archive -DestinationPath "$publishDirectory$finalExeName-$finalBuildVersion.zip" -CompressionLevel "Fastest" -Update
