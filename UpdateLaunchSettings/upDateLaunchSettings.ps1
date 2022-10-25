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

$launchSettings|foreach{
    
$launchSettingsObj =  Get-Content $_|ConvertFrom-Json -Depth 9
$launchSettingsObj.profiles[0].PSObject.Properties.Value|Add-member -NotePropertyName environmentVariables -NotePropertyValue $emptyDictionary -Force
$launchSettingsObj.profiles[0].PSObject.Properties.Value.environmentVariables|Add-member -NotePropertyMembers $environmentVars -TypeName PSObject -Force
      
 

#check if there is an existing file from a previous run, if so append 1 to the number of the old file

$filesInDir = Get-ChildItem $_.DirectoryName
$newestFile=$filesInDir|Where-Object{$_.BaseName -like '*launchSettings-*'}|Select-Object *|Sort-Object CreationTime -Bottom 1
try{
    if ($newestFile.BaseName.Split('-').Count -gt 0){
        $fileArray=$newestFile.BaseName.Split('-')
        $fileNumber=$fileArray[1].Split('.')
        $fileCount=[int]$fileNumber[0]+1
    }
    else{
        $fileCount=0
    }
}
catch{
    $fileCount=0
}
#rename current file by appending -<number>.old and save the updated file

$backupName = $_.BaseName+"-"+$fileCount+".json.old"
try{
    Rename-Item $_ -NewName $backupName -force
    $launchSettingsObj|ConvertTo-Json -depth 9|Out-File $_ -force -NoClobber
    Write-Host("Updated with new environment variables:$($_)")
}
catch{
    Write-Host("Problem updating with new environment variables:$($_)") -ForegroundColor Cyan
}
}



