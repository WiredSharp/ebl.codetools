using System;
using System.IO;

namespace VSProjectNormalizer
{
	public class Settings
	{
	    private const string ACCEPTANCE_TEST_OUTPUT_PATH = @"Acceptance\$(AssemblyName)";
	    private const string TEST_OUTPUT_PATH = @"Tests\$(AssemblyName)";
	    private const string BIN_OUTPUT_PATH = @"Bin\$(AssemblyName)";
	    private const string INTERMEDIATE_OUTPUT_PATH = @"obj\$(AssemblyName)";
        private const string DEFAULT_SOLUTION_DIR = @"..\..\";
        private const string DEFAULT_SOLUTION_NAME = @"$(AssemblyName)";
        private const string PLATFORM_OUTPUT_PATH= @"$(Platform)";
        private const string EXTERNAL_BUILD_FOLDER= @"$(BuildDir)$(SolutionName)\$(Configuration)";
        private const string DEFAULT_BUILD_FOLDER= @"$(SolutionDir)Artifacts\$(Configuration)";

        public static Settings Default { get; } = new Settings();

	    public string ExternalBuildPrefix { get; set; }

	    public string DefaultBuildPrefix { get; set; }

	    public string AcceptanceTestOutputPath { get; set; }

	    public string TestOutputPath { get; set;  }

		public string BinOutputPath { get; set;  }

		public string IntermediateOutputPath { get; set; }

		public string BuildPath { get; set; }
	    public string DefaultSolutionDir { get; set; }
	    public string DefaultSolutionName { get; set; }

	    public string PlatformOutputPath { get; set; }

	    public string WithPlatform(string path)
        {
            return Path.Combine(path, PlatformOutputPath);
        }

	    public Settings()
	    {
	        IntermediateOutputPath = INTERMEDIATE_OUTPUT_PATH;
	        BinOutputPath = BIN_OUTPUT_PATH;
	        TestOutputPath = TEST_OUTPUT_PATH;
	        AcceptanceTestOutputPath = ACCEPTANCE_TEST_OUTPUT_PATH;
	        DefaultSolutionDir = DEFAULT_SOLUTION_DIR;
	        DefaultSolutionName = DEFAULT_SOLUTION_NAME;
	        PlatformOutputPath = PLATFORM_OUTPUT_PATH;
            ExternalBuildPrefix = EXTERNAL_BUILD_FOLDER;
	        DefaultBuildPrefix = DEFAULT_BUILD_FOLDER;
	    }
	}
}