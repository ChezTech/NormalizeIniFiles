# Normalize all character .ini files

$AllCharNames = @(
"aaaaa",
"bbbbb",
"ccccc"
)

$BaseEQFolder = "C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest"
$NormalizeExecutablePath = ".\NormalizeIniFiles.exe"

function NormalizeFile()
{
    param ($FilePath)

    # "Normalizing file: " + $FilePath

    Start-Process `
        -FilePath $NormalizeExecutablePath `
        -ArgumentList """$FilePath""" `
        -NoNewWindow `
        -Wait
}

function NormalizeAllFilesForName()
{
    param ($CharName)
    "=============== Normalizing character: " + $CharName
    
    NormalizeFile( Join-Path -Path $BaseEQFolder -ChildPath ($CharName + "_eqclient.ini"))
    NormalizeFile( Join-Path -Path $BaseEQFolder -ChildPath ($CharName + "_test.ini"))
    NormalizeFile( Join-Path -Path $BaseEQFolder -ChildPath ("UI_" + $CharName + "_test.ini"))
}


$AllCharNames | ForEach-Object {NormalizeAllFilesForName($_)}

#NormalizeFile("C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest\aaaaa_test.ini")
