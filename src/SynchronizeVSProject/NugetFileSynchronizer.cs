#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (17:06)
// ///
// ///
#endregion

using System.IO;
using CodeTools.Nuget.Helpers;

namespace SynchronizeVSProject
{
    /// <summary>
    /// 
    /// </summary>
    public class NugetFileSynchronizer
    {
        /// <summary>
        /// Update the specified nuget file <param name="nugetFile"></param>.
        /// if multi-framework targeted, <param name="outputPathRoot"></param> represents the root of all output paths.
        /// <example>
        /// <param name="outputPathRoot"/>\net35\...
        /// <param name="outputPathRoot"/>\net40\...
        /// <param name="outputPathRoot"/>\net45\...
        /// </example>
        /// if only one framework is targeted, <param name="outputPathRoot"></param> represents the regular output path.
        /// <param name="outputPathRoot"></param> represents the output path
        /// if <param name="packageFile"></param> is provided, packages will be updated as well
        /// update version
        /// update copyright
        /// update description
        /// update author
        /// update project url
        /// update framework assemblies
        /// update package dependencies from {project folder}\packages.config
        /// files should be handled by standard folder structure https://docs.microsoft.com/en-us/nuget/schema/nuspec
        /// </summary>
        /// <param name="nugetFile">The nuget file.</param>
        /// <param name="outputPathRoot">The output path root.</param>
        /// <param name="packageFile">The package file.</param>
        /// <returns></returns>
        public static string Synchronize(FileInfo nugetFile, DirectoryInfo outputPathRoot, FileInfo packageFile = null)
        {
            NugetFile nuget = NugetFile.Load(nugetFile);
            return nuget.ToString();
        }
    }
}