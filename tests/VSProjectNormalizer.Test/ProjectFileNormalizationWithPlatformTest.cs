using NUnit.Framework;

namespace VSProjectNormalizer.Test
{
	[TestFixture]
	public class ProjectFileNormalizationWithPlatformTest : ProjectFileNormalizationTestBase
	{
		private const string BUILD_PREFIX_WITH_PLATFORM = @"$(BuildDir)$(SolutionName)\$(Configuration).$(Platform)\";
		private const string DEFAULT_BUILD_PREFIX_WITH_PLATFORM = @"$(SolutionDir)$(Configuration).$(Platform)\";

		protected override string BuildPrefix
		{
			get { return BUILD_PREFIX_WITH_PLATFORM; }
		}

		protected override string DefaultBuildPrefix
		{
			get { return DEFAULT_BUILD_PREFIX_WITH_PLATFORM; }
		}

		protected override VSProjectNormalizer.Settings NewSettings
		{
			get
			{
				return new VSProjectNormalizer.Settings()
				{
					AcceptanceTestOutputPath = "ProjectFileNormalizationWithPlatformTest.AcceptanceTestOutputPath"
					,BinOutputPath = "ProjectFileNormalizationWithPlatformTest.BinOutputPath"
					,BuildPath = "ProjectFileNormalizationWithPlatformTest.BuildPath"
					,IntermediateOutputPath = "ProjectFileNormalizationWithPlatformTest.IntermediateOutputPath"
					,TestOutputPath = "ProjectFileNormalizationWithPlatformTest.TestOutputPath"
					,UsePlatform = true
				};
			}
		}
	}
}