using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	[TestFixture]
	public class ProjectFileNormalizationWithoutPlatformTest : ProjectFileNormalizationTestBase
	{
		private const string BUILD_PREFIX_WITHOUT_PLATFORM = @"$(BuildDir)$(SolutionName)\$(Configuration)\";
		private const string DEFAULT_BUILD_PREFIX_WITHOUT_PLATFORM = @"$(SolutionDir)$(Configuration)\";

		protected override string BuildPrefix => BUILD_PREFIX_WITHOUT_PLATFORM;

	    protected override string DefaultBuildPrefix => DEFAULT_BUILD_PREFIX_WITHOUT_PLATFORM;

	    protected override Settings NewSettings => new Settings()
		                                           {
		                                               BuildPath = "ProjectFileNormalizationWithoutPlatformTest.BuildPath"
		                                               ,UsePlatform = false
		                                           };

        [Test]
        public void outputpath_does_not_end_with_platform_tag(
            [Values(@"playground\regular.csproj.xml"
                , @"playground\test.csproj.xml"
                , @"playground\acceptance.csproj.xml")] string projectFile)
        {
            XElement root = Normalize(projectFile);
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
            IEnumerable<XElement> matchingNodes = TestHelpers.FindNodes(root, "IntermediateOutputPath");
            Assert.IsFalse(matchingNodes.Any(n => n.Value.EndsWith("$Platform")), "no intermediate output path should end with $Platform tag");
        }
    }
}