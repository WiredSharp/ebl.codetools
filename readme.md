# build automation

handle build automation from projects layout to build and test. Any operation can be initiated from command line. Most operations should be 
- folder and files layout
    - project creation helpers
    - keep source normalized
- package management
    - synchronize version between dependencies
    - retrieve dependencies from various sources
- build tasks
    - handle versioning
    - create package
    - handle multiple configurations

## Project bootstrap

generate project from scratch

## [Cake](https://cakebuild.net) setup

### downloading the bootstrapper

**Windows**

`Invoke-WebRequest https://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1`

**Linux**

`curl -Lsfo build.sh https://cakebuild.net/download/bootstrapper/linux`

**OS X**

`curl -Lsfo build.sh https://cakebuild.net/download/bootstrapper/osx`

### important files

*build.ps1 and build.sh*
> These are bootstrapper scripts that ensure you have Cake and other required dependencies installed. The bootstrapper scripts are also responsible for invoking Cake. These files are optional, and not a hard requirement. If you would prefer not to use these scripts you can invoke Cake directly from the command line, once you have downloaded and extracted it.

*build.cake*
> This is the actual build script. It doesn't have to be named this but this will be found by default.

*tools/packages.config*
> This is the package configuration that tells the bootstrapper script what NuGet packages to install in the tools folder. An example of this is Cake itself or additional tools such as unit test runners, ILMerge etc.

### running the script

**Windows**

`build.ps1`

**Linux**

`build.sh`

**OS X**

`build.sh`


