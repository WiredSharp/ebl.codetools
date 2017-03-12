using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeTools.Test.Common;
using NUnit.Framework;

namespace CodeTools.VisualStudio.Tools.Test
{
	[TestFixture]
	public class VSProjectNormalizerTest
	{
		protected Settings Settings;
		internal const string OUTPUT_PATH_TAG = "OutputPath";
		internal const string INTERMEDIATE_OUTPUT_PATH = "IntermediateOutputPath";
		internal const string BIN_OUTPUT_PATH = @"$(SolutionDir)artifacts\$(Configuration)\Bin\$(AssemblyName)$(PlatformPath)";
		internal const string TEST_OUTPUT_PATH = @"$(SolutionDir)artifacts\$(Configuration)\Test\$(AssemblyName)$(PlatformPath)";
		internal const string ACCEPTANCE_OUTPUT_PATH = @"$(SolutionDir)artifacts\$(Configuration)\Acceptance\$(AssemblyName)$(PlatformPath)";

		[SetUp]
		public void Setup()
		{
			Settings = NewSettings;
		}

		protected Settings NewSettings => new Settings()
		{
			CommonPropsFile = "CommonPropsFile"
			 ,
			ProjectBuildFolder = "ProjectBuildFolder"
			 ,
			ProjectIntermediateFolder = "ProjectIntermediateFolder"
			 ,
			SolutionBuildFolder = "SolutionBuildFolder"
			 ,
			SolutionIntermediateFolder = "SolutionIntermediateFolder"
		};

		[Test]
		public void execution_is_idempotent()
		{
			const string projectFile = @"test.csproj.xml";
			var normalizer = new VSProjectNormalizer.VSProjectNormalizer(Settings);
			var first = normalizer.Normalize(projectFile.GetTestFileInfo());
			Assert.AreEqual(first, normalizer.Normalize(first), "second execution is not idempotent");
		}

		[Test]
		[Sequential]
		public void outputpath_is_correctly_set_with_projecttype_tag([Values(@"regular.csproj.xml"
																										  ,@"acceptance.csproj.xml"
																										  ,@"test.csproj.xml")] string projectFile
																						, [Values(BIN_OUTPUT_PATH, ACCEPTANCE_OUTPUT_PATH, TEST_OUTPUT_PATH)] string expectedOutputPath)
		{
			Settings = Settings.Default;
			XElement root = Normalize(projectFile);
			root.AssertExactMatch(OUTPUT_PATH_TAG
										 , Settings.ProjectBuildFolder
										 , expectedOutputPath);
		}

		[Test]
		public void outputpath_is_correctly_set([Values(@"regular.csproj.xml"
																						  ,@"acceptance.csproj.xml"
																						  ,@"test.csproj.xml")] string projectFile)
		{
			XElement root = Normalize(projectFile);
			root.AssertExactMatch(OUTPUT_PATH_TAG
										 , Settings.ProjectBuildFolder
										 , Settings.SolutionBuildFolder);
		}

		[Test]
		public void intermediate_outputpath_is_correctly_set([Values(@"regular.csproj.xml"
																						  ,@"acceptance.csproj.xml"
																						  ,@"test.csproj.xml"
																						  ,@"website.csproj.xml")] string projectFile)
		{
			XElement root = Normalize(projectFile);
			root.AssertExactMatch(INTERMEDIATE_OUTPUT_PATH
										 , Settings.ProjectIntermediateFolder
										 , Settings.SolutionIntermediateFolder);
		}

		[Test]
		public void shared_props_are_imported_with_condition(
			 [Values(@"regular.csproj.xml"
					 , @"test.csproj.xml"
					 , @"acceptance.csproj.xml"
					 , @"website.csproj.xml")] string projectFile)
		{
			XElement normalized = Normalize(projectFile);
			XElement import = normalized.FindNodes("Import").FirstOrDefault(i => i.Attribute("Project")?.Value == Settings.CommonPropsFile);
			Assert.IsNotNull(import, "shared import has not been inserted");
			XAttribute condition = import.Attribute("Condition");
			Assert.IsNotNull(condition, "no condition has been set on shared import");
			Assert.That(condition.Value, Does.Contain("'$(SolutionDir)' != ''"), "unexpected condition on shared import");
		}

		[Test]
		public void xml_declaration_is_returned()
		{
			const string projectFile = @"regular.csproj.xml";
			var normalizer = new VSProjectNormalizer.VSProjectNormalizer(Settings);
			string normalized = normalizer.Normalize(projectFile.GetTestFileInfo());
			Assert.That(normalized, Does.StartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"), "xml declaration are omitted");
		}

		[Test]
		public void website_project_output_path_is_not_modified()
		{
			const string projectFile = @"website.csproj.xml";
			XElement normalized = projectFile.GetTestFileInfo().Normalize(Settings);
			IEnumerable<XElement> outputPathNodes = normalized.FindNodes("OutputPath");
			Assert.IsTrue(outputPathNodes.All(node => node.Value == @"bin\"));
		}

		protected XElement Normalize(string projectFile)
		{
			return projectFile.GetTestFileInfo().Normalize(Settings);
		}
	}

	internal static class TestHelpers
	{
		public static XElement Normalize(this FileInfo projectFile, Settings settings)
		{
			var normalizer = new VSProjectNormalizer.VSProjectNormalizer(settings);
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