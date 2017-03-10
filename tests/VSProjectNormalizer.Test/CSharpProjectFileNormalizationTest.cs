using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeTools.Test.Common;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	[TestFixture]
	public class CSharpProjectFileNormalizationTest
	{
		protected Settings Settings;
        internal const string OUTPUT_PATH_TAG = "OutputPath";
        internal const string INTERMEDIATE_OUTPUT_PATH = "IntermediateOutputPath";

        private const string BUILD_PREFIX_WITHOUT_PLATFORM = @"$(BuildDir)$(SolutionName)\$(Configuration)\";
        private const string DEFAULT_BUILD_PREFIX_WITHOUT_PLATFORM = @"$(SolutionDir)Artifacts\$(Configuration)\";

        protected string BuildPrefix => BUILD_PREFIX_WITHOUT_PLATFORM;

        protected string DefaultBuildPrefix => DEFAULT_BUILD_PREFIX_WITHOUT_PLATFORM;

        [SetUp]
		public void Setup()
		{
			Settings = NewSettings;
		}

        protected Settings NewSettings => new Settings()
        {
            AcceptanceTestOutputPath = "ACC"
            ,BinOutputPath = "BIN"
            ,IntermediateOutputPath = "INT"
            ,TestOutputPath = "TST"
        };

        [Test]
        public void execution_is_idempotent()
        {
            const string projectFile = @"test.csproj.xml";
            Settings.BuildPath = "BuildPath";
            var normalizer = new VSProjectNormalizer(Settings);
            var first = normalizer.Normalize(CodeTools.Test.Common.TestHelpers.GetTestFileInfo(projectFile));
            Assert.AreEqual(first, normalizer.Normalize(first), "second execution is not idempotent");
        }

        [Test]
        public void regular_project_outputpath_node_if_builddir_set()
        {
            CheckOutputPathWhenBuildDirSet(@"regular.csproj.xml", Settings.BinOutputPath);
        }

        [Test]
        public void regular_project_outputpath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"regular.csproj.xml";
            XElement root = Normalize(projectFile);
            root.AssertEqualsTo(OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.BinOutputPath);
        }

        [Test]
        public void intermediatepath_node_if_builddir_set([Values(@"regular.csproj.xml"
                                                                    ,@"acceptance.csproj.xml"
                                                                    ,@"test.csproj.xml"
                                                                    ,@"website.csproj.xml")] string projectFile)
        {
            Settings.BuildPath = "BuildPath";
            XElement root = Normalize(projectFile);
            root.AssertExactMatch(INTERMEDIATE_OUTPUT_PATH
                , Path.Combine(Settings.BuildPath, Settings.IntermediateOutputPath)
                , Settings.WithPlatform(Path.Combine(Settings.BuildPath, Settings.IntermediateOutputPath)));
        }

        [Test]
        public void regular_project_intermediatepath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"regular.csproj.xml";
            XElement root = Normalize(projectFile);
            root.AssertEqualsTo(INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void test_project_outputpath_node_if_builddir_set()
        {
            CheckOutputPathWhenBuildDirSet(@"test.csproj.xml", Settings.TestOutputPath);
        }

	    [Test]
        public void test_project_outputpath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"test.csproj.xml";
            XElement root = Normalize(projectFile);
            root.AssertEqualsTo(OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.TestOutputPath);
        }

        [Test]
        public void test_project_intermediatepath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"test.csproj.xml";
            XElement root = Normalize(projectFile);
            root.AssertEqualsTo(INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void acceptance_project_outputpath_node_if_builddir_set()
        {
            CheckOutputPathWhenBuildDirSet(@"acceptance.csproj.xml", Settings.AcceptanceTestOutputPath);
        }
        [Test]
        public void acceptance_project_outputpath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"acceptance.csproj.xml";
            XElement root = Normalize(projectFile);
            root.AssertEqualsTo(OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.AcceptanceTestOutputPath);
        }

        [Test]
        public void acceptance_project_intermediatepath_node_is_replaced_if_builddir_not_set()
        {
            const string projectFile = @"acceptance.csproj.xml";
            XElement root = Normalize(projectFile);
            root.AssertEqualsTo(INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
        }

        [Test]
        public void build_dir_property_is_added_and_unique_if_builddir_set(
            [Values(@"regular.csproj.xml"
                , @"test.csproj.xml"
                , @"acceptance.csproj.xml")] string projectFile)
        {
            Settings.BuildPath = "BuildPath";
            XElement root = Normalize(projectFile);
            root.AssertIsUniqueAndEqualsTo("BuildDir", Settings.BuildPath);
        }

        [Test]
        public void build_dir_property_is_not_set_if_builddir_config_not_set(
            [Values(@"regular.csproj.xml"
                , @"test.csproj.xml"
                , @"acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
            Assert.IsFalse(root.FindNodes("BuildDir").Any(), "buildDir element should not have been set");
        }

        [Test]
        public void intermediate_outputpath_is_correctly_set_when_builddir_is_set(
            [Values(@"regular.csproj.xml"
                , @"test.csproj.xml"
                , @"acceptance.csproj.xml"
                , @"website.csproj.xml")] string projectFile)
        {
            XElement normalized = Normalize(projectFile);
            Settings.BuildPath = "BuildPath";
            normalized.AssertExactMatch(INTERMEDIATE_OUTPUT_PATH
                                  , Path.Combine(Settings.ExternalBuildPrefix, Settings.IntermediateOutputPath)
                                  , Settings.WithPlatform(Path.Combine(Settings.ExternalBuildPrefix, Settings.IntermediateOutputPath))
                                  , Path.Combine(Settings.DefaultBuildPrefix, Settings.IntermediateOutputPath)
                                  , Settings.WithPlatform(Path.Combine(Settings.DefaultBuildPrefix, Settings.IntermediateOutputPath))
                                  );
        }

        [Test]
		public void xml_declaration_is_returned()
		{
			const string projectFile = @"regular.csproj.xml";
            Settings.BuildPath = "BuildPath";
            var normalizer = new VSProjectNormalizer(Settings);
			string normalized = normalizer.Normalize(CodeTools.Test.Common.TestHelpers.GetTestFileInfo(projectFile));
			Assert.That(normalized, Does.StartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"), "xml declaration are omitted");
		}

		[Test]
		public void website_project_output_path_is_not_modified()
		{
			const string projectFile = @"website.csproj.xml";
            Settings.BuildPath = "BuildPath";
            XElement normalized = CodeTools.Test.Common.TestHelpers.GetTestFileInfo(projectFile).Normalize(Settings);
		    IEnumerable<XElement> outputPathNodes = normalized.FindNodes("OutputPath");
            Assert.IsTrue(outputPathNodes.All(node => node.Value == @"bin\"));
		}

        [Test]
        public void website_project_intermediate_path_is_modified_when_builddir_is_not_set()
        {
            const string projectFile = @"website.csproj.xml";
            XElement normalized = CodeTools.Test.Common.TestHelpers.GetTestFileInfo(projectFile).Normalize(Settings);
            normalized.AssertExactMatch(INTERMEDIATE_OUTPUT_PATH
                                  , Path.Combine(Settings.ExternalBuildPrefix, Settings.IntermediateOutputPath)
                                  , Settings.WithPlatform(Path.Combine(Settings.ExternalBuildPrefix, Settings.IntermediateOutputPath))
                                  , Path.Combine(Settings.DefaultBuildPrefix, Settings.IntermediateOutputPath)
                                  , Settings.WithPlatform(Path.Combine(Settings.DefaultBuildPrefix, Settings.IntermediateOutputPath))
                                  );
        }

	    [Test]
	    public void solutionDir_is_defined()
	    {
            const string projectFile = @"regular.csproj.xml";
            XElement normalized = CodeTools.Test.Common.TestHelpers.GetTestFileInfo(projectFile).Normalize(Settings);
            IEnumerable<XElement> solutionDirNodes = normalized.FindNodes("SolutionDir");
            Assert.IsTrue(solutionDirNodes.Any(), "solution dir has not been defined");
            Assert.IsTrue(solutionDirNodes.Any(node => node.Attribute("Condition") != null), "conditional attribute is not set on solution dir node");
        }

        [Test]
        public void solutionName_is_defined()
        {
            const string projectFile = @"regular.csproj.xml";
            XElement normalized = CodeTools.Test.Common.TestHelpers.GetTestFileInfo(projectFile).Normalize(Settings);
            IEnumerable<XElement> solutionNameNodes = normalized.FindNodes("SolutionName");
            Assert.IsTrue(solutionNameNodes.Any(), "solution name has not been defined");
            Assert.IsTrue(solutionNameNodes.Any(node => node.Attribute("Condition") != null), "conditional attribute is not set on solution name node");
        }

        private void CheckOutputPathWhenBuildDirSet(string filepath, string settingsOutputPath)
        {
            Settings.BuildPath = "BuildPath";
            XElement root = Normalize(filepath);
            root.AssertExactMatch(OUTPUT_PATH_TAG
                                  , Path.Combine(Settings.BuildPath, settingsOutputPath)
                                  , Settings.WithPlatform(Path.Combine(Settings.BuildPath, settingsOutputPath)));
        }

        protected XElement Normalize(string projectFile)
        {
            return CodeTools.Test.Common.TestHelpers.GetTestFileInfo(projectFile).Normalize(Settings);
        }
	}

    internal static class TestHelpers
    {
        public static XElement Normalize(this FileInfo projectFile, Settings settings)
        {
            var normalizer = new VSProjectNormalizer(settings);
            string normalized = normalizer.Normalize(projectFile);
            File.WriteAllText(
                              Path.Combine(Path.GetDirectoryName(projectFile.FullName), Path.GetFileNameWithoutExtension(projectFile.Name)) +
                              ".normalized.xml",
                              normalized);
            XElement root = XElement.Parse(normalized);
            return root;
        }
    }
}