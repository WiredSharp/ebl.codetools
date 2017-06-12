using NuGet.Packaging;

namespace CodeTools.VisualStudio.Tools
{
    public class ToolFiles : ManifestFile
    {
        public ToolFiles()
        {
            Target = PackagingConstants.Folders.Tools;
        }
    }
}