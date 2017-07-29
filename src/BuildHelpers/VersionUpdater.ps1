[CmdletBinding()]
Param
(
   [string]$VersionInfoFilePath = '.\Shared\VersionInfo.cs',
   [string]$SrcPath = $env:BUILD_SOURCESDIRECTORY
)

function Get-VersionPrefix {
   Param
   (
      [Parameter(Mandatory = $true)][string]$VersionInfoFilePath
   )

   if (!(Test-Path $VersionInfoFilePath)) {
      throw [System.IO.FileNotFoundException] "VersionInfo file '$VersionInfoFilePath' does not exist"
   }
   $Major = $null 
   $Minor = $null
   Select-String -Path $VersionInfoFilePath -Pattern 'AssemblyInformationalVersion\("(\d+)\.(\d+)\.([^"]*)"' `
      | ForEach-Object {
      if ($_.Matches.Success) {
         Write-Debug "found informational version..."         
         $Major = $_.Matches.Groups[1].Value
         $Minor = $_.Matches.Groups[2].Value            
      }
   }
   if ($Major -eq $null) {
      Write-Debug "retrieving version from assemblyversion..."
      Select-String -Path $VersionInfoFilePath -Pattern 'AssemblyVersion\("(\d+)\.(\d+)\.([^"]*)"' `
         | ForEach-Object {
         if ($_.Matches.Success) {
            $Major = $_.Matches.Groups[1].Value
            $Minor = $_.Matches.Groups[2].Value            
         }
      }
   }
   if ($Major -eq $null)
   {
      throw "unable to retrieve version information from $VersionInfoFilePath, check file content for AssemblyInformationalVersion"
   }
   return $Major, $Minor
}

function Update-AssemblyInfoVersionFiles {
   Param
   (
      [Parameter(Mandatory = $true)][string]$Major,
      [Parameter(Mandatory = $true)][string]$Minor,   
      [string]$Patch = $env:BUILD_BUILDID,
      [string]$SrcPath = $env:BUILD_SOURCESDIRECTORY,
      [switch]$UpdateAssemblyVersion = $false,
      [switch]$UpdateAssemblyFileVersion = $false      
   )

   if (!(Test-Path $SrcPath)) {
      throw [System.IO.DirectoryNotFoundException] "source path '$SrcPath' does not exist"
   }

   if ($Patch -eq "") {
      Write-Verbose "building Patch version field from date..." -Verbose
      #calculation Julian Date 
      $year = Get-Date -format yy
      $julianYear = $year.Substring(0)
      $dayOfYear = (Get-Date).DayofYear
      $Patch = $julianYear + "{0:D3}" -f $dayOfYear
   }

   Write-Verbose "Updating versions in path $SrcPath with version $Major.$Minor.$Patch..." -Verbose
   $AllVersionFiles = @()
   $AllVersionFiles += Get-ChildItem $SrcPath "AssemblyInfo.cs" -recurse | ForEach-Object -MemberName FullName
   $AllVersionFiles += Get-ChildItem $SrcPath "VersionInfo.cs" -recurse | ForEach-Object -MemberName FullName

   Write-Debug "$AllVersionFiles"

   Write-Debug "Updating Assembly Informational Version to $Major.$Minor.$Patch"
   Write-Debug "Updating Assembly Version Major to $Major"
   Write-Debug "Synchronize File Version with Assembly Version"
 
   foreach ($file in $AllVersionFiles) {
      Write-Verbose "updating $file..."
      #version replacements
      $content = (Get-Content $file)
      $content = $content -replace 'AssemblyDescription\("([^"]*)"\)', "AssemblyDescription(""`$1 built by TFS Build $Major.$Minor.$Patch"")"
      $content = $content -replace 'AssemblyInformationalVersion\("[^"]*"\)', "AssemblyInformationalVersion(""$Major.$Minor.$Patch"")"
      if ($UpdateAssemblyVersion)
      {
         $content = $content -replace 'AssemblyVersion\("[0-9]+([^"]*)"\)', "AssemblyVersion(""$Major`$1"")"
      }
      if ($UpdateAssemblyFileVersion)
      {
         $content = $content -replace 'AssemblyFileVersion\("[0-9]+([^"]*)"\)', "AssemblyFileVersion(""$Major`$1"")"
      }
      Set-Content -Value $content -Path $file
   }
}

$Major, $Minor = Get-VersionPrefix $VersionInfoFilePath
Update-AssemblyInfoVersionFiles -Major $Major -Minor $Minor -Patch "" -SrcPath $SrcPath