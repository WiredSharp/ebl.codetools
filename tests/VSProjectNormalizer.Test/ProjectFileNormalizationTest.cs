using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	[TestFixture]
	public class ProjectFileNormalizationTest
	{
		protected Settings Settings;
        internal const string OUTPUT_PATH_TAG = "OutputPath";
        internal const string INTERMEDIATE_OUTPUT_PATH = "IntermediateOutputPath";

        private const string BUILD_PREFIX_WITHOUT_PLATFORM = @"$(BuildDir)$(SolutionName)\$(Configuration)\";
        private const string DEFAULT_BUILD_PREFIX_WITHOUT_PLATFORM = @"$(SolutionDir)$(Configuration)\";

        protected string BuildPrefix => BUILD_PREFIX_WITHOUT_PLATFORM;

        protected string DefaultBuildPrefix => DEFAULT_BUILD_PREFIX_WITHOUT_PLATFORM;

        [SetUp]
		public void Setup()
		{
			Settings = NewSettings;
		}

        protected Settings NewSettings => new Settings()
        {
            AcceptanceTestOutputPath = TestContext.CurrentContext.Test.Name + ".AcceptanceTestOutputPath"
            ,BinOutputPath = TestContext.CurrentContext.Test.Name + ".BinOutputPath"
            ,IntermediateOutputPath = TestContext.CurrentContext.Test.Name + ".IntermediateOutputPath"
            ,TestOutputPath = TestContext.CurrentContext.Test.Name + ".TestOutputPath"
        };

        [Test]
        public void execution_is_idempotent()
        {
            const string projectFile = @"playground\test.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            var normalizer = new VSProjectNormalizer(Settings);
            var first = normalizer.Normalize(new FileInfo(projectFile));
            Assert.AreEqual(first, normalizer.Normalize(first), "second execution is not idempotent");
        }

        [Test]
        public void regular_project_outputpath_node_is_replaced_and_unique_if_builddir_set()
        {
            const string projectFile = @"playground\regular.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertIsUniqueAndEqualsTo(root, OUTPUT_PATH_TAG, BuildPrefix + Settings.BinOutputPath);
        }

        [Test]
        public void regular_project_outputpath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"playground\regular.csproj.xml";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertEqualsTo(root, OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.BinOutputPath);
        }

        [Test]
        public void regular_project_intermediatepath_node_is_replaced_and_unique_if_builddir_set()
        {
            const string projectFile = @"playground\regular.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertIsUniqueAndEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, BuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void regular_project_intermediatepath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"playground\regular.csproj.xml";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void test_project_outputpath_node_is_replaced_and_unique_if_builddir_set()
        {
            const string projectFile = @"playground\test.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertIsUniqueAndEqualsTo(root, OUTPUT_PATH_TAG, BuildPrefix + Settings.TestOutputPath);
        }

        [Test]
        public void test_project_outputpath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"playground\test.csproj.xml";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertEqualsTo(root, OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.TestOutputPath);
        }

        [Test]
        public void test_project_intermediatepath_node_is_replaced_and_unique_if_builddir_set()
        {
            const string projectFile = @"playground\test.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertIsUniqueAndEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, BuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void test_project_intermediatepath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"playground\test.csproj.xml";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void acceptance_project_outputpath_node_is_replaced_and_unique_if_builddir_set()
        {
            const string projectFile = @"playground\acceptance.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertIsUniqueAndEqualsTo(root, OUTPUT_PATH_TAG, BuildPrefix + Settings.AcceptanceTestOutputPath);
        }

        [Test]
        public void acceptance_project_outputpath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"playground\acceptance.csproj.xml";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertEqualsTo(root, OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.AcceptanceTestOutputPath);
        }

        [Test]
        public void acceptance_project_intermediatepath_node_is_replaced_and_unique_if_builddir_set()
        {
            const string projectFile = @"playground\acceptance.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertIsUniqueAndEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, BuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void acceptance_project_intermediatepath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"playground\acceptance.csproj.xml";
            XElement root = Normalize(projectFile);
            TestHelpers.AssertEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void build_dir_property_is_added_and_unique_if_builddir_set(
            [Values(@"playground\regular.csproj.xml"
                , @"playground\test.csproj.xml"
                , @"playground\acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            TestHelpers.AssertIsUniqueAndEqualsTo(root, "BuildDir", Settings.BuildPath);
        }

        [Test]
        public void build_dir_property_is_not_set_if_builddir_config_not_set(
            [Values(@"playground\regular.csproj.xml"
                , @"playground\test.csproj.xml"
                , @"playground\acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
            Assert.IsFalse(TestHelpers.FindNodes(root, "BuildDir").Any(), "buildDir element should not have been set");
        }

        [Test]
        public void outputpath_does_not_end_with_platform_tag(
            [Values(@"playground\regular.csproj.xml"
                , @"playground\test.csproj.xml"
                , @"playground\acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            IEnumerable<XElement> matchingNodes = TestHelpers.FindNodes(root, "OutputPath");
            Assert.IsFalse(matchingNodes.Any(n => n.Value.EndsWith("$Platform")), "no output path should end with $Platform tag");
        }

        [Test]
        public void intermediate_outputpath_ends_with_platform_tag(
            [Values(@"playground\regular.csproj.xml"
                , @"playground\test.csproj.xml"
                , @"playground\acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            IEnumerable<XElement> matchingNodes = TestHelpers.FindNodes(root, "IntermediateOutputPath");
            Assert.IsFalse(matchingNodes.Any(n => n.Value.EndsWith("$Platform")), "no intermediate output path should end with $Platform tag");
        }

        [Test]
		public void xml_declaration_is_returned()
		{
			const string projectFile = @"playground\regular.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            var normalizer = new VSProjectNormalizer(Settings);
			string normalized = normalizer.Normalize(new FileInfo(projectFile));
			Assert.That(normalized, Is.StringStarting("<?xml version=\"1.0\" encoding=\"utf-8\"?>"), "xml declaration are omitted");
		}

		[Test]
		public void website_project_output_path_is_not_modified()
		{
			const string projectFile = @"playground\website.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
            XElement normalized = TestHelpers.Normalize(new FileInfo(projectFile), Settings);
		    IEnumerable<XElement> outputPathNodes = TestHelpers.FindNodes(normalized, "OutputPath");
            Assert.IsTrue(outputPathNodes.All(node => node.Value == @"bin\"));
		}

        [Test]
        public void website_project_intermediate_path_is_modified_when_builddir_is_set()
        {
            const string projectFile = @"playground\website.csproj.xml";
            Settings.BuildPath = TestContext.CurrentContext.Test.Name + ".BuildPath";
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
            XElement normalized = TestHelpers.Normalize(new FileInfo(projectFile), Settings);
            IEnumerable<XElement> outputPathNodes = TestHelpers.FindNodes(normalized, "IntermediateOutputPath");
            Assert.IsTrue(outputPathNodes.Any(node => @"$(SolutionDir)$(Configuration)\" + Settings.IntermediateOutputPath == node.Value), @"'$(SolutionDir)$(Configuration)\" + Settings.IntermediateOutputPath + "' intermediate output path value not found");
            Assert.IsTrue(outputPathNodes.Any(node => @"$(BuildDir)$(SolutionName)\$(Configuration)\" + Settings.IntermediateOutputPath == node.Value), @"$(BuildDir)$(SolutionName)\$(Configuration)\" + Settings.IntermediateOutputPath + "' intermediate output path value not found");
        }

	    [Test]
	    public void solutionDir_is_defined()
	    {
            const string projectFile = @"playground\regular.csproj.xml";
            XElement normalized = TestHelpers.Normalize(new FileInfo(projectFile), Settings);
            IEnumerable<XElement> solutionDirNodes = TestHelpers.FindNodes(normalized, "SolutionDir");
            Assert.IsTrue(solutionDirNodes.Any(), "solution dir has not been defined");
            Assert.IsTrue(solutionDirNodes.Any(node => node.Attribute("Condition") != null), "conditional attribute is not set on solution dir node");
        }

        [Test]
        public void solutionName_is_defined()
        {
            const string projectFile = @"playground\regular.csproj.xml";
            XElement normalized = TestHelpers.Normalize(new FileInfo(projectFile), Settings);
            IEnumerable<XElement> solutionNameNodes = TestHelpers.FindNodes(normalized, "SolutionName");
            Assert.IsTrue(solutionNameNodes.Any(), "solution name has not been defined");
            Assert.IsTrue(solutionNameNodes.Any(node => node.Attribute("Condition") != null), "conditional attribute is not set on solution name node");
        }

        protected XElement Normalize(string projectFile)
        {
            return TestHelpers.Normalize(new FileInfo(projectFile), Settings);
        }
    }
}