#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-09 (11:12)
// ///
// ///
#endregion

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeTools.MSBuild.Helpers.VisualStudio;
using CodeTools.Test.Common;
using NUnit.Framework;

namespace SynchronizeVSProject.Test
{
    [TestFixture]
    public class ProjectSynchronizerTest
    {
        [OneTimeSetUp]
        public void FixtureSetup()
        {
            Trace.Listeners.Add(new TestTraceListener());
        }

        [Test]
        public void guid_is_modified()
        {
            XElement cloned = CloneTo(@"regular.csproj.xml", NetFramework.v35);

            string projectFileContent = File.ReadAllText(@"regular.csproj.xml".GetTestFileInfo().FullName);
            XElement origin = XElement.Parse(projectFileContent);
            Assert.AreNotEqual(origin.FindNodes("ProjectGuid").Single().Value, cloned.FindNodes("ProjectGuid").Single().Value, "project GUID should have been changed");
        }

        [Test]
        public void target_framework_version_is_updated()
        {
            XElement cloned = CloneTo(@"regular.csproj.xml", NetFramework.v35);

            Assert.AreEqual(NetFramework.v35.MsbuildTag, cloned.FindNodes("TargetFrameworkVersion").Single().Value, "project GUID should have been changed");
        }

        [Test]
        public void cloned_project_file_is_created_in_expected_folder()
        {
            FileInfo targetFile = @"regular.net40\regular.csproj".GetTestFileInfo();
            if (targetFile.Directory.Exists)
            {
                targetFile.Directory.Delete(true);
            }
            CloneTo(@"regular\regular.csproj", NetFramework.v40);
            FileAssert.Exists(targetFile, "generated file has not been created in expected location");
        }

        [Test]
        public void local_compile_element_are_converted_to_link()
        {
            XElement cloned = CloneTo(@"regular\regular.csproj", NetFramework.v40);
            XElement[] compileNodes = cloned.FindNodes("Compile");
            foreach (XElement compileNode in compileNodes)
            {
                CheckIsLinkedNode(compileNode);
            }
        }

        private void CheckIsLinkedNode(XElement compileNode)
        {
            Assert.AreEqual("Compile", compileNode.Name.LocalName, "unexpected compile node tag");
            Assert.IsTrue(compileNode.FindNodes("Link").Any(), $"no 'Link' child found on node {compileNode}");
        }

        private static XElement CloneTo(string testFile, NetFramework targetFrameworkVersion)
        {
            FileInfo testFileInfo = testFile.GetTestFileInfo();
            var synchronizer = new ProjectSynchronizer(testFileInfo);
            return XElement.Parse(File.ReadAllText(synchronizer.CloneTo(targetFrameworkVersion).FullName));
        }
    }

    public class TestTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            TestContext.Write(message);
        }

        public override void WriteLine(string message)
        {
            TestContext.WriteLine(message);
        }
    }
}