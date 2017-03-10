using System;
using System.IO;

namespace VSProjectNormalizer
{
	public class Settings
	{
        private const string SOLUTION_BUILD_FOLDER = @"$(SolutionDir)artifacts\$(Configuration)\{ProjectType}\$(AssemblyName)$(PlatformPath)";
        private const string SOLUTION_INTERMEDIATE_FOLDER = @"$(SolutionDir)obj\$(Configuration)\$(AssemblyName)$(PlatformPath)";
        private const string PROJECT_BUILD_FOLDER= @"!artifacts\$(Configuration)$(PlatformPath)";
        private const string PROJECT_INTERMEDIATE_FOLDER = @"!obj\$(Configuration)$(PlatformPath)";
        private const string SOLUTIONDIR_BUILD_COMMON_PROPS = "$(SolutionDir)Build\\common.props";

	    public static Settings Default { get; } = new Settings();

        public string ProjectIntermediateFolder { get; set; }

        public string ProjectBuildFolder { get; set; }

        public string SolutionIntermediateFolder { get; set; }

        public string SolutionBuildFolder { get; set; }

        public string CommonPropsFile { get; set; }

	    public Settings()
	    {
	        SolutionBuildFolder = SOLUTION_BUILD_FOLDER;
	        SolutionIntermediateFolder = SOLUTION_INTERMEDIATE_FOLDER;
	        ProjectBuildFolder = PROJECT_BUILD_FOLDER;
	        ProjectIntermediateFolder = PROJECT_INTERMEDIATE_FOLDER;
	        CommonPropsFile = SOLUTIONDIR_BUILD_COMMON_PROPS;
	    }
	}
}