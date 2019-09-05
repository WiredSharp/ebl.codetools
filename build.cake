const string Solution = "CodeTools.sln";
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var outputDirectory = Argument("outputDir", $"./artifacts/{configuration}");
var binDirectory = Argument("binDir", "C:/bin");

Task("Default")
  .Does(() =>
{
  Information("Hello World!");
});

Task("Clean")
  .Does(() => {
    var folders = new[]{"obj",outputDirectory };
    DeleteDirectories(folders.Where(folder => DirectoryExists(folder)), new DeleteDirectorySettings { Recursive=true });
  });

Task("Restore")
  .Does(() => {
    DotNetCoreRestore(Solution);
  });

Task("Build")
  .IsDependentOn("Restore")
  .Does(() => {
    DotNetCoreBuild(Solution, new DotNetCoreBuildSettings {
         Configuration = configuration,
         OutputDirectory = outputDirectory      
      });
  });

Task("Local")
  .IsDependentOn("Build")
  .Does(() => {
	Information("publishing to ")
  });

Task("Run-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit(outputDirectory + "test/**/*.Tests.dll");
});

Task("Rebuild")
  .IsDependentOn("Clean")
  .IsDependentOn("Build").Does(() => {});

RunTarget(target);