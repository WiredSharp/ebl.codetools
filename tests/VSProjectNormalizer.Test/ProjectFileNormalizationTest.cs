using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	[TestFixture]
	public class ProjectFileNormalizationTest
	{
		protected VSProjectNormalizer.Settings Settings;

		[SetUp]
		public void Setup()
		{
			Settings = NewSettings;
		}

		public VSProjectNormalizer.Settings NewSettings
		{
			get
			{
				return new VSProjectNormalizer.Settings()
				{
					AcceptanceTestOutputPath = "ProjectFileNormalizationTest.AcceptanceTestOutputPath"
					,BinOutputPath = "ProjectFileNormalizationTest.BinOutputPath"
					,BuildPath = "ProjectFileNormalizationTest.BuildPath"
					,IntermediateOutputPath = "ProjectFileNormalizationTest.IntermediateOutputPath"
					,TestOutputPath = "ProjectFileNormalizationTest.TestOutputPath"
				};
			}
		}

		[Test]
		public void xml_declaration_is_returned()
		{
			const string projectFile = @"playground\regular.csproj.xml";
			var normalizer = new VSProjectNormalizer(Settings);
			string normalized = normalizer.Normalize(new FileInfo(projectFile));
			Assert.That(normalized, Is.StringStarting("<?xml version=\"1.0\" encoding=\"utf-8\"?>"), "xml declaration are omitted");
		}

		[Test]
		public void only_csharp_project_are_modified()
		{
			const string projectFile = @"playground\website.csproj.xml";
			var normalizer = new VSProjectNormalizer(Settings);
			string normalized = normalizer.Normalize(new FileInfo(projectFile));
			Assert.That(normalized, NetBike.XmlUnit.NUnitAdapter.IsXml.Equals(File.ReadAllText(projectFile)), "website project should not be modified");			
		}
	}
}