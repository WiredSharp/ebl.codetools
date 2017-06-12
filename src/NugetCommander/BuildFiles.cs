using NuGet.Packaging;

namespace CodeTools.VisualStudio.Tools
{
    public class BuildFiles : ManifestFile
    {
        public BuildFiles()
        {
            Target = PackagingConstants.Folders.Build;
        }
    }
}