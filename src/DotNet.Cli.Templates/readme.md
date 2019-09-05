# DotNet new templates 

These templates create a simple solution with two projects

one core library and one test project. Regular projects are stored in a *src* subfolder, test projects are stored in *test* subfolder.
The following parameters are available:

- **Author**: generated package author
- **Company**: assembly company
- **ArtifactFolder**: define root folder for output path
- **IntermediateFolder**: define base folder for intermediate folder path
- **TargetFrameworkOverride**: define Target framework, default is .Net Core 2.2

# TODO

[ ] find a way to generate a solution with template. Currently solution is generated through [DotNet.Cli.Tools](https://github.com/zericco/DotNet.Cli.Tools).

in template.json, the following does not work (2019-06-22)
``` json
  "postActions": [
    {
      "description": "create solution.",
      "args": {
        "executable": "dotnet",
        "args": "new sln"
      },
      "manualInstructions": [
        { "text": "Run 'dotnet new sln'" }
      ],
      "actionId": "3A7C4B45-1F5D-4A30-959A-51B88E82B5D2",
      "continueOnError": false
    },
    {
      "description": "add projects to solution",
      "manualInstructions": [
        { "text": "Run 'dotnet sln add **\\*.csproj'" }
      ],
      "actionId": "D396686C-DE0E-4DE6-906D-291CD29FC5DE",
      "args": {
        "files": "1"
      },
      "continueOnError": true
    }
  ]
```
