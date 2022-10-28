#This script finds all launchSettings.json file in solution, and adds local environment variables to match
#terraform deployment. This keeps local dev consistent with Azure production deployment

$launchSettings = Get-ChildItem ../launchSettings.json -Recurse 
$configurationReferenceFile = "../terraform/appconfigurationsettingsv1.json"
$configurationObject=Get-Content $configurationReferenceFile|ConvertFrom-Json -depth 9
$emptyDictionary = New-Object System.Collections.Generic.Dictionary"[String,String]"
$environmentVars = New-Object System.Collections.Generic.Dictionary"[String,String]"
Foreach ($kvPair in $configurationObject.confsettings){
$environmentVars.Add($kvPair.key,$kvPair.value)
}

#need to tell rest api to run in development mode when running locally
$environmentVars.Add("ASPNETCORE_ENVIRONMENT","Development")
$launchSettings|foreach{
    
$launchSettingsObj =  Get-Content $_|ConvertFrom-Json -Depth 9
$launchSettingsObj.profiles[0].PSObject.Properties.Value|Add-member -NotePropertyName environmentVariables -NotePropertyValue $emptyDictionary -Force
$launchSettingsObj.profiles[0].PSObject.Properties.Value.environmentVariables|Add-member -NotePropertyMembers $environmentVars -TypeName PSObject -Force
      
 

#check if there is an existing file from a previous run, if so remove previous file and rename

#$filesInDir = Get-ChildItem $_.DirectoryName
$fullPath = $_.DirectoryName+"\launchSettings.json.old"
if (Test-Path -Path $fullPath -PathType Leaf){
    Remove-Item $fullPath -Force
}

#rename current file and save the updated file

$backupName = "launchSettings.json.old"
try{
    Rename-Item $_ -NewName $backupName -force
    $launchSettingsObj|ConvertTo-Json -depth 9|Out-File $_ -force -NoClobber
    Write-Host("Updated with new environment variables:$($_)")
}
catch{
    Write-Host("Problem updating with new environment variables:$($_)") -ForegroundColor Cyan
}
}



