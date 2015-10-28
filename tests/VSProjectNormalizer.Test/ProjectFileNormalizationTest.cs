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
		private VSProjectNormalizer.Settings _settings;

		[SetUp]
		public void Setup()
		{
			_settings = new VSProjectNormalizer.Settings()
			{
				AcceptanceTestOutputPath = "ProjectFileNormalizationTest.AcceptanceTestOutputPath"
				,BinOutputPath = "ProjectFileNormalizationTest.BinOutputPath"
				,BuildPath = "ProjectFileNormalizationTest.BuildPath"
				,IntermediateOutputPath = "ProjectFileNormalizationTest.IntermediateOutputPath"
				,TestOutputPath = "ProjectFileNormalizationTest.TestOutputPath"
			};
		}

		[Test]
		public void regular_project_outputpath_node_is_replaced_and_unique()
		{
			const string projectFile = @"playground\regular.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, "OutputPath", _settings.BinOutputPath);
		}

		[Test]
		public void execution_is_idempotent()
		{
			const string projectFile = @"playground\test.csproj.xml";
			var normalizer = new VSProjectNormalizer(_settings);
			var first = normalizer.Normalize(new FileInfo(projectFile));
			Assert.AreEqual(first, normalizer.Normalize(first), "second execution is not idempotent");
		}

		[Test]
		public void test_project_outputpath_node_is_replaced_and_unique()
		{
			const string projectFile = @"playground\test.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, "OutputPath", _settings.TestOutputPath);
		}

		[Test]
		public void acceptance_project_outputpath_node_is_replaced_and_unique()
		{
			const string projectFile = @"playground\acceptance.csproj.xml";
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, "OutputPath", _settings.AcceptanceTestOutputPath);
		}

		[Test]
		public void project_intermediate_path_node_is_replaced_and_unique(
			[Values(@"playground\regular.csproj.xml"
			,@"playground\test.csproj.xml"
			,@"playground\acceptance.csproj.xml")] string projectFile)
		{
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, "IntermediateOutputPath", _settings.IntermediateOutputPath);
		}

		[Test]
		public void build_dir_property_is_added_and_unique(
			[Values(@"playground\regular.csproj.xml"
			, @"playground\test.csproj.xml"
			, @"playground\acceptance.csproj.xml")] string projectFile)
		{
			XElement root = Normalize(projectFile);
			AssertIsUniqueAndEqualsTo(root, "BuildDir", _settings.BuildPath);
		}

		private void AssertIsUniqueAndEqualsTo(XElement root, string localName, string expectedValue)
		{
			IEnumerable<XElement> matchingNodes = FindNodes(root, localName);
			Assert.IsNotNull(matchingNodes, "no " + localName + " inserted");
			Assert.AreEqual(1, matchingNodes.Count(), "only one " + localName + " node should remain");
			Assert.AreEqual(expectedValue, matchingNodes.First().Value, "unexpected value for " + localName);
		}

		private static IEnumerable<XElement> FindNodes(XElement root, string localName)
		{
			XNamespace defaultNamespace = root.GetDefaultNamespace();
			var outputPathNodes = root.Descendants(defaultNamespace.GetName(localName));
			return outputPathNodes;
		}

		private XElement Normalize(string projectFile)
		{
			var normalizer = new VSProjectNormalizer(_settings);
			string normalized = normalizer.Normalize(new FileInfo(projectFile));
			File.WriteAllText(
				Path.Combine(Path.GetDirectoryName(projectFile), Path.GetFileNameWithoutExtension(projectFile)) + ".normalized.xml",
				normalized);
			XElement root = XElement.Parse(normalized);
			return root;
		}
	}
}