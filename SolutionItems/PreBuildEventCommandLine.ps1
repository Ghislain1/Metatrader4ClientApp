param(
    
    [Parameter(
        Position=0,
        Mandatory=$true)]
    [String]$OutPath   
)

$ErrorActionPreference = "stop"

# Fix wrong path
# Quotation marks are required if a blank is in the path... If there is no blank space in the path, a quote will be add to the end...
if(-not($OutPath.StartsWith('"')))
{
    $OutPath = $OutPath.TrimEnd('"')
}



# Test if files are already there... (for fun twice)
if((Test-Path -Path "$OutPath\MT4ServerAPI.dll") -and (Test-Path -Path "$OutPath\MT4ServerAPI.xml"))
{
    Write-Host "MT4ServerAPI.dll   exist! Continue..."  
}
else
{
  Copy-Item "..\SolutionItems\MT4ServerAPI.xml" -Destination "$OutPath"
  Copy-Item "..\SolutionItems\MT4ServerAPI.dll" -Destination "$OutPath"
  Write-Host "MT4ServerAPI.dll    has been copied!..." 
  Write-Host "To $OutPath" 
}