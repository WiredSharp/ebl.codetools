[CmdletBinding(SupportsShouldProcess)]
Param(
    # build semantic version major field
    [Parameter()][string]$Major = "1"
    # build semantic version minor field
    ,[Parameter()][string]$Minor = "0"
    # build semantic version patch field
    ,[Parameter()][string]$Patch = "0"
    # build configuration
    ,[Parameter()][string]$Configuration = "Release"
    )

$version = "${Major}.${Minor}.${Patch}"

dotnet pack -c $Configuration --version-suffix $version
dotnet new -i "bin\${Configuration}\Zericco.Templates.${version}.nupkg"
