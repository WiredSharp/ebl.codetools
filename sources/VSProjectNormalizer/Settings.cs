using System;

namespace VSProjectNormalizer
{
	public class Settings
	{
		private const string WITH_PLATFORM = @".$(Platform)\";
		private const string DEFAULT_BUILD_FOLDER = @"$(SolutionDir)$(Configuration)";
		private const string SPECIFIC_BUILD_FOLDER = @"$(BuildDir)$(SolutionName)\$(Configuration)";

		private const string ACCEPTANCE_TEST_OUTPUT_PATH = @"acceptance\$(AssemblyName)";
		private const string TEST_OUTPUT_PATH = @"tests\$(AssemblyName)";
		private const string BIN_OUTPUT_PATH = @"bin\$(AssemblyName)";
		private const string INTERMEDIATE_OUTPUT_PATH = @"obj\$(AssemblyName)";

		private static readonly Lazy<Settings> DEFAULT = new Lazy<Settings>();
		private string _acceptanceTestOutputPath;
		private string _testOutputPath;
		private string _binOutputPath;
		private string _intermediateOutputPath;

		public static Settings Default
		{
			get { return DEFAULT.Value; }
		}

		public bool UsePlatform { get; set; }

		public string BuildPrefix
		{
			get { return UsePlatform ? SPECIFIC_BUILD_FOLDER + WITH_PLATFORM : SPECIFIC_BUILD_FOLDER + @"\"; }
		}

		public string DefaultBuildPrefix
		{
			get { return UsePlatform ? DEFAULT_BUILD_FOLDER + WITH_PLATFORM : DEFAULT_BUILD_FOLDER + @"\"; }
		}

		public string AcceptanceTestOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_acceptanceTestOutputPath) ? ACCEPTANCE_TEST_OUTPUT_PATH : _acceptanceTestOutputPath;
			}
			set { _acceptanceTestOutputPath = value; }
		}

		public string TestOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_testOutputPath) ? TEST_OUTPUT_PATH : _testOutputPath;
			}
			set { _testOutputPath = value; }
		}

		public string BinOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_binOutputPath) ? BIN_OUTPUT_PATH : _binOutputPath;
			}
			set { _binOutputPath = value; }
		}

		public string IntermediateOutputPath
		{
			get
			{
				return String.IsNullOrEmpty(_intermediateOutputPath) ? INTERMEDIATE_OUTPUT_PATH : _intermediateOutputPath;
			}
			set { _intermediateOutputPath = value; }
		}

		public string BuildPath { get; set; }

		public Settings()
		{
			AcceptanceTestOutputPath = ACCEPTANCE_TEST_OUTPUT_PATH;
			TestOutputPath = TEST_OUTPUT_PATH;
			BinOutputPath = BIN_OUTPUT_PATH;
			IntermediateOutputPath = INTERMEDIATE_OUTPUT_PATH;
		}
	}
}