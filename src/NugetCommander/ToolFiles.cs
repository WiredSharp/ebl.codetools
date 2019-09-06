using NuGet.Packaging;

namespace CodeTools.NugetCommander
{
    public class ToolFiles : ManifestFile
    {
        public ToolFiles()
        {
            Target = PackagingConstants.Folders.Tools;
        }
    }
}