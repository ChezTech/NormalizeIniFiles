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
    Start-Process -FilePath $NormalizeExecutablePath -ArgumentList $FilePath -WindowStyle Hidden
    #-NoNewWindow # This doesn't work though it seems like it should be the better option instead of hiding the window
}

function NormalizeAllFilesForName()
{
    param ($CharName)


    NormalizeFile( Join-Path -Path $BaseEQFolder -ChildPath ($CharName + "_eqclient.ini"))
    NormalizeFile( Join-Path -Path $BaseEQFolder -ChildPath ($CharName + "_test.ini"))
    NormalizeFile( Join-Path -Path $BaseEQFolder -ChildPath ("UI_" + $CharName + "_test.ini"))
}


$AllCharNames | ForEach-Object {NormalizeAllFilesForName($_)}
