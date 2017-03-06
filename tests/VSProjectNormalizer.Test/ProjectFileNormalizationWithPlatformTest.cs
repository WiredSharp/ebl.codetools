using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	[TestFixture]
	public class ProjectFileNormalizationWithPlatformTest : ProjectFileNormalizationTestBase
	{
		private const string BUILD_PREFIX = @"$(BuildDir)$(SolutionName)\$(Configuration)\";
		private const string DEFAULT_BUILD_PREFIX = @"$(SolutionDir)$(Configuration)\";

		protected override string BuildPrefix => BUILD_PREFIX;

	    protected override string DefaultBuildPrefix => DEFAULT_BUILD_PREFIX;

	    protected override Settings NewSettings => new Settings()
		                                           {
		                                               UsePlatform = true
		                                               ,BuildPath = "ProjectFileNormalizationWithPlatformTest.BuildPath"
		                                           };

        [Test]
        public void outputpath_ends_with_platform_tag(
            [Values(@"playground\regular.csproj.xml"
                , @"playground\test.csproj.xml"
                , @"playground\acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
            IEnumerable<XElement> matchingNodes = TestHelpers.FindNodes(root, "OutputPath");
            Assert.IsTrue(matchingNodes.All(n => n.Value.EndsWith("$(Platform)")), "all output path should end with $Platform tag");
        }

        [Test]
        public void intermediate_outputpath_ends_with_platform_tag(
            [Values(@"playground\regular.csproj.xml"
                , @"playground\test.csproj.xml"
                , @"playground\acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
            IEnumerable<XElement> matchingNodes = TestHelpers.FindNodes(root, "IntermediateOutputPath");
            Assert.IsTrue(matchingNodes.All(n => n.Value.EndsWith("$(Platform)")), "all intermediate output path should end with $Platform tag");
        }
    }
}