[CmdletBinding(SupportsShouldProcess)]
Param(
    )
    
New-Item "test_project" -ItemType Directory
Push-Location
try {
    Set-Location "test_project"
    dotnet new "zericcosol"        
}
finally {
    Pop-Location    
}