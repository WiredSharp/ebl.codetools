using NuGet.Packaging;

namespace CodeTools.NugetCommander
{
    public class BuildFiles : ManifestFile
    {
        public BuildFiles()
        {
            Target = PackagingConstants.Folders.Build;
        }
    }
}