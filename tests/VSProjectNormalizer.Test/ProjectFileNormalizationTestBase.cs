using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	public abstract class ProjectFileNormalizationTestBase
	{
		protected Settings Settings;
		internal const string OUTPUT_PATH_TAG = "OutputPath";
		internal const string INTERMEDIATE_OUTPUT_PATH = "IntermediateOutputPath";
		protected abstract string BuildPrefix { get; }
		protected abstract string DefaultBuildPrefix { get; }
		protected abstract Settings NewSettings { get; }

		[SetUp]
		public void Setup()
		{
			Settings = NewSettings;
		}

		[Test]
		public void execution_is_idempotent()
		{
			const string projectFile = @"playground\test.csproj.xml";
			var normalizer = new VSProjectNormalizer(Settings);
			var first = normalizer.Normalize(new FileInfo(projectFile));
			Assert.AreEqual(first, normalizer.Normalize(first), "second execution is not idempotent");
		}

		[Test]
		public void regular_project_outputpath_node_is_replaced_and_unique_if_builddir_set()
		{
			const string projectFile = @"playground\regular.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, OUTPUT_PATH_TAG, BuildPrefix  + Settings.BinOutputPath);
		}

		[Test]
		public void regular_project_outputpath_node_is_replaced_if_builddir_not_set()
		{
			const string projectFile = @"playground\regular.csproj.xml";
			Settings.BuildPath = null;
			XElement root = Normalize(projectFile);
			AssertEqualsTo(root, OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.BinOutputPath);
		}

		[Test]
		public void regular_project_intermediatepath_node_is_replaced_and_unique_if_builddir_set()
		{
			const string projectFile = @"playground\regular.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, BuildPrefix + Settings.IntermediateOutputPath);
		}

		[Test]
		public void regular_project_intermediatepath_node_is_replaced_if_builddir_not_set()
		{
			const string projectFile = @"playground\regular.csproj.xml";
			Settings.BuildPath = null;
			XElement root = Normalize(projectFile);
			AssertEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
		}

		[Test]
		public void test_project_outputpath_node_is_replaced_and_unique_if_builddir_set()
		{
			const string projectFile = @"playground\test.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, OUTPUT_PATH_TAG, BuildPrefix + Settings.TestOutputPath);
		}

		[Test]
		public void test_project_outputpath_node_is_replaced_if_builddir_not_set()
		{
			const string projectFile = @"playground\test.csproj.xml";
			Settings.BuildPath = null;
			XElement root = Normalize(projectFile);
			AssertEqualsTo(root, OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.TestOutputPath);
		}

		[Test]
		public void test_project_intermediatepath_node_is_replaced_and_unique_if_builddir_set()
		{
			const string projectFile = @"playground\test.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, BuildPrefix + Settings.IntermediateOutputPath);
		}

		[Test]
		public void test_project_intermediatepath_node_is_replaced_if_builddir_not_set()
		{
			const string projectFile = @"playground\test.csproj.xml";
			Settings.BuildPath = null;
			XElement root = Normalize(projectFile);
			AssertEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
		}

		[Test]
		public void acceptance_project_outputpath_node_is_replaced_and_unique_if_builddir_set()
		{
			const string projectFile = @"playground\acceptance.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, OUTPUT_PATH_TAG, BuildPrefix + Settings.AcceptanceTestOutputPath);
		}

		[Test]
		public void acceptance_project_outputpath_node_is_replaced_if_builddir_not_set()
		{
			const string projectFile = @"playground\acceptance.csproj.xml";
			Settings.BuildPath = null;
			XElement root = Normalize(projectFile);
			AssertEqualsTo(root, OUTPUT_PATH_TAG, DefaultBuildPrefix + Settings.AcceptanceTestOutputPath);
		}

		[Test]
		public void acceptance_project_intermediatepath_node_is_replaced_and_unique_if_builddir_set()
		{
			const string projectFile = @"playground\acceptance.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, BuildPrefix + Settings.IntermediateOutputPath);
		}

		[Test]
		public void acceptance_project_intermediatepath_node_is_replaced_if_builddir_not_set()
		{
			const string projectFile = @"playground\acceptance.csproj.xml";
			Settings.BuildPath = null;
			XElement root = Normalize(projectFile);
			AssertEqualsTo(root, INTERMEDIATE_OUTPUT_PATH, DefaultBuildPrefix + Settings.IntermediateOutputPath);
		}

		[Test]
		public void build_dir_property_is_added_and_unique_if_builddir_set(
			[Values(@"playground\regular.csproj.xml"
				, @"playground\test.csproj.xml"
				, @"playground\acceptance.csproj.xml")] string projectFile)
		{
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, "BuildDir", Settings.BuildPath);
		}

		[Test]
		public void build_dir_property_is_not_set_if_builddir_config_not_set(
			[Values(@"playground\regular.csproj.xml"
				, @"playground\test.csproj.xml"
				, @"playground\acceptance.csproj.xml")] string projectFile)
		{
			Settings.BuildPath = null;
			XElement root = Normalize(projectFile);
			Assert.IsFalse(Enumerable.Any<XElement>(FindNodes(root, "BuildDir")), "buildDir element should not have been set");
		}

		protected void AssertIsUniqueAndEqualsTo(XElement root, string localName, string expectedValue)
		{
			IEnumerable<XElement> matchingNodes = FindNodes(root, localName);
			Assert.IsNotNull(matchingNodes, "no " + localName + " inserted");
			Assert.AreEqual(1, matchingNodes.Count(), "only one " + localName + " node should remain");
			Assert.AreEqual(expectedValue, matchingNodes.First().Value, "unexpected value for " + localName);
		}

		protected void AssertEqualsTo(XElement root, string localName, string expectedValue)
		{
			IEnumerable<XElement> matchingNodes = FindNodes(root, localName);
			Assert.IsNotNull(matchingNodes, "no " + localName + " inserted");
			Assert.IsTrue(matchingNodes.Any(n => n.Value == expectedValue), "unexpected value for " + localName);
		}

		protected static IEnumerable<XElement> FindNodes(XElement root, string localName)
		{
			XNamespace defaultNamespace = root.GetDefaultNamespace();
			var outputPathNodes = root.Descendants(defaultNamespace.GetName(localName));
			return outputPathNodes;
		}

		protected XElement Normalize(string projectFile)
		{
			var normalizer = new VSProjectNormalizer(Settings);
			string normalized = normalizer.Normalize(new FileInfo(projectFile));
			File.WriteAllText(
				Path.Combine(Path.GetDirectoryName(projectFile), Path.GetFileNameWithoutExtension(projectFile)) + ".normalized.xml",
				normalized);
			XElement root = XElement.Parse(normalized);
			return root;
		}
	}
}