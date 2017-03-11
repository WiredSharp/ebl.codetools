#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (17:26)
// ///
// ///
#endregion

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using CodeTools.Test.Common;
using NUnit.Framework;

namespace SynchronizeVSProject.Test
{
    /// <summary>
    /// update version
    /// update copyright
    /// update description
    /// update author
    /// update project url
    /// update framework assemblies
    /// update package dependencies from {project folder}\packages.config
    /// files should be handled by standard folder structure https://docs.microsoft.com/en-us/nuget/schema/nuspec
    /// </summary>
    [TestFixture]
    public class NugetFileSynchronizerTest
    {
        [Test]
        public void i_can_update_version()
        {
            string nugetFileContent = NugetFileSynchronizer.Synchronize(@"regular\regular.nuspec".GetTestFileInfo(), @"regular\output".GetTestDirectoryInfo(), @"regular\regular.nuspec".GetTestFileInfo());
            XElement nugetFile = XElement.Parse(nugetFileContent);
            Assembly generatedAssembly = Assembly.LoadFile(@"regular\output\net40\EDO.Diagnostics.Core.dll".GetTestFileInfo().FullName);
            Assert.AreEqual(generatedAssembly.GetInformationalVersion(), nugetFile.FindNodes("version").Single().Value, "unexpected version");
        }
    }
}