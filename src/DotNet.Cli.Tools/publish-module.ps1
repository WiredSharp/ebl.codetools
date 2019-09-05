# publish PS module locally
[CmdletBinding(SupportsShouldProcess)]
Param(
   # semantic version of module
   [Parameter()][string]$Version,
   # define auto-increment mode
   [ValidateSet("Major", "Minor", "Patch")]
   [Parameter()][string]$AutoMode,
   # module name
   [Parameter()][string]$ModuleName = "dotnetcli-projects"
)

Set-StrictMode -Version Latest

##############################
# functions
##############################
$_manifestVersionPattern = "ModuleVersion\s*=\s*('|"")(?<version>(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+))('|"")"

function update-ManifestVersion {
   Param(
      # new version
      [Parameter()][string]$NewVersion,
      # manifest file content
      [Parameter()][string]$ManifestContent
   )
   #ModuleVersion = '1.2.0'
   return $manifestContent -replace $_manifestVersionPattern, "ModuleVersion = $NewVersion"
}

function get-ManifestVersion {
   Param(
      # manifest file content
      [Parameter()][string]$ManifestContent
   )
   Write-Verbose "reading version from manifest file..."
   if ($ManifestContent -match $_manifestVersionPattern) {
      return [int]$Matches.major, [int]$Matches.minor, [int]$Matches.patch
   }
}

function update-MajorField {
   Param(
      # semantic version fields
      [Parameter()][int[]]$VersionFields
   )
   Write-Debug "major"
   Write-Verbose "updating major..."
   return (1+$VersionFields[0]), 0, 0
}

function update-MinorField {
   Param(
      # semantic version fields
      [Parameter()][int[]]$VersionFields
   )
   Write-Verbose "updating minor..."
   return $VersionFields[0], (1+$VersionFields[1]), 0
}

function update-PatchField {
   Param(
      # semantic version fields
      [Parameter()][int[]]$VersionFields
   )
   Write-Verbose "updating patch..."
   return $VersionFields[0], $VersionFields[1], (1+$VersionFields[2])
}

#
#
#

# PS module lookup path
#TODO: improve target folder lookup
$moduleFolder = $env:PSModulePath.Split(";")[0]

$targetFolder = Join-Path -Path $moduleFolder -ChildPath $ModuleName

# update PS module version if applicable

$manifestPath = Join-Path $ModuleName "${ModuleName}.psd1"
[string]$manifestContent = Get-Content $manifestPath -Raw

[int[]]$previousVersionFields = get-ManifestVersion $manifestContent
$previousVersion = $previousVersionFields -join "."

Write-Debug "after retrieving manifest version"

if ($Version) {
   if ($AutoMode) {
      Write-Warning "AutoMode ignored as version is provided"
   }
}
else {
   if ($AutoMode) {
      switch ($AutoMode) {
         "Major" { $Version = (update-MajorField $previousVersionFields) -join "." }
         "Minor" { $Version = (update-MinorField $previousVersionFields) -join "." }
         "Patch" { $Version = (update-PatchField $previousVersionFields) -join "." }
         Default { Write-Error "${AutoMode}: unknown update mode" }
      }
   }
   else {
      Write-Warning "nothing to do"
      return
   }
}

Write-Verbose "updating manifest version ${previousVersion} to ${Version}..."

Set-Content $manifestPath (update-ManifestVersion $Version $manifestContent)

Write-Verbose "publishing module $ModuleName to $targetFolder..."

Copy-Item -Recurse -Path ".\${ModuleName}" -Destination $targetFolder -Force

