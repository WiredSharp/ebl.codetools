#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-07 (15:25)
// ///
// ///
#endregion

using System.IO;
using CodeTools.Core.Projects;
using NUnit.Framework;

namespace CodeTools.Core.Tests
{
    [TestFixture]
    public class ProjectTest
    {
        [Test]
        public void i_can_instantiate()
        {
            var project = new CSharpProject(GetFile("dll.csproj.xml"));
        }

        [Test]
        public void i_can_retrieve_target_framework_version()
        {
            var project = CSharpProject.Parse(GetFile("dll.csproj.xml"));
            Assert.AreEqual("v4.0.3", project.TargetFrameworkVersion);
        }

        private static FileInfo GetFile(string filename)
        {
            return new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "playground", filename));
        }
    }
}