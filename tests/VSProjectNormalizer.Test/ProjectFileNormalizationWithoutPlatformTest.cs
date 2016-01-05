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

		protected override string BuildPrefix
		{
			get { return BUILD_PREFIX_WITHOUT_PLATFORM; }
		}

		protected override string DefaultBuildPrefix
		{
			get { return DEFAULT_BUILD_PREFIX_WITHOUT_PLATFORM; }
		}

		protected override VSProjectNormalizer.Settings NewSettings
		{
			get
			{
				return new VSProjectNormalizer.Settings()
				{
					AcceptanceTestOutputPath = "ProjectFileNormalizationWithoutPlatformTest.AcceptanceTestOutputPath"
					,
					BinOutputPath = "ProjectFileNormalizationWithoutPlatformTest.BinOutputPath"
					,
					BuildPath = "ProjectFileNormalizationWithoutPlatformTest.BuildPath"
					,
					IntermediateOutputPath = "ProjectFileNormalizationWithoutPlatformTest.IntermediateOutputPath"
					,
					TestOutputPath = "ProjectFileNormalizationWithoutPlatformTest.TestOutputPath"
				};
			}
		}
	}
}