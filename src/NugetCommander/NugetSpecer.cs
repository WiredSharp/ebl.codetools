#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-14 (12:45)
// ///
// ///
#endregion

using System;
using System.IO;
using CodeTools.Nuget.Helpers;

namespace CodeTools.VisualStudio.Tools
{
    internal class NugetSpecer
    {
        public string GenerateNuSpec()
        {
            NugetFile nuspec = NugetFile.Create();
            Update(nuspec);
            return nuspec.ToXml();
        }

        private void Update(NugetFile nuspec)
        {
            nuspec.Id = "";
            nuspec.Version = "";
            nuspec.Authors = new [] { "" };
            nuspec.Description = "";
        }

        public void GenerateNuSpec(FileInfo nuspecFile)
        {
            if (nuspecFile == null) throw new ArgumentNullException(nameof(nuspecFile));
            File.WriteAllText(nuspecFile.FullName, GenerateNuSpec());
        }
    }
}