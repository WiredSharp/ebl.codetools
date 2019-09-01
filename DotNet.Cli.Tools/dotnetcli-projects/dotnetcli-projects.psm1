Set-StrictMode -Version Latest

$sourceFolder = "src"
$testFolder = "test"

function New-ConsoleProject
{
    Param(
        # indicate the project name
        [Parameter(Mandatory=$True, HelpMessage="project name")] [string] $name
        # indicate the root namespace for projects
        ,[Parameter(HelpMessage="root namespace")] [string] $RootNamespace
        # indicate reference(s) for project
        ,[Parameter(HelpMessage="project references")] [string[]] $Packages = @()
    )
 
    if (!$RootNamespace) {
        $RootNamespace = [System.IO.Path]::GetFileName((Get-Location).Path)
    }
    
    # create console project
    dotnet new console --name ${name} -o "${sourceFolder}\${name}\" --no-restore
    dotnet add ".\${sourceFolder}\${name}\${name}.csproj" reference ".\${sourceFolder}\${RootNamespace}.core\${RootNamespace}.core.csproj"

    foreach ($reference in $Packages) {
        Write-Debug "adding test project reference: $reference"
        dotnet add ".\${sourceFolder}\${name}\${name}.csproj" package $reference
    }

    dotnet sln add ".\${sourceFolder}\${name}\${name}.csproj"
}

function Use-Template([string] $template) {
    $targetPath = $template -replace ".tpl.ps1", ""
    if (Test-Path $targetPath) {
        Write-Information "$targetPath already exists, skipping"
        return
    }
    Write-Information "generating $targetPath"
    if (!((Split-Path $targetPath) -eq "")) {
        $folder = (Split-Path (Split-Path $targetPath) -Leaf)
        if (!(Test-Path $folder)) {
            Write-Debug "$folder does not exists, create it"
            New-Item -Path $folder -ItemType Directory > $null
        }
    }
    . "$($template.FullName)" > $targetPath
}

function Use-Templates([System.IO.DirectoryInfo] $sourceFolder) {
    Write-Information "applying templates from folder ${sourceFolder}..."
    foreach ($template in Get-ChildItem -Path $sourceFolder.FullName -Filter "*.tpl.ps1") {
        Use-Template $template.Name
    }
    foreach ($subFolder in Get-ChildItem -Path $sourceFolder.FullName -Directory) {
        Write-Information "applying templates from folder ${sourceFolder}\${subFolder}..."
        foreach ($template in Get-ChildItem -Path $subFolder.FullName -Filter "*.tpl.ps1") {
            Use-Template (Join-Path $subFolder.Name $template.Name)
        }    
    }
}

function New-Solution
{
    [CmdletBinding()]
    Param(
        # indicate the root namespace for projects
        [Parameter(HelpMessage="root namespace")] [string] $RootNamespace
        # indicate reference(s) for core project
        ,[Parameter(HelpMessage="core project references")] [string[]] $CorePackages
        # indicate reference(s) for test project
        ,[Parameter(HelpMessage="test project references")] [string[]] $TestPackages = @()
    )

    if (!$RootNamespace) {
        $RootNamespace = [System.IO.Path]::GetFileName((Get-Location).Path)
    }

    Set-StrictMode -Version Latest

    if (!$RootNamespace) {
        $RootNamespace = [System.IO.Path]::GetFileName((Get-Location).Path)
    }
    
    # # create core project
    # Write-Verbose "creating core project: ${RootNamespace}.core..."
    # dotnet new zericcolib --name "${RootNamespace}.core" -o "${sourceFolder}\${RootNamespace}.core\" --no-restore
    # foreach ($reference in $CorePackages) {
    #     Write-Debug "adding core project reference: $reference"
    #     dotnet add ".\${sourceFolder}\${RootNamespace}.core\${RootNamespace}.core.csproj" package $CorePackages
    # }

    # # create test project
    # Write-Verbose "creating test project: ${RootNamespace}.tests..."
    # dotnet new zericcout --name "${RootNamespace}.tests" -o "${testFolder}\${RootNamespace}.tests\" --no-restore
    # foreach ($reference in $TestPackages) {
    #     Write-Debug "adding test project reference: $reference"
    #     dotnet add ".\${testFolder}\${RootNamespace}.tests\${RootNamespace}.tests.csproj" package $reference
    # }

    if (!(Test-Path "ArtifactFolder")) {
        $ArtifactFolder = "artifacts"
    }
    if (!(Test-Path "IntermediateFolder")) {
        $IntermediateFolder = "artifacts\obj"
    }
    if (!(Test-Path "Author")) {
        $Author = "EBL"
    }
    if (!(Test-Path "Company")) {
        $Company = "EBL Inc."
    }

    dotnet new zericcosol --Author $Author --ArtifactFolder $ArtifactFolder --IntermediateFolder $IntermediateFolder --Company $Company

    # create solution
    dotnet new sln
    foreach ($csproj in (Get-ChildItem -Recurse -Path "." -Filter "*.csproj")) {
        dotnet sln add $csproj.FullName
    }

    git init > $null
    git add .\src > $null
    git add .\test > $null
    git add .\ > $null
}

