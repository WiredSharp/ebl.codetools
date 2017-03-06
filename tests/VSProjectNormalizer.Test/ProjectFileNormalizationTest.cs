using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NetBike.XmlUnit.NUnitAdapter;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	[TestFixture]
	public class ProjectFileNormalizationTest
	{
		protected Settings Settings;

		[SetUp]
		public void Setup()
		{
			Settings = NewSettings;
		}

		public Settings NewSettings => new Settings()
		                               {
		                                   AcceptanceTestOutputPath = TestContext.CurrentContext.Test.Name + ".AcceptanceTestOutputPath"
		                                   ,BinOutputPath = TestContext.CurrentContext.Test.Name + ".BinOutputPath"
		                                   ,BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath"
		                                   ,IntermediateOutputPath = TestContext.CurrentContext.Test.Name + ".IntermediateOutputPath"
		                                   ,TestOutputPath = TestContext.CurrentContext.Test.Name + ".TestOutputPath"
		                               };

	    [Test]
		public void xml_declaration_is_returned()
		{
			const string projectFile = @"playground\regular.csproj.xml";
			var normalizer = new VSProjectNormalizer(Settings);
			string normalized = normalizer.Normalize(new FileInfo(projectFile));
			Assert.That(normalized, Is.StringStarting("<?xml version=\"1.0\" encoding=\"utf-8\"?>"), "xml declaration are omitted");
		}

		[Test]
		public void website_project_output_path_is_not_modified()
		{
			const string projectFile = @"playground\website.csproj.xml";
		    XElement normalized = TestHelpers.Normalize(new FileInfo(projectFile), Settings);
		    IEnumerable<XElement> outputPathNodes = TestHelpers.FindNodes(normalized, "OutputPath");
            Assert.IsTrue(outputPathNodes.All(node => node.Value == @"bin\"));
		}

        [Test]
        public void website_project_intermediate_path_is_modified_when_builddir_is_set()
        {
            const string projectFile = @"playground\website.csproj.xml";
            XElement normalized = TestHelpers.Normalize(new FileInfo(projectFile), Settings);
            IEnumerable<XElement> outputPathNodes = TestHelpers.FindNodes(normalized, "IntermediateOutputPath");
            foreach (XElement node in outputPathNodes)
            {
                Assert.AreEqual(@"$(BuildDir)$(SolutionName)\$(Configuration)\" + Settings.IntermediateOutputPath, node.Value, "unexpected intermediate output path value");
            }
        }

        [Test]
        public void website_project_intermediate_path_is_modified_when_builddir_is_not_set()
        {
            const string projectFile = @"playground\website.csproj.xml";
            Settings.BuildPath = null;
            XElement normalized = TestHelpers.Normalize(new FileInfo(projectFile), Settings);
            IEnumerable<XElement> outputPathNodes = TestHelpers.FindNodes(normalized, "IntermediateOutputPath");
            Assert.IsTrue(outputPathNodes.Any(node => @"$(SolutionDir)$(Configuration)\" + Settings.IntermediateOutputPath == node.Value), @"'$(SolutionDir)$(Configuration)\" + Settings.IntermediateOutputPath + "' intermediate output path value not found");
            Assert.IsTrue(outputPathNodes.Any(node => @"$(BuildDir)$(SolutionName)\$(Configuration)\" + Settings.IntermediateOutputPath == node.Value), @"$(BuildDir)$(SolutionName)\$(Configuration)\" + Settings.IntermediateOutputPath + "' intermediate output path value not found");
        }
    }
}