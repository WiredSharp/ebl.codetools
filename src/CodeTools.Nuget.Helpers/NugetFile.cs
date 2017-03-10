#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (17:48)
// ///
// ///
#endregion

using System.IO;
using CodeTools.Helpers.Core;

namespace CodeTools.Nuget.Helpers
{
    public class NugetFile: XmlFileWrapper
    {
        public static NugetFile Load(FileInfo nugetFile)
        {
            return new NugetFile(nugetFile);
        }

        public NugetFile(FileInfo file)
            :base(file)
        {
            
        }

        public NugetFile(string fileContent) : base(fileContent)
        {
        }
    }
}