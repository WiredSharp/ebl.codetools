using System.IO;
using NuGet.Frameworks;
using NuGet.Packaging;

namespace CodeTools.VisualStudio.Tools
{
    public class LibraryFiles : ManifestFile
    {
        public NuGetFramework TargetFramework { get; set; }

        public LibraryFiles(NuGetFramework tarGetFramework)
        {
            TargetFramework = tarGetFramework;
            Target = Path.Combine(PackagingConstants.Folders.Lib, TargetFramework.GetShortFolderName());
        }
    }
}