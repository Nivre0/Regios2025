$ErrorActionPreference = "Stop"

function Invoke-SqlScript {
    param (
        [string]$Message,
        [string]$ScriptPath,
        [string]$DatabaseName = "DatabaseName"
    )

    Write-Host $Message
    
    $fullScriptPath = "$PSScriptRoot/$ScriptPath"
    
    & sqlcmd -S "(localdb)\MSSQLLocalDB" -d $DatabaseName -E -i $fullScriptPath
}


Invoke-SqlScript `
  -Message "Creating DB DatabaseName" `
  -ScriptPath "sql/database.sql" `
  -DatabaseName "master" 