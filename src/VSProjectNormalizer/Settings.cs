using System;
using System.IO;

namespace VSProjectNormalizer
{
	public class Settings
	{
		private static readonly Lazy<Settings> DEFAULT = new Lazy<Settings>();

		public static Settings Default => DEFAULT.Value;

        public string ExternalBuildPrefix => Resource.Default.EXTERNAL_BUILD_FOLDER + @"\";

        public string DefaultBuildPrefix => Resource.Default.DEFAULT_BUILD_FOLDER + @"\";

	    public string AcceptanceTestOutputPath { get; set; }

	    public string TestOutputPath { get; set;  }

		public string BinOutputPath { get; set;  }

		public string IntermediateOutputPath { get; set; }

		public string BuildPath { get; set; }
	    public string DefaultSolutionDir { get; set; }
	    public string DefaultSolutionName { get; set; }

	    public string WithPlatform(string path)
        {
            return Path.Combine(path, Resource.Default.PLATFORM_OUTPUT_PATH);
        }

	    public Settings()
	    {
	        IntermediateOutputPath = Resource.Default.INTERMEDIATE_OUTPUT_PATH;
	        BinOutputPath = Resource.Default.BIN_OUTPUT_PATH;
	        TestOutputPath = Resource.Default.TEST_OUTPUT_PATH;
	        AcceptanceTestOutputPath = Resource.Default.ACCEPTANCE_TEST_OUTPUT_PATH;
	        DefaultSolutionDir = Resource.Default.DEFAULT_SOLUTION_DIR;
	        DefaultSolutionName = Resource.Default.DEFAULT_SOLUTION_NAME;
	    }
	}
}