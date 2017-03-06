using System;
using System.IO;

namespace VSProjectNormalizer
{
	public class Settings
	{
		private const string WITH_PLATFORM = "$(Platform)";
		private const string DEFAULT_BUILD_FOLDER = @"$(SolutionDir)$(Configuration)";
		private const string SPECIFIC_BUILD_FOLDER = @"$(BuildDir)$(SolutionName)\$(Configuration)";

		private const string ACCEPTANCE_TEST_OUTPUT_PATH = @"Acceptance\$(AssemblyName)";
		private const string TEST_OUTPUT_PATH = @"Tests\$(AssemblyName)";
		private const string BIN_OUTPUT_PATH = @"Bin\$(AssemblyName)";
		private const string INTERMEDIATE_OUTPUT_PATH = @"obj\$(AssemblyName)";

		private static readonly Lazy<Settings> DEFAULT = new Lazy<Settings>();
		private string _acceptanceTestOutputPath;
		private string _testOutputPath;
		private string _binOutputPath;
		private string _intermediateOutputPath;

		public static Settings Default => DEFAULT.Value;

	    public bool UsePlatform { get; set; }

		public string BuildPrefix => SPECIFIC_BUILD_FOLDER + @"\";

	    public string DefaultBuildPrefix => DEFAULT_BUILD_FOLDER + @"\";

	    public string AcceptanceTestOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_acceptanceTestOutputPath) ? HandlePlatform(ACCEPTANCE_TEST_OUTPUT_PATH) : _acceptanceTestOutputPath;
			}
			set { _acceptanceTestOutputPath = value; }
		}

	    public string TestOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_testOutputPath) ? HandlePlatform(TEST_OUTPUT_PATH) : _testOutputPath;
			}
			set { _testOutputPath = value; }
		}

		public string BinOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_binOutputPath) ? HandlePlatform(BIN_OUTPUT_PATH) : _binOutputPath;
			}
			set { _binOutputPath = value; }
		}

		public string IntermediateOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_intermediateOutputPath) ? HandlePlatform(INTERMEDIATE_OUTPUT_PATH) : _intermediateOutputPath;
			}
			set { _intermediateOutputPath = value; }
		}

		public string BuildPath { get; set; }

        private string HandlePlatform(string path)
        {
            return UsePlatform ? Path.Combine(path, WITH_PLATFORM) : path;
        }
	}
}